var table;

var reimbursements = [];
var reimb_lines = [];
var selected_line;


var pending = true;
var approved = true;
var rejected = true;

var a_field;
var r_field;
var d_field;

var check_fp;
var check_fa;
var check_fr;

var button_a;
var button_d;

document.addEventListener("DOMContentLoaded", function() {
    table = document.getElementById("table");

    a_field = document.getElementById("inputAuthor");
    a_field.disabled = true;
    r_field = document.getElementById("inputResolver");
    r_field.disabled = true;
    d_field = document.getElementById("inputDescription");
    d_field.disabled = true;
    button_a = document.getElementById("button_a");
    button_d = document.getElementById("button_d");
    button_a.disabled = true;
    button_d.disabled = true;
    check_fp = document.getElementById("check_fp");
    check_fa = document.getElementById("check_fa");
    check_fr = document.getElementById("check_fr");

    fetch('http://localhost:8080/ERS/finance/getUser', {
    method: 'GET',
    credentials: 'include'
    })
    .then(resp => resp.json())
    .then(data => {
        const u_label = document.getElementById('user_label');
        console.log("Got User Data: " + data);
        const username = data;
        u_label.innerHTML = "Logged in as: " + username;
        
    });
    //loadReimbursements();
    update();
    setInterval(update, 10000);
});

function filter() {
    button_a.disabled = true;
    button_d.disabled = true;
    clear();
    update();
}

function clickHandler(event, id, index) {
    // Deselects last line
    var div1;
    if (selected_line) {
        div1 = document.getElementById(selected_line);
        div1.style.backgroundColor="#ffffff";
        console.log("Deselecting " + selected_line); // never reached!
    }

    console.log("Selecting " + id);
    selected_line = id;
    div1 = document.getElementById(id);
    div1.style.backgroundColor="#33bbff";
    // fills right side with data for selected line
    a_field.value  = reimbursements[index].author;
    r_field.value  = reimbursements[index].resolver;
    d_field.value  = reimbursements[index].description;

    button_a.disabled = false;
    button_d.disabled = false;
}

function approve() {
    var id = selected_line.substring(1);
    fetch('http://localhost:8080/ERS/finance/approve/' + id, {
    method: 'GET',
    credentials: 'include'
    })
    .then(res => {
        update();
    });
    
    button_a.disabled = true;
    button_d.disabled = true;
    clear();
}
function deny() {
    var id = selected_line.substring(1);
    fetch('http://localhost:8080/ERS/finance/deny/' + id, {
    method: 'GET',
    credentials: 'include'
    })
    .then(res => {
        update();
    });
    
    button_a.disabled = true;
    button_d.disabled = true;
    clear();
}

function clear() {
    var div1;
    if (selected_line) {
        div1 = document.getElementById(selected_line);
        div1.style.backgroundColor="#ffffff";
        console.log("Deselecting " + selected_line); // never reached!
    }
    a_field.value  = "";
    r_field.value  = "";
    d_field.value  = "";
    const e_table = document.getElementById('e-table');
    e_table.innerHTML = `<thead>
        <tr>
            <th class="c1">Amount</th> <th class="c2">Type</th> <th class="c3">Submitted</th> <th class="c4">Resolved</th> <th class="c5">Status</th>
        </tr>
    </thead>
    <tbody>
        <tr><td>Waiting for Database...</td></tr>
    </tbody>`;
}

function update() {
    console.log("CB: " + document.getElementById("check_fp"));
    pending = check_fp.checked;
    approved = check_fa.checked;
    rejected = check_fr.checked;
    var terms = pending + "/" + approved + "/" + rejected;
    fetch('http://localhost:8080/ERS/finance/' + terms, {
    method: 'GET',
    credentials: 'include'
    })
    .then(resp => resp.json())
    .then(data => {
        const e_table = document.getElementById('e-table');
        reimbursements = data;
        
        reimb_lines = reimbursements.map((r, index) => `
        <tr id="r${r.id}" onclick="clickHandler(event, 'r${r.id}', ${index})">
            <td class="c1">$${r.amount}</td>  <td class="c2">${r.type}</td> <td class="c3">${r.submittedString}</td> <td class="c4">${r.resolvedString}</td> <td class="c5">${r.status}</td>
        </tr>
        `);
        e_table.innerHTML = `<thead>
        <tr>
        <th class="c1">Amount</th> <th class="c2">Type</th> <th class="c3">Submitted</th> <th class="c4">Resolved</th> <th class="c5">Status</th>
        </tr> </thead><tbody>` + reimb_lines.join('') + "</tbody>";
        
        
    });
}

function logout() {
    fetch('http://localhost:8080/ERS/f_logout', {
    method: 'GET',
    credentials: 'include'
    });
    window.location = '../login.html';
}

/*
const reimb_res = {
    id,
    resolverID,
    status
}
console.log(reimb_res);

fetch('http://localhost:8080/ERS/finance', {
method: 'POST',
body: JSON.stringify(reimb_res),
headers: {
    'Content-Type': 'application/json'
},
credentials: 'include'
})
.then(res => {
    console.log("Posted: " + res);
})
.catch(err => {
    console.log(err);
})
*/