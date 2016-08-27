rabbitOperationsApp.controller('topBarController', function ($scope, $modal) {
    $scope.showWhatsNew = function () {
        var modalInstance = $modal.open({
            templateUrl: '/Content/App/Dashboard/Popups/whatsNew.html?version=0.15.0',
            controller: 'whatsNewController',
            size: 'lg'
        });
    }
});
