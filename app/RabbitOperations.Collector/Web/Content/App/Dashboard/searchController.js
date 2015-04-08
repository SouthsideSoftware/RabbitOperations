rabbitOperationsApp.controller('searchController', function ($scope, $http, $modal, searchService) {
    $scope.pageInfo = searchService.pageInfo;
    $scope.searchResults = searchService.searchResults;

    $scope.searchFields = ['Any:', 'ClassName:', 'EnvironmentId:', 'Header:', 'IsError:', 'TimeSent:'];

    $scope.$watch(function () { return searchService.searchResults }, function (searchResults) {
        $scope.searchResults = searchResults;
    });

    $scope.toggleSort = function(field) {
        searchService.toggleSort(field);
    };

    $scope.newSearch = function() {
        searchService.newSearch();
    };

    $scope.search = function() {
        searchService.search();
    }

    $scope.$watch("pageInfo.take", function () {
        if ($scope.pageInfo.totalItems > 0) {
            searchService.newSearch();
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