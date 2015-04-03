rabbitOperationsApp.controller('searchController', function ($scope, $http, $modal) {
    $scope.formInfo = {
    };
    $scope.searchResults = {
        results: []
    };

    $scope.search = function (item, event) {
        $http.get("/api/v1/Messages/" + $scope.formInfo.searchString).success(function (data, status, headers, config) {
            $scope.searchResults = data;
            //$scope.$apply();
        }).error(function (data, status, headers, config) {
            alert("AJAX failed!");
        });
        //alert("search on " + $scope.formInfo.searchString)
    }

    $scope.showDetails = function(item, event) {
        var modalInstance = $modal.open({
            templateUrl: '/Content/App/Dashboard/searchDetails.html',
            controller: 'searchDetailController',
            size: 'lg',
            resolve: {
                item: function() {
                    return item;
                }
            }
        });

        modalInstance.opened.then(function() {
            //hljs.initHighlighting();
        });
    }
});

rabbitOperationsApp.controller('searchDetailController', function($scope, $modalInstance, item) {
    $scope.item = item;

    $scope.ok = function () {
        $modalInstance.close();
    };
})