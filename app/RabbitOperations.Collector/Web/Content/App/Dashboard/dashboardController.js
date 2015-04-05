rabbitOperationsApp.controller('dashboardController', function ($scope, $http) {
    $scope.displayStats = true;

  $http.get("/api/v1/QueuePollers").success(function (data, status, headers, config) {
    $scope.queues = data.activePollers;
  }).error(function (data, status, headers, config) {
    alert("AJAX failed!");
  });

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
        if (parts.length === 2) {
          var queue = _.find($scope.queues,
              function (queue) {
                if (queue.queueSettings.environmentId === parts[0] &&
                  queue.queueSettings.queueName === parts[1]) {
                  queue.messageRate = element.Value.OneMinuteRate.toFixed(2);
                  $scope.$apply();
                }
              }
          );
        }
        console.log();
      }
    });
    //$scope.getQueuesFromMessage(message);
  };
  // Start the connection.
  $.connection.hub.start().done(function () {
  });
});