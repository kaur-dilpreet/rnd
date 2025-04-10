$(window).load(function () {
    if (bms.inIframe())
        window.parent.postMessage('url?', '*');
});

$(document).ready(function () {
    var eventMethod = window.addEventListener ? "addEventListener" : "attachEvent";
    var eventer = window[eventMethod];
    var messageEvent = eventMethod == "attachEvent" ? "onmessage" : "message";

    bms.parentUrl = '';

    eventer(messageEvent, function (e) {
        var key = e.message ? "message" : "data";
        var data = e[key];

        bms.parentUrl = data;
    }, false);

    if (!bms.inIframe())
        $('body').addClass('noiframe');
    //else
    //    console.log(document.referrer);

    var keepSessionAlive = function (sendTimezone) {
        var offset = new Date().getTimezoneOffset();

        var data = {};

        if (sendTimezone)
            data = { timezoneOffset: offset };

        $.ajax({
            type: 'GET',
            cache: false,
            data: data,
            url: bms.baseUrl + 'home/keepsessionalive',
            success: function (data) {

            },
            error: function (jqXHR, textStatus, errorThrown) {

            }
        });
    };

    keepSessionAlive(true);

    setInterval(function () {
        keepSessionAlive(false);
    }, 30000);

    //$('#menu').click(function (e) {
    //    e.preventDefault();
    //    e.stopPropagation();

    //    $('#mainMenu').toggleClass('open');
    //});

    $(document).click(function (e) {
        $('.hideOnDocumentClick').hide();
    });

    $(document).mousedown(function (e) {
        $('#mainMenu').removeClass('open');
        $('#mainMenu .category').removeClass('active');
    });

    $('table.hasDetails').each(function () {
        $(this).tableWithDetails();
    });

    $('form').each(function () {
        $(this).prepareForm();
    });

    $('body').setAllSVG();

    $('body').setAllTooltips();
});

$.fn.setSVG = function () {
    var $img = jQuery(this);
    var imgID = $img.attr('id');
    var imgClass = $img.attr('class');
    var imgURL = $img.attr('src');

    if (imgURL == undefined)
        return;

    if (imgURL.startsWith("~/"))
        imgURL = bms.baseUrl + imgURL.substr(2);

    jQuery.get(imgURL, function (data) {
        // Get the SVG tag, ignore the rest
        var $svg = jQuery(data).find('svg');

        // Add replaced image's ID to the new SVG
        if (typeof imgID !== 'undefined') {
            $svg = $svg.attr('id', imgID);
        }

        $svg.attr('style', $img.attr('style'));

        var svgClass = $svg.attr('class');
        if (svgClass == undefined)
            svgClass = '';

        // Add replaced image's classes to the new SVG
        if (typeof imgClass !== 'undefined') {
            $svg = $svg.attr('class', svgClass + ' ' + imgClass + ' replaced-svg');
        }

        if ($img.attr('title') != undefined && $img.attr('title') != '') {
            $svg.addClass('setTooltip');
            $svg.attr('title', $img.attr('title'));
        }

        // Remove any invalid XML tags as per http://validator.w3.org
        $svg = $svg.removeAttr('xmlns:a');

        // Check if the viewport is set, if the viewport is not set the SVG wont't scale.
        if (!$svg.attr('viewBox') && $svg.attr('height') && $svg.attr('width')) {
            $svg.attr('viewBox', '0 0 ' + $svg.attr('height') + ' ' + $svg.attr('width'))
        }

        // Replace image with new SVG
        $img.replaceWith($svg);

    }, 'xml');

    return $(this);
};

bms.inIframe = function () {
    try {
        return window.self !== window.top;
    } catch (ex) {
        return true;
    }
}

$.fn.setAllSVG = function () {
    $(this).find('img.svg').each(function () {
        $(this).setSVG();
    });

    return $(this);
};

$.fn.setTooltip = function() {
    if ($(this).attr('title') != undefined && $(this).attr('title') != '') {
        $(this).tooltipster({
            trigger: 'hover',
            theme: 'tooltipster-borderless'
        });
    }
}

$.fn.setAllTooltips = function () {
    $(this).find('.setTooltip').each(function () {
        $(this).setTooltip();
    });
}

if (!String.prototype.trim) {
    String.prototype.trim = function () {
        return this.replace(/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, '');
    };
}

if (!String.prototype.replaceAll) {
    String.prototype.replaceAll = function (target, replacement) {
        return this.split(target).join(replacement);
    };
}

if (!String.prototype.endsWith) {
    String.prototype.endsWith = function (searchString, position) {
        var subjectString = this.toString();
        if (typeof position !== 'number' || !isFinite(position) || Math.floor(position) !== position || position > subjectString.length) {
            position = subjectString.length;
        }
        position -= searchString.length;
        var lastIndex = subjectString.lastIndexOf(searchString, position);
        return lastIndex !== -1 && lastIndex === position;
    };
}

if (!Array.prototype.includes) {
    Array.prototype.includes = function (searchElement /*, fromIndex*/) {
        'use strict';
        if (this === null) {
            throw new TypeError('Array.prototype.includes called on null or undefined');
        }

        var O = Object(this);
        var len = parseInt(O.length, 10) || 0;
        if (len === 0) {
            return false;
        }
        var n = parseInt(arguments[1], 10) || 0;
        var k;
        if (n >= 0) {
            k = n;
        } else {
            k = len + n;
            if (k < 0) { k = 0; }
        }
        var currentElement;
        while (k < len) {
            currentElement = O[k];
            if (searchElement === currentElement ||
                (searchElement !== searchElement && currentElement !== currentElement)) { // NaN !== NaN
                return true;
            }
            k++;
        }
        return false;
    };
}

bms.refreshPage = function () {
    var href = window.location.href;

    if (href.indexOf('#') > -1)
        href = href.substr(0, href.indexOf('#'));

    window.location = href;
};

$.fn.hasScrollBar = function () {
    return this.get(0).scrollHeight > this.height();
};

bms.isPageScrollBarVisible = function () {
    return $('html').hasScrollBar();
};

bms.showMainLoader = function () {
    $('#mainLoader').css('left', (($(window).width() - $('#mainLoader').width()) / 2) + 'px');

    $('#mainLoader').slideDown(100);
    $('#darkMask').show();
};

bms.hideMainLoader = function () {
    $('#mainLoader').slideUp(100);
    $('#darkMask').hide();
};

bms.showTempMessage = function (message, closeInSeconds, onClosing, showMask) {
    if (closeInSeconds === undefined || closeInSeconds === 0)
        closeInSeconds = 5;

    var messageDialog = $('<div />', {
        class: 'tempMessage'
    }).appendTo('body');

    messageDialog.css('left', ($(window).width() - messageDialog.width()) / 2);

    var messageDiv = $('<div />', {
        class: 'tempMessageWrapper',
        html: '<p>' + message + '</p>'
    }).appendTo(messageDialog);

    if (showMask)
        $('#darkMask').show();

    messageDialog.fadeIn(400, function () {
        setTimeout(function () {
            messageDialog.fadeOut(200, function () {
                ;
                messageDialog.remove();
                $('#darkMask').hide();
                if (onClosing !== undefined) {
                    onClosing();
                }
            });
        }, closeInSeconds * 1000);

    });
};

bms.getQueryParameterByName = function (name, queryString) {
    if (!queryString) {
        queryString = window.location.href;
    }
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(queryString);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}