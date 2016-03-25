var _port = process.argv[ process.argv.length - 1 ];

var http = require('http');
var fs = require('fs');
var WSServer = require('websocket').server;
var plainHttpServer = http.createServer(function(req, res) {
	res.writeHead(200, { 'Content-Type': 'text/html'});
	res.end('Hello world');
}).listen(_port);
var webSocketServer = new WSServer({httpServer: plainHttpServer});

var _start = 0;
var _connections = {};
var _syncs = [];
var _historys = [];

console.log("# Now waiting sockets. port=" + _port);

webSocketServer.on('request', function (req) {
	var websocket = req.accept(null, req.origin);
	var count = countKeys(_connections);
	if(count == 0){
		resetPassedTime();
		loadHistory(1, getRandom(10));
		loadHistory(2, getRandom(10));
	}
	console.log(getPassedString() + " key=" + req.key + ",connections=" + count);
	// グループの指定
	var group = calculateBestGroup();
	websocket.group = group;
	_connections[req.key] = websocket;

	var history = [];
	var start = makeHistoryItem( { nam:"sta", sta:"accept", grp:group, id:req.key } );
	websocket.send(JSON.stringify(start));

	websocket.on('message', function(msg) {
		//dump(req.key + ">>" + msg.binaryData);
		var list = JSON.parse(msg.binaryData);
		for(var i in list)
		{
			var obj = makeHistoryItem( list[i] );
			_syncs.push(obj);
			history.push(obj);
			if(obj.DATA.nam == "sta" && obj.DATA.sta == "dead"){
				saveHistory(group, getRandom(10), history);
				history = [];
			}
		}
		//broadcast(msg.binaryData);	// まとめて送信するので不要
	});

	websocket.on('close', function (code,desc) {
		console.log('[disconnected] id=' + req.key);
		delete _connections[req.key];
		var obj = makeHistoryItem( { nam:"sta", sta:"dead", grp:group, id:req.key.substr(0,4) } );
		history.push(obj);
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
	if(history.length < 300) return;

	var start =  history[0].TIME;
	for(var i = 0; i < history.length; i++) history[i].TIME -= start;
	fs.writeFile("../history/" + _port + "/" + group + "/" + index + ".log", JSON.stringify(history) , function(err) { if(err!=null)console.log(err); });
	dump("[history] save=" + group + "/" + index);
}
function loadHistory(group, index){
    fs.readFile("../history/" + _port + "/" + group + "/" + index + ".log",
        function(err, data) {
            if(err){ dump("[history] FAIL to load=" + group + "/" + index); return; }
            var list = JSON.parse(data);
            var current = getPassedTime();
            for(var i = 0; i < list.length; i++) list[i].TIME += current;
            _historys.push(list);
			dump("[history] load=" + group + "/" + index);
        }
    );
}
function updateHistory(){
	var current = getPassedTime();
	for(var i in _historys){
		var history = _historys[i];
		for(var j in history){
        	if(history[j].TIME > current) break;
			var group = history[j].DATA.grp;
			_syncs.push((history[j]));
			delete history[j];
			if(history.length -1 == j){
				delete _historys[i];
				if(countKeys(_connections) > 0) loadHistory(group, getRandom(10));
				if(countKeys(_connections) == 0){ _historys = []; dump("[history] reset"); }
			}
        }
    }
}
function makeHistoryItem(item){
	var o = new Object();
	o.TIME = getPassedTime();
	o.DATA = item;
	return o;
}

function calculateBestGroup() {
	var grp1 = 0;
	var grp2 = 0;
	for (var key in _connections) {
		if (_connections[key].group == 1) grp1++;
		if (_connections[key].group == 2) grp2++;
	}
	dump("[group] 1=" + grp1 + ",2=" + grp2);
	if (grp1 == grp2) return 1 + getRandom(2);
	if (grp1 > grp2) return 2; else return 1;
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
