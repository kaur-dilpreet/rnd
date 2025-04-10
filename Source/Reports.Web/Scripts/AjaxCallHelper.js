bms.preprocessAjaxData = function (data, onSuccess, onError) {
    if (data.Result == undefined) {
        if (onSuccess != undefined) {
            onSuccess(data);
        }
    }
    else {
        if (!data.Result) {
            if (data.ErrorCode != undefined && data.ErrorCode > 0) {
                bms.hideMainLoader();

                if (data.ErrorCode == 401) {
                    bms.showTempMessage(data.Message, 2, function () {
                        window.location = bms.baseUrl + "account/login";
                    });
                }
                else {
                    if (onError == undefined)
                        bms.showErrorMessage(data.Message);
                }
                return false;
            }
            else {
                bms.hideMainLoader();

                if (onError == undefined) {
                    bms.showErrorMessage(data.Message);
                }
            }

            if (onError != undefined) {
                onError(data);
            }
        }
        else
        {
            if (onSuccess != undefined) {
                onSuccess(data);
            }
        }
    }
}

bms.processAjaxError = function (jqXHR, textStatus, errorThrown, onDone) {
    bms.showGeneralError();
    bms.hideMainLoader();

    if (onDone != undefined) {
        onDone(data);
    }
};