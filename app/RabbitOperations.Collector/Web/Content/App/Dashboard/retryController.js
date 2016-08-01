rabbitOperationsApp.controller('retryController',
  function ($scope, $modalInstance, item, $http, retryService) {
    $scope.retryDestinationInfo = item;
    $scope.destinations = item.retryDestinations.join(',');
    $scope.numberOfMessages = item.retryIds.length;

    $scope.retry = function() {
      $modalInstance.close();
    };

    $scope.cancel = function () {
      $modalInstance.close();
    };
  });
