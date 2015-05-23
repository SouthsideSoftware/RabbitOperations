rabbitOperationsApp.controller('searchController', function ($scope, $http, $modal, searchService, retryService, notificationService) {
    $scope.pageInfo = searchService.pageInfo;
    $scope.searchResults = searchService.searchResults;
    $scope.searchProgress = searchService.searchProgress;
    $scope.allowRetry = false;

    $scope.searchFields = ['AdditionalErrorStatus:', 'Any:', 'ApplicationId:', 'ClassName:', 'Header:', 'IsError:', 'TimeSent:'];

    $scope.$watch(function () { return searchService.searchResults }, function (searchResults) {
        $scope.allowRetry = false;
        $scope.searchResults = searchResults;
    });

    $scope.toggleSort = function(field) {
        searchService.toggleSort(field);
    };

    $scope.newSearch = function () {
        searchService.newSearch();
    };

    $scope.search = function () {
        searchService.search();
    };

    $scope.closeAlert = function(index) {
        searchService.closeAlert(index);
    };

    $scope.selectionChanged = function() {
        var selected = _.filter($scope.searchResults.results, function(item) {
            return item.isSelected;
        });
        var allSelectedAllowRetry = _.every(selected, function(item) {
            return item.canRetry;
        });
        $scope.allowRetry = selected.length > 0 && allSelectedAllowRetry;
    };

    $scope.$watch("pageInfo.take", function () {
        if ($scope.pageInfo.totalItems > 0) {
            $scope.newSearch();
        }
    });

    $scope.showDetails = function (item, event) {
        if (!$scope.searchInProgress) {
            self.fetchModal = notificationService.modal("Getting Details...");
            var url = "/api/v1/Messages/" + item.id;
            $http.get(url).success(function (data, status, headers, config) {
                var modalInstance = $modal.open({
                    templateUrl: '/Content/App/Dashboard/Popups/searchDetails.html?version=1',
                    controller: 'searchDetailController',
                    size: 'lg',
                    resolve: {
                        item: function() {
                            return data;
                        }
                    }
                });
                self.fetchModal.close();
            }).error(function (jqXHR, textStatus, errorThrown) {
                self.fetchModal.close();
                notificationService.error("Could not get message to display: " + textStatus);
            });
        }
    };

    $scope.retry = function () {
        var selected = _.filter($scope.searchResults.results, function (item) {
            return item.isSelected;
        });
        retryService.retry(selected).then(function(updatedItems) {
            $scope.selectionChanged();
        });
    };
});
