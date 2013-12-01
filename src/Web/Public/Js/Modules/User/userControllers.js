'use strict';

var userControllers = angular.module('userControllers', []);

userControllers.controller('UserListController', ['$scope', '$http', function ($scope, $http) {

    $http.get('User/GetUsers').success(function (data) {
        $scope.users = data;
    });

    $scope.startEdit = function (user) {

        user.editing = true;

        user.newEmail = user.Email;
        user.newDisplayName = user.DisplayName;
        user.newIsActive = user.IsActive;
        user.newRoles = angular.copy(user.AllRoles);
    };
    
    $scope.save = function (user) {
        $http.post('User/UpdateUser', user).success(function (data) {

            user.editing = false;

            user.Email = data.Email;
            user.DisplayName = data.DisplayName;
            user.GravatarHash = data.GravatarHash;
            user.Roles = data.Roles;
            user.AllRoles = data.AllRoles;
            user.IsActive = data.IsActive;
        });
    };
}]);