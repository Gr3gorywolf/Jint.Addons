//@jint-ignore
var express = require('express');
var fs=require("fs");
var app = express();
app.use("/", express.static("public"))
//@end-jint-ignore



/*@jint
    var app = new Server()
 @end-jint*/
var pokemonList=[];
loadPokemons();

app.get("/api/numeros/aleatorios", function(req, res){
    var nums = [];
    for (let i = 0; i < 3; i++) {
        nums.push(Math.floor(Math.random() * 99) + 1);
    }
	//@jint-ignore
    res.send(`
    <h4 style="color:red">Sus numeros de la suerte son:  ${nums.join("-")}</h4>
    `);
	//@end-jint-ignore

	/*@jint
	res.send("sus numeros de la suerte son"+ nums.join("-") )
	@end-jint*/
});

app.get("/api/calcular/:type/:nums", function(req, res) {
    var type = req.params.type;
    var numbers = req.params.nums.toString().split(",");
    var total = 0;
    for (var n of numbers) {
        let parametednumber = parseInt(n);
        if (type == "suma") {
            total += parametednumber;
        }
    }
	//@jint-ignore
    res.send(`
  <h4 style="color:red">El resultado es:  ${total}</h4>
  `);
  //@end-jint-ignore

  /*@jint
  res.send("El resultado es:"+ total )
  @end-jint*/
});
app.get("/api/pokemon/:id", function(req, res) {
    var number=001;
    if(req.params.id=="aleatorio"){
        number=Math.floor(Math.random()*806)-1;
    }else{
        number=parseInt(req.params.id);
    }

    var pokeId=number.toString().padStart(3,"0");
    console.log(pokeId);
    var selectedPokemon=getPokemon(pokeId);
    //@jint-ignore
    res.send(`
    <h3>Su pokemon es:</h3>
    <h5>${selectedPokemon.name}</h5>
    <img style="height:400px;width:400px" src="${selectedPokemon.sprites.animated}"/>
    `);
	 //@end-jint-ignore
	  /*@jint
  res.send("Su pokemon es:"+ selectedPokemon.name);
  @end-jint*/
});


app.get("/api/img", function(req, res) {
    //@jint-ignore
    res.send(`
    <img style="height:400px;width:400px" src="https://picsum.photos/200/300"/>
    `);
	//@end-jint-ignore

	 /*@jint
	  res.send("<img style='height:400px;width:400px' src='https://picsum.photos/200/300'/>");
	 @end-jint*/
});




app.listen(3000, (args) => {
    console.log("Servidor iniciado");
});

function loadPokemons(){
//@jint-ignore
   pokemonList=JSON.parse( fs.readFileSync("pokemons.json")).results;
//@end-jint-ignore
   /*@jint
       
     pokemonList = JSON.parse(File.read("../../../web/data/pokemons.json")).results;
      
   @end-jint*/
}
function getPokemon(id){
    for(let pok of pokemonList){
        if(pok.national_number==id){
            return pok;
        }
    }
}