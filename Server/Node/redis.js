var client = require('redis').createClient();

var key = 'hoge:fuga';
var value = 'piyo';

// 文字列を保存する
client.set(key, value, function(){
  // コールバック
});

// 文字列を取得する
client.get(key, function(err, val){
  // コールバック
  if (err) return console.log(err);
  // エラーが無ければデータを取得できたということ
  console.log(val);
});
