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

console.log(getPassedString() + " Now waiting sockets.");

webSocketServer.on('request', function (req) {
	var websocket = req.accept(null, req.origin);
	_connections[req.key] = websocket;
	console.log(getPassedString() + " key=" + req.key + ",connecttions=" + count(_connections));
	var start = new Object();
	var group = 1;
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
		fs.writeFile("../history/" + group + "-" + getRandom(10) + ".txt", JSON.stringify(history) , function(err) { if(err!=null)console.log(err); });
	});
});

function dump(obj){
	if(typeof obj === 'string' || typeof obj === 'number'){ console.log(obj); return; }
	for(var i in obj) console.log("\t" + i + "=" + obj[i]);
}
function count(obj){
	var cnt = 0;
	for(var i in obj) cnt++;
	return cnt;
}
function getPassedTime(){
	if(_start == 0){ _start = new Date().getTime(); }
	return new Date().getTime() - _start;
}
function getPassedString(){
	return "#" + getPassedTime() + "\t";
}
function getRandom(range){
	return new Date().getTime() % range;
}

function broadcast(message){
	for(var i in _connections) _connections[i].send(message);
}
function broadcasting(){
	if(_syncs.length == 0) return;
	//dump(JSON.stringify(_syncs));
	broadcast(JSON.stringify(_syncs));
	_syncs = [];
}
setInterval(broadcasting, 100);
