//Dashboard arrow disappear when clicked
const upicon = document.getElementById('up-icon');
const icon = document.getElementById('down-icon');
const title = document.getElementById('title');
function clickDashboard() {
    icon.classList.remove('displayed');
    icon.classList.add('noDisplay');
    upicon.classList.add('displayed');
    if (window.location.pathname == "/Dashboard/Freshmen") {
        title.innerHTML = "FRESHMEN DASHBOARD"
    } else if (window.location.pathname == "/Dashboard/Sophomore") {
        title.innerHTML = "SOPHOMORE DASHBOARD"
    } else if (window.location.pathname == "/Dashboard/Junior") {
        title.innerHTML = "JUNIOR DASHBOARD"
    } else if (window.location.pathname == "/Dashboard/Senior") {
        title.innerHTML = "SENIOR DASHBOARD"
    }
}
//Dashboard arrow disappear onscroll
window.addEventListener('scroll', () => {
    if (window.scrollY >= 50) {
        clickDashboard();
    }
    
});
//Dashboard arrow appear when scroll to top
window.onscroll = function (ev) {
    if (window.scrollY == 0) {
        icon.classList.remove('noDisplay');
        icon.classList.add('displayed');
        upicon.classList.remove('displayed');
        title.innerHTML = "DASHBOARD"
    }
};