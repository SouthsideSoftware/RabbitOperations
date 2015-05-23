rabbitOperationsApp.controller('searchDetailController', function($scope, $modalInstance, item, $http, notificationService, searchService, retryService, $modal) {
    $scope.displayHeaders = false;
    $scope.displayBody = true;
    $scope.displayRetries = true;
    $scope.displayStackTrace = false;
    $scope.title = item.id > 0 ? "Message " + item.id : "Retry of Message " + item.originalId;
    $scope.toDisplayDuration = function (duration) {
        if (duration !== undefined) {
            var hours = duration.days * 24 + duration.hours;
            return hours + ":" + duration.minutes + ":" + duration.seconds + "." + duration.milliseconds;
        } else {
            return "UNK";
        }
    };

    $scope.parseBody = function (body) {
        try {
            return JSON.parse(body);
        } catch (err) {
            return body;
        }
    };

    $scope.message = {
        item: item,
        body: item !== undefined && item.body !== undefined ? JSON.stringify($scope.parseBody(item.body), null, 2) : '',
        headers: item !== undefined && item.headers !== undefined ? JSON.stringify(item.headers, null, 2) : '',
        timeSent: item !== undefined && item.timeSent !== undefined ? moment(item.timeSent).format('MM/DD/YYYY HH:mm:ss') : '',
        processingTime: item !== undefined ? $scope.toDisplayDuration(item.processingTime) : 'UNK',
        totalTime: item !== undefined ? $scope.toDisplayDuration(item.totalTime) : 'UNK'
    };

    $scope.message.stackTrace = undefined;
    if (item !== undefined && item.headers !== undefined) {
        if ($scope.message.item.headers["nServiceBus.ExceptionInfo.StackTrace"] !== undefined){
            $scope.message.stackTrace = $scope.message.item.headers["nServiceBus.ExceptionInfo.StackTrace"];
        }
    }

    $scope.retries = [];
    if (item.retries.length > 0) {
        _.each(item.retries, function (retry) {
            $scope.retries.push({
                item: _.extend(retry, {originalId: $scope.message.item.id}),
                isError: retry.isError,
                timeSent: retry.timeSent !== undefined ? moment(retry.timeSent).format('MM/DD/YYYY HH:mm:ss') : '',
                processingTime: $scope.toDisplayDuration(retry.processingTime),
                totalTime: $scope.toDisplayDuration(retry.totalTime),
                additionalErrorStatusString: retry.additionalErrorStatusString
            });
        });
    };

    $scope.showRetryDetails = function (retryItem) {
        $modal.open({
            templateUrl: '/Content/App/Dashboard/Popups/searchDetails.html?version=1',
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
        retryService.retry([$scope.message.item]).then(function(updatedItems) {
            if (updatedItems.length > 0) {
                $scope.message.item = updatedItems[0];
            }
        });
    };
})
