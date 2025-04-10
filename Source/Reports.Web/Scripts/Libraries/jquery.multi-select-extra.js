$.fn.rbmultiselect = function (selectableHeaderText, selectionHeaderText, selectableFooterText, selectionFooterText) {
    if ($(this) == undefined)
        return false;

    if (selectableFooterText == undefined || selectableFooterText == '')
        selectableFooterText = 'Click to select...';

    if (selectionFooterText == undefined || selectionFooterText == '')
        selectionFooterText = 'Click to deselect...';

    var selectableOptgroup = $(this).find('optgroup').length > 0;

    $(this).multiSelect({
        selectableOptgroup: selectableOptgroup,
        keepOrder: true,
        selectableHeader: "<div class=\"headerText\">" + selectableHeaderText + "</div>",
        selectionHeader: "<div class=\"headerText\">" + selectionHeaderText + "</div>",
        selectableFooter: "<div class=\"footerText\">" + selectableFooterText + "</div>",
        selectionFooter: "<div class=\"footerText\">" + selectionFooterText + "</div>",
        afterInit: function (ms) {
            var that = this,
                $selectableSearch = that.$selectableUl.prev(),
                $selectionSearch = that.$selectionUl.prev(),
                selectableSearchString = '#' + that.$container.attr('id') + ' .ms-elem-selectable:not(.ms-selected)',
                selectionSearchString = '#' + that.$container.attr('id') + ' .ms-elem-selection.ms-selected';

            //that.qs1 = $selectableSearch.quicksearch(selectableSearchString)
            //.on('keydown', function (e) {
            //    if (e.which === 40) {
            //        that.$selectableUl.focus();
            //        return false;
            //    }
            //});

            //that.qs2 = $selectionSearch.quicksearch(selectionSearchString)
            //.on('keydown', function (e) {
            //    if (e.which == 40) {
            //        that.$selectionUl.focus();
            //        return false;
            //    }
            //});
        },
        afterSelect: function () {
            this.qs1.cache();
            this.qs2.cache();
        },
        afterDeselect: function () {
            this.qs1.cache();
            this.qs2.cache();
        }
    });
};