rabbitOperationsApp.controller('topBarController', function ($scope, $modal) {
    $scope.showWhatsNew = function () {
        var modalInstance = $modal.open({
            templateUrl: '/Content/App/Dashboard/Popups/whatsNew.html',
            controller: 'whatsNewController',
            size: 'lg'
        });
    }
});