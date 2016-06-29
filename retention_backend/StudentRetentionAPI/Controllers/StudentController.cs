using StudentRetentionAPI.Models.ModelsDTO;
using StudentRetentionAPI.Services.Repositories;
using StudentRetentionAPI.Services.Services;
using System.Collections.Generic;

namespace StudentRetentionAPI.Controllers
{
	/// <summary>
	/// This controller will populate our front end
	/// </summary>
	public class StudentController
	{
		private readonly StudentService _service;

		/// <summary>
		/// constructor func to impl uow with appdatacontext
		/// </summary>
		public StudentController()
		{
			_service = new StudentService(new UnitOfWork<AppDataContext>());
		}

		/// <summary>
		/// gets students at this moment only fake list
		/// </summary>
		/// <returns></returns>
		public List<StudentDTO> GetStudents()
		{
			var list = new List<StudentDTO>
			{
				new StudentDTO
				{
					Name       = "Daníel",
					SSN        = "1203735289",
					RiskFactor = 0.33
				}
			};
			return list;
		}
	}
}