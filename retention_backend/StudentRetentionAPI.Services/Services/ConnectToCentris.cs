using System;
using System.Net.Http.Headers;
using System.Configuration;
using System.Net.Http;
using Thinktecture.IdentityModel.Clients;
using Newtonsoft.Json.Linq;
using StudentRetentionAPI.Services.Repositories;
using System.Threading;

namespace StudentRetentionAPI.Services.Services
{
	public class ConnectToCentris
	{
		private readonly string     ApiUser;
		private readonly string     ApiPassword;
		private readonly HttpClient HttpClient;
		private IUnitOfWork         uow;
		private bool                _hasToken;

		/// <summary>
		/// Constructor takes care of setting connection parameters to connect to Centris Api
		/// Connection settings are kept in App.config in Listener project and can easily be changed if we need to
		/// use localhost insted of actual Centris system.
		/// </summary>
		/// <param name="_uow"></param>
		public ConnectToCentris(IUnitOfWork _uow)
		{
			uow             = _uow;
			var apiEndpoint = new Uri(ConfigurationManager.AppSettings["APIEndpoint"]);
			ApiUser         = ConfigurationManager.AppSettings["APIUser"];
			ApiPassword     = ConfigurationManager.AppSettings["APIPass"];
			HttpClient      = new HttpClient {BaseAddress = apiEndpoint};
			HttpClient.DefaultRequestHeaders.Accept.Clear();
			HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		/// <summary>
		/// Syncronos function that sends Get request to Centris or localhost to get all students when new semester starts
		/// and returns a JSarray with all students.
		/// </summary>
		/// <returns></returns>
		public JArray GetStudentList()
		{
			#region connecting to centris

			if (!_hasToken)
			{
				var endpoint = ConfigurationManager.AppSettings["OAuth2TokenEndpoint"];
				var clientId = ConfigurationManager.AppSettings["ResourceOwnerClient"];
				var secret   = ConfigurationManager.AppSettings["ResourceOwnerClientSecret"];
				var client   = new OAuth2Client(new Uri(endpoint), clientId, secret);

				try
				{
					var token = client.RequestAccessTokenUserName(ApiUser, ApiPassword, "publicAPI");
					HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
					_hasToken = true;
				}
				catch (Exception e)
				{
					Console.WriteLine("Caught: {0}", e.Message);
				}
			}

			#endregion

			var response = HttpClient.GetAsync("semesters/current/students").Result;

			if (response.IsSuccessStatusCode)
			{
				var responseContent = response.Content;
				var responseString  = responseContent.ReadAsStringAsync().Result;
				return JArray.Parse(responseString);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Syncronos function that sends Get request to Centris to get all grades for
		/// a given assignment id and course id
		/// </summary>
		/// <param name="courseinstanceID"></param>
		/// <param name="assignmentID"></param>
		/// <returns></returns>
		public JArray GetGrades(string courseinstanceID, string assignmentID)
		{
			#region connecting to centris

			if (!_hasToken)
			{
				var endpoint = ConfigurationManager.AppSettings["OAuth2TokenEndpoint"];
				var clientId = ConfigurationManager.AppSettings["ResourceOwnerClient"];
				var secret   = ConfigurationManager.AppSettings["ResourceOwnerClientSecret"];
				var client   = new OAuth2Client(new Uri(endpoint), clientId, secret);

				try
				{
					var token = client.RequestAccessTokenUserName(ApiUser, ApiPassword, "publicAPI");
					HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
					_hasToken = true;
				}
				catch (Exception e)
				{
					Console.WriteLine("Caught: {0}", e.Message);
				}
			}

			#endregion

			var getstring = string.Format("courses/{0}/assignments/{1}/handins", courseinstanceID, assignmentID);
			var response  = HttpClient.GetAsync(getstring).Result;

			if (response.IsSuccessStatusCode)
			{
				var responseContent = response.Content;
				var responseString  = responseContent.ReadAsStringAsync().Result;
				return JArray.Parse(responseString);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Note this function gets all courses on the current semester
		/// TO get for specific semester use api/v1/courses/?semester=20133
		/// </summary>
		/// <returns></returns>
		public JArray GetCourses()
		{
			#region connecting to centris

			if (!_hasToken)
			{
				var endpoint = ConfigurationManager.AppSettings["OAuth2TokenEndpoint"];
				var clientId = ConfigurationManager.AppSettings["ResourceOwnerClient"];
				var secret   = ConfigurationManager.AppSettings["ResourceOwnerClientSecret"];
				var client   = new OAuth2Client(new Uri(endpoint), clientId, secret);

				try
				{
					var token = client.RequestAccessTokenUserName(ApiUser, ApiPassword, "publicAPI");
					HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
					_hasToken = true;
				}
				catch (Exception e)
				{
					Console.WriteLine("Caught: {0}", e.Message);
				}
			}

			#endregion

			var getstring = "courses/";
			var response  = HttpClient.GetAsync(getstring).Result;

			if (response.IsSuccessStatusCode)
			{
				var responseContent = response.Content;
				var responseString  = responseContent.ReadAsStringAsync().Result;
				return JArray.Parse(responseString);
			}
			return null;
		}
	}
}