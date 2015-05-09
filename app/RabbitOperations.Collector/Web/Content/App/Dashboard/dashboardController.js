rabbitOperationsApp.controller('dashboardController', function ($scope, $http, searchService, queueService) {
    $scope.displayStats = true;
    $scope.applications = [];
    $scope.queues = [];

    $scope.$watch(function () { return queueService.queues }, function (queues) {
        $scope.queues = queues;
    });

    $scope.$watch(function () { return queueService.applications }, function (applications) {
        $scope.applications = applications;
    });

    $scope.quickSearch = function(applicationId, isErrorQueue) {
        searchService.pageInfo.searchString = 'ApplicationId:' + applicationId;
        if (isErrorQueue !== undefined) {
            searchService.pageInfo.searchString += ' AND IsError:' + isErrorQueue;
        }
        searchService.newSearch();
    };

    // Declare a proxy to reference the hub.
    var serverUpdates = $.connection.messagePulseHub;
    // Create a function that the hub can call to broadcast messages.
     serverUpdates.client.pulse = function (messageType, message) {
    var stats = angular.fromJson(message);
    _.each(stats, function (element, index, list) {
        var prefix = 'RabbitOperations.QueuePoller.Messages';
        var index = element.Name.indexOf(prefix);
        if (index === 0) {
            var parts = element.Name.substring(index + prefix.length + 1).split('.');
            var targetQueueName = '';
            if (parts.length >= 2) {
                for (x = 1; x < parts.length; x++) {
                    if (targetQueueName.length > 0) {
                        targetQueueName += '.';
                    }
                    targetQueueName += parts[x];
                }
                var queue = _.find($scope.queues, function(queue) {
                    if (queue.queueSettings.applicationId === parts[0] && queue.queueSettings.queueName === targetQueueName) {
                        queue.messageRate = element.Value.OneMinuteRate.toFixed(2);
                        $scope.$apply();
                    }
               });
            }
        }
    });
  };
  // Start the connection.
  $.connection.hub.start().done(function () {
  });
});