//Dropdown option with placeholder text color
const select = document.querySelector('select');
function changeOptionColor() {
    if (document.getElementById('yearLevel').value != "placeholder") {
        select.classList.remove('option-placeholder');
        select.classList.add('option-color');
    }
}

select.addEventListener("click", function () {
    changeOptionColor();
});

select.addEventListener("change", function () {
    changeOptionColor();
});