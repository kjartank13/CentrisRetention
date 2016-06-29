using System.Collections.Generic;
using System.Web.Http;
using StudentRetentionAPI.Services.Services;
using StudentRetentionAPI.Services.Repositories;
using StudentRetentionAPI.Models.ModelsDTO;
using Newtonsoft.Json.Linq;

// We don't need XML comments for this class:
#pragma warning disable 1591

namespace StudentRetentionAPI.Controllers
{
	[RoutePrefix("api/retention")]
	public class RetentionController : ApiController
	{
		private readonly APIService _service;

		public RetentionController()
		{
			_service = new APIService(new UnitOfWork<AppDataContext>());
		}

		[HttpGet]
		[Route("")]
		public List<StudentDTO> GetAllStudents()
		{
			return _service.getAllStudents();
		}

		[HttpGet]
		[Route("{ssn}/details/")]
		public StudentDTO GetStudent(string ssn)
		{
			return _service.getStudent(ssn);
		}

		[HttpGet]
		[Route("{ssn}/details/overview")]
		public List<StudenthistoryDTO> GetStudentHistory(string ssn)
		{
			return _service.getStudentHistory(ssn);
		}

		[HttpGet]
		[Route("{ssn}/details/getcommunications")]
		public List<CommunicationDTO> GetCommunications(string ssn)
		{
			return _service.getCommunications(ssn);
		}

		[HttpPost]
		[Route("{ssn}/details/postcommunications")]
		public void PostCommunication(JObject data)
		{
			_service.postCommunication(data);
		}
	}
}