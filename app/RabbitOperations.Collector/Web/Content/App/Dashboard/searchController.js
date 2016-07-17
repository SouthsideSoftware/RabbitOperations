rabbitOperationsApp.controller("searchController",
  function($scope, $http, $modal, searchService, retryService, notificationService) {
    $scope.pageInfo = searchService.pageInfo;
    $scope.searchResults = searchService.searchResults;
    $scope.searchProgress = searchService.searchProgress;
    $scope.targetPage = undefined;
    $scope.allowRetry = false;

    $scope.searchFields = [
      "AdditionalErrorStatus:", "Any:", "ApplicationId:", "ClassName:", "DocId:", "Header:", "IsError:", "TimeSent:"
    ];

    $scope.tips = [
      '"DocId:[123456 TO 200000]" to search on a range of document ids',
      '"DocId:123456" to search for a particular document',
      'add "AND ApplicationId:real-prod" to limit the search by application',
      '"TimeSent:[NULL TO 2016-07-15T05:00:00Z]" finds all records up to the time specfied (UTC -- display time is local)',
      'The default search field includes all data',
      '"ClassName:class" searches for all messages of a particular class type',
      '"Header:text" searches only the message headers for the specified text',
      'Add "AND IsError:true" to only search for errors'
    ];

    $scope.currentTip = $scope.tips[Math.floor(Math.random() * $scope.tips.length)];

    $scope.$watch(function() { return searchService.searchResults },
      function(searchResults) {
        $scope.allowRetry = false;
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
    };

    $scope.goToPage = function() {
      searchService.goToPage($scope.targetPage);
      $scope.targetPage = undefined;
    };

    $scope.closeAlert = function(index) {
      searchService.closeAlert(index);
    };

    $scope.selectionChanged = function() {
      var selected = _.filter($scope.searchResults.results,
        function(item) {
          return item.isSelected;
        });
      var allSelectedAllowRetry = _.every(selected,
        function(item) {
          return item.canRetry;
        });
      $scope.allowRetry = selected.length > 0 && allSelectedAllowRetry;
    };

    $scope.$watch("pageInfo.take",
      function() {
        if ($scope.pageInfo.totalItems > 0) {
          $scope.newSearch();
        }
      });

    $scope.showDetails = function(item, event) {
      if (!$scope.searchInProgress) {
        self.fetchModal = notificationService.modal("Getting Details...");
        var url = "/api/v1/Messages/" + item.id;
        $http.get(url)
          .success(function(data, status, headers, config) {
            var modalInstance = $modal.open({
              templateUrl: "/Content/App/Dashboard/Popups/searchDetails.html?version=3",
              controller: "searchDetailController",
              size: "lg",
              resolve: {
                item: function() {
                  return data;
                }
              }
            });
            self.fetchModal.close();
          })
          .error(function(jqXHR, textStatus, errorThrown) {
            self.fetchModal.close();
            notificationService.error("Could not get message to display: " + textStatus);
          });
      }
    };

    $scope.retry = function() {
      var selected = _.filter($scope.searchResults.results,
        function(item) {
          return item.isSelected;
        });
      retryService.retry(selected)
        .then(function(updatedItems) {
          $scope.selectionChanged();
        });
    };
  });