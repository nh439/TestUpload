function showpass() {
    var x = document.getElementById("checkbox_0");
    var f = document.getElementById("PSD");

    var i = x.checked;
    if (i) {
        f.style.display = '';
    }
    else {
        f.style.display = 'none';
    }

}
function ShowFile() {
    var table = document.getElementById("Tablebody");
    var Files = document.getElementById('Files');
    var t = document.getElementById("Total");
    var total = 0;
    const max = 2147483648;
    $("#Tablebody").empty();
    if (Files.files.length > 0) {
        document.getElementById("S01").disabled = false;
    }
    else {
        document.getElementById("S01").disabled = true;
    }
    for (var i = 0; i < Files.files.length; i++) {
        console.log(i);
        var fst = Files.files[i];
        console.log(fst);
        var row = table.insertRow(i);
        var cell1 = row.insertCell(0);
        var cell2 = row.insertCell(1);
        var cell3 = row.insertCell(2);
        //  var cell4 = row.insertCell(3);
        cell1.innerHTML = fst.name;
        cell2.innerHTML = fst.type;
        cell3.innerHTML = fst.size + " bytes";
        //  cell4.innerHTML = "<i class='trash alternate icon'></i>";
        total = total + fst.size;
    }
    var si = total;
    var se = "";
    if (si <= 1024) {
        se = si + " Bytes";
    }
    else if (si <= (1024 * 1024)) {
        se = (si / 1024).toFixed(2) + " KB";
    }
    else if (si <= (1024 * 1024 * 1024)) {
        se = (si / (1024 * 1024)).toFixed(2) + " MB";
    }
    else {
        se = (si / (1024 * 1024 * 1024)).toFixed(2) + " GB";
    }
    t.innerHTML = "<h1>" + Files.files.length + " Total " + se + "</h1>";
    if (total >= max) {
        document.getElementById("S01").disabled = true;
    }
    else {
        document.getElementById("S01").disabled = false;
    }
}
function Submitted() {
    var myModal = new bootstrap.Modal(document.getElementById('Subbmitted'), {
        keyboard: false
    })
    myModal.show();

}