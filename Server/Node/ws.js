var http = require('http');
var fs = require('fs');
var WSServer = require('websocket').server;
var plainHttpServer = http.createServer(function(req, res) {
	res.writeHead(200, { 'Content-Type': 'text/html'});
	res.end('Hello world');
}).listen(55555);
var webSocketServer = new WSServer({httpServer: plainHttpServer});

var _start = 0;
var _connections = {};
var _syncs = [];
var _historys = [];

console.log("# Now waiting sockets.");

webSocketServer.on('request', function (req) {
	var websocket = req.accept(null, req.origin);
	var count = countKeys(_connections);
	if(count == 0){
		resetPassedTime();
		loadHistory(1, 0);
		loadHistory(2, 0);
	}
	console.log(getPassedString() + " key=" + req.key + ",connections=" + count);
	_connections[req.key] = websocket;
	var start = new Object();
	var group = 1 + getRandom(2);
	var history = [];
	start.TIME = getPassedTime();
	start.DATA = { nam:"sta", sta:"accept", grp:group, id:req.key };
	websocket.send(JSON.stringify(start));

	websocket.on('message', function(msg) {
		//dump(req.key + ">>" + msg.binaryData);
		var list = JSON.parse(msg.binaryData);
		for(var i in list)
		{
			var obj = new Object();
			obj.TIME = getPassedTime();
			obj.DATA = list[i];
			_syncs.push(obj);
			history.push(obj);
		}
		//broadcast(msg.binaryData);	// まとめて送信するので不要
	});

	websocket.on('close', function (code,desc) {
		console.log(req.key + '>disconnected');
		delete _connections[req.key];
		saveHistory(group, getRandom(10), history);
	});
});

function dump(obj){
	if(typeof obj === 'string' || typeof obj === 'number'){ console.log(obj); return; }
	for(var i in obj) console.log("\t" + i + "=" + obj[i]);
}
function countKeys(obj){
	var cnt = 0;
	for(var i in obj) cnt++;
	return cnt;
}
function resetPassedTime(){ _start = new Date().getTime(); dump("# reset-time=" + _start); }
function getPassedTime(){
	if(_start == 0) resetPassedTime();
	return new Date().getTime() - _start;
}
function getPassedString(){
	return "#" + getPassedTime() + "\t";
}
function getRandom(range){
	return Math.floor( Math.random() * range );
}
function saveHistory(group, index, history){
	var start =  history[0].TIME;
	for(var i = 0; i < history.length; i++) history[i].TIME -= start;
    fs.writeFile("../history/" + group + "/" + index + ".log", JSON.stringify(history) , function(err) { if(err!=null)console.log(err); });
}
function loadHistory(group, index){
    fs.readFile("../history/" + group + "/" + index + ".log",
        function(err, data) {
            if(err) throw err;
            var list = JSON.parse(data);
            var current = getPassedTime();
            for(var i = 0; i < list.length; i++) list[i].TIME += current;
            _historys.push(list);
			dump("## length=" + list.length);
        }
    );
}
function updateHistory(){
	var current = getPassedTime();
    for(var i in _historys){
        var history = _historys[i];
		if(history.length == 0){ dump("# history=" + _historys.length); delete _historys[i]; continue; }
        for(var j in history){
        	if(history[j].TIME > current) break;
			_syncs.push((history[j]));
			delete history[j];
        }
    }
}

// Broadcasting
function broadcast(message){
	for(var i in _connections) _connections[i].send(message);
}
function broadcasting(){
	updateHistory();
	if(_syncs.length == 0) return;
	broadcast(JSON.stringify(_syncs));
	_syncs = [];
}
setInterval(broadcasting, 100);
