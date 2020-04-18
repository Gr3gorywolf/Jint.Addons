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
   res.send('Yo soy la pampara prendidisimaaaa', 200, '')
});
srv.post('/api/v1/submit', function (req, res) {
   users.push(req.data);
   res.send({
      status: "posted",
      message: "ok"
   }, 200);
});

srv.get('/mustache/index', function (req, res) {
   res.render(basePath + 'templates/home.mustache', { klk: "awawawawaw" });
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

