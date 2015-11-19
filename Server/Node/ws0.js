var WebSocketServer = require('websocket').Server
  , fs              = require('fs')
  , http            = require('http')
  , template        = 'index.html'
  , server          = httpServer(function (req, res) {
                          index(template, req, res);
                    })
  , wSServer        = new WebSocketServer({ // この段階で port 指定すると、http サーバーは ws モジュール内部のものを使うようだ
                        "server" : server
//                    ,   "path"   : '/websocket' // パス指定すると、このパスのみ生きるっぽい
                    })
  , port            = 12345
  , connects        = []
;

wSServer.on('connection', function (ws) {
    log("WebSocketServer connected");

    connects.push(ws); // 配列にソケットを格納
    broadcast('connected sockets: ' + connects.length);

    ws.on('message', function (message) {
        log('received -' + message);
        broadcast(message);
    });

    ws.on('close', function () {
        log('stopping client send "close"');

        // 接続切れのソケットを配列から除外
        connects = connects.filter(function (conn, i) {
            return (conn === ws) ? false : true;
        });

        broadcast('connected sockets: ' + connects.length);
    });
});

server.listen(port);
log('Server Start on port -' + port + '- ');

function broadcast (message) {
    connects.forEach(function (socket, i) {
        socket.send(message);
    });
}

function log (str) {
    console.log((new Date).toString() + ' "' + str + '"');
}

function httpServer (onRequest) {
    var _server = http.createServer();

    _server.on('request', function (req, res) {
        log('httpServer on request');
        if (typeof onRequest === 'function') onRequest(req, res);
    });

    _server.on('close', function () {
        log('httpServer closing');
    });

    return _server;
}

function index (template, req, res) {
    fs.stat(template, function (err, stats) {
        if (err) return _error(err);
        if (! stats.isFile()) return _error('not file');

        fs.readFile(template, 'utf-8', function (err, data) {
            if (err) return _error(err);

            res.writeHead(200, {'Content-Type' : 'text/html' });
            res.write(data);
            res.end();
            log('raed file and pirnt: ' + template);
        });
    });
}

function _error (res, err) {
    res.writeHead(500, {'Content-Type' : 'text/plain' });
    res.end(err);
    log(err);
}
