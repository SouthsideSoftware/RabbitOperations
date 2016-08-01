rabbitOperationsApp.service('retryService', function ($http, $q, searchService, notificationService, $modal) {
    var self = this;

    this.prepareRetry = function (items, forceRetry, callback) {
      self.callback = callback;
      if (forceRetry === undefined) forceRetry = false;
      var retryIds = [];
      _.each(items, function (retryItem) {
        if (retryItem.id) {
          retryIds.push(retryItem.id);
        } else {
          retryIds.push(retryItem.Id);
        }
      });
      self.fetchModal = notificationService.modal("Getting destination information...");
      var url = "/api/v1/Messages/RetryDestinations/";
      $http.post(url, { retryIds: retryIds, forceRetry: forceRetry })
        .success(function (data, status, headers, config) {
          var modalInstance = $modal.open({
            templateUrl: "/Content/App/Dashboard/Popups/setRetryDestination.html?version=0.12.0",
            controller: "retryController",
            size: "lg",
            resolve: {
              item: function () {
                return { data: data, callback: self.callback };
              }
            }
          });
          self.fetchModal.close();
        })
        .error(function (jqXHR, textStatus, errorThrown) {
          self.fetchModal.close();
          notificationService.error("Could not get destination information: " + textStatus);
        });
    };
});