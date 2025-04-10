$(document).ready(function () {
    $('header #menu').mousedown(function (e) {
        e.stopPropagation();
    });

    $('header #menu').click(function (e) {
        e.preventDefault();

        if ($('#mainMenu').hasClass('open')) {
            $('#mainMenu .category').removeClass('active');
            $('#mainMenu').removeClass('open');
        }
        else {
            $('#mainMenu').addClass('open');
        }
    });

    $('#mainMenu').mousedown(function (e) {
        e.stopPropagation();
    });

    $('#mainMenu li.category').click(function (e) {
        e.preventDefault();
        e.stopPropagation();

        if ($(this).hasClass('active'))
            return;

        //$(this).parents('#mainMenu').find('li.active').removeClass('active');
        $(this).siblings('.category.active').find('.active').removeClass('active');
        $(this).siblings('.category.active').removeClass('active');
        
        $(this).addClass('active');
    });

    $('#mainMenu li.category a').click(function (e) {
        e.stopPropagation();

        $('#mainMenu .category').removeClass('active');
        $('#mainMenu').removeClass('open');
    });
});