using System.Web.Http;
using System.Web.Http.Cors;

namespace StudentRetentionAPI
{
	/// <summary>
	/// Webapi cfg to connect to entris
	/// </summary>
	public static class WebApiConfig
	{
		/// <summary>
		/// Http mapper for centris/angular
		/// </summary>
		/// <param name="config"></param>
		public static void Register(HttpConfiguration config)
		{
			config.MapHttpAttributeRoutes();
			var cors = new EnableCorsAttribute("*", "*", "*");
			config.EnableCors(cors);
			config.EnsureInitialized();
		}
	}
}