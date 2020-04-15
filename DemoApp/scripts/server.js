let srv = new Server();
srv.get('*', function (req, res) {
    res.redirect('index.html');
});
srv.get('/api/v1/index', function (req, res) {
    res.send('Yo soy la pampara prendidisimaaaa', 200, '')
})
srv.post('/post/data', function (req, res) {
    res.send('Yo ser duro', 200, 'text/plain');
    console.log(req.data);
});
srv.staticFiles('public');
srv.start(3000);
console.log('servidor iniciado en el puerto 3000');
console.read();