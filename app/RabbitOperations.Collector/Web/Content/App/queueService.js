rabbitOperationsApp.service('queueService', function ($http) {
    var self = this; 
    
    this.environments = [];
    this.queues = [];

    $http.get("/api/v1/QueuePollers").success(function (data, status, headers, config) {
        self.queues = data.activePollers;
        var newEnvironments = [];
        _.each(self.queues, function (queue) {
            if (_.find(self.environments, function (environment) { return environment.environmentId === queue.queueSettings.environmentId }) === undefined) {
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