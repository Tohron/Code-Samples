var signin;

document.addEventListener("DOMContentLoaded", function() {
  signin = document.getElementById("signin");
});

function login(event) {
  event.preventDefault();
  const username = document.getElementById('inputUsername').value;
  const password = document.getElementById('inputPassword').value;
  const roleSelect = document.getElementById('inputRole');
  const roleString = roleSelect.options [roleSelect.selectedIndex] .value;
  //console.log(roleSelect.options [roleSelect.selectedIndex] .value); // is e or f
  var role;
  if (roleString === "f") {
      role = "FINANCE";
      console.log("Logging in as finance...");
  } else {
      role = "EMPLOYEE";
      console.log("Logging in as employee...");
  }

  const cred = {
    username,
    password,
    role
  }
  console.log(cred);
  signin.innerHTML = `<label>Logging in...</label>`;

  fetch('http://localhost:8080/ERS/login', {
    method: 'POST',
    body: JSON.stringify(cred),
    headers: {
      'Content-Type': 'application/json'
    },
    credentials: 'include'
  })
    .then(res => {
      console.log("Response: " + res);
      if (res.status === 200) {
        if (role === "FINANCE") {
          window.location = '../finance.html';
        } else {
          window.location = '../employee.html';
        }
      } else {
        resetLogin();
      }
    })
    .catch(err => {
      console.log(err);
      resetLogin();
    })
}

function resetLogin() {
  signin.innerHTML = `<img class="mb-4" src="https://getbootstrap.com/assets/brand/bootstrap-solid.svg" alt="" width="72" height="72">
  <h1 class="h3 mb-3 font-weight-normal">Please sign in to Employee Reimbursement System</h1>
  <label for="inputUsername" class="sr-only">Username</label>
  <input type="text" id="inputUsername" class="form-control" placeholder="Username" required autofocus>
  <label for="inputPassword" class="sr-only">Password</label>
  <input type="password" id="inputPassword" class="form-control" placeholder="Password" required>
  <label>Role:
    <select id="inputRole">
        <option selected="selected" value="e">Employee</option>
        <option value="f">Finance Manager</option>
    </select>
  </label>
  <button class="btn btn-lg btn-primary btn-block" type="submit">Sign in</button>
  <p class="mt-5 mb-3 text-muted">&copy; 2018</p>`;
}