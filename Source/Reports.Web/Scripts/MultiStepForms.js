$(document).ready(function () {
    $(document).on('click', '.multiStepForm .multiStepFormNext', function (e) {
        if ($(this).parents('.step').validateElement()) {
            var next = $(this).parents('.step').nextAll('.step').not('.skipThisStep').first();

            if (next.length > 0) {
                $(this).parents('.step').hide();
                next.show();

                var title = $(this).parents('.ovDialog').data('title') + ' - ' + next.data('title');

                $(this).parents('.ui-dialog').find('.ui-dialog-title').html(title);
            }
        }
    });

    $(document).on('click', '.multiStepForm .multiStepFormBack', function (e) {
        var prev = $(this).parents('.step').prevAll('.step').not('.skipThisStep').first();

        if (prev.length > 0) {
            $(this).parents('.step').hide();
            prev.show();

            var title = $(this).parents('.ovDialog').data('title') + ' - ' + prev.data('title');

            $(this).parents('.ui-dialog').find('.ui-dialog-title').html(title);
        }
    });
});