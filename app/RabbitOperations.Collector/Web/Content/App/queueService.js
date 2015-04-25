rabbitOperationsApp.service('queueService', function ($http) {
    var self = this; 
    
    this.applications = [];
    this.queues = [];

    $http.get("/api/v1/QueuePollers").success(function (data, status, headers, config) {
        self.queues = data.activePollers;
        var newApplications = [];
        _.each(self.queues, function (queue) {
            var found = _.find(newApplications, function(application) { return application.applicationId === queue.queueSettings.applicationId });
            if (found === undefined) {
                newApplications.push({
                    applicationId: queue.queueSettings.applicationId,
                    applicationName: queue.queueSettings.applicationName,
                    rabbitManagementWebUrl: queue.queueSettings.rabbitManagementWebUrl
                });
            }
        });
        self.applications = newApplications;
    }).error(function (data, status, headers, config) {
        alert("AJAX failed!");
    });
});