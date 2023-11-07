//Index of carousel on display
var myCarousel = document.getElementById('postCarousel')
var totalItems = $('.carousel-item').length;
var currentIndex = $('.carousel-item.active').index();
$('.num').html('' + currentIndex + '/' + totalItems + '');

$('#postCarousel').carousel({
    interval: false
});

$('#postCarousel').on('slid.bs.carousel', function () {
    currentIndex = $('.carousel-item.active').index();
    $('.num').html('' + currentIndex + '/' + totalItems + '');
    containImg();
});

//Height of Picture/Vid
containImg();
function containImg() {
    const item = document.querySelector(".carousel-item.active");
    const img = document.querySelector(".carousel-item.active img");
    var url = img.src;
    const bg = document.querySelector(".blurredBG");
    var height = img.clientHeight;
    if (height > 600) {
        item.classList.add('addHeight');
        img.classList.add('objectFitContain');
        bg.classList.add('displayed');
        bg.style.backgroundImage = "url("+url+")";
    }
    if (height == 600) {
        item.classList.remove('addHeight');
        img.classList.remove('objectFitContain');
        bg.classList.remove('displayed');
        height = img.clientHeight;
        if (height > 600) {
            item.classList.add('addHeight');
            img.classList.add('objectFitContain');
            bg.classList.add('displayed');
            bg.style.backgroundImage = "url(" + url + ")";
        }
    }
}

//Focus on textarea when comment button is clicked
function commentOnTextarea() {
    var textarea = document.querySelector(".textarea");
    textarea.focus();
    textarea.select();
}