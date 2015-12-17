var http = require('http');
var WSServer = require('websocket').server;
var url = require('url');
var plainHttpServer = http.createServer(function(req, res) {
	res.writeHead(200, { 'Content-Type': 'text/html'});
	res.end('Hello world');
}).listen(12345);
var webSocketServer = new WSServer({httpServer: plainHttpServer});

var connections = {};
var syncs = [];
console.log("[info] Now waiting sockets.");

webSocketServer.on('request', function (req) {
	var websocket = req.accept(null, req.origin);
	connections[req.key] = websocket;
	dump(req.key + ">connected count=" + count(connections));
	websocket.send('[{"nam":"sta","sta":"start","id":"' + req.key + '"}]');

	websocket.on('message', function(msg) {
		//console.log('"' + msg + '" is recieved from ' + req.origin + '!');
		//dump(req.key + ">>" + msg.binaryData);
		var list = JSON.parse(msg.binaryData);
		for(var i in list) syncs.push(list[i]);
		//broadcast(msg.binaryData);
	});

	websocket.on('close', function (code,desc) {
		console.log(req.key + '>disconnected');
		delete connections[req.key];
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
function broadcast(message){
	for(var i in connections) connections[i].send(message);
}

function broadcasting(){
	if(syncs.length == 0) return;
	//dump(JSON.stringify(syncs));
	broadcast(JSON.stringify(syncs));
	syncs = [];
}
setInterval(broadcasting, 100);
