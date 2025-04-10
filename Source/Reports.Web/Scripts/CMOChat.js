$(document).ready(function () {
    $('#Question').keydown(function (e) {
        if (e.keyCode == 40) // arrow down
        {
            if ($('.questionListWrapper ul li').length > 0) {
                e.preventDefault();
                if ($('.questionListWrapper ul li.selected').length > 0) {
                    if ($('.questionListWrapper ul li.selected').next('li').length > 0) {
                        var next = $('.questionListWrapper ul li.selected').next('li');

                        $('.questionListWrapper ul li.selected').removeClass('selected');
                        next.addClass('selected');
                    } else {
                        var next = $('.questionListWrapper ul li').first('li');

                        $('.questionListWrapper ul li.selected').removeClass('selected');
                        next.addClass('selected');
                    }
                }
                else {
                    $('.questionListWrapper ul li').first().addClass('selected');
                }
            }
        }
        else if (e.keyCode == 38) // arrow up
        {
            if ($('.questionListWrapper ul li').length > 0) {
                e.preventDefault();
                if ($('.questionListWrapper ul li.selected').length > 0) {
                    if ($('.questionListWrapper ul li.selected').prev('li').length > 0) {
                        var next = $('.questionListWrapper ul li.selected').prev('li');

                        $('.questionListWrapper ul li.selected').removeClass('selected');
                        next.addClass('selected');
                    } else {
                        var next = $('.questionListWrapper ul li').last('li');

                        $('.questionListWrapper ul li.selected').removeClass('selected');
                        next.addClass('selected');
                    }
                }
                else {
                    $('.questionListWrapper ul li').last().addClass('selected');
                }
            }
        }
        else if (e.keyCode == 13) {
            if ($('.questionListWrapper ul li.selected').length > 0) {
                e.preventDefault();

                $('.questionListWrapper ul li.selected').trigger('mousedown');
            }
        }
    });

    $('#cmoChatPage').on('click', '.suggestion', function (e) {
        e.preventDefault();

        $('#cmoChatDialog #Question').val($(this).text().trim());
        $('.questionListWrapper').remove();

        bms.showMainLoader();

        $.ajax({
            type: 'POST',
            cache: false,
            data: { question: $(this).text().trim() },
            url: bms.baseUrl + 'cmoChat/asksuggestedquestion',
            success: function (data) {
                bms.preprocessAjaxData(data, function (data) {
                    bms.hideMainLoader();

                    $('#cmoChatDialog #Question').val('');
                    $('.questionListWrapper').remove();

                    $(data.Object.trim()).prependTo($('#cmoChatPage .historyWrapper'));
                    $('#cmoChatPage .suggestions').replaceWith(data.Message.trim());
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bms.processAjaxError(jqXHR, textStatus, errorThrown);
            }
        });
    });

    $('#Question').blur(function () {
        $('.questionListWrapper').remove();
    });

    $('#Question').bind('input', function () {
        var question = $(this).val();

        if (question.length >= 3) {
            if (bms.listQuestionAjaxCall != undefined)
                bms.listQuestionAjaxCall.abort();

            var self = $(this);

            bms.listQuestionAjaxCall = $.ajax({
                type: 'GET',
                cache: false,
                data: { question: question },
                url: bms.baseUrl + 'cmochat/listquestions',
                success: function (data) {
                    bms.preprocessAjaxData(data, function (data) {
                        bms.listQuestionAjaxCall = undefined;

                        if (data.List.length > 0) {
                            var questionListWrapper = $('.questionListWrapper');

                            if (questionListWrapper.length == 0) {
                                questionListWrapper = $('<div />', {
                                    class: 'questionListWrapper'
                                }).insertAfter(self);

                                $('<ul />').appendTo(questionListWrapper);
                            }

                            var ul = questionListWrapper.find('ul');

                            ul.html('');

                            for (var i = 0; i < data.List.length; i++) {
                                $('<li />', {
                                    html: data.List[i]
                                }).mousedown(function (e) {
                                    e.preventDefault();

                                    bms.showMainLoader();
                                    $('#cmoChatDialog #Question').val($(this).text().trim());
                                    $('.questionListWrapper').remove();

                                    $.ajax({
                                        type: 'POST',
                                        cache: false,
                                        data: { question: $(this).text().trim() },
                                        url: bms.baseUrl + 'cmoChat/askpredefinedquestion',
                                        success: function (data) {
                                            bms.preprocessAjaxData(data, function (data) {
                                                bms.hideMainLoader();

                                                $('#cmoChatDialog #Question').val('');
                                                $('.questionListWrapper').remove();

                                                $(data.Object.trim()).prependTo($('#cmoChatPage .historyWrapper'));
                                                $('#cmoChatPage .suggestions').replaceWith(data.Message.trim());
                                            });
                                        },
                                        error: function (jqXHR, textStatus, errorThrown) {
                                            bms.processAjaxError(jqXHR, textStatus, errorThrown);
                                        }
                                    });

                                }).appendTo(ul);
                            }
                        }
                        else {
                            $('.questionListWrapper').remove();
                        }
                    });
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    bms.listQuestionAjaxCall = undefined;

                    if (textStatus != 'abort')
                        bms.processAjaxError(jqXHR, textStatus, errorThrown);
                }
            });
        }
        else {
            $('.questionListWrapper').remove();
        }
    });

    $('#showSQLQueries').change(function () {
        if ($(this).is(':checked')) {
            $(this).parents('.history').addClass('showSQLQueries');
        }
        else {
            $(this).parents('.history').removeClass('showSQLQueries');
        }
    });

    $('#showGraphs').change(function () {
        if (this.checked)
            $('.history').addClass('showCharts')
        else
            $('.history').removeClass('showCharts')
    });

    $('#cmoChatPage').on('change', '.history input[type=radio]', function (e) {
        e.preventDefault();

        var self = $(this);

        bms.showMainLoader();

        $.ajax({
            type: 'GET',
            cache: false,
            data: { uniqueId: self.parents('.questionContainer').data('uniqueid'), isCorrect: $(this).val() },
            url: bms.baseUrl + 'cmoChat/questionfeedback',
            success: function (data) {
                bms.preprocessAjaxData(data, function (data) {
                    if (self.val() == "false")
                        self.parents('.questionContainer').addClass('notCorrect');
                    else
                        self.parents('.questionContainer').removeClass('notCorrect');

                    bms.hideMainLoader();
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bms.processAjaxError(jqXHR, textStatus, errorThrown);
            }
        });
    });

    $('#cmoChatPage #loadMore').click(function (e) {
        e.preventDefault();

        var self = $(this);

        bms.showMainLoader();

        $.ajax({
            type: 'GET',
            cache: false,
            data: { skip: $('.questionContainer').length },
            url: bms.baseUrl + 'cmoChat/loadmore',
            success: function (data) {
                bms.preprocessAjaxData(data, function (data) {
                    if (data.Object.trim() != '') {
                        $(data.Object.trim()).appendTo($('#cmoChatPage .historyWrapper'))
                    }
                    else
                        $('#cmoChatPage #loadMore').hide();

                    bms.hideMainLoader();
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bms.processAjaxError(jqXHR, textStatus, errorThrown);
            }
        });
    });

    $('#cmoChatPage').on('click', '.tableActions .exportToCSV', function (e) {
        e.preventDefault();

        let csv_data = [];
        let table = $(this).parents('.answer').find('table').get(0);

        if ($(table).length > 0) {
            // Get each row data
            let rows = table.getElementsByTagName('tr');
            for (let i = 0; i < rows.length; i++) {

                // Get each column data
                let cols = rows[i].querySelectorAll('td,th');

                // Stores each csv row data
                let csvrow = [];
                for (let j = 0; j < cols.length; j++) {

                    // Get the text data of each cell of
                    // a row and push it to csvrow
                    csvrow.push($(cols[j]).text().replace(',', ''));
                }

                // Combine each column value with comma
                csv_data.push(csvrow.join(","));
            }
            // Combine each row data with new line character
            csv_data = csv_data.join('\n');

            CSVFile = new Blob([csv_data], { type: "text/csv" });

            // Create to temporary link to initiate
            // download process
            let temp_link = document.createElement('a');

            // Download csv file
            temp_link.download = "table.csv";
            let url = window.URL.createObjectURL(CSVFile);
            temp_link.href = url;

            // This link should not be displayed
            temp_link.style.display = "none";
            document.body.appendChild(temp_link);

            // Automatically click the link to trigger download 
            temp_link.click();
            document.body.removeChild(temp_link);
        }
    });

    $('#cmoChatPage').on('click', '.createTicket', function (e) {
        e.preventDefault();

        var self = $(this);

        bms.showMainLoader();

        $.ajax({
            type: 'GET',
            cache: false,
            data: { questionId: self.parents('.questionContainer').data('uniqueid') },
            url: bms.baseUrl + 'cmochat/createticket',
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
});


bms.frmCMOChatComplete = function (ajaxContext, formId) {
    var response = ajaxContext.responseText;
    var statusCode = ajaxContext.status;

    bms.formSubmitComplete(formId, response, statusCode, function (data) {
        $('#cmoChatDialog #Question').val('');

        $(data.Object.trim()).prependTo($('#cmoChatPage .historyWrapper'));
        $('#cmoChatPage .suggestions').replaceWith(data.Message.trim());
    });
};

bms.frmCreateTicketComplete = function (ajaxContext, formId) {
    var response = ajaxContext.responseText;
    var statusCode = ajaxContext.status;

    bms.formSubmitComplete(formId, response, statusCode, function (data) {
        $('#createTicketDialog').hideModal();

        $('.questionContainer[data-uniqueid=' + data.UniqueId + '] .createTicketContainer').remove();

        bms.showSuccessMessage('A ticket has been created.');
    });
};