//Navbar Scroll Script
const nav = document.querySelector('nav');
window.addEventListener('scroll', () => {
    if (window.scrollY >= 50) {
        nav.classList.add('active-nav');
        document.querySelector('.link1').style = "text-shadow:none";
        document.querySelector('.link2').style = "text-shadow:none";
        document.querySelector('.link3').style = "text-shadow:none";
        document.querySelector('.menu-icon').style = "text-shadow:none";
    }
    else {
        nav.classList.remove('active-nav');
        document.querySelector('.link1').style = "text-shadow:-1px 0px 0px #f0b81a,1px 0px 0px #f0b81a,0px -1px 0px #f0b81a,0px 1px 0px #f0b81a,0px 2px 0px #f0b81a,0px 3px 0px #f0b81a;"
        document.querySelector('.link2').style = "text-shadow:-1px 0px 0px #f0b81a,1px 0px 0px #f0b81a,0px -1px 0px #f0b81a,0px 1px 0px #f0b81a,0px 2px 0px #f0b81a,0px 3px 0px #f0b81a;"
        document.querySelector('.link3').style = "text-shadow:-1px 0px 0px #f0b81a,1px 0px 0px #f0b81a,0px -1px 0px #f0b81a,0px 1px 0px #f0b81a,0px 2px 0px #f0b81a,0px 3px 0px #f0b81a;"
        document.querySelector('.menu-icon').style = "text-shadow:-1px 0px 0px #f0b81a,1px 0px 0px #f0b81a,0px -1px 0px #f0b81a,0px 1px 0px #f0b81a,0px 2px 0px #f0b81a;"
    }
})

//Have Navbar bg if page loaded on on top
$(window).on('load', function () {
    let scrollTop = document.documentElement.scrollTop;
    if (scrollTop != 0) {
        nav.classList.add('active-nav');
        document.querySelector('.link1').style = "text-shadow:none";
        document.querySelector('.link2').style = "text-shadow:none";
        document.querySelector('.link3').style = "text-shadow:none";
            document.querySelector('.menu-icon').style = "text-shadow:none";
    }
})

//Textarea height
var txHeight = "40";
const tx = document.getElementsByTagName("textarea");

for (let i = 0; i < tx.length; i++) {
    if (tx[i].value == '' && window.location.pathname == "/Dashboard/Post") {
        tx[i].setAttribute("style", "height:" + txHeight + "px;overflow-y:hidden;");
        tx[i].addEventListener("input", OnInput, false);
        tx[i].addEventListener('keyup', OnInput, false);
    } else if (tx[i].value == '' && window.location.pathname == "/Profile/EditProfile") {
        txHeight = "100";
        tx[i].setAttribute("style", "height:" + txHeight + "px;overflow-y:hidden;");
        tx[i].addEventListener("input", OnInput, false);
        tx[i].addEventListener('keyup', OnInput, false);
    }/*else {
        tx[i].setAttribute("style", "height:" + (tx[i].scrollHeight) + "px;overflow-y:hidden;");
    }*/
    
}

function OnInput(e) {
    if (this.scrollHeight > txHeight) {
        this.style.height = 0;
        this.style.height = (this.scrollHeight) + "px";
    } else if (this.scrollHeight <= txHeight) {
        this.style.height = txHeight + "px";
    }
}