"use strict";

angular.module("retentionApp").controller("CommunicationsController", 
function($scope, $location, $state, $stateParams, RetentionResource, UserService) {

	var SSN = $stateParams.ssn;
	var user = UserService.getFullName();
	$scope.communications = [];

	RetentionResource.getCommunications(SSN).success(function(data) {
		data.reverse();
		$scope.communications = data;
	});

	$scope.onSubmit = function onSubmit() {
		var date = new Date();
		var day = date.getDate();
		var month = (date.getMonth() + 1);
		var year = date.getFullYear();
		var refinedDate = day + "/" + month + "/" + year;

		RetentionResource.postCommunication(SSN, refinedDate, $scope.message + " - " + user).success(function(data) {
			var log = {
				Date: refinedDate,
				Message: $scope.message + " - " + user
			};
			$scope.communications.unshift(log);
		}).then(function() {
			$scope.message = "";
		});
	};
});