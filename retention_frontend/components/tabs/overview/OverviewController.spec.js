"use strict";

describe("OverviewController", function() {

	beforeEach(module("retentionApp"));
	beforeEach(module("sharedServices"));
	beforeEach(module("mockConfig"));

	var OverviewController;
	var scope;
	var location;
	var state;
	var stateParams;
	var mockStateParams = {ssn: "6969696969"};

	var mockHistory = [{
		StudentID: 1,
		Date: "4/28/2016 12:00:00 AM",
		RiskFactor: 67.0,
		DeltaReason: "All work and no play",
		MaxFactor: 1.0
	}, {
		StudentID: 1,
		Date: "4/29/2016 12:00:00 AM",
		RiskFactor: 69.0,
		DeltaReason: "Makes Jack a dull boy",
		MaxFactor: 1.0
	}, {
		StudentID: 1,
		Date: "5/20/2016 12:00:00 AM",
		RiskFactor: 71.0,
		DeltaReason: "Redrum, redrum",
		MaxFactor: 1.0
	}];

	var mockResource = {
		getStudentHistory: function getStudentHistory(SSN) {
			return {
				success: function(fn) {
					fn(mockHistory);
					return {
						then: function(fn) {
							fn();
						}
					};
				}
			};
		}
	};

	beforeEach(inject(function ($controller, $rootScope, _$location_, _$state_, _$stateParams_) {
			scope = $rootScope.$new();
			location = _$location_;
			state = _$state_;
			stateParams = _$stateParams_;
			OverviewController = $controller("OverviewController", {
				$scope: scope,
				$location: location,
				$state: state,
				$stateParams: mockStateParams,
				RetentionResource: mockResource
		});
	}));

	it ("should define the makeGraph function", function() {
		expect(scope.makeGraph).toBeDefined();
	});

	it ("should make the graph data when calling makeGraph with the history array", function() {
		scope.makeGraph(mockHistory);
		expect(scope.labels.length).toBe(3);
		expect(scope.data.length).toBe(1);
		expect(scope.chart_options.scaleBeginAtZero).toBe(true);
	});

});