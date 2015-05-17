rabbitOperationsApp.controller('dashboardController', function ($scope, $http, searchService, queueService) {
    $scope.displayStats = true;
    $scope.applications = [];
    $scope.evenApplication = [];
    $scope.oddApplications = [];
    $scope.displayRate = 1;
    $scope.tabularDisplay = true;

    $scope.options = {
        chart: {
            type: 'pieChart',
            height: 150,
            donut: false,
            x: function(d) { return d.shortName; },
            y: function(d) { return d.displayRate; },
            showLabels: true,
            labelType: "value",
            pie: {
                //startAngle: function(d) { return d.startAngle / 2 - Math.PI / 2 },
                //endAngle: function(d) { return d.endAngle / 2 - Math.PI / 2 },
                dispatch: {
                    elementClick: function(e) { $scope.pieSegmentClicked(e); }
                },
                color: ['green', 'red']
            },
            transitionDuration: 500,
            showLegend: false
}
    };

    $scope.hasPieData = function(application) {
        var someZero = _.some(application.queues, function(queue){
            return (queue === undefined || queue.displayRate === "--" || queue.displayRate == 0);
        });

        return !someZero;
    }

    $scope.pieSegmentClicked = function(e) {
        $scope.quickSearch(e.point.applicationId, e.point.isErrorQueue);
    };

    $scope.$watch(function() { return $scope.displayRate }, function(displayRate) {
        _.each($scope.applications, function(application) {
            $scope.displayRateForQueue(application.queues[0]);
            $scope.displayRateForQueue(application. queues[1]);
        });
    });

    $scope.$watch(function () { return queueService.applications }, function (applications) {
        $scope.applications = applications;
        var even = [];
        var odd = [];
        for (var x = 0; x < applications.length; x++) {
            if (x % 2 === 0) {
                even.push(applications[x]);
            } else {
                odd.push(applications[x]);
            }
        }
        $scope.evenApplications = even;
        $scope.oddApplications = odd;
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
                    var application = _.find($scope.applications, function(application) {
                        var queue = undefined;
                        if (application.applicationId === parts[0] && application.queues[0].queueName === targetQueueName) {
                            queue = application.queues[0];
                        } else if (application.applicationId === parts[0] && application.queues[1].queueName === targetQueueName) {
                            queue = application.queues[1];
                        }
                        if (queue !== undefined) {
                            queue.oneMinuteRate = element.Value.OneMinuteRate.toFixed(2);
                            queue.fiveMinuteRate = element.Value.FiveMinuteRate.toFixed(2);
                            queue.fifteenMinuteRate = element.Value.FifteenMinuteRate.toFixed(2);
                            queue.meanRate = element.Value.MeanRate.toFixed(2);
                            queue.oldCount = queue.count;
                            queue.count = element.Value.Count;
                            $scope.displayRateForQueue(queue);
                            $scope.$apply();
                        }

                   });
                }
            }
        });
     };

    $scope.displayRateForQueue = function(queue) {
        switch ($scope.displayRate) {
        case 1:
            queue.displayRate = queue.oneMinuteRate;
            break;
        case 5:
            queue.displayRate = queue.fiveMinuteRate;
            break;
        case 15:
            queue.displayRate = queue.fifteenMinuteRate;
            break;
        default:
            queue.displayRate = queue.meanRate;
            break;
        }
    };

  // Start the connection.
  $.connection.hub.start().done(function () {
  });
});
