rabbitOperationsApp.service('queueService', function ($http, notificationService) {
    var self = this;

    this.applications = [];

    $http.get("/api/v1/QueuePollers").success(function (data, status, headers, config) {
        var newApplications = [];
        _.each(data.activePollers, function (queue) {
            var found = _.find(newApplications, function(application) { return application.applicationId === queue.queueSettings.applicationId });
            if (found === undefined) {
                found = {
                    applicationId: queue.queueSettings.applicationId,
                    applicationName: queue.queueSettings.applicationName,
                    rabbitManagementWebUrl: queue.queueSettings.rabbitManagementWebUrl,
                    queues: []
                };
                found.queues[0] = { shortName: "audit", oneMinueRate: 0, fiveMinuteRate: 0, fifteenMinuteRate: 0, meanRate: 0, displayRate: "--", maxRate: 0, queueName: "", applicationId: found.applicationId, isErrorQueue: false, count:0, oldCount:0, gaugeColor: ['#00f000'] },
                found.queues[1] = { shortName: "error", oneMinueRate: 0, fiveMinuteRate: 0, fifteenMinuteRate: 0, meanRate: 0, displayRate: "--", maxRate: 0, queueName: "", applicationId: found.applicationId, isErrorQueue: true, count:0, oldCount:0, gaugeColor: ['#00f000'] }
                newApplications.push(found);
            }
            if (queue.queueSettings.isErrorQueue) {
                found.queues[1].queueName = queue.queueSettings.queueName;
            } else {
                found.queues[0].queueName = queue.queueSettings.queueName;
            }
        });
        self.applications = newApplications;
    }).error(function (data, status, headers, config) {
        notificationService.error("Could not get application list from server");
    });
});
