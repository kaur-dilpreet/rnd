$.fn.treeView = function (option) {
    if (option == 'selected-id') {
        if ($(this).find('.selected') != undefined)
            return $(this).find('.selected').parents('li').data('uniqueid');
        else
            return undefined;
    }

    if (option == 'selected') {
        return $(this).find('.selected');
    }

    if (option == 'moveup') {
        if ($(this).find('.selected') != undefined) {
            var selected = $(this).find('.selected').parents('li');
            selected.prev().before(selected);
        }
        return true;
    }

    if (option == 'movedown') {
        if ($(this).find('.selected') != undefined) {
            var selected = $(this).find('.selected').parents('li');
            selected.next().after(selected);
        }
        return true;
    }

    $(this).addClass('treeView');

    $(this).on('mouseenter', '.folder.userCanModify > div', function (e) {
        e.stopPropagation();

        if (!$(this).parents('.treeView').hasClass('disabled'))
            $(this).find('.actionsWrapper').show();
    });
    
    $(this).on('mouseleave', '.folder.userCanModify > div', function (e) {
        e.stopPropagation();

        $(this).find('.actionsWrapper').hide();
    });

    $(this).on('click', 'li > div > a', function (e) {
        e.preventDefault();

        if (!$(this).parents('.treeView').hasClass('disabled')) {
            if (!$(this).parents('.treeView').prop('loading')) {
                if ($(this).hasClass('isOpen')) {
                    $(this).parents('li').first().find('ul').remove();
                    $(this).removeClass('isOpen');
                    
                } else {
                    $(this).parents('li').first().treeNodeClick(e);
                }
            }
        }
    });

    $(this).on('click', 'li > div > span', function (e) {
        e.preventDefault();

        if (!$(this).parents('.treeView').hasClass('disabled')) {
            if (!$(this).hasClass('selected')) {
                if ($(this).parents('li').find('.actionsWrapper').length > 0)
                    $(this).parents('li').first().find('> div .actionsWrapper').show();

                if ($(this).parents('li').data('canusermodify') == '0')
                    $(this).parents('.treeView').siblings('.addButton').hide();
                else
                    $(this).parents('.treeView').siblings('.addButton').show();

                $(this).parents('.treeView').find('li span').removeClass('selected');
                $(this).parents('.treeView').find('li a').removeClass('selected');
                $(this).parents('.treeView').find('li div').removeClass('selected');

                $(this).addClass('selected');
                $(this).siblings('a').addClass('selected');
                $(this).parent().addClass('selected');

                if (option != undefined && option['onSelect'] != undefined)
                    option.onSelect(e, $(this).parents('li').data('uniqueid'));
            }
        }
    });

    $.fn.treeNodeClick = function (e) {
        e.preventDefault();
        e.stopPropagation();

        if (!$(this).parents('.treeView').prop('loading')) {
            $(this).find('> div > a').addClass('isOpen');

            $(this).parents('.treeView').prop('loading', true);

            var thisLi = $(this);
            if ($(this).find('ul').length > 0) {
                $(this).find('ul').remove();
            }

            var uniqueId = $(this).data('uniqueid');
            if (uniqueId == undefined)
                uniqueId = '';

            var url = $(this).parents('ul.treeView').data('geturl').replace('{0}', uniqueId);

            thisLi.addClass('loading');

            $.ajax({
                cache: false,
                url: url,
                success: function (data) {
                    bms.preprocessAjaxData(data, function (data) {
                        thisLi.removeClass('loading');

                        var liClass= 'folder';
                        //var isDroppable = false;
                        //var onDrop;

                        //if (thisLi.parents('.treeView').data('droppable') != undefined) {
                        //    liClass = liClass + ' ' + thisLi.parents('.treeView').data('droppable');
                        //    isDroppable = true;

                        //    onDrop = thisLi.parents('.treeView').data('droppable');
                        //}

                        $(data.Object.trim()).appendTo(thisLi);
                        thisLi.setAllSVG();
                        thisLi.setAllTooltips();
                        //if (data.List.length > 0) {
                        //    var ul = $('<ul />').appendTo(thisLi);
                            
                        //    for (var i = 0; i < data.List.length; i++) {
                                
                        //        var li = $('<li />', {
                        //            'class': liClass + ' ' + (data.List[i].CanUserModify ? 'userCanModify' : ''),
                        //            'data-uniqueid': data.List[i].UniqueId,
                        //            'data-canusermodify': data.List[i].CanUserModify ? '1' : '0',
                        //        }).appendTo(ul);

                        //        var actions = '';

                        //        if (data.List[i].CanUserModify && thisLi.parents('.treeView').hasClass('userCanModify'))
                        //            actions = '<div class="actionsWrapper"><div class="actions"><a id="editAssetFolder" href="#">e</a><a id="deleteAssetFolder" href="#">d</a></div></div>';

                        //        var div = $('<div />', {
                        //            class: data.List[i].CanAddAsset ? 'droppable' : '',
                        //            'html': $('<a class="folder" href="#"><img src="' + bms.baseUrl + 'Content/Images/Icons/folder-close.svg" class="folderClose svg" /><img src="' + bms.baseUrl + 'Content/Images/Icons/folder-open.svg" class="folderOpen svg" /></a><span>' + data.List[i].Name + '</span>' + actions),
                        //        }).appendTo(li);

                        //        if (option != undefined && option['onOpen'] != undefined)
                        //            option.onOpen(li.find('a'));
                        //    }

                        //    ul.find('img.svg').each(function () {
                        //        $(this).setSVG();
                        //    });

                        //} else {
                        //}
                    });

                    thisLi.parents('.treeView').removeProp('loading');
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    bms.processAjaxError(jqXHR, textStatus, errorThrown, function (data) {
                        thisLi.parents('.treeView').removeProp('loading');
                        thisLi.removeClass('loading');
                    });
                }
            });
        }
    };

    $(this).show();
};