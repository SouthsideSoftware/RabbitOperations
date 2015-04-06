rabbitOperationsApp.controller('navBarController', function ($scope, $location) {
    $scope.selected = "search";
    if ($location.absUrl().toLowerCase().indexOf("/tail") > -1) {
        $scope.selected = "tail";
    }

    $scope.navigateTo = function(feature) {
        $scope.selected = feature;
    };
});