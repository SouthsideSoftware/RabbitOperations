rabbitOperationsApp.controller('searchDetailController', function($scope, $modalInstance, item, $http, notificationService, searchService, retryService, $modal) {
    $scope.displayHeaders = false;
    $scope.displayBody = true;
    $scope.displayRetries = true;
    $scope.displayStackTrace = false;
    $scope.title = item.Id > 0 ? "Message " + item.Id : "Retry of Message " + item.originalId;
    $scope.forceRetry = false;

    $scope.parseBody = function (body) {
        try {
            return JSON.parse(body);
        } catch (err) {
            return body;
        }
    };

    $scope.message = {
        item: item,
        body: item !== undefined && item.Body !== undefined ? JSON.stringify($scope.parseBody(item.Body), null, 2) : '',
        headers: item !== undefined && item.Headers !== undefined ? JSON.stringify(item.Headers, null, 2) : '',
        timeSent: item !== undefined && item.TimeSent !== undefined ? moment(item.TimeSent).format('MM/DD/YYYY HH:mm:ss') : '',
        processingTime: item !== undefined ? item.ProcessingTime : 'UNK',
        totalTime: item !== undefined ? item.TotalTime : 'UNK'
    };

    $scope.message.stackTrace = undefined;
    $scope.message.errorMessage = undefined;
    if (item !== undefined && item.Headers !== undefined) {
        if ($scope.message.item.Headers["NServiceBus.ExceptionInfo.StackTrace"] !== undefined){
            $scope.message.stackTrace = $scope.message.item.Headers["NServiceBus.ExceptionInfo.StackTrace"];
        }
        if ($scope.message.item.Headers["NServiceBus.ExceptionInfo.Message"] !== undefined) {
            $scope.message.errorMessage = $scope.message.item.Headers["NServiceBus.ExceptionInfo.Message"];
        }
    }

    $scope.retries = [];
    if (item.Retries.length > 0) {
        _.each(item.Retries, function (retry) {
            $scope.retries.push({
                item: _.extend(retry, {originalId: $scope.message.item.Id}),
                isError: retry.IsError,
                timeSent: retry.TimeSent !== undefined ? moment(retry.TimeSent).format('MM/DD/YYYY HH:mm:ss') : '',
                processingTime: retry.ProcessingTime,
                totalTime: retry.TotalTime,
                additionalErrorStatusString: retry.AdditionalErrorStatusString
            });
        });
    };

    $scope.showRetryDetails = function (retryItem) {
        $modal.open({
            templateUrl: '/Content/App/Dashboard/Popups/searchDetails.html?version=0.15.0',
            controller: 'searchDetailController',
            size: 'lg',
            resolve: {
                item: function () {
                    return retryItem;
                }
            }
        });
    };

    $scope.ok = function () {
        $modalInstance.close();
    };

    $scope.retry = function () {
      retryService.prepareRetry([$scope.message.item], $scope.forceRetry, $scope.update);
    };

    $scope.update = function(updatedItems) {
      if (updatedItems.length > 0) {
        $scope.message.item = updatedItems[0];
      }
      $modalInstance.close();
    }
})
