rabbitOperationsApp.controller('searchController', function ($scope, $http, $modal) {
    $scope.searchResults = {
        results: []
    };
    $scope.pageInfo =
    {
        page: 1,
        take: 10,
        totalItems: 0,
        totalPages: 0,
        searchString: undefined,
        sortField: "TimeSent",
        sortAscending: false
    };

    $scope.newSearch = function() {
        $scope.pageInfo.page = 1;
        $scope.pageInfo.totalItems = 0;
        $scope.pageInfo.totalPages = 0;
        $scope.search();
    }

    $scope.searchFields = ['Any:', 'ClassName:', 'EnvironmentId:', 'IsError:', 'TimeSent:']

    $scope.search = function () {
        var url = "/api/v1/Messages/" + $scope.pageInfo.searchString + "?page=" + ($scope.pageInfo.page - 1) + "&take=" + $scope.pageInfo.take + "&sortField=" + $scope.pageInfo.sortField + "&sortAscending=" + $scope.pageInfo.sortAscending
        $http.get(url).success(function (data, status, headers, config) {
            $scope.searchResults = data;
            $scope.pageInfo.totalItems = data.totalResults;
            $scope.pageInfo.totalPages = Math.ceil(data.totalResults / $scope.pageInfo.take);
        }).error(function (data, status, headers, config) {
            alert("AJAX failed!");
        });
    }

    $scope.toggleSort = function (field) {
        //default sort on new field to true, otherwise toggle
        if ($scope.pageInfo.sortField !== field) {
            $scope.pageInfo.sortAscending = true;
        } else {
            $scope.pageInfo.sortAscending = !$scope.pageInfo.sortAscending;
        }
        $scope.pageInfo.sortField = field;
        $scope.newSearch();
    }

    $scope.$watch("pageInfo.take", function () {
        if ($scope.pageInfo.totalItems > 0) {
            $scope.newSearch();
        }
    });

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