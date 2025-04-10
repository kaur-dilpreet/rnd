$(document).ready(function () {
    $('#requestAccess').click(function (e) {
        e.preventDefault();

        if (!$('#accessReports').is(':checked') && !$('#accessMART').is(':checked') && !$('#accessAskBI').is(':checked') && !$('#accessCMOChat').is(':checked') && !$('#accessSDRAI').is(':checked') && !$('#accessChatGPI').is(':checked')) {
            bms.showErrorMessage('Please select at least one report.');
            return false;
        }

        bms.showMainLoader();

        $.ajax({
            type: 'POST',
            cache: false,
            data: { accessReports: $('#accessReports').is(':checked'), accessMART: $('#accessMART').is(':checked'), accessAskBI: $('#accessAskBI').is(':checked'), accessCMOChat: $('#accessCMOChat').is(':checked'), accessSDRAI: $('#accessSDRAI').is(':checked'), accessChatGPI: $('#accessChatGPI').is(':checked')  },
            url: bms.baseUrl + 'requestaccess/submit',
            success: function (data) {
                bms.preprocessAjaxData(data, function (data) {
                    if (data.Message != '') {
                        bms.showTempMessage(data.Message, 10);

                        setTimeout(function () {
                            window.location = bms.baseUrl;
                        }, 5000)
                    }
                    else {
                        bms.showTempMessage("Your request has been submitted successfully.", 6);

                        setTimeout(function () {
                            bms.refreshPage();
                        }, 5000)
                    }

                    bms.hideMainLoader();
                    $('#clearMask').show();
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bms.processAjaxError(jqXHR, textStatus, errorThrown);
            }
        });
    });
});

bms.frmUpdateRequestComplete = function (ajaxContext, formId) {
    var response = ajaxContext.responseText;
    var statusCode = ajaxContext.status;

    bms.formSubmitComplete(formId, response, statusCode, function (data) {
        window.location = '/requestaccess/thankyou';
    });
};