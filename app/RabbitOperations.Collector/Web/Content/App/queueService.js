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
                    auditQueue: { shortName: "audit", oneMinueRate: 0, fiveMinuteRate: 0, fifteenMinuteRate: 0, meanRate: 0, displayRate: "--", queueName: "" },
                    errorQueue: { shortName: "error", oneMinueRate: 0, fiveMinuteRate: 0, fifteenMinuteRate: 0, meanRate: 0, displayRate: "--", queueName: "" }
                };
                newApplications.push(found);
            }
            if (queue.queueSettings.isErrorQueue) {
                found.errorQueue.queueName = queue.queueSettings.queueName;
            } else {
                found.auditQueue.queueName = queue.queueSettings.queueName;
            }
        });
        self.applications = newApplications;
    }).error(function (data, status, headers, config) {
        notificationService.error("Could not get application list from server");
    });
});