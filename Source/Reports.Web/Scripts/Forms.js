
bms.formSubmit = function () {
    //$(this).find('input[type="submit"]').attr('disabled', 'disabled');
    //$('#clearMask').show();

    if (!$(this).valid()) {
        //var div = $('<div/>', {
        //    html: $(this).find('.validationMessage').html()
        //});

        //div.find('.field-validation-error').append('<br />');

        //window.bms.showErrorMessage(div.html());
    }
    else {
        //$(this).find('input[type=submit]').css('min-width', $(this).find('input[type=submit]').width() + 18 + 'px');
        //$(this).find('input[type=submit]').css('width', $(this).find('input[type=submit]').width() + 18 + 'px');
        //$(this).find('input[type=submit]').addClass('loading');
        $(this).find('input[type=submit]').attr('disabled', 'disabled');

        if ($(this).data('ajax'))
            $(this).find('input[type=submit]').addClass('loadingButton');
    }
};

$.fn.clearForm = function () {
    $(this).find('input').each(function () {
        if ($(this).attr('type') == 'text' || $(this).attr('type') == 'password' || $(this).attr('type') == 'email')
            $(this).val('');

        if ($(this).attr('type') == 'checkbox')
            $(this).attr('checked', false);

        if ($(this).siblings('.textPlaceHolder').length > 0)
            $(this).siblings('.textPlaceHolder').show();
    });

    $(this).find('select').each(function () {
        $(this).val($(this).find('option').first().val());
    });

    $(this).find('textarea').each(function () {
        $(this).text('');
        $(this).val('');

        if ($(this).siblings('.textPlaceHolder').length > 0)
            $(this).siblings('.textPlaceHolder').show();
    });
}

bms.formBegin = function (formId) {
    bms.formSubmitBegin(formId);
};

bms.formSubmitBegin = function (formId) {
    bms.showMainLoader();
};

bms.formSubmitComplete = function (formId, response, statusCode, onSucess, onError, shouldHideLoader) {
    if (shouldHideLoader == undefined)
        shouldHideLoader = true;

    $('#' + formId + ' input[type=submit]').attr('disabled', false);
    $('#' + formId + ' input[type=submit]').removeClass('loadingButton');

    if (onSucess != undefined) {
        if (statusCode == 200) {
            var data = $.parseJSON(response);

            if (data.Result) {
                if (shouldHideLoader)
                    bms.hideMainLoader();

                onSucess(data);
            }
            else {
                bms.hideMainLoader();

                if (onError != undefined)
                    onError(data);
                else
                    bms.showErrorMessage(data.Message);
            }
        }
        else {
            bms.hideMainLoader();
            window.bms.showErrorMessage(bms.generalErrorMessage);
        }
    }
};

$.fn.requiredFields = function () {
    $(this).find('.required').each(function () {
        if (!$(this).hasClass('required-field-added')) {
            $(this).addClass('required-field-added');

            $(this).append('<span class="requiredStar">&nbsp;*</span>');
        }
    });

    $(this).find('input[data-val-required], textarea[data-val-required], select[data-val-required]').each(function () {
        if (!$(this).hasClass('required-field-added')) {
            $(this).addClass('required-field-added');
            if ($(this).attr('type') != 'hidden' && $(this).attr('datatextplaceholder') != undefined && $(this).attr('datatextplaceholder').indexOf('</label>') >= 0) {
                $(this).attr('datatextplaceholder', $(this).attr('datatextplaceholder').replace('</label>', '<span class="requiredStar">&nbsp;*</span></label>'));
            }
            else if ($(this).attr('type') != 'hidden') {
                if ($(this).attr('type') == 'checkbox') {
                    if ($(this).hasClass('required'))
                        $(this).parent().append('<span class="requiredStar">&nbsp;*</span>');
                }
                else {
                    if ($(this).parent().find('label').length > 0)
                        $('<span />', { class: "requiredStar", html: "&nbsp;*" }).appendTo($(this).parent().find('label'));
                }
            }
        }
    });

    //$(this).find('select[data-val-required]').each(function () {
    //    if ($(this).hasClass('hasTextPlaceholder')) {
    //        var optionHtml = $(this).find('option').first().html() + '<span class="requiredStar">&nbsp;*</span></label>';

    //        $(this).find('option').first().html(optionHtml);
    //    }
        
    //    if ($(this).parent().prev().hasClass('label'))
    //        $(this).parent().prev().append('<span class="requiredStar">&nbsp;*</span>');
    //});
};

$.fn.prepareForm = function () {
    var form = $(this)

    if (form.prop('tagName') != 'FORM')
        form = $(this).find('form');

    if (form.hasClass('form-prepared'))
        return;

    form.addClass('form-prepared');

    form.bind('submit', window.bms.formSubmit);

    form.removeData('validator');
    form.removeData('unobtrusiveValidation');

    $.validator.unobtrusive.parse(form);
    $(this).requiredFields();

    $(this).addTextPlaceHolder();
    //$(this).find('.captcha').each(function () {
    //    $(this).captcha();
    //});



    form.find('textarea').each(function () {
        if ($(this).hasClass('addTinymce')) {
            if (tinymce != undefined) {
                tinymce.execCommand('mceRemoveEditor', false, $(this).attr('id'));
                tinymce.execCommand('mceAddEditor', false, $(this).attr('id'));
            }
        }
        else {
            $(this).textareaCharLimit();
        }
    });

    //$(this).find('select.hasTextPlaceholder').each(function () {
    //    if ($(this).val() == '')
    //        $(this).addClass('notSelected');

    //    $(this).change(function () {
    //        if ($(this).val() == '')
    //            $(this).addClass('notSelected');
    //        else
    //            $(this).removeClass('notSelected');
    //    });
    //});

    form.find('input[type=text]').each(function () {
        $(this).attr('autocomplete', 'off');
    });

    form.find('.uploader').each(function () {
        $(this).fileUpload();
    });

    form.find('.imageUploader').each(function () {
        $(this).fileUpload();
    });

    form.find('.videoUploader').each(function () {
        $(this).fileUpload();
    });

    form.find('.datePicker').each(function () {
        $(this).rbDatepicker();
    });

    if (form.find('.assetAttachment').length > 0) {
        if (form.find('.assetAttachment').data('empty') != "1") {
            form.find('.assetAttachment').each(function () {
                $(this).assetAttachment();
            });
        }
    }

    form.find('label').click(function (e) {
        e.preventDefault();
    });
};

$.fn.rbDatepicker = function () {
    if (!$(this).hasClass('hasDatepicker')) {
        $('<div />', { class: 'clear' }).insertAfter(this);

        $(this).datepicker({
            minDate: $(this).data('mindate') != undefined ? $(this).data('mindate') : '',
            maxDate: $(this).data('maxdate') != undefined ? $(this).data('maxdate') : '',
            showOn: "button",
            buttonImage: bms.baseUrl + "content/images/icons/calendar.png",
            buttonImageOnly: true,
            buttonText: "Select date",
            altField: $(this).data('alt-field')
        });

        if ($(this).parents('.twoColumnForm').length > 0)
            $(this).parent().css('width', '310px');

        $(this).parent().css('padding-right', '40px');

        $(this).addClass('hasDatepicker');

        $(this).keydown(function (e) {
            e.preventDefault();
            e.stopPropagation()
        })
    }
};

$.fn.addTextPlaceHolder = function () {
    var hideTextPlaceHolder = function (self, element, withoutAnimation) {
        if (element.parent().hasClass('text')) {
            if (withoutAnimation)
                element.css({ 'top': '-20px', 'left': '0px', 'font-size': '9pt' }).addClass('asLabel');
            else
                element.animate({ 'top': '-20px', 'left': '0px', 'font-size': '9pt' }, 200).addClass('asLabel');
        }
        else
        {
            if (withoutAnimation)
                element.css({ 'top': '-4px', 'left': '0px', 'font-size': '9pt' }).addClass('asLabel');
            else
                element.animate({ 'top': '-4px', 'left': '0px', 'font-size': '9pt' }, 200).addClass('asLabel');
        }
    };

    var showTextPlaceHolder = function (self, element, withoutAnimation) {
        if (withoutAnimation)
            element.css({ 'top': element.data('originaltop') + 'px', 'left': '12px', 'font-size': '11pt' }).removeClass('asLabel');
        else
            element.animate({ 'top': element.data('originaltop') + 'px', 'left': '12px', 'font-size': '11pt' }, 200).removeClass('asLabel');
    };

    $(this).find('input[type="text"], input[type="email"], input[type="password"], textarea, select').each(function () {
        if (!$(this).hasClass('text-placeholder-added')) {
            $(this).addClass('text-placeholder-added');

            if ($(this).attr('dataTextPlaceHolder') != undefined) {
                if ($(this).parent().find('.textPlaceHolder').length == 0) {
                    var origTop = 0;

                    if ($(this).parent().hasClass('text')) {
                        origTop = $(this).prop("tagName").toUpperCase() == 'TEXTAREA' ? '6' : '6';
                    }
                    else {
                        origTop = $(this).prop("tagName").toUpperCase() == 'TEXTAREA' ? '19' : '25';
                    }

                    $('<div />', {
                        html: $(this).attr('dataTextPlaceHolder'),
                        Class: 'textPlaceHolder',
                        style: 'top:' + origTop + 'px', // + ($(this).outerHeight() / 5) + 'px;',
                        //'data-originaltop': $(this).outerHeight() / 5
                        'data-originaltop': origTop
                    }).bind('click', bms.textPlaceHolderClick).insertAfter($(this));

                    if ($(this).val() != '')
                        hideTextPlaceHolder($(this), $(this).parent().find('.textPlaceHolder'), true);

                    var self = $(this);

                    var func = function () {
                        if (self.attr('type') == 'password') {
                            self.focus();
                            self.parents('form').find('input, select, textarea').filter(':visible:first').focus();
                        }
                        else if (self.val() != '') {
                            hideTextPlaceHolder(self, self.parent().find('.textPlaceHolder'), false);
                        }
                    };

                    setTimeout(function () {
                        func();
                    }, 1000);

                    $(this).listenForNotEmpty();

                    //$(this).bind('blur', function () {
                    //    if ($(this).attr('dataTextPlaceHolder') != undefined) {
                    //        if ($(this).val() == '')
                    //            showTextPlaceHolder($(this), $(this).parent().find('.textPlaceHolder'), true);
                    //    }
                    //});

                    //$(this).bind('keyup', function (e) {
                    //    if (e.keyCode != 9) {
                    //        if ($(this).attr('dataTextPlaceHolder') != undefined) {
                    //            if ($(this).val() != '')
                    //                hideTextPlaceHolder($(this), $(this).parent().find('.textPlaceHolder'), false);
                    //            else
                    //                showTextPlaceHolder($(this), $(this).parent().find('.textPlaceHolder'), false);
                    //        }
                    //    }
                    //});

                    //$(this).bind('change', function () {
                    //    if ($(this).attr('dataTextPlaceHolder') != undefined) {
                    //        if ($(this).val() != '')
                    //            hideTextPlaceHolder($(this), $(this).parent().find('.textPlaceHolder'), false);
                    //        else
                    //            showTextPlaceHolder($(this), $(this).parent().find('.textPlaceHolder'), false);
                    //    }
                    //});

                    //if ($(this).prop('tagName') == 'SELECT')
                    //{
                    $(this).bind('focus', function () {
                        hideTextPlaceHolder($(this), $(this).parent().find('.textPlaceHolder'), false);
                    });

                    $(this).bind('keyup', function () {
                        hideTextPlaceHolder($(this), $(this).parent().find('.textPlaceHolder'), false);
                    });

                    $(this).blur(function (e) {
                        if ($(this).val() == '')
                            showTextPlaceHolder($(this), $(this).parent().find('.textPlaceHolder'), false);
                    });
                    //}
                }
            }
        }
    });
};

$.fn.listenForNotEmpty = function (options) {
    if ($(this).prop('tagName') == "SELECT")
        return;

    var settings = $.extend({
        interval: 200 // in microseconds 
    }, options);

    var jquery_object = this;
    var current_focus = null;

    jquery_object.filter(":input").add(":input", jquery_object).focus(function () {
        current_focus = this;
    }).blur(function () {
        current_focus = null;
    });

    setInterval(function () {
        // allow 
        jquery_object.filter(":input").add(":input", jquery_object).each(function () {
            // set data cache on element to input value if not yet set 
            if (($(this).data('change_listener') == undefined || $(this).data('change_listener') == '') && $(this).val() == '') {
                $(this).data('change_listener', $(this).val());
                return;
            }
            // return if the value matches the cache 
            if ($(this).data('change_listener') != '' && $(this).val() != '') {
                return;
            }
            // ignore if element is in focus (since change event will fire on blur) 
            //if (this == current_focus) {
            //    return;
            //}
            // if we make it here, manually fire the change event and set the new value 
            $(this).trigger('change');
            $(this).data('change_listener', $(this).val());
        });
    }, settings.interval);
    return this;
};

$.fn.listenForChange = function (options) {
    if ($(this).prop('tagName') == "SELECT")
        return;

    var settings = $.extend({
        interval: 200 // in microseconds 
    }, options);

    var jquery_object = this;
    var current_focus = null;

    jquery_object.filter(":input").add(":input", jquery_object).focus(function () {
        current_focus = this;
    }).blur(function () {
        current_focus = null;
    });

    setInterval(function () {
        // allow 
        jquery_object.filter(":input").add(":input", jquery_object).each(function () {
            // set data cache on element to input value if not yet set 
            if (($(this).data('change_listener') == undefined || $(this).data('change_listener') == '') && $(this).val() == '') {
                $(this).data('change_listener', $(this).val());
                return;
            }
            // return if the value matches the cache 
            if ($(this).data('change_listener') == $(this).val()) {
                return;
            }
            // ignore if element is in focus (since change event will fire on blur) 
            //if (this == current_focus) {
            //    return;
            //}
            // if we make it here, manually fire the change event and set the new value 
            $(this).trigger('change');
            $(this).data('change_listener', $(this).val());
        });
    }, settings.interval);
    return this;
};

bms.textPlaceHolderClick = function () {
    if ($(this).prev().val() != '')
        $(this).hide();

    $(this).prev().focus();
};

bms.getTextAreaLength = function (value) {
    var v = $("<textarea> \r\n </textarea>").val();

    var clientSideLineBreak = v.substr(1, v.length - 2);
    var serverSideLineBreak = "\r\n";

    value = value.replaceAll(clientSideLineBreak, serverSideLineBreak);

    var l = value.length;

    return l;
};

$.fn.textareaCharLimit = function () {
    if ($('#' + $(this).attr('datacharlimitfield')).length > 0)
        $('#' + $(this).attr('datacharlimitfield')).parent().addClass('textCharLimit');

    $(this).bind('paste', function (e) {
        if ($(this).attr('dataLimit') != undefined) {
            var limit = $(this).attr('dataLimit');
            var text = '';
            var length = bms.getTextAreaLength($(this).val());

            if (e.originalEvent.clipboardData != undefined)
                length += e.originalEvent.clipboardData.getData('text').length;
            else
                length += clipboardData.getData('text').length;

            var count = limit - length;

            if (count < 0)
                count = 0;

            if (length > limit) {
                e.preventDefault();

                return;
            }

            if ($('#' + $(this).attr('datacharlimitfield')).length > 0)
                $('#' + $(this).attr('datacharlimitfield')).html(count);
        }
    });

    $(this).bind('keydown', function (e) {
        if ($(this).attr('dataLimit') != undefined) {
            var limit = $(this).attr('dataLimit');
            var length = bms.getTextAreaLength($(this).val());

            if (e.keyCode == 9)
                length += '\t'.length;
            else if (e.keyCode == 13)
                length += 2;
            else if (e.keyCode != 16 && e.keyCode != 17 && e.keyCode != 18 && e.keyCode != 19 && e.keyCode != 20 && e.keyCode != 27 && e.keyCode != 33 &&
                e.keyCode != 34 && e.keyCode != 35 && e.keyCode != 36 && e.keyCode != 37 && e.keyCode != 38 && e.keyCode != 39 && e.keyCode != 40 && e.keyCode != 45) {
                length += bms.getTextAreaLength(String.fromCharCode(e.keyCode))
            }
            
            var count;
            if (length >= limit && e.keyCode != 8 && e.keyCode != 46) {
                e.preventDefault();
                count = 0;
            }
            else {
                if (e.keyCode == 8 || e.keyCode == 46) {
                    length -= 2;
                    if (length < 0)
                        length = 0;
                }
                count = limit - length;
                if (count < 0)
                    count = 0;
            }
        }

        if ($('#' + $(this).attr('datacharlimitfield')).length > 0)
            $('#' + $(this).attr('datacharlimitfield')).html(count);
    });

    $(this).bind('keyup', function (e) {
        if ($(this).attr('dataLimit') != undefined) {
            var limit = $(this).attr('dataLimit');
            var length = bms.getTextAreaLength($(this).val());

            var count = limit - length;
            if (count < 0)
                count = 0;

            if ($(this).attr('datacharlimitfield') != undefined)
                $('#' + $(this).attr('datacharlimitfield')).html(count);
        }
    });
};

$.fn.validateElement = function () {
    var form = $(this)

    if (form.prop('tagName') != 'FORM')
        form = $(this).parents('form');

    var validator = form.validate();

    var isValid = true;

    $(this).find('input').each(function () {
        if ($(this).attr('type') != undefined) {
            if ($(this).attr('type') != 'checkbox') {
                validator.element($(this));

                if ($(this).hasClass('input-validation-error'))
                    isValid = false;
            }
        }
    });

    $(this).find('select').each(function () {
        validator.element($(this));

        if ($(this).hasClass('input-validation-error'))
            isValid = false;
    });

    $(this).find('textarea').each(function () {
        validator.element($(this));

        if ($(this).hasClass('input-validation-error'))
            isValid = false;
    });

    return isValid;
};

(function ($) {
    //re-set all client validation given a jQuery selected form or child    
    $.fn.resetValidation = function () {
        var $form = this.closest('form');
        //reset jQuery Validate's internals        
        $form.validate().resetForm();
        //reset unobtrusive validation summary, if it exists        
        $form.find("[data-valmsg-summary=true]").removeClass("validation-summary-errors").addClass("validation-summary-valid").find("ul").empty();
        //reset unobtrusive field level, if it exists        
        $form.find("[data-valmsg-replace]").removeClass("field-validation-error").addClass("field-validation-valid").empty();
        return $form;
    };

    //reset a form given a jQuery selected form or a child    
    //by default validation is also reset    
    $.fn.formReset = function (resetValidation) {
        var $form = this.closest('form');
        $form[0].reset();
        if (resetValidation == undefined || resetValidation) {
            $form.resetValidation();
        }
        return $form;
    }
})(jQuery);