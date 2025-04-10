$(document).ready(function () {
    $(document).click(function (e) {
        $('.rbcontextmenu').hide();
    });

    /*
    $('.contextMenu').each(function () {
        $(this).rbsontextmenu();
    });
    */

    $(document).on('click', '.rbcontextmenuBtn a', function (e) {
        e.preventDefault();
        e.stopPropagation();

        var contextMenu = $('#' + $(this).parent().data('menuid'));

        if (contextMenu != undefined) {
            if (!contextMenu.is(':visible')) {
                var yOffset = $(this).height() + 10;

                var left = 0;
                var top = 0;

                left = $(this).parent().position().left - contextMenu.outerWidth() + $(this).parent().width() + parseInt($(this).parent().css('margin-left'));
                top = $(this).parent().position().top + yOffset;

                if ($(this).parent().offset().top + contextMenu.outerHeight() + yOffset > $(window).height() + $(window).scrollTop() - $('footer').outerHeight()) {
                    top -= contextMenu.outerHeight() + $(this).parent().height() + 18;

                    if (top + $(this).parent().offset().top - $(window).scrollTop() < 0) {
                        top = $(window).scrollTop();
                        left -= $(this).parent().width() + 10;
                    }
                }

                $('.rbcontextmenu').hide();

                contextMenu.css({ 'top': top, 'left': left }).slideDown(200);

                contextMenu.find('a').click(function () {
                    $(this).parents('.rbcontextmenu').hide();
                });
            }
            else {
                contextMenu.slideUp(200);
            }
        }
    });
});