rabbitOperationsApp.controller('searchController', function ($scope, $http) {
    $scope.formInfo = {
    };
    $scope.searchResults = {
        results: []
    };

    $scope.click = function (item, event) {
        $http.get("/api/v1/Messages/" + $scope.formInfo.searchString).success(function (data, status, headers, config) {
            $scope.searchResults = data;
            //$scope.$apply();
        }).error(function (data, status, headers, config) {
            alert("AJAX failed!");
        });
        //alert("search on " + $scope.formInfo.searchString)
    }
});