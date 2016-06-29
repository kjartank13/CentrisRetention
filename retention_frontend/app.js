"use strict";

angular.module("retentionApp", [
	"sharedServices",
	"pascalprecht.translate",
	"ui.bootstrap",
	"ui.router",
	"chart.js"
]).config(function($stateProvider, $urlRouterProvider /*, $tooltipProvider*/) {

	$stateProvider
	.state("retention", {
		url: "/retention",
		templateUrl: "retention/index/index.tpl.html",
		controller: "RetentionListController"
	})
	.state("retentiondetails", {
		url: "/retention/:ssn",
		templateUrl: "retention/components/retention-details/retentiondetails.tpl.html",
		controller: "RetentionDetailsController"
	})
	.state("retentiondetails.overview", {
		url: "/overview",
		templateUrl: "retention/components/tabs/overview/retention-overview.tpl.html",
		controller: "OverviewController"
	})
	.state("retentiondetails.communications", {
		url: "/communications",
		templateUrl: "retention/components/tabs/communications/retention-communications.tpl.html",
		controller: "CommunicationsController"
	});
/*
	$tooltipProvider.setTriggers({
		"mouseenter": "mouseleave",
		"click": "click",
		"focus": "blur",
		"never": "mouseleave",
		"now": "onload"  // <- This ensures the tooltip will go away on mouseleave
	});
*/
});