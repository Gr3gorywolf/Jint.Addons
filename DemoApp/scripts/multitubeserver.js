let srv = new Server();
srv.appendResponseHeader("Access-Control-Allow-Origin", "*");
srv.appendResponseHeader("Access-Control-Allow-Credentials", "true");
srv.appendResponseHeader("Access-Control-Allow-Methods", "*");
srv.appendResponseHeader("Access-Control-Allow-Headers", "*");
let basePath = "../../../web/";
let filesFolder = 'D:/prueba-multitubeweb-rest';
srv.get("/downloaded.gr3d",function(req,res){
	return res.file(filesFolder+"/downloaded.gr3d");
});

srv.get("/downloaded.gr3d2",function(req,res){
	return res.file(filesFolder+"/downloaded.gr3d2");
});
let users = []
srv.staticFolder(filesFolder + '/frontend');
srv.staticFolder(filesFolder + '/portraits');
srv.staticFolder("E:/Backups/backup microsd 16GB/YTDownloads");
srv.listen(3000);
console.log('servidor iniciado en ' + srv.hostname + ':' + srv.port);
console.read();
srv.stop();

