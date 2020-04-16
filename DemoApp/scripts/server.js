let srv = new Server();
let basePath = "../../../web/";
let names = [
"james",
"danny",
"ocean",
"modalis"
]
srv.get('/api/v1/index', function (req, res) {
    res.send('Yo soy la pampara prendidisimaaaa', 200, '')
});

srv.get('/mustache/index', function(req,res){
   res.view(basePath + 'templates/hello.mustache', JSON.parse(File.read(basePath +  "data/data.json")));
});

srv.get('/mustache/home', function(req,res){
   res.view(basePath + 'templates/home.mustache', JSON.parse(File.read(basePath +  "data/data.json")));
})

srv.get('/mustache/dd', function(req,res){
   res.dd(names);
})

srv.clientSideRouting(true);
srv.staticFolder('public');
srv.staticFolder('music');
srv.start(3000);
console.log('servidor iniciado en el puerto 3000');
console.read();
srv.stop();
