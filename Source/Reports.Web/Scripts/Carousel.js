$(document).ready(function () {
    //var swipeOptions = {
    //    triggerOnTouchEnd: true,
    //    swipeStatus: swipeStatus,
    //    allowPageScroll: "vertical",
    //    threshold: 75
    //};


    //if ($('body').hasClass('mobile')) {
    //    $('.carousel .blocks').each(function () {
    //        $(this).swipe(swipeOptions);
    //    });
    //}
    

    $(document).on('click', '.carousel .rightArrow', function (e) {
        e.preventDefault();

        var blocksLeft = parseInt($(this).parents('.carousel').data('left'));

        if ($(this).hasClass('disabled')) {
            $(this).parents('.carousel').find('.blocks').animate({ 'left': blocksLeft + 'px' }, 300, 'easeOutQuart');
            return;
        }

        var blockWidth = $(this).parents('.carousel').find('.blocks .block').first().outerWidth()
        var blocksWidth = $(this).parents('.carousel').find('.blocks').outerWidth();
        var blocksWrapperWidth = $(this).parents('.carousel').find('.blocksWrapper').outerWidth();

        blocksLeft -= blockWidth + (parseInt($(this).parents('.carousel').find('.blocks .block').first().css('margin-right')) + parseInt($(this).parents('.carousel').find('.blocks .block').first().css('margin-left')));

        if ($(this).parents('.carousel').find('.blocksWrapper').outerWidth() - (blocksWidth + blocksLeft) < blockWidth) {
            $(this).parents('.carousel').find('.leftArrow').removeClass('disabled');
            $(this).parents('.carousel').find('.blocks').animate({ 'left': blocksLeft + 'px' }, 300, 'easeOutQuart');

            $(this).parents('.carousel').data('left', blocksLeft);
        }

        blocksLeft -= blockWidth + (parseInt($(this).parents('.carousel').find('.blocks .block').first().css('margin-right')) + parseInt($(this).parents('.carousel').find('.blocks .block').first().css('margin-left')));

        if ($(this).parents('.carousel').find('.blocksWrapper').outerWidth() - (blocksWidth + blocksLeft) >= blockWidth) 
            $(this).addClass('disabled');
    });

    $(document).on('click', '.carousel .leftArrow', function (e) {
        e.preventDefault();

        var blockWidth = $(this).parents('.carousel').find('.blocks .block').first().outerWidth()

        var blocksLeft = parseInt($(this).parents('.carousel').data('left'));

        if ($(this).hasClass('disabled')) {
            $(this).parents('.carousel').find('.blocks').animate({ 'left': blocksLeft + 'px' }, 300, 'easeOutQuart');
            return;
        }

        blocksLeft += blockWidth + (parseInt($(this).parents('.carousel').find('.blocks .block').first().css('margin-right')) + parseInt($(this).parents('.carousel').find('.blocks .block').first().css('margin-left')));

        if (blocksLeft <= 0) {
            $(this).parents('.carousel').find('.rightArrow').removeClass('disabled');
            $(this).parents('.carousel').find('.blocks').animate({ 'left': blocksLeft + 'px' }, 300, 'easeOutQuart');

            $(this).parents('.carousel').data('left', blocksLeft);
        }

        if (blocksLeft == 0)
            $(this).addClass('disabled');
    });

    //function swipeStatus(event, phase, direction, distance) {
    //    //If we are moving before swipe, and we are going L or R in X mode, or U or D in Y mode then drag.
    //    var element = $(event.currentTarget);

    //    var blockWidth = element.find('.block').first().outerWidth() + parseInt(element.find('.block').first().css('margin-left')) + parseInt(element.find('.block').first().css('margin-right'));

    //    if (phase == "move" && (direction == "left" || direction == "right")) {
    //        var duration = 0;

    //        var left = parseInt(element.parents('.carousel').data('left'));

    //        if (direction == "left") {
    //            left -= distance;

    //            if (left < -1 * element.width() + blockWidth)
    //                left = -1 * element.width() + blockWidth;

    //            element.css('left', left + 'px');

    //        } else if (direction == "right") {
    //            left += distance;

    //            if (left > 0)
    //                left = 0;

    //            element.css('left', left + 'px');
    //        }
    //    } else if (phase == "cancel") {
    //        var blockLeft = element.parents('.carousel').data('left');

    //        element.animate({ 'left': blockLeft + 'px' }, 300, 'easeOutQuart');
    //    } else if (phase == "end") {
    //        if (direction == "right") {
    //            element.parents('.carousel').find('.leftArrow').click();
    //        } else if (direction == "left") {
    //            element.parents('.carousel').find('.rightArrow').click();
    //        }
    //    }
    //};

    if ($('.carousel').length > 0)
        bms.resizeCarousel();
});

$(window).load(function () {
    if ($('.carousel').length > 0)
        bms.resizeCarousel();
});

$(window).resize(function () {
    if ($('.carousel').length > 0)
        bms.resizeCarousel();
});

bms.resizeCarousel = function (carousel) {
    var scrollBarWidth = 17;

    if ($('body').hasClass('mobile'))
        scrollBarWidth = 0;

    if (carousel == undefined)
        carousel = $('.carousel');
    else
        scrollBarWidth = 0;

    carousel.each(function () {
        $(this).width($(this).parents('.reportsCarousel').width() - parseInt($(this).css('margin-left')) - parseInt($(this).css('margin-right')) - scrollBarWidth);
        $(this).find('.blocks').stop().css('left', '0px');

        var carouselWidth = $(this).width();

        var blocksWidth = $(this).find('.block').length * ($(this).find('.block').first().outerWidth() + parseInt($(this).find('.block').first().css('margin-left')) + parseInt($(this).find('.block').first().css('margin-right')));

        $(this).find('.blocks').width(blocksWidth);

        if (blocksWidth > carouselWidth - 100) {
            if ($('body').hasClass('mobile')) {
                $(this).find('.arrows.rightArrow').removeClass('disabled');
            }
            else {
                $(this).find('.arrows').css('display', 'inline-block');
            }
            $(this).find('.blocksWrapper').css('margin-left', '0px');
            $(this).find('.blocksWrapper').width(carouselWidth - ($(this).find('.leftArrow').width() + parseInt($(this).find('.leftArrow').css('margin-right'))) * 2);
        }
        else {
            if ($('body').hasClass('mobile')) {
                $(this).find('.arrows').addClass('disabled');
            }
            else {
                $(this).find('.arrows').css('display', 'none');
            }
            $(this).find('.blocksWrapper').width(carouselWidth);
            $(this).find('.blocksWrapper').css('margin-left', ($(this).find('.leftArrow').width() + parseInt($(this).find('.leftArrow').css('margin-right')))  + 'px');
        }
    });
}