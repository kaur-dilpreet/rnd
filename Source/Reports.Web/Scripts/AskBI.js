$(document).ready(function () {
    $('.scrollbar-outer').scrollbar();

    $('.histories').accordion({
        collapsible: true,
        heightStyle: 'content'
    });

    $('#showSQLQueries').change(function () {
        if ($(this).is(':checked')) {
            $(this).parents('.answers').addClass('showSQLQueries');
        }
        else {
            $(this).parents('.answers').removeClass('showSQLQueries');
        }
    });

    $('#askBIPage').on('change', '.answers input[type=radio]', function (e) {
        e.preventDefault();

        var self = $(this);

        bms.showMainLoader();

        $.ajax({
            type: 'GET',
            cache: false,
            data: { uniqueId: self.parents('.questionContainer').data('uniqueid'), isCorrect: $(this).val() },
            url: bms.baseUrl + 'askbi/questionfeedback',
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

    $('#askBIPage').on('click', '.histories .question', function (e) {
        e.preventDefault();

        $('#Question').val($(this).text().trim());
        $('#Category').val($(this).data('category'));
        $('#askBIDialog form').submit();
    });

    //$('#askBIPage #loadMore').click(function (e) {
    //    e.preventDefault();

    //    var self = $(this);

    //    bms.showMainLoader();

    //    $.ajax({
    //        type: 'GET',
    //        cache: false,
    //        data: { skip: $('.questionContainer').length },
    //        url: bms.baseUrl + 'askbi/loadmore',
    //        success: function (data) {
    //            bms.preprocessAjaxData(data, function (data) {
    //                if (data.Object.trim() != '') {
    //                    $(data.Object.trim()).appendTo($('#askBIPage .historyWrapper.scroll-content .histories'))
    //                }
    //                else
    //                    $('#askBIPage #loadMore').hide();

    //                bms.hideMainLoader();
    //            });
    //        },
    //        error: function (jqXHR, textStatus, errorThrown) {
    //            bms.processAjaxError(jqXHR, textStatus, errorThrown);
    //        }
    //    });
    //});

    $('#askBIPage').on('click', '.tableActions .exportToCSV', function (e) {
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

    $('#askBIPage').on('click', '.createTicket', function (e) {
        e.preventDefault();

        var self = $(this);

        bms.showMainLoader();

        $.ajax({
            type: 'GET',
            cache: false,
            data: { questionId: self.parents('.questionContainer').data('uniqueid') },
            url: bms.baseUrl + 'askbi/createticket',
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

    //$('#askBIPage').on('click', '.refreshAnswer', function (e) {
    //    e.preventDefault();

    //    var self = $(this);

    //    self.parents('.questionContainer').addClass('refreshing');
    //    self.parents('.questionContainer').find('.answerWrapper').html('').addClass('animatedBackground');

    //    $.ajax({
    //        type: 'POST',
    //        cache: false,
    //        data: { SessionId: $('#askBIDialog #SessionId').val(), Question: self.parents('.questionContainer').find('.question .questionWrapper').text().trim(), Category: self.parents('.questionContainer').data('category') },
    //        url: bms.baseUrl + 'askbi/askquestion',
    //        success: function (data) {
    //            bms.preprocessAjaxData(data, function (data) {
    //                self.parents('.questionContainer').replaceWith($(data.Object.trim()));
    //                self.parents('.questionContainer').removeClass('refreshing');

    //                bms.hideMainLoader();
    //            });
    //        },
    //        error: function (jqXHR, textStatus, errorThrown) {
    //            self.parents('.questionContainer').find('.answerWrapper').removeClass('animatedBackground');
    //            bms.processAjaxError(jqXHR, textStatus, errorThrown);
    //        }
    //    });
    //});

    setInterval(function () {
        $('.questionContainer .answer.inProgress').each(function () {
            var self = $(this);

            $.ajax({
                type: 'GET',
                cache: false,
                data: { id: $(this).parents('.questionContainer').data('uniqueid') },
                url: bms.baseUrl + 'askbi/checkquestion',
                success: function (data) {
                    bms.preprocessAjaxData(data, function (data) {
                        var answer = $(data.Object.trim());

                        if (!answer.find('answer').hasClass('inProgress')) {
                            self.parents('.questionContainer').replaceWith(answer);
                            self.parents('.questionContainer').removeClass('refreshing');
                        }

                        bms.hideMainLoader();
                    });
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    self.parents('.questionContainer').find('.answerWrapper').removeClass('animatedBackground');
                    bms.processAjaxError(jqXHR, textStatus, errorThrown);
                }
            });

        });
    }, 3000)
});


bms.frmAskBIComplete = function (ajaxContext, formId) {
    var response = ajaxContext.responseText;
    var statusCode = ajaxContext.status;

    bms.formSubmitComplete(formId, response, statusCode, function (data) {
        var questionText = $('#askBIDialog #Question').val();
        var category = $('#askBIDialog #Category').val();

        var question = $(data.Object.trim());

        var questionExists = false;

        $('#askBIPage .histories .question').each(function () {
            if ($(this).text().trim().toLowerCase() == question.find('.questionWrapper').text().trim().toLowerCase() && $(this).data('category') == question.data('category'))
                questionExists = true;
        });

        if (!questionExists) {
            var questionLi = $('<li />', {
                class: 'question',
                'data-category': category,
                html: '<div class="questionText">' + questionText + '</div>'
            });

            var ul = $('.histories').find('ul[data-category="' + category + '"]');

            if (ul.length == 0) {
                var h3 = $('<h3 />', {
                    html: category
                }).prependTo('.histories');

                ul = $('<ul />', {
                    'data-category': category
                }).insertAfter(h3);
            }

            questionLi.prependTo(ul);

            $('.histories').accordion("refresh");
        }

        $('#askBIDialog #Question').val('');

        $(data.Object.trim()).prependTo($('#askBIPage .answersWrapper.scroll-content'));
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