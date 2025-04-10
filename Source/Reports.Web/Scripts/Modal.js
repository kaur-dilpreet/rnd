$.fn.showModal = function (options) {
    $(this).appendTo('body');

    var title = $(this).find('.ovDialog').attr('data-title');

    if ($(this).find('.multiStepForm').length > 0)
        title = title + ' - ' + $(this).find('.step1').data('title');

    if ($(this).find('.ovDialog').data('height') == '100%') {
        $(this).find('.ovDialog').height($(window).height() - 100);
    }

    var width = $(this).find('.ovDialog').outerWidth() + 40;

    if (width > $(window).width())
        width = $(window).width();

    $(this).dialog({
        autoOpen: false,
        height: 'auto',
        width: width,
        modal: true,
        title: title,
        resizable: false
    }).dialog('open');

    var subtitle = $(this).find('.ovDialog').data('subtitle');

    if (subtitle != undefined && subtitle != '') {
        $('<div />', {
            class: 'subtitle',
            html: subtitle
        }).appendTo($(this).parents('.ui-dialog').find('.ui-dialog-titlebar'));

        $(this).parents('.ui-dialog').find('.ui-dialog-titlebar').addClass('hasSubtitle');
    }

    $(this).find('.cancelBtn').each(function () {
        $(this).click(function (e) {
            e.preventDefault();

            $(this).parents('.ovDialog').hideModal();
        });
    });

    if ($(this).find('.hpeDialog').length > 0)
        $(this).parents('.ui-dialog').find('.ui-dialog-titlebar').hide();
    else
        $(this).parents('.ui-dialog').find('.ui-dialog-titlebar-close .ui-button-text').html('');

    if ($(window).width() < 630) {
        var titleBarHeight = $(this).parents('.ui-dialog').find('.ui-dialog-titlebar').outerHeight();
        var padding = parseInt($(this).parents('.ui-dialog').find('.ui-dialog-content').css('padding-top')) + parseInt($(this).parents('.ui-dialog').find('.ui-dialog-content').css('padding-bottom'));
        var margin = parseInt($(this).parents('.ui-dialog').find('.ui-dialog-content').css('margin-top')) + parseInt($(this).parents('.ui-dialog').find('.ui-dialog-content').css('margin-bottom'));

        $(this).parents('.ui-dialog').find('.ui-dialog-content').height($(window).height() - titleBarHeight - padding - margin);
    }

    $(this).parents('.ui-dialog').find('.ui-dialog-titlebar-close').bind('click', function (e) {
        e.preventDefault();

        if ($(this).parents('.ui-dialog').find('.mce-tinymce').length > 0) {
            $(this).parents('.ui-dialog').find('.mce-tinymce').each(function () {
                tinymce.execCommand('mceFocus', false, $(this).parents('.textBox').find('textarea').attr('id'));
                tinymce.execCommand('mceRemoveEditor', false, $(this).parents('.textBox').find('textarea').attr('id'));

                //tinymce.remove('#' + $(this).parents('.textBox').find('textarea').attr('id'));
            });
        }

        $(this).parents('.ui-dialog').find('.ovDialog').parent().remove();
        $(this).parents('.ui-dialog').remove();
        $('body').css('overflow-y', 'scroll');
    });

    if ($(window).width() < 630) {
        $('html').css('overflow-y', 'hidden');
    }

    if ($(this).parents('.ui-dialog').outerHeight() < $(window).height()) {
        $(this).parents('.ui-dialog').css('position', 'fixed');
    }
    else {
        if ($(this).parents('.ui-dialog').offset().top < $(window).scrollTop()) {
            $(window).animate({ 'scrollTop': $(this).parents('.ui-dialog').offset().top }, 200);
        }
    }

    $(this).centerDialog();
}

$(window).resize(function () {
    $('.ovDialog').each(function () {
        $(this).centerDialog();

        if ($(this).parents('.ui-dialog').outerHeight() < $(window).height())
            $(this).parents('.ui-dialog').css('position', 'fixed');
        else
            $(this).parents('.ui-dialog').css('position', 'absolute');
    });
});

$.fn.centerDialog = function () {
    //var dialogWidth = $(this).parents('.ui-dialog').width();
    //var dialogHeight = $(this).parents('.ui-dialog').height();
    //var windowWidth = $(window).width();
    //var windowHeight = $(window).height();
    //var left = (windowWidth - dialogWidth) / 2;
    //var top = (windowHeight - dialogHeight) / 2;

    //top = Math.max(top, 0);

    //$(this).parents('.ui-dialog').css({ left: left + 'px', top: top + 'px' });
};

$.fn.hideModal = function () {
    if ($(this).find('.mce-tinymce').length > 0) {
        $(this).find('.mce-tinymce').each(function () {
            tinymce.execCommand('mceFocus', false, $(this).parents('.textBox').find('textarea').attr('id'));
            tinymce.execCommand('mceRemoveEditor', false, $(this).parents('.textBox').find('textarea').attr('id'));

            //tinymce.remove('#' + $(this).parents('.textBox').find('textarea').attr('id'));
        });
    }

    $(this).parent().dialog('destroy');
    $(this).parents('.ui-dialog').remove();
    $(this).parent().remove();
    $(this).remove();
    $('body').css('overflow-y', 'scroll');
};

bms.showErrorMessage = function (message, title) {
    if (title == undefined)
        title = 'Error';

    bms.showMessage('e', message, title);
};

bms.showSuccessMessage = function (message) {
    bms.showMessage('s', message, 'Success');
};

bms.showWarningMessage = function (message) {
    bms.showMessage('w', message, 'Warning');
};

bms.showQuestionMessage = function (message) {
    bms.showMessage('i', message, 'Info');
};

bms.showGeneralError = function () {
    bms.showErrorMessage(bms.generalErrorMessage);
};

bms.showMessage = function (icon, message, title) {
    if (title == undefined) {
        title = '';
    }

    var divIcon = $('<div/>', {
        'class': 'icon ' + icon + 'Icon'
    });

    //var divTitle = $('<div/>', {
    //    'class': 'title',
    //    'html': '<span class="titleText">' + title + '</span><a href="#" class="cancelBtn"><img src="' + bms.baseUrl + 'content/images/help-close.png" /></a>'
    //});

    //var divTitle = $('<div/>', {
    //    'class': 'title',
    //    'html': '<span class="titleText">' + title + '</span>'
    //});

    var divMessage = $('<div/>', {
        'class': 'message',
        'html': message
    });

    var divMessgaeBox = $('<div/>', {
        'class': 'messagebox ovDialog',
        'data-title': title
    });

    var ovDialog = $('<div/>');

    //divIcon.appendTo(divTitle);
    //divTitle.appendTo(divMessgaeBox);
    divMessage.appendTo(divMessgaeBox);
    divMessgaeBox.appendTo(ovDialog)

    ovDialog.showModal();
};

bms.showModalYesNo = function (message, title, yes, no, yesCaption, noCaption) {
    if (title == undefined) {
        title = '';
    }

    var divIcon = $('<div/>', {
        'class': 'icon wIcon'
    });

    var divMessage = $('<div/>', {
        'class': 'message',
        'html': message
    });

    var divButtons = $('<div/>', {
        'class': 'divSubmit',
    });

    if (yesCaption == undefined)
        yesCaption = 'Yes';

    if (noCaption == undefined)
        noCaption = 'No';

    var yesButton = $('<input />', {
        'type': 'submit',
        'class': 'yesButton button primaryButton',
        'value': yesCaption
    }).click(function () {
        divMessgaeBox.hideModal();
        if (yes != undefined)
            yes();
    }).appendTo(divButtons);

    var noButton = $('<input />', {
        'type': 'button',
        'class': 'noButton button',
        'value': noCaption
    }).click(function () {
        divMessgaeBox.hideModal();
        if (no != undefined)
            no();
    }).appendTo(divButtons);

    var divMessgaeBox = $('<div/>', {
        'class': 'messagebox ovDialog yesNoMessageBox deleteForm form',
        'data-title': title
    });

    var ovDialog = $('<div/>');

    //divIcon.appendTo(divMessgaeBox);
    divMessage.appendTo(divMessgaeBox);
    divButtons.appendTo(divMessgaeBox);
    divMessgaeBox.appendTo(ovDialog)

    ovDialog.showModal();

    return divMessgaeBox;
};

bms.showMessageWithCloseButton = function (message) {
    var divIcon = $('<div/>', {
        'class': 'icon wIcon'
    });

    var divMessage = $('<div/>', {
        'class': 'message',
        'html': message
    });

    var divButtons = $('<div/>', {
        'class': 'divSubmit',
    });

    var noCaption = 'Close';

    var noButton = $('<input />', {
        'type': 'button',
        'class': 'noButton button',
        'value': noCaption
    }).click(function () {
        divMessgaeBox.hideModal();
    }).appendTo(divButtons);

    var divMessgaeBox = $('<div/>', {
        'class': 'messagebox ovDialog yesNoMessageBox deleteForm form'
    });

    var ovDialog = $('<div/>');

    //divIcon.appendTo(divMessgaeBox);
    divMessage.appendTo(divMessgaeBox);
    divButtons.appendTo(divMessgaeBox);
    divMessgaeBox.appendTo(ovDialog)

    ovDialog.showModal();

    return divMessgaeBox;
};