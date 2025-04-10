$(document).ready(function () {
    //$('#requestsPage').on('click', '.approve', function (e) {
    //    e.preventDefault();

    //    bms.showMainLoader();

    //    var self = $(this);

    //    $.ajax({
    //        type: 'GET',
    //        cache: false,
    //        data: {uniqueId: $(this).parents('tr').data('uniqueid') },
    //        url: bms.baseUrl + 'requestaccess/approve',
    //        success: function (data) {
    //            bms.preprocessAjaxData(data, function (data) {
    //                self.parents('tr').replaceWith($(data.Object.trim()));

    //                bms.hideMainLoader();
    //            });
    //        },
    //        error: function (jqXHR, textStatus, errorThrown) {
    //            bms.processAjaxError(jqXHR, textStatus, errorThrown);
    //        }
    //    });
    //});

    $('#requestsPage').on('click', '.changeAccessLevel', function (e) {
        e.preventDefault();

        bms.showMainLoader();

        var self = $(this);

        $.ajax({
            type: 'GET',
            cache: false,
            data: { uniqueId: $(this).parents('tr').data('uniqueid') },
            url: bms.baseUrl + 'requestaccess/changeaccesslevel',
            success: function (data) {
                bms.preprocessAjaxData(data, function (data) {
                    var bmsDialog = $('<div/>', { html: data });
                    bmsDialog.showModal();
                    bmsDialog.prepareForm();
                    bms.hideMainLoader();
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bms.processAjaxError(jqXHR, textStatus, errorThrown);
            }
        });
    });

    $('#requestsPage').on('click', '.deny', function (e) {
        e.preventDefault();

        bms.showMainLoader();

        var self = $(this);

        $.ajax({
            type: 'GET',
            cache: false,
            data: { uniqueId: $(this).parents('tr').data('uniqueid') },
            url: bms.baseUrl + 'requestaccess/deny',
            success: function (data) {
                bms.preprocessAjaxData(data, function (data) {
                    var bmsDialog = $('<div/>', { html: data });
                    bmsDialog.showModal();
                    bmsDialog.prepareForm();
                    bms.hideMainLoader();
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bms.processAjaxError(jqXHR, textStatus, errorThrown);
            }
        });
    });

    $('#requestsPage').on('click', '.addExportAll', function (e) {
        e.preventDefault();

        bms.showMainLoader();

        var self = $(this);

        $.ajax({
            type: 'GET',
            cache: false,
            data: { uniqueId: $(this).parents('tr').data('uniqueid'), canExportAll: true },
            url: bms.baseUrl + 'requestaccess/setcanexportallpermission',
            success: function (data) {
                bms.preprocessAjaxData(data, function (data) {
                    self.parents('tr').replaceWith($(data.Object.trim()));

                    bms.hideMainLoader();
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bms.processAjaxError(jqXHR, textStatus, errorThrown);
            }
        });
    });

    $('#requestsPage').on('click', '.removeExportAll', function (e) {
        e.preventDefault();

        bms.showMainLoader();

        var self = $(this);

        $.ajax({
            type: 'GET',
            cache: false,
            data: { uniqueId: $(this).parents('tr').data('uniqueid'), canExportAll: false },
            url: bms.baseUrl + 'requestaccess/setcanexportallpermission',
            success: function (data) {
                bms.preprocessAjaxData(data, function (data) {
                    self.parents('tr').replaceWith($(data.Object.trim()));

                    bms.hideMainLoader();
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bms.processAjaxError(jqXHR, textStatus, errorThrown);
            }
        });
    });
});

bms.frmChangeAccessLevelComplete = function (ajaxContext, formId) {
    var response = ajaxContext.responseText;
    var statusCode = ajaxContext.status;

    bms.formSubmitComplete(formId, response, statusCode, function (data) {
        $('#changeAccessLevelDialog').hideModal();

        $('#requestsPage #requestsTableWrapper table#requestsTable tr[data-uniqueid=' + data.UniqueId + ']').replaceWith($(data.Object.trim()));
    });
};

bms.frmDenyAccessRequestComplete = function (ajaxContext, formId) {
    var response = ajaxContext.responseText;
    var statusCode = ajaxContext.status;

    bms.formSubmitComplete(formId, response, statusCode, function (data) {
        $('#denyAccessRequestDialog').hideModal();

        $('#requestsPage #requestsTableWrapper table#requestsTable tr[data-uniqueid=' + data.UniqueId + ']').replaceWith($(data.Object.trim()));
    });
};