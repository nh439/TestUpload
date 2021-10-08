const { table } = require("console");

function Commonsearch() {
    var input, filter, table, tr, td, i, txtValue;
    input = document.getElementById("KeyInp");
    filter = input.value.toUpperCase();
    table = document.getElementById("Myupload");
    tr = table.getElementsByTagName("tr");
    for (i = 0; i < tr.length; i++) {
        td = tr[i].getElementsByTagName("td")[2];
        if (td) {
            txtValue = td.textContent || td.innerText;
            if (txtValue.toUpperCase().indexOf(filter) > -1) {
                tr[i].style.display = "";
            } else {
                tr[i].style.display = "none";
            }
        }



    }

}

function sortTable(n) {
    var table, rows, switching, i, x, y, shouldSwitch, dir, switchcount = 0;
    table = document.getElementById("Myupload");
    switching = true;
    // Set the sorting direction to ascending:
    dir = "asc";
    /* Make a loop that will continue until
    no switching has been done: */
    while (switching) {
        // Start by saying: no switching is done:
        switching = false;
        rows = table.rows;
        /* Loop through all table rows (except the
        first, which contains table headers): */
        for (i = 1; i < (rows.length - 1); i++) {
            // Start by saying there should be no switching:
            shouldSwitch = false;
            /* Get the two elements you want to compare,
            one from current row and one from the next: */
            x = rows[i].getElementsByTagName("TD")[n];
            y = rows[i + 1].getElementsByTagName("TD")[n];
            /* Check if the two rows should switch place,
            based on the direction, asc or desc: */
            if (dir == "asc") {
                if (x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase()) {
                    // If so, mark as a switch and break the loop:
                    shouldSwitch = true;
                    break;
                }
            } else if (dir == "desc") {
                if (x.innerHTML.toLowerCase() < y.innerHTML.toLowerCase()) {
                    // If so, mark as a switch and break the loop:
                    shouldSwitch = true;
                    break;
                }
            }
        }
        if (shouldSwitch) {
            /* If a switch has been marked, make the switch
            and mark that a switch has been done: */
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;
            // Each time a switch is done, increase this count by 1:
            switchcount++;
        } else {
            /* If no switching has been done AND the direction is "asc",
            set the direction to "desc" and run the while loop again. */
            if (switchcount == 0 && dir == "asc") {
                dir = "desc";
                switching = true;
            }
        }
    }
    
}
function DeleteF(Url) {
    var i = confirm("Are You Sure?")
    if (i) {
        window.location.href = Url;
    }
}

function DeleteM() {
    var Table = document.getElementById("Myupload");
    console.log(Table);
  //  var j = 0;
    var checkstate = Table.getElementsByTagName("input");
    // for(var row in Table.rows) {
    for (j = 0; j < Table.rows.length; j++)
        var row = Table.rows[j];
    if (row[0].value=="1") {
            var h = row.cells[1].innerText;
            console.log("File : "+h);
    }
    if (row[0].value == "2") {
        var h = row.cells[1].innerText;
        console.log("Blob : " + h);
    }
      //  j++;
    console.log(selectedFile)
}

function Formatting() {
    var myModal = new bootstrap.Modal(document.getElementById('Formatting'), {
        keyboard: false
    })
    myModal.show();

}
    
