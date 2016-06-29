using Newtonsoft.Json.Linq;
using StudentRetentionAPI.Models.ModelsDTO;
using StudentRetentionAPI.Services.Models.Entitys;
using StudentRetentionAPI.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentRetentionAPI.Services.Services
{
	public class APIService
	{
		private readonly IUnitOfWork                 _uow;
		private readonly IRepository<Student>        _students;
		private readonly IRepository<StudentHistory> _studenthistory;
		private readonly IRepository<Communication>  _communications;

		public APIService(IUnitOfWork uow)
		{
			_uow            = uow;
			_students       = uow.GetRepository<Student>();
			_studenthistory = uow.GetRepository<StudentHistory>();
			_communications = uow.GetRepository<Communication>();
		}

		/// <summary>
		/// Cast Student to StudentDTO
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public StudentDTO Student2StudentDTO(Student s)
		{
			return new StudentDTO
			{
				Name         = s.Name,
				SSN          = s.SSN,
				DepartmentID = s.DepartmentID,
				MajorID      = s.MajorID,
				MajorName    = s.MajorName,
				RiskFactor   = s.RiskFactor,
				LocalStudent = s.LocalStudent
			};
		}

		/// <summary>
		/// Cast StudentHistory to StudentHistoryDTO
		/// </summary>
		/// <param name="sh"></param>
		/// <returns></returns>
		public StudenthistoryDTO Studenthistory2StudenthistoryDTO(StudentHistory sh)
		{
			return new StudenthistoryDTO
			{
				StudentID   = sh.StudentID,
				Date        = sh.Date,
				RiskFactor  = sh.RiskFactor,
				DeltaReason = sh.DeltaReason,
				MaxFactor   = sh.MaxFactor
			};
		}

		/// <summary>
		/// Cast Communication to CommunicationDTO
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public CommunicationDTO Communication2CommunicationDTO(Communication c)
		{
			return new CommunicationDTO
			{
				SSN     = c.SSN,
				Date    = c.Date,
				Message = c.Message
			};
		}

		/// <summary>
		/// Returns List of all students
		/// </summary>
		/// <returns></returns>
		public List<StudentDTO> getAllStudents()
		{
			var studentList               = _students.All().ToList();
			List<StudentDTO> listToReturn = new List<StudentDTO>();

			foreach (var s in studentList)
			{
				listToReturn.Add(Student2StudentDTO(s));
			}
			return listToReturn;
		}

		/// <summary>
		/// Returns a single student by given ssn
		/// </summary>
		/// <param name="SSN"></param>
		/// <returns></returns>
		public StudentDTO getStudent(string SSN)
		{
			var studentToGet = _students.All().FirstOrDefault(x => x.SSN == SSN);
			return Student2StudentDTO(studentToGet);
		}

		/// <summary>
		/// Returns student history that has the given SSN
		/// </summary>
		/// <param name="SSN"></param>
		/// <returns></returns>
		public List<StudenthistoryDTO> getStudentHistory(string SSN)
		{
			var student     = _students.All().FirstOrDefault(x => x.SSN == SSN);
			var historyList = _studenthistory.All().Where(y => y.StudentID == student.ID).ToList();
			List<StudenthistoryDTO> listToReturn = new List<StudenthistoryDTO>();
			foreach (var h in historyList)
			{
				listToReturn.Add(Studenthistory2StudenthistoryDTO(h));
			}
			return listToReturn;
		}

		/// <summary>
		/// Returns all communication for a single student wit hgiven SSN
		/// </summary>
		/// <param name="SSN"></param>
		/// <returns></returns>
		public List<CommunicationDTO> getCommunications(string SSN)
		{
			var communicationsList = _communications.All().Where(x => x.SSN == SSN).ToList();
			var listToReturn = new List<CommunicationDTO>();
			foreach (var c in communicationsList)
			{
				listToReturn.Add(Communication2CommunicationDTO(c));
			}
			return listToReturn;
		}

		public void postCommunication(JObject data)
		{
			var communication = new Communication
			{
				SSN     = data["SSN"].ToString(),
				Date    = data["date"].ToString(),
				Message = data["message"].ToString()
			};
			_communications.Add(communication);
			_uow.Save();
		}
	}
}