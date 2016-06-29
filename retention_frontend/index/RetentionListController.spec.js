"use strict";

describe("RetentionListController", function(){

	beforeEach(module("retentionApp"));
	beforeEach(module("sharedServices"));
	beforeEach(module("mockConfig"));

	var RetentionListController;
	var scope;
	var state;
	var location;
	var stateParams;

	var mockStudentList = [{
		Name: "Jon Snow",
		SSN: "112233-4455",
		RiskFactor: 80.01,
		DepartmentID: 1,
		LocalStudent: true 
	}, {
		Name: "Kahl Drogo",
		SSN: "424242-4242",
		RiskFactor: 80.01,
		DepartmentID: 2,
		LocalStudent: false
	},{
		Name: "Tyrion Lannister",
		SSN: "010203-0405",
		RiskFactor: 33.22,
		DepartmentID: 3,
		LocalStudent: false
	}, {
		Name: "Arya Stark",
		SSN: "908070-6050",
		RiskFactor: 33.22,
		DepartmentID: 3,
		LocalStudent: true
	}];

	var mockResource = {
		getAllStudents: function getAllStudents() {
			return {
				success: function(fn) {
					fn(mockStudentList);
					return {
						then: function(fn) {
							fn();
						}
					};
				}
			};
		}
	};

	beforeEach(function(){
		mockResource.getAllStudents();
	});

	beforeEach(inject(function ($controller, $rootScope, _$location_, _$state_, _$stateParams_){
		scope					= $rootScope.$new();
		scope.students			= [];
		scope.DepartmentID		= 0;
		state					= _$state_;
		location				= _$location_;
		stateParams				= _$stateParams_;
		RetentionListController	= $controller("RetentionListController", {
			$scope: scope,
			$state: state,
			$location: location,
			$stateParams: stateParams,
			RetentionResource: mockResource
		});
	}));

	it ("should get a list of students from the resource, sorted by risk factor and name", function() {
		expect(scope.students).toBe(mockStudentList);
	});

	it ("should filter by department, given a DepartmentID", function() {
		var department = {
			ID: 3
		};
		scope.updateDepartment(department);
		expect(scope.students.length).toBe(2);
	});

	it ("should only show all students when using Allir as a filter", function() {
		scope.switchSelected("Allir");
		expect(scope.students.length).toBe(4);
	});

	it ("should only show local students when using Staðarnemar as a filter", function() {
		scope.switchSelected("Staðarnemar");
		expect(scope.students.length).toBe(2);
	});

	it ("should only show distance students when using Fjarnemar as a filter", function() {
		scope.switchSelected("Fjarnemar");
		expect(scope.students.length).toBe(2);
	});

	it ("should be able to filter by student type and department", function() {
		scope.switchSelected("Staðarnemar");
		var department = {
			ID: 1
		};
		scope.updateDepartment(department);
		expect(scope.students.length).toBe(1);
	});

});