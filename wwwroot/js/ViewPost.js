function initializeCarousel(postId) {
    //Index of carousel on display
    var totalItems = $('#postCarousel_' + postId + ' .carousel-item').length;
    var currentIndex = $('#postCarousel_' + postId + ' .carousel-item.active').index();

    $('#postCarousel_' + postId).carousel({
        interval: false
    });

    $('#postCarousel_' + postId).on('slid.bs.carousel', function () {
        currentIndex = $('#postCarousel_' + postId + ' .carousel-item.active').index();
        updateCarouselInfo(postId);
        containImg(postId);
    });

    //Handle next button click
    $('#postCarousel_' + postId + ' .carousel-control-next').on('click', function () {
        currentIndex = $('#postCarousel_' + postId + ' .carousel-item.active').index();
        updateCarouselInfo(postId);
    });

    //Handle previous button click
    $('#postCarousel_' + postId + ' .carousel-control-prev').on('click', function () {
        currentIndex = $('#postCarousel_' + postId + ' .carousel-item.active').index();
        updateCarouselInfo(postId);
    });

    //Initialize
    updateCarouselInfo(postId);
    containImg(postId);

    $(window).on('resize', function () {
        debouncedContainImg(postId);
    });
    
    //Be able to click video controls
    var container = $('#postCarousel_' + postId + ' .carousel-item');
    var prevControl = $('#postCarousel_' + postId + ' .carousel-control-prev');
    var nextControl = $('#postCarousel_' + postId + ' .carousel-control-next');
    console.log(document.querySelector('#postCarousel_' + postId + ' video'))
    if (document.querySelector('#postCarousel_' + postId + ' video')) {
        console.log('inside')
        prevControl.addClass('prevMargin');
        nextControl.addClass('nextMargin');
    }
}

const debouncedContainImg = debounce(containImg, 50);
// Debounce function to limit the rate of execution
function debounce(func, delay) {
    let timer;
    return function () {
        const context = this;
        const args = arguments;
        clearTimeout(timer);
        timer = setTimeout(function () {
            func.apply(context, args);
        }, delay);
    };
}

// Function to update the active index / total images info
function updateCarouselInfo(postId) {
    var currentIndex = $('#postCarousel_' + postId + ' .carousel-item.active').index();
    var totalItems = $('#postCarousel_' + postId + ' .carousel-item').length;
    currentIndex +- 1;
    $('#postCarousel_' + postId + ' .num').html('' + currentIndex + '/' + totalItems + '');
    containImg(postId);
    //updateBlurredBackground(postId, imageUrl);
}

//Height of Picture/Vid
function containImg(postId) {
    const item = $('#postCarousel_' + postId + ' .carousel-item.active');
    const img = $('#postCarousel_' + postId + ' .carousel-item.active img');

    if (img.length > 0 && img.attr('src')) {
        var url = img.attr('src');
        var newImg = new Image();
        newImg.src = url;

        const bg = $('#postCarousel_' + postId + ' .blurredBG');
        var height = img.height();//img.clientHeight

        if (height > 600) {
            item.addClass('addHeight');
            img.addClass('objectFitContain');
            bg.addClass('displayed');
            bg.css('backgroundImage', 'url(' + url + ')');
        }
        if (height == 600) {
            item.removeClass('addHeight');
            img.removeClass('objectFitContain');
            bg.removeClass('displayed');
            height = img.clientHeight;
        }
        if (height < 600) {
            item.removeClass('addHeight');
            img.removeClass('objectFitContain');
            bg.removeClass('displayed');
        }

        newImg.onload = function () {
            if (height > 600) {
                item.addClass('addHeight');
                img.addClass('objectFitContain');
                bg.addClass('displayed');
                bg.css('backgroundImage', 'url(' + url + ')');
            }
            if (height == 600) {
                item.removeClass('addHeight');
                img.removeClass('objectFitContain');
                bg.removeClass('displayed');
                height = img.clientHeight;
            }
            if (height < 600) {
                item.removeClass('addHeight');
                img.removeClass('objectFitContain');
                bg.removeClass('displayed');
            }
        }
    }
}

//Focus on textarea when comment button is clicked
function commentOnTextarea() {
    var textarea = document.querySelector(".textarea");
    textarea.focus();
    textarea.select();
}