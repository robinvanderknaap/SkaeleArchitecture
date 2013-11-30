'use strict';

var userModule = angular.module('userModule', [
    'ngRoute',
    'userControllers'
]);

userModule.config(['$routeProvider',
  function ($routeProvider) {
      $routeProvider.
        when('/users', {
            templateUrl: 'user/userlist',
            controller: 'UserListController'
        }).
        when('/users/:userId', {
            templateUrl: 'user/userdetail',
            controller: 'UserDetailController'
        }).
        otherwise({
            redirectTo: '/users'
        });
  }]);

