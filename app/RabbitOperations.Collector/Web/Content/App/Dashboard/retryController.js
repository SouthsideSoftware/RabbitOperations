rabbitOperationsApp.controller('retryController',
  function ($scope, $modalInstance, item, $http, retryService, notificationService, searchService) {
    $scope.retryDestinationInfo = item.data;
    $scope.destinations = item.data.retryDestinations.join(',');
    $scope.numberOfMessages = item.data.retryIds.length;
    $scope.callback = item.callback;
    $scope.overrideDestination = undefined;

    $scope.retry = function () {
      self.modalNoty = notificationService.modal("Retrying...");
      var updatedItems = [];
      $http.put('/api/v1/messages/retry', { retryIds: item.data.retryIds, forceRetry: item.data.forceRetry, userSuppliedRetryDestination : $scope.overrideDestination }).success(function (data, status, headers, config) {
        var failures = _.filter(data.retryMessageItems, function (retryMessageItem) {
          return retryMessageItem.additionalInfo !== undefined &&
            retryMessageItem.additionalInfo !== null &&
            retryMessageItem.additionalInfo.length > 0;
        });
        var messagePrefix = data.retryMessageItems.length > 0 ? "Messages" : "Message";
        _.each(data.retryMessageItems, function (retryMessageItem) {
          updatedItems.push(searchService.changeStatusOfItem(retryMessageItem.retryId, retryMessageItem.additionalErrorStatusOfOriginalMessage, retryMessageItem.canRetryOfOriginalMessage));
        });
        self.modalNoty.close();
        if (failures.length === 0) {
          notificationService.success(messagePrefix + " retrying");
        } else {
          _.each(failures,
            function(failure) {
              notificationService.error("Error on retry " + failure.retryId + " error is " + failure.additionalInfo);
            });
        }
        $scope.completed(updatedItems);
      }).error(function (jqXHR, textStatus, errorThrown) {
        self.modalNoty.close();
        notificationService.error("Error calling retry service: " + textStatus);
        $scope.completed(updatedItems);
      });
    };

    $scope.cancel = function () {
      $modalInstance.close();
    };

    $scope.completed = function(updatedItems) {
      $scope.callback(updatedItems);
      $modalInstance.close();
    }
  });
