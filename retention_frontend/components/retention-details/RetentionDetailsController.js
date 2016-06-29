"use strict";

angular.module("retentionApp").controller("RetentionDetailsController", 
function($scope, $location, $state, $stateParams, RetentionResource) {

	var SSN = $stateParams.ssn;
	RetentionResource.getSingleStudent(SSN).success(function(data){
		$scope.thisStudent = data;
	});

	$scope.go = function(route) {
		$state.go(route);
	};

	$scope.active = function(route) {
		return $state.is(route);
	};

	$scope.tabs = [
		{ heading: "ret.Overview", route:"retentiondetails.overview", active:true },
		{ heading: "ret.Communications", route:"retentiondetails.communications", active:false }
	];

	$scope.$on("$stateChangeSuccess", function stateChangeSuccess() {
		if ($state.current.name === "retentiondetails") {
			$state.go("retentiondetails.overview");
		} else {
			$scope.tabs.forEach(function forEachTab(tab) {
				tab.active = $state.is(tab.route);
			});
		}
	});

});