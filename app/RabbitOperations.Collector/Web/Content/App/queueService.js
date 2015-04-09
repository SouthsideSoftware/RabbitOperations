rabbitOperationsApp.service('queueService', function ($http) {
    var self = this; 
    
    this.environments = [];
    this.queues = [];

    $http.get("/api/v1/QueuePollers").success(function (data, status, headers, config) {
        self.queues = data.activePollers;
        var newEnvironments = [];
        _.each(self.queues, function (queue) {
            var found = _.find(newEnvironments, function(environment) { return environment.environmentId === queue.queueSettings.environmentId });
            if (found === undefined) {
                newEnvironments.push({
                    environmentId: queue.queueSettings.environmentId,
                    environmentName: queue.queueSettings.environmentName,
                    rabbitManagementWebUrl: queue.queueSettings.rabbitManagementWebUrl
                });
            }
        });
        self.environments = newEnvironments;
    }).error(function (data, status, headers, config) {
        alert("AJAX failed!");
    });
});