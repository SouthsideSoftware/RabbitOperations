rabbitOperationsApp.controller('navBarController', function ($scope, $location, queueService) {
    $scope.selected = "search";
    $scope.queues = [];
    $scope.applications = [];

    if ($location.absUrl().toLowerCase().indexOf("/tail") > -1) {
        $scope.selected = "tail";
    }

    $scope.navigateTo = function(feature) {
        $scope.selected = feature;
    };

    $scope.$watch(function () { return queueService.queues }, function (queues) {
        $scope.queues = queues;
    });

    $scope.$watch(function () { return queueService.applications }, function (applications) {
        $scope.applications = applications;
    });
});