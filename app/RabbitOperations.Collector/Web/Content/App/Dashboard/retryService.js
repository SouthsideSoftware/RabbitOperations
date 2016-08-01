rabbitOperationsApp.service('retryService', function ($http, $q, searchService, notificationService, $modal) {
    var self = this;

    this.retry = function (items, forceRetry) {
        if (forceRetry === undefined) forceRetry = false;
        var func = this;
        var deferred = $q.defer();
        var promise = deferred.promise;
        self.modalNoty = notificationService.modal("Retrying...");
        var updatedItems = [];
        var retryIds = [];
        _.each(items, function (retryItem) {
          if (retryItem.id) {
            retryIds.push(retryItem.id);
          } else {
            retryIds.push(retryItem.Id);
          }
        });
        $http.put('/api/v1/messages/retry', { retryIds: retryIds, forceRetry: forceRetry }).success(function(data, status, headers, config) {
            var failures = _.filter(data.retryMessageItems, function(retryMessageItem) {
                return !retryMessageItem.isRetrying;
            });
            var messagePrefix = data.retryMessageItems.length > 0 ? "Messages" : "Message";
            _.each(data.retryMessageItems, function(retryMessageItem) {
                updatedItems.push(searchService.changeStatusOfItem(retryMessageItem.retryId, retryMessageItem.additionalErrorStatusOfOriginalMessage, retryMessageItem.canRetryOfOriginalMessage));
            });
            self.modalNoty.close();
            if (failures.length === 0) {
                notificationService.success(messagePrefix + " retrying");
            } else {
                notificationService.error("Some or all of the selected messages failed retry. Check the status of the selected messages for more details");
            }
            deferred.resolve(updatedItems);
        }).error(function(jqXHR, textStatus, errorThrown) {
            self.modalNoty.close();
            notificationService.error("Error calling retry service: " + textStatus);
            deferred.resolve(updatedItems);
        });

        return promise;
    };

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