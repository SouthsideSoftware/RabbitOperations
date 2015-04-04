rabbitOperationsApp.controller('searchController', function ($scope, $http, $modal) {
    $scope.formInfo = {
    };
    $scope.searchResults = {
        results: []
    };
    $scope.pageInfo =
    {
        page: 1,
        take: 20,
        totalItems: 0
    };

    $scope.search = function (item, event) {
        var url = "/api/v1/Messages/" + $scope.formInfo.searchString + "?page=" + ($scope.pageInfo.page - 1) + "&take=" + $scope.pageInfo.take
        $http.get(url).success(function (data, status, headers, config) {
            $scope.searchResults = data;
        }).error(function (data, status, headers, config) {
            alert("AJAX failed!");
        });
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
    }
});

rabbitOperationsApp.controller('searchDetailController', function($scope, $modalInstance, item) {
    $scope.toDisplayDuration = function (duration) {
        if (duration !== undefined) {
            var hours = duration.days * 24 + duration.hours;
            return hours + ":" + duration.minutes + ":" + duration.seconds + "." + duration.milliseconds;
        } else {
            return "UNK";
        }
    }

    $scope.message = {
        item: item,
        body: item !== undefined && item.body !== undefined ? JSON.stringify(JSON.parse(item.body), null, 2) : '',
        headers: item !== undefined && item.headers !== undefined ? JSON.stringify(item.headers, null, 2) : '',
        timeSent: item !== undefined && item.timeSent !== undefined ? moment(item.timeSent).format('MM/DD/YYYY HH:mm:ss') : '',
        processingTime: item !== undefined ? $scope.toDisplayDuration(item.processingTime) : 'UNK',
        totalTime: item !== undefined ? $scope.toDisplayDuration(item.totalTime) : 'UNK'
    };

    $scope.ok = function () {
        $modalInstance.close();
    };
})