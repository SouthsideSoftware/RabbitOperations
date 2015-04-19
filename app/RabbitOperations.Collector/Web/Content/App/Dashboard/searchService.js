rabbitOperationsApp.service('searchService', function ($http) {
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
    this.searchProgress = 0;

    this.newSearch = function () {
        self.pageInfo.page = 1;
        self.pageInfo.totalItems = 0;
        self.pageInfo.totalPages = 0;
        self.search();
    }

    self.search = function () {
        self.searchProgress = 10;
        var url = "/api/v1/Messages/" + self.pageInfo.searchString + "?page=" + (self.pageInfo.page - 1) + "&take=" + self.pageInfo.take + "&sortField=" + self.pageInfo.sortField + "&sortAscending=" + self.pageInfo.sortAscending;
        self.searchProgress = 30;
        $http.get(url).success(function (data, status, headers, config) {
            _.each(data.results, function(element, index, list) {
                element.formattedTimeSent = element !== undefined && element.timeSent !== undefined ? moment(element.timeSent).format('MM/DD/YYYY HH:mm:ss') : '';
            });
            self.searchProgress = 100;
            self.searchResults = data;
            self.pageInfo.totalItems = data.totalResults;
            self.pageInfo.totalPages = Math.ceil(data.totalResults / self.pageInfo.take);
            self.searchProgress = 0;
        }).error(function (data, status, headers, config) {
            self.searchProgress = 0;
            alert("AJAX failed!");
        });
    }

    self.toggleSort = function (field) {
        if (self.searchProgress === 0) {
            //default sort on new field to true, otherwise toggle
            if (self.pageInfo.sortField !== field) {
                self.pageInfo.sortAscending = true;
            } else {
                self.pageInfo.sortAscending = !self.pageInfo.sortAscending;
            }
            self.pageInfo.sortField = field;
            self.newSearch();
        }
    }
});