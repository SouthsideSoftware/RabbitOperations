﻿rabbitOperationsApp.controller('topBarController', function ($scope, $modal) {
    $scope.showWhatsNew = function () {
        var modalInstance = $modal.open({
            templateUrl: '/Content/App/Dashboard/Popups/whatsNew.html?version=3',
            controller: 'whatsNewController',
            size: 'lg'
        });
    }
});
