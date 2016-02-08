rabbitOperationsApp.service('notificationService', function () {
    var self = this;
    this.defaultOptions = $.noty.defaults;

    this.alert = function(message, settings) {
        return noty(_.extend({}, self.defaultOptions, { text: message, timeout: 5000, theme: "bootstrapTheme", closeWith: ['click', 'button'] }, settings));
    };

    this.success = function (message, settings) {
        return noty(_.extend({}, self.defaultOptions, { text: message, timeout: 5000, type: "success", theme: "bootstrapTheme", closeWith: ['click', 'button'] }, settings));
    };

    this.error = function(message, settings) {
        return noty(_.extend({}, self.defaultOptions, { text: message, timeout: 5000, type: "error", theme: "bootstrapTheme", closeWith: ['click', 'button'] }, settings));
    };

    this.modal = function(message, settings) {
        var finalSettings = _.extend({}, self.defaultOptions, {
            text: message,
            modal: true,
            theme: "bootstrapTheme",
            animation: {
                open: { height: 'toggle' },
                close: { height: 'toggle' },
                easing: 'swing',
                speed: 0 // opening & closing animation speed
            }
        }, settings);
        return noty(finalSettings);
    };
});