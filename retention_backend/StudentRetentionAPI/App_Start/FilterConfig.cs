using System.Web.Mvc;

namespace StudentRetentionAPI
{
	/// <summary>
	/// This guy needs comment aswell
	/// </summary>
	public class FilterConfig
	{
		/// <summary>
		/// This guy needs comment aswell
		/// </summary>
		/// <param name="filters"></param>
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}
