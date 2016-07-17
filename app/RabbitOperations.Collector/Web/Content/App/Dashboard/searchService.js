rabbitOperationsApp.service("searchService",
  function($http, notificationService) {
    var self = this;
    this.searchResults = {
      results: []
    };
    this.pageInfo =
    {
      page: 1,
      take: 10,
      totalItems: 0,
      totalPages: 0,
      searchString: undefined,
      sortField: "TimeSent",
      sortAscending: false
    };

    this.newSearch = function() {
      self.pageInfo.page = 1;
      self.pageInfo.totalItems = 0;
      self.pageInfo.totalPages = 0;
      self.search();
    };

    self.goToPage = function(targetPage) {
      if (targetPage < self.pageInfo.totalPages) {
        self.pageInfo.page = targetPage;
      } else {
        self.pageInfo.page = self.pageInfo.totalPages;
      }
      self.search();
    };

    self.search = function() {
      self.searchingModal = notificationService.modal("Searching...");
      var url = "/api/v1/Messages/search/" +
        self.pageInfo.searchString +
        "?page=" +
        (self.pageInfo.page - 1) +
        "&take=" +
        self.pageInfo.take +
        "&sortField=" +
        self.pageInfo.sortField +
        "&sortAscending=" +
        self.pageInfo.sortAscending;
      $http.get(url)
        .success(function(data, status, headers, config) {
          _.each(data.results,
            function(element, index, list) {
              element.formattedTimeSent = element !== undefined && element.timeSent !== undefined
                ? moment(element.timeSent).format("MM/DD/YYYY HH:mm:ss")
                : "";
            });
          self.searchResults = data;
          self.pageInfo.totalItems = data.totalResults;
          self.pageInfo.totalPages = Math.ceil(data.totalResults / self.pageInfo.take);
          self.searchingModal.close();
        })
        .error(function(jqXHR, textStatus, errorThrown) {
          self.searchingModal.close();
          notificationService.error("Error calling search service: " + textStatus);
        });
    };
    self.changeStatusOfItem = function(id, status, canRetry) {
      var matchingSearchResult = _.find(self.searchResults.results,
        function(item) {
          return item.id === id;
        });
      if (matchingSearchResult !== undefined) {
        matchingSearchResult.additionalErrorStatusString = status;
        matchingSearchResult.canRetry = canRetry;
        matchingSearchResult.isSelected = false;
      };

      return matchingSearchResult;
    };

    self.toggleSort = function(field) {
      if (!self.searchInProgress) {
        //default sort on new field to true, otherwise toggle
        if (self.pageInfo.sortField !== field) {
          self.pageInfo.sortAscending = true;
        } else {
          self.pageInfo.sortAscending = !self.pageInfo.sortAscending;
        }
        self.pageInfo.sortField = field;
        self.newSearch();
      }
    };
  });