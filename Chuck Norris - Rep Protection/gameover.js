var sound = new Audio('Defeat.wav');
sound.autoplay = true; // ------------------ Erratic whether or not sound plays!!
document.addEventListener("DOMContentLoaded", function() {
    console.log("Defeat!");
    sound.autoplay = true; // no observed effect
    //sound.play();
    //sound.play();
});
