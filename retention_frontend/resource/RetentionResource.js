"use strict";

angular.module("retentionApp").factory("RetentionResource", 
function($http, ENV) {
	return {
		getAllStudents: function() {
			return $http.get(ENV.retentionApiEndpoint + "/api/retention/");
		},
		getSingleStudent: function(SSN) {
			return $http.get(ENV.retentionApiEndpoint + "/api/retention/" + SSN + "/details/");
		},
		getStudentHistory: function(SSN) {
			return $http.get(ENV.retentionApiEndpoint + "/api/retention/" + SSN + "/details/overview");
		},
		getCommunications: function(SSN) {
			return $http.get(ENV.retentionApiEndpoint + "/api/retention/" + SSN + "/details/getcommunications");
		},
		postCommunication: function(SSN, date, message) {
			var obj = {
				SSN: SSN,
				date: date,
				message: message
			};
			var data = JSON.stringify(obj);
			return $http.post(ENV.retentionApiEndpoint + "/api/retention/" + SSN + "/details/postcommunications", data);
		}
	};
});