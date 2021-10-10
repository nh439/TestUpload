function passcheck() {
    var i = document.getElementById('newpass').value;
    var j = document.getElementById('ret').value;
    if (i === j && i != "") {
        document.getElementById('ms1').style.display = " none";
        document.getElementById('Sub01').disabled = false;
    }
    else {
        document.getElementById('ms1').style.display = " block";
        document.getElementById('Sub01').disabled = true;
    }
}
function HE() {
    var i = document.getElementById('newpass').value;
    if (i) {
        document.getElementById('Sub01').disabled = true;
    }
    else {
        document.getElementById('Sub01').disabled = false;
    }
}