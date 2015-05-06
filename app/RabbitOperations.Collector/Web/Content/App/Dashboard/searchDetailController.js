rabbitOperationsApp.controller('searchDetailController', function($scope, $modalInstance, item, $http, notificationService) {
    $scope.toDisplayDuration = function(duration) {
        if (duration !== undefined) {
            var hours = duration.days * 24 + duration.hours;
            return hours + ":" + duration.minutes + ":" + duration.seconds + "." + duration.milliseconds;
        } else {
            return "UNK";
        }
    };

    $scope.parseBody = function (body) {
        var bodyToReturn;
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

    $scope.ok = function () {
        $modalInstance.close();
    };

    $scope.retry = function () {
        $scope.modalNoty = notificationService.modal("Retrying...");
        $http.put('/api/v1/messages/retry', { retryIds: [item.id] }).success(function (data, status, headers, config) {
            $scope.modalNoty.close();
            if (data.retryMessageItems[0].isRetrying) {
                notificationService.success("Message " + data.retryMessageItems[0].retryId + " is now retrying on queue " + data.retryMessageItems[0].retryQueue);
            } else {
                notificationService.error("Failed to retry: " + data.retryMessageItems[0].additionalInfo);
            }
        }).error(function (jqXHR, textStatus, errorThrown) {
            $scope.modalNoty.close();
            notificationService.error("Error calling retry service: " + textStatus);
        });
    }
})