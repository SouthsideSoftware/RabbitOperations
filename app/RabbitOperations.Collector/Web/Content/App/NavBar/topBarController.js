rabbitOperationsApp.controller('topBarController', function ($scope, $modal) {
    $scope.showWhatsNew = function () {
        var modalInstance = $modal.open({
            templateUrl: '/Content/App/Dashboard/Popups/whatsNew.html?version=0.14.0',
            controller: 'whatsNewController',
            size: 'lg'
        });
    }
});
