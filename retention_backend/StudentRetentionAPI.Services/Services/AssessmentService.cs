using StudentRetentionAPI.Services.Models.Entitys;
using StudentRetentionAPI.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace StudentRetentionAPI.Services.Services
{
	public class AssessmentService
	{
		private readonly IUnitOfWork                 _uow;
		private readonly IRepository<Student>        _students;
		private readonly IRepository<Grade>          _grades;
		private readonly IRepository<Assignment>     _assignments;
		private readonly IRepository<StudentHistory> _studenthistory;
		private readonly StudentService              _studentService;

		/// <summary>
		/// Variables that decide how much studentfactor should change
		/// This is kept in Web.confic since theese values might need to change easily
		/// </summary>
		double asessmentView;
		double lateHandin;
		double comment;
		double handin;

		public AssessmentService(IUnitOfWork uow)
		{
			_uow            = uow;
			_students       = uow.GetRepository<Student>();
			_grades         = uow.GetRepository<Grade>();
			_assignments    = uow.GetRepository<Assignment>();
			_studenthistory = uow.GetRepository<StudentHistory>();
			_studentService = new StudentService(uow);
		}

		/// <summary>
		/// This function calculates accessment when grades are posted
		/// </summary>
		/// <param name="assId"></param>
		/// <param name="reason"></param>
		public void AssessGrade(string assId, string reason)
		{
			lateHandin       = Convert.ToDouble(ConfigurationManager.AppSettings.Get("LateHandin"));
			var assignmentid = int.Parse(assId);
			var weight       = 0.0;

			var gradeList  = _grades.All().Where(x => x.AssignmentID == assignmentid).ToList();
			var assignment = _assignments.All().SingleOrDefault(x => x.AssignmentID == assignmentid);
			if (assignment != null)
			{
				weight = assignment.Weight;
				Console.WriteLine(weight);
			}
			if (weight == 0.0)
			{
				weight = 1;
			}
			foreach (var g in gradeList)
			{
				var factorChange = 0.0;
				var student      = _students.All().SingleOrDefault(x => x.ID == g.StudentID);
				if (g.AssignmentGrade < 5 && g.AssignmentGrade != 0.0)
				{
					factorChange += (Math.Sqrt(5 - g.AssignmentGrade)*(weight/5));
				}
				else if (g.AssignmentGrade >= 5)
				{
					factorChange -= (Math.Sqrt(g.AssignmentGrade - 4) * (weight / 5));
				}
				else if (g.AssignmentGrade == 0.0)
				{
					factorChange += weight/4;
				}

				if (g.LateHandin == "True")
				{
					factorChange += lateHandin;
				}
				// ReSharper disable once PossibleNullReferenceException
				student.RiskFactor += factorChange;
				StudentRiskFactor(student);
				UpdateStudentHistory(student, factorChange, reason);
				_uow.Save();
			}
		}

		/// <summary>
		/// This function gets called when student views somthing like lecture or notes
		/// Calculates new assesment
		/// </summary>
		/// <param name="ssn"></param>
		public void AssessView(string ssn)
		{
			asessmentView       = Convert.ToDouble(ConfigurationManager.AppSettings.Get("AsessmentView"));
			var student         = _studentService.GetStudentFunc(ssn);
			var factorChange    = -asessmentView;
			student.RiskFactor += factorChange;

			StudentRiskFactor(student);
			UpdateStudentHistory(student, factorChange, null);
			_uow.Save();
		}

		/// <summary>
		/// This function calculates new assecement
		/// When student comments or shows activity on chatboards
		/// </summary>
		/// <param name="ssn"></param>
		public void AssessComment(string ssn)
		{
			comment             = Convert.ToDouble(ConfigurationManager.AppSettings.Get("Comment"));
			var student         = _studentService.GetStudentFunc(ssn);
			var factorChange    = -comment;
			student.RiskFactor += factorChange;
			//Risk factor cannot go above 100% or below 0%
			StudentRiskFactor(student);
			UpdateStudentHistory(student, factorChange, null);
			_uow.Save();
		}

		/// <summary>
		/// This funtion gets called when students hand in paper/onlineexam/projects
		/// and calculates new assecment
		/// </summary>
		/// <param name="members">Team members in the handin</param>
		/// <param name="isLateHandin">Was the assignment handed in late?</param>
		/// <param name="reason">Reason for change in risk factor</param>
		public void AssessHandin(List<string> members, bool isLateHandin, string reason)
		{
			handin = Convert.ToDouble(ConfigurationManager.AppSettings.Get("Handin"));

			foreach (var m in members)
			{
				var student         = _studentService.GetStudentFunc(m);
				var factorChange    = -0.5;

				if (isLateHandin)
				{
					factorChange += 0.25;
				}
				if (members.Count > 1)
				{
					factorChange -= handin;
				}

				student.RiskFactor += factorChange;
				StudentRiskFactor(student);
				UpdateStudentHistory(student, factorChange, reason);
				_uow.Save();
			}
		}

		/// <summary>
		/// Update the risk factor in the student's retention history
		/// </summary>
		/// <param name="s"> The Student </param>
		/// <param name="factorChange"> How much is the factor changing </param>
		/// <param name="reason"> Why is the factor changing </param>
		public void UpdateStudentHistory(Student s, double factorChange, string reason)
		{
			factorChange = Math.Abs(factorChange);
			var sh = _studenthistory.All().SingleOrDefault(x => x.StudentID == s.ID && x.Date == DateTime.Today);
			if (sh == null)
			{
				StudentHistory sh2 = new StudentHistory
				{
					StudentID   = s.ID,
					Date        = DateTime.Today,
					RiskFactor  = s.RiskFactor,
					DeltaReason = reason,
					MaxFactor   = factorChange
				};

				_studenthistory.Add(sh2);
				_uow.Save();
			}
			else
			{
				sh.RiskFactor = s.RiskFactor;
				if (factorChange >= sh.MaxFactor)
				{
					sh.MaxFactor   = factorChange;
					sh.DeltaReason = reason;
				}
				_uow.Save();
			}
		}

		/// <summary>
		/// Risk factor cannot go above 100% or below 0%
		/// </summary>
		/// <param name="student"></param>
		public void StudentRiskFactor(Student student)
		{
			if (student.RiskFactor > 100)
			{
				student.RiskFactor = 100;
			}
			else if (student.RiskFactor < 0)
			{
				student.RiskFactor = 0;
			}
		}
	}
}