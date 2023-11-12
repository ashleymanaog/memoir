const btnRegister = document.getElementById('btnRegister');

/*var oldpassword;
var newpassword;
var confirmnewpassword;
var oldpasswordError;
var newpasswordError;
var confirmnewpasswordError;*/

/*if (window.location.pathname == "/Home/Registration") {
    password = document.getElementById('password');
    yearLevel = document.getElementById('yearLevel');
    passwordError = document.getElementById('passwordError');
    yearLevelError = document.getElementById('yearLevelError');
} else if (window.location.pathname == "/Profile/EditProfile") {
    oldpassword = document.getElementById('oldpassword');
    newpassword = document.getElementById('newpassword');
    confirmnewpassword = document.getElementById('confirmnewpassword');
    oldpasswordError = document.getElementById('oldpasswordError');
    newpasswordError = document.getElementById('newpasswordError');
    confirmnewpasswordError = document.getElementById('confirmnewpasswordError');
}*/
var firstnameErr = "";
var lastnameErr = "";
var usernameErr = "";
var passwordErr = "";
var emailErr = "";
var yearLevelErr = "";

var firstname = document.getElementById('firstname');
var lastname = document.getElementById('lastname');
var username = document.getElementById('username');
var email = document.getElementById('email');
var password = document.getElementById('password');
var yearLevel = document.getElementById('yearLevel');

var firstnameError = document.getElementById('firstnameError');
var lastnameError = document.getElementById('lastnameError');
var usernameError = document.getElementById('usernameError');
var emailError = document.getElementById('emailError');
var passwordError = document.getElementById('passwordError');
var yearLevelError = document.getElementById('yearLevelError');

var errCount = 0;

btnRegister.addEventListener('click', function () {
    //FirstName
    if (firstname.value.trim() == "") {
        firstnameErr = "This field is required";
        firstnameError.innerHTML = "This field is required";
        errCount++;
    } else if (/[0-9]/.test(firstname.value)) { //has number
        firstnameErr = "Number is not allowed";
        firstnameError.innerHTML = "Number is not allowed";
        errCount++;
    } else {
        firstnameError.innerHTML = ""
    }

    //Lastname
    if (lastname.value.trim() == "") {
        lastnameErr = "This field is required";
        lastnameError.innerHTML = "This field is required";
        errCount++;
    } else if (/[0-9]/.test(lastname.value)) { //has number
        lastnameErr = "Number is not allowed";
        lastnameError.innerHTML = "Number is not allowed";
        errCount++;
    } else {
        lastnameError.innerHTML = ""
    }

    //Username
    if (username.value.trim() == "") {
        usernameErr = "This field is required";
        usernameError.innerHTML = "This field is required";
        errCount++;
    } else {
        usernameError.innerHTML = ""
    }

    //Password
    if (password.value == "") {
        passwordErr = "This field is required";
        passwordError.innerHTML = "This field is required";
        errCount++;
    } else {
        passwordError.innerHTML = ""
    }

    //Email Address
    var validRegex = /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/;
    if (email.value.trim() == "") {
        emailErr = "This field is required";
        emailError.innerHTML = "This field is required";
        errCount++;
    }
    else if (email.value.match(validRegex)) {
        emailError.innerHTML = ""
    }
    else {
        emailErr = "Not a valid email address";
        emailError.innerHTML = "Not a valid email address";
        errCount++;
    }

    //Year Level
    if (yearLevel.value == "placeholder") {
        yearLevelErr = "A year level is not selected"
        yearLevelError.innerHTML = "A year level is not selected"
        errCount++;
    } else {
        yearLevelError.innerHTML = ""
    }
    
    alert('errCount: ' + errCount);
    if (errCount == 0) {
        btnRegister.setAttribute('type', 'submit');
        btnRegister.click();
    }
    errCount = 0;
}, false);