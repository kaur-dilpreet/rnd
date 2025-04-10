$(document).ready(function () {
    $(document).on('click', '.rbTabs .rbTabButton', function (e) {
		e.preventDefault();

		var pageNumber = '';
		var shouldPushBrowserHistory = false;

		var targetTab = $($(this).data('target'));

		if (!targetTab.is(':visible'))
		{
			$('.rbTabDetails .rbTab').removeClass('activeTab');

			targetTab.addClass('activeTab');

			var self = $(this);

			if ($(this).hasClass('rbTabRefresh')) {
			    var refreshUrl = $(this).data('refreshurl');
			    var refreshContainerString = $(this).data('refreshcontainer') != undefined ? $(this).data('refreshcontainer') : '';
			    var refreshContainer = $(this).data('target') + ' ' + refreshContainerString;

			    if ($(this).hasClass('rbTabRefreshForce'))
			    {
			        $(refreshContainer).html('');
			    }

			    if ($(refreshContainer).html().trim() == '') {
			        shouldPushBrowserHistory = true;

			        if ($(refreshContainer).data('haspagination'))
			            pageNumber = 1;

			        if ($(self.data('target')).find('.rbRefresh').length > 0)
			            $(self.data('target')).find('.rbRefresh img').addClass('loading');
			        else
			            bms.showMainLoader();

			        var filterData = '';

			        if (bms.filterData != undefined)
			            filterData = bms.filterData;

			        $.ajax({
			            type: 'GET',
			            cache: false,
			            data: filterData,
			            url: bms.baseUrl + refreshUrl,
			            success: function (data) {
			                bms.preprocessAjaxData(data, function (data) {
			                    $(refreshContainer).html(data.Object);

			                    //if ($(refreshContainer).find('table.hasDetails').length > 0)
			                    //    $(refreshContainer).find('table.hasDetails').tableWithDetails();

			                    //if ($(refreshContainer).find('form').length > 0)
			                    //    $(refreshContainer).prepareForm();

			                    //if ($(refreshContainer).find('.blocks').length > 0) {
			                    //    $(refreshContainer).find('.blocks').each(function () {
			                    //        $(this).blocks();
			                    //    });
			                    //}

                                if ($(refreshContainer).find('table.hasDetails').length > 0) {
                                    $(refreshContainer).find('table.hasDetails').each(function () {
                                        $(this).tableWithDetails();
                                    });
                                }

			                    if ($(refreshContainer).data('draggable') != undefined) {
			                        $(refreshContainer).find($(refreshContainer).data('draggable')).each(function () {
			                            $(this).draggable({
			                                revert: "true", opacity: 0.7, helper: "clone",
			                                beforeStart: function () {
			                                    var onDragStart = $(refreshContainer).data('ondragstart');

			                                    if (onDragStart != undefined && ov[onDragStart] != undefined)
			                                        ov[onDragStart]($(this));
			                                }
			                            });
			                        });
			                    }

			                    //if ($(refreshContainer).data('droppable') != undefined) {
			                    //    var onDrop = $(refreshContainer).data('ondrop');

			                    //    $($(refreshContainer).data('droppable')).each(function () {
			                            
			                    //        if (!$(this).hasClass('ui-droppable')) {
			                    //            $(this).droppable();
			                    //            $(this).droppable({
			                    //                drop: function (event, ui) {
			                    //                    if (onDrop != undefined && ov[onDrop] != undefined)
			                    //                        ov[onDrop](event, ui, $(this));
			                    //                }
			                    //            });
			                    //        }
			                    //    });
			                    //}
			                });

			                //if ($(self.data('target')).find('.rbcarousel').length > 0) {
			                //    $(self.data('target')).find('.rbcarousel').each(function () {
			                //        $(this).rbcarousel();
			                //    });
			                //}

			                if ($(self.data('target')).find('.rbRefresh').length > 0)
			                    $(self.data('target')).find('.rbRefresh img').removeClass('loading');
			                else
			                    bms.hideMainLoader();
			            },
			            error: function (jqXHR, textStatus, errorThrown) {
			                bms.processAjaxError(jqXHR, textStatus, errorThrown, function (data) {
			                    self.parents('.rbTabs').find('.rbTabButton').removeClass('activeTabButton');
			                    self.addClass('activeTabButton');

			                    if ($(self.data('target')).find('.rbRefresh').length > 0)
			                        $(self.data('target')).find('.rbRefresh img').removeClass('loading');

			                });
			            }
			        });
			    }
			    else {
			        if ($(refreshContainer).data('haspagination')) {
			            if ($(refreshContainer).find('.pagination').length > 0)
			                pageNumber = $(refreshContainer).find('.pagination').data('currentpage');
			            else
			                pageNumber = 1;
			        }

			        shouldPushBrowserHistory = true;
			    }
			}
			else
			{
			    shouldPushBrowserHistory = true;
			}
			
			
			self.parents('.rbTabs').find('.rbTabButton').removeClass('activeTabButton');

			self.addClass('activeTabButton');
		    
			if (self.hasClass('dontPushBrowserHistory'))
			    shouldPushBrowserHistory = false;

			//if (shouldPushBrowserHistory) 
            //    bms.pushBrowserHistory(e, pageNumber);
		}
	});
});

$.fn.openTab = function (target) {
    $(this).find('.rbTabButton').removeClass('activeTabButton');
    $(this).find('.rbTabButtons  .rbTabButton[data-target=' + target + ']').addClass('activeTabButton');

    $(this).find('.rbTabDetails .rbTab').removeClass('activeTab');

    $(target).addClass('activeTab');
};