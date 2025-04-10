$(document).ready(function () {
    $(document).on('click', '.pagination .gotoPage a#gotoPageNumber', function (e) {
        e.preventDefault();

        var pageNumber = parseInt($(this).parents('.gotoPage').find('#pageNumber').val());

        bms.pushBrowserHistory(e, pageNumber);

        $(this).parents('.pagination').paginationGoToPage(pageNumber);
    });

    $(document).on('keydown', '.pagination .gotoPage #pageNumber', function (e) {
        if (e.keyCode == 13) {
            var pageNumber = parseInt($(this).val());

            if (pageNumber > 0)
                $(this).parents('.gotoPage').find('a#gotoPageNumber').click();
        }
    });

    $(document).on('click', '.pagination ul li a', function (e) {
        e.preventDefault();

        var url = $(this).attr('href');
        var pageNumber = url.substring(url.lastIndexOf('/') + 1);

        bms.pushBrowserHistory(e, pageNumber);
        bms.loadContent(url, $(this).parents('.pagination').data('target'));
    });
});

$.fn.paginationGoToPage = function(pageNumber)
{
    var lastPageNumber = $(this).data('lastpagenumber');

    if (pageNumber < 1)
        pageNumber == 1;

    if (pageNumber > lastPageNumber)
        pageNumber = lastPageNumber;

    bms.loadContent($(this).find('#gotoPageNumber').attr('href') + pageNumber, $(this).find('#gotoPageNumber').parents('.pagination').data('target'));
}

bms.loadContent = function (refreshUrl, refreshContainer) {
    bms.showMainLoader();

    var pageNumber = refreshUrl.substring(refreshUrl.lastIndexOf('/') + 1);
    
    var filterData = '';

    if (bms.filterData != undefined)
        filterData = bms.filterData;

    $.ajax({
        type: 'GET',
        cache: false,
        data: filterData,
        url: refreshUrl,
        success: function (data) {
            bms.preprocessAjaxData(data, function (data) {
                $(refreshContainer).html(data.Object);
                if ($(refreshContainer).find('table.hasDetails').length > 0)
                    $(refreshContainer).find('table.hasDetails').tableWithDetails();

                bms.hideMainLoader();
            });
        },
        error: function (jqXHR, textStatus, errorThrown) {
            bms.processAjaxError(jqXHR, textStatus, errorThrown);
        }
    });
}