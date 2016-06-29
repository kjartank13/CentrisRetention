"use strict";

angular.module("retentionApp").controller("RetentionListController",
function($scope, $location, $stateParams, RetentionResource) {

	$scope.loadingData = false;
	var selectedStudents = [];
	var allStudents = [];
	var studentType = "Allir";

	$scope.filterOptions = [
		"Allir",
		"Staðarnemar",
		"Fjarnemar"
	];

	RetentionResource.getAllStudents().success(function(data){
		for (var i = 0; i < data.length; i++) {
			data[i].RiskFactor = Number(data[i].RiskFactor).toFixed(2);
		}
		data.sort(function(a, b) {
			var sortByFactor = b.RiskFactor - a.RiskFactor;
			if (sortByFactor !== 0) {
				return sortByFactor;
			}
			if (a.Name < b.Name) {
				return -1;
			}
			if (a.Name > b.Name) {
				return 1;
			}
			return 0;
		});
		$scope.students = data;
		allStudents = data;
	}).then(function () {
		$scope.loadingData = true;
	});

	var queryString = $location.search();
	var defaultDepartmentID = queryString.department;
	$scope.DepartmentID = 0;

	$scope.switchSelected = function switchSelected(filter) {
		studentType = filter;
		updateList();
	};

	var typeToBool = function typeToBool() {
		if (studentType === "Allir") {
			return undefined;
		} else if (studentType === "Staðarnemar") {
			return true;
		} else if (studentType === "Fjarnemar") {
			return false;
		}
	};

	var updateList = function updateList() {
		selectedStudents = [];

		if (Number($scope.DepartmentID) === 0) {
			if (studentType === "Allir") {
				selectedStudents = allStudents;
			} else if (studentType === "Staðarnemar") {
				for (var j = 0; j < allStudents.length; j++) {
					if (allStudents[j].LocalStudent === typeToBool()) {
						selectedStudents.push(allStudents[j]);
					}
				}
			} else if (studentType === "Fjarnemar") {
				for (var k = 0; k < allStudents.length; k++) {
					if (allStudents[k].LocalStudent === typeToBool()) {
						selectedStudents.push(allStudents[k]);
					}
				}
			} else {
				//Error handling
				console.log("Student-type Error");
			}

			$scope.students = selectedStudents;
			return;
		}

		for (var i = 0; i < allStudents.length; i++) {
			if (studentType === "Allir") {
				if (Number(allStudents[i].DepartmentID) === $scope.DepartmentID) {
					selectedStudents.push(allStudents[i]);
				}
			} else {
				if (Number(allStudents[i].DepartmentID) === $scope.DepartmentID && allStudents[i].LocalStudent === typeToBool()) {
					selectedStudents.push(allStudents[i]);
				}
			}
		}
		$scope.students = selectedStudents;
	};

	$scope.updateDepartment = function updateDepartment(department) {
		if (department === undefined) {
			$scope.DepartmentID = 0;
		} else {
			$scope.DepartmentID = department.ID;
		}
		$scope.isSearching = true;
		$location.search("department", $scope.DepartmentID);
		updateList();
	};

});