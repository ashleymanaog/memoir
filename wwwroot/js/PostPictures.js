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
const item = document.querySelector(".carousel-item");
const screenheight = screen.height;
item.style.height = screenheight;

containImg();
function containImg() {
    const bg = document.querySelector(".blurredBG");
    const img = document.querySelector(".carousel-item.active img");
    if (img != null) {
        var url = img.src;
        bg.style.backgroundImage = "url(" + url + ")";
    }
}