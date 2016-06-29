"use strict";

describe("CommunicationsController", function(){

	beforeEach(module("retentionApp"));
	beforeEach(module("sharedServices"));
	beforeEach(module("mockConfig"));

	var scope;
	var CommunicationsController;
	var SSN = "1010102020";
	var state;
	var stateParams;
	var location;

	var mockCommunicationsList = [{
		SSN: "1010102020",
		Date: "01/01",
		Message: "Bauð nemanda aukatíma"
	}, {
		SSN: "1010102020",
		Date: "02/01",
		Message: "Setti nemanda í vinnuhóp"
	}];

	var mockResource = {
		getCommunications: function getCommunications(SSN) {
			return {
				success: function(fn) {
					fn(mockCommunicationsList);
				}
			};
		},
		postCommunication: function postCommunication(SSN, refinedDate, message) {
			return {
				success: function(fn) {
					fn();
					return {
						then: function(fn) {
							fn();
						}
					};
				}
			};
		}
	};

	var mockUserService = {
		getFullName: function getFullName() {
			return "Bruce Lee";
		}
	};

	beforeEach(function() {
		spyOn(mockResource, "getCommunications").and.callThrough();
	});

	beforeEach(inject(function ($controller, $rootScope, _$location_, _$state_, _$stateParams_) {
		scope = $rootScope.$new();
		scope.message = "This is a test";
		scope.user = "";
		location = _$location_;
		state = _$state_;
		stateParams = _$stateParams_;
		stateParams.ssn = "1010102020";
		CommunicationsController = $controller("CommunicationsController", {
			$scope:				scope,
			$location:			location,
			$state:				state,
			$stateParams:		stateParams,
			RetentionResource:	mockResource,
			UserService:		mockUserService
		});
	}));

	it ("should define the onSubmit function", function() {
		expect(scope.onSubmit).toBeDefined();
	});

	it ("should define the current username", function() {
		scope.user = mockUserService.getFullName();
		expect(scope.user).toBe("Bruce Lee");
	});

	it ("should define the communications list", function() {
		expect(scope.communications).toBeDefined();
	});

	it ("should call getCommunications at startup with SSN", function() {
		expect(mockResource.getCommunications).toHaveBeenCalledWith(SSN);
	});

	it ("should call postCommunication in onSubmit", function() {
		spyOn(mockResource, "postCommunication").and.callThrough();
		scope.onSubmit();
		expect(mockResource.postCommunication).toHaveBeenCalled();
	});

});