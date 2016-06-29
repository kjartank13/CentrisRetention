"use strict";

describe("RetentionDetailsController", function() {

	beforeEach(module("retentionApp"));
	beforeEach(module("sharedServices"));
	beforeEach(module("mockConfig"));

	var SSN = "6969696969";
	var retentionDetailsController, scope;
	var rootScope;
	var $location;
	var $mockState = {
		current: {
			name: "retentiondetails"
		}
	};

	$mockState.go = function(route) {};
	$mockState.is = function(route) {};
	var mockStateParams = {ssn: "6969696969"};

	var mockStudent = [{
		Name: "Johnny Bravo",
		SSN: "6969696969",
		DepartmentID: "11",
		MajorID: "111",
		MajorName: "Pretty mama",
		RiskFactor: 69.0,
		LocalStudent: true
	}];

	var mockResource = {
		getSingleStudent: function getSingleStudent(SSN) {
			return {
				success: function(fn) {
					fn(mockStudent);
				}
			};
		}
	};

	beforeEach(function () {
		spyOn(mockResource, "getSingleStudent").and.callThrough();
	});

	beforeEach(inject(function($controller, $rootScope, $location, $state, $stateParams, RetentionResource) {
		rootScope = $rootScope;
		scope = $rootScope.$new();
		retentionDetailsController = $controller("RetentionDetailsController", {
			$scope: scope,
			$location: $location,
			$state: $mockState,
			$stateParams: mockStateParams,
			RetentionResource: mockResource
		});
	}));


	it ("should have called getSingleStudent with SSN", function () {
		expect(mockResource.getSingleStudent).toHaveBeenCalledWith(SSN);
	});

	it ("should define scope.go", function() {
		expect(scope.go).toBeDefined();
	});

	it ("should define scope.active", function() {
		expect(scope.active).toBeDefined();
	});

	it ("should call $state.go from scope.go", function () {
		spyOn($mockState, "go");
		scope.go(scope.tabs[0].route);
		expect($mockState.go).toHaveBeenCalledWith(scope.tabs[0].route);
	});

	it("should call $state.is from scope.active", function() {
		spyOn($mockState, "is");
		scope.active(scope.tabs[0].route);
		expect($mockState.is).toHaveBeenCalledWith(scope.tabs[0].route);
	});

	it ("should call go at statechange when state name is retentiondetails", function () {
		spyOn($mockState, "go");
		rootScope.$broadcast("$stateChangeSuccess");
		expect($mockState.go).toHaveBeenCalled();
	});

	it ("should have called state.is when state name isn't retentiondetails", function () {
		$mockState.current.name = "false_state";
		spyOn($mockState, "is");
		rootScope.$broadcast("$stateChangeSuccess");
		expect($mockState.is).toHaveBeenCalled();
	});

});