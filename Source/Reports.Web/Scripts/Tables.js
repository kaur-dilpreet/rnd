$.fn.tableWithDetails = function () {
    if (!$(this).hasClass('tableWithDetailsActivated')) {
        $(this).addClass('tableWithDetailsActivated');
        $(this).find('> tbody > tr.master').each(function () {

            $(this).click(function (e) {
                if ($(this).parents('table').hasClass('selectable'))
                    return true;

                if (e.target.nodeName == 'A')
                    return;

                e.preventDefault();

                var self = $(this);

                if (self.hasClass('active')) {
                    if (self.next().next().length == 0) {
                        self.find('td').css('border-bottom-Style', 'none');
                    }
                    self.next().find('td > div').slideUp(300, function () {
                        $(this).parents('tr').prev().removeClass('active');
                    });
                }
                else {
                    if (self.parents('table').hasClass('hasDetailsNoRefresh')) {
                        self.showDetails();
                    }
                    else
                        self.getTableDetails();
                }
            });

        });
    }
};

$.fn.getTableDetails = function () {
    bms.showMainLoader();

    var self = $(this);

    var id = self.data('uniqueid')

    if (id == undefined)
        id = self.data('id');

    $.ajax({
        type: 'GET',
        cache: false,
        data: { 'id': id },
        url: self.parents('table').data('details'),
        success: function (data) {
            bms.preprocessAjaxData(data, function (data) {
                bms.hideMainLoader();

                self.showDetails(data);
            });
        },
        error: function (jqXHR, textStatus, errorThrown) {
            bms.processAjaxError(jqXHR, textStatus, errorThrown);
        }
    });
}

$.fn.showDetails = function (data) {
    self = $(this);

    self.parents('table').find('tr.active').each(function () {
        if (self.data('id') != $(this).data('id')) {
            if ($(this).next().next().length == 0) {
                $(this).find('td').css('border-bottom-Style', 'none');
            }
            $(this).next().find('td > div').slideUp(300, function () {
                $(this).parents('tr').prev().removeClass('active');
            });
        }
    });

    if (data != undefined)
        if (self.next().find('td').html() != '')
            self.next().find('td').html('');

    data = '<div>' + data + '</div>';

    self.next().find('td').html(data);

    if (!self.hasClass('active')) {
        self.next().find('td').first().find('> div').first().hide();

        if (self.next().next().length == 0) {
            self.find('td').css('border-bottom-Style', 'solid');
            self.find('td').css('border-bottom-size', '1px');
            self.find('td').css('border-bottom-color', self.parents('table').css('border-bottom-color'));
        }

        //self.next().find('td').find('td.actions .rbcontextmenu').each(function () {
        //    $(this).hide();
        //});

        self.next().find('td').first().find('> div').first().slideDown(300, function () {
            $(this).css({ 'overflow': 'visible' });
        }); //self.next().find('td > div').slideDown(300);
        self.addClass('active');
    }
}