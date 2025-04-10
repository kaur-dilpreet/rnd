$(document).ready(function () {
    $(document).on('click', '.rbBarChart .rbBarChartFooter .rbBarChartPrev', function (e) {
        e.preventDefault();

        var currentPage = $(this).parents('.rbBarChart').find('.rbBarChartBody .rbBarChartRow.active').first().data('page');

        if (currentPage > 1) {
            $(this).parents('.rbBarChart').find('.rbBarChartBody .rbBarChartRow.active').removeClass('active');
            $(this).parents('.rbBarChart').find('.rbBarChartBody .rbBarChartRow[data-page=' + (currentPage - 1) + ']').addClass('active');
            $(this).siblings('.rbBarChartNext').removeClass('rbBarChartPageDisabled');

            if (currentPage - 1 == 1)
                $(this).addClass('rbBarChartPageDisabled');
        }
    });

    $(document).on('click', '.rbBarChart .rbBarChartFooter .rbBarChartNext', function (e) {
        e.preventDefault();

        var currentPage = $(this).parents('.rbBarChart').find('.rbBarChartBody .rbBarChartRow.active').first().data('page');
        var pageCount = $(this).parents('.rbBarChart').find('.rbBarChartBody').data('pagecount');

        if (currentPage < pageCount) {
            $(this).parents('.rbBarChart').find('.rbBarChartBody .rbBarChartRow.active').removeClass('active');
            $(this).parents('.rbBarChart').find('.rbBarChartBody .rbBarChartRow[data-page=' + (currentPage + 1) + ']').addClass('active');
            $(this).siblings('.rbBarChartPrev').removeClass('rbBarChartPageDisabled');

            if (currentPage + 1 == pageCount)
                $(this).addClass('rbBarChartPageDisabled');
        }
    });
});