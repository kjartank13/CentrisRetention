"use strict";

angular.module("retentionApp").controller("OverviewController", 
function($scope, $location, $state, $stateParams, RetentionResource) {

	var SSN = $stateParams.ssn;
	$scope.loadingData = false;
	var thisHistory = [];


	$scope.makeGraph = function makeGraph(thisHistory) {
		var labels = [];
		var data = [];
		var reasons = [];
		var defaultReason = "No significant change";

		for (var i = 0; i < thisHistory.length; i++) {
			var date = new Date(thisHistory[i].Date);
			var refinedDate = date.getDate() + "/" + (date.getMonth() + 1);
			labels.push(refinedDate);
			data.push(thisHistory[i].RiskFactor);
			reasons.push(thisHistory[i].DeltaReason);
		}

		$scope.labels = labels;
		$scope.data = [data];

		$scope.chart_options = {
			scaleBeginAtZero: true,
			tooltipTemplate: function(label) {
				for (var j = 0; j < thisHistory.length; j++) {
					var thisDate = new Date(thisHistory[j].Date);
					var thisRefinedDate = thisDate.getDate() + "/" + (thisDate.getMonth() + 1);
					if (reasons[j] === null) {
						reasons[j] = defaultReason;
					}
					if (thisRefinedDate === label.label) {
						return Number(data[j]).toFixed(2) + "% - " + reasons[j];
					}
				}
			}
		};
	};

	RetentionResource.getStudentHistory(SSN).success(function(data) {
		thisHistory = data;
		thisHistory.sort(function(a, b) {
			return new Date(a.Date).getTime() - new Date(b.Date).getTime();
		});
		if (thisHistory.length > 30)
		{
			thisHistory = thisHistory.slice(thisHistory.length - 30, thisHistory.length);
		}
		$scope.makeGraph(thisHistory);
	}).then(function() {
		$scope.loadingData = true;
	});

});