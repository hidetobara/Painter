var http = require('http');
var WSServer = require('websocket').server;
var url = require('url');
var plainHttpServer = http.createServer(function(req, res) {
	res.writeHead(200, { 'Content-Type': 'text/html'});
	res.end('Hello world');
}).listen(12345);
var webSocketServer = new WSServer({httpServer: plainHttpServer});

var connections = {};
console.log("[info] Now waiting sockets.");

webSocketServer.on('request', function (req) {
	var websocket = req.accept(null, req.origin);
	connections[req.key] = websocket;
	dump("request>" + req.key + " count=" + count(connections));

	websocket.on('message', function(msg) {
		console.log('"' + msg + '" is recieved from ' + req.origin + '!');
		dump(msg);
		websocket.send('sended from WebSocket Server' + req.key);
	});

	websocket.on('close', function (code,desc) {
		console.log('connection released! : ' + req.key);
		delete connections[req.key];
	});
});

function dump(obj)
{
	if(typeof obj === 'string'){ console.log('string=' + obj); return; }
	if(typeof obj === 'number'){ console.log('number=' + obj); return; }
	for(var i in obj) console.log("\t" + i + "=" + obj[i]);
}
function count(obj)
{
	var cnt = 0;
	for(var i in obj) cnt++;
	return cnt;
}
