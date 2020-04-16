 var P = ["\\", "|", "/", "-"];
  var x = 0;
  let interval  = setInterval(function() {
    console.clear();
    console.log("\r" + P[x++]);
    x &= 3;
  }, 250);

console.read();
 cleanInterval(interval);