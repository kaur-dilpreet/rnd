(function ($) {
    $.widget("nmk.rbSwitchCheck", {
        options: {
            value: false,
            onSwitch: function (value, element) { }
        },
        value: function (value) {
            if (value === undefined) {
                return this.options.value;
            } else {
                this._setCheckboxValue(value);
            }
        },
        _create: function () {
            var checkbox = this.element;

            this._elements = {};
            this._elements.checkInput = checkbox.siblings('input[name=' + checkbox.attr('name') + ']');

            checkbox.addClass('rbSwitchBoxAdded');
            checkbox.hide();

            this._elements.switchBox = $('<div />', {
                class: 'rbSwitchBox'
            }).insertBefore(checkbox);

            if (this.options.value) {
                this._elements.switchBox.addClass('checked');
                checkbox.attr('checked', 'checked');
            }

            var lever = $('<div />', {
                class: 'lever'
            }).appendTo(this._elements.switchBox);

            if (checkbox.is(':checked')) {
                this._elements.switchBox.addClass('checked');
                this.options.value = true;
            }
            var thisWidget = this;

            this._elements.switchBox.click(function (e) {
                e.preventDefault();

                if (thisWidget._elements.switchBox.hasClass('checked')) {
                    thisWidget._setCheckboxValue(false);
                }
                else {
                    thisWidget._setCheckboxValue(true);
                }

                thisWidget.element.trigger('change');
            });
        },
        _setCheckboxValue: function (value) {
            this.options.value = value;
            var triggerOnSwitch = false;

            if (this.options.value != this._elements.switchBox.hasClass('checked'))
                triggerOnSwitch = true;

            if (this.options.value) {
                this._elements.switchBox.addClass('checked');
                this.element.attr('checked', 'checked');

                if (this._elements.checkInput.length > 0)
                    this._elements.checkInput.val('true');
            }
            else {
                this._elements.switchBox.removeClass('checked');
                this.element.removeProp('checked');

                if (this._elements.checkInput.length > 0)
                    this._elements.checkInput.val('false');
            }

            if (triggerOnSwitch && this.options.onSwitch != undefined) {
                this.options.onSwitch(this.options.value, $(this.element));
            }
        }
    });
})(jQuery);