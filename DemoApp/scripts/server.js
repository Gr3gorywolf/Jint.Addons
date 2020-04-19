let srv = new Server();
srv.appendResponseHeader("Access-Control-Allow-Origin", "*");
srv.appendResponseHeader("Access-Control-Allow-Credentials", "true");
srv.appendResponseHeader("Access-Control-Allow-Methods", "*");
srv.appendResponseHeader("Access-Control-Allow-Headers", "*");
let basePath = "../../../web/";
let names = [
   "james",
   "danny",
   "ocean",
   "modalis"
]
let users = []
srv.get('/api/v1/index', function (req, res) {
   console.log(req.query.hola);
   console.log(req.query.mundo);
   res.redirect('/mustache/form');
});
srv.post('/api/v1/form', function (req, res) {
   users.push(req.data);
   console.log(req.data);
   res.render(basePath + 'templates/form.mustache', { klk: "awawawawaw" });
});

srv.get('/mustache/form', function (req, res) {
   res.render(basePath + 'templates/form.mustache', { klk: "awawawawaw" });
});

srv.get('/mustache/index', function (req, res) {
   res.render(basePath + 'templates/hello.mustache', { klk: "awawawawaw" });
});

srv.get('/mustache/500', function (req, res) {
   res.view(null, JSON.parse(File.read(basePath + "data/data.json")));
});

srv.get('/mustache/dd', function (req, res) {
   res.dd(users);
});
srv.allowClientSideRouting(true);
srv.staticFolder('public')
srv.listen(3000);
console.log('servidor iniciado en ' + srv.hostname + ':' + srv.port);
console.read();
srv.stop();

