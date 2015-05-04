(function (angular, $) {
    return angular.module('notyModule', []).provider('noty', function () {
        var settings = $.noty.defaults;
        settings = angular.extend(settings, { timeout: 5000, theme: "bootstrapTheme",  closeWith: ['click', 'button'] });

        return {
            settings: settings,
            $get: function () {
                var callNoty = function (newSettings) {
                    return noty(newSettings || {});
                };

                return {
                    show: function (message, type, additionalSettings) {
                        callNoty(angular.extend(settings, { text: message || settings.text, type: type || settings.type }, additionalSettings));
                    },

                    showAlert: function (message, additionalSettings) {
                        callNoty(angular.extend(settings, { text: message || settings.text, type: "alert"}, additionalSettings));
                    },

                    showSuccess: function (message, additionalSettings) {
                        callNoty(angular.extend(settings, { text: message || settings.text, type: "success"}, additionalSettings));
                    },

                    showError: function (message, additionalSettings) {
                        callNoty(angular.extend(settings, { text: message, type: "error"}, additionalSettings));
                    },

                    closeAll: function () {
                        return $.noty.closeAll();
                    },
                    clearShowQueue: function () {
                        return $.noty.clearQueue();
                    }.bind(this)
                }
            }

        };
    })
}(angular, jQuery));