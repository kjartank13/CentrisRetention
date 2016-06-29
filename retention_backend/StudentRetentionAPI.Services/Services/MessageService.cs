using System;
using System.Threading;
using Newtonsoft.Json.Linq;
using StudentRetentionAPI.Services.Repositories;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace StudentRetentionAPI.Services.Services
{
	public class MessageService
	{
		private readonly IUnitOfWork       _uow;
		private readonly StudentService    _studentService;
		private readonly AssessmentService _assessmentService;

		/// <summary>
		/// Setup for Unit Of Work Patters 
		/// </summary>
		/// <param name="uow"></param>
		public MessageService(IUnitOfWork uow)
		{
			_uow               = uow;
			_studentService    = new StudentService(uow);
			_assessmentService = new AssessmentService(uow);
		}

		/// <summary>
		/// This function takes in a JArray of all events that have happened and need to be processed
		/// The array is sorted oldest first newest last
		/// </summary>
		/// <param name="list"></param>
		public void WorkOnJson(JArray list)
		{
			foreach (var item in list)
			{
				var content = item.ToString();
				var x       = JObject.Parse(content);

				//Splitting the key at each '.'
				//Becomes a string array, u, of length 4
				//u[0] is Type
				//u[1] is Subtype
				//u[2] is Operation
				//u[3] is Body
				var u = x["keys"].ToString().Split('.');

				#region calling type functions

				if (u[0] == "Topic")
				{
					TopicMessage(u, x);
				}
				else if (u[0] == "Discussion")
				{
					DiscussionMessage(u, x);
				}
				else if (u[0] == "Students")
				{
					StudentMessage(u, x);
				}
				else if (u[0] == "Assignment")
				{
					AssignmentMessage(u, x);
				}
				else if (u[0] == "OnlineExam")
				{
					OnlineExamMessage(u, x);
				}

				#endregion
			}
		}

		#region Type functions

		public void TopicMessage(string[] key, JObject message)
		{
			if (key[3] == "View")
			{
				var ssn = message["Item"]["SSN"].ToString();
				_assessmentService.AssessView(ssn);
			}
		}

		public void DiscussionMessage(string[] key, JObject message)
		{
			if (key[2] == "Thread" && key[3] == "Create")
			{
				var ssn = message["Item"]["SSN"].ToString();
				_assessmentService.AssessComment(ssn);
			}
		}

		public void StudentMessage(string[] key, JObject message)
		{
			if (key[1] == "Registrations")
			{
				if (key[2] == "Create")
				{
					var studenttoadd = JObject.Parse(message["Item"]["Student"].ToString());
					studenttoadd.Add("DepartmentID", message["Item"]["Student"]["Department"]["ID"].ToString());
					_studentService.AddStudent(studenttoadd);
				}
			}
		}

		public void AssignmentMessage(string[] key, JObject message)
		{
			if (key[1] == "Grades")
			{
				AssignmentGrades(key, message);
			}
			else if (key[1] == "None")
			{
				if (key[2] == "Create")
				{
					AddAssignment(key, message);
				}
			}
			else if (key[1] == "Handin")
			{
				var lateHandin = false;

				if (message["Item"]["Deadline"] != null)
				{
					var handinDate = DateTime.Parse(message["Item"]["HandinDate"].ToString());
					var deadline = DateTime.Parse(message["Item"]["Deadline"].ToString());
					if (DateTime.Compare(handinDate, deadline) < 0)
					{
						lateHandin = true;
					}
				}
				var deserialize = JsonConvert.DeserializeObject<dynamic>(message.ToString());
				var members     = new List<string>();

				foreach (var x in deserialize.Item.Group.Members)
				{
					members.Add(Convert.ToString(x.SSN));
				}
				_assessmentService.AssessHandin(members, lateHandin, "Student handed assignment in.");
			}
		}

		public void OnlineExamMessage(string[] key, JObject message)
		{
			if (key[1] == "None")
			{
				if (key[2] == "Create")
				{
					AddOnlineExam(key, message);
				}
				// TODO: Get Grades for onlineExams when they are handed in
				// This is still in developement in CentrisApi so we must wait
				// for that to finish
			}
		}

		#endregion

		#region SubType functions

		public void AssignmentGrades(string[] key, JObject message)
		{
			if (key[2] == "Publish")
			{
				var courseInstanceId = message["Item"]["Course"]["InstanceID"].ToString();
				var assignmentId     = message["Item"]["Assignment"]["ID"].ToString();
				_studentService.GetGrades(courseInstanceId, assignmentId);
				_assessmentService.AssessGrade(assignmentId, "Student received grade.");
			}
		}

		public void AddOnlineExam(string[] key, JObject message)
		{
			var onlineExamId = message["Item"]["ExamInstanceID"].ToString();
			_studentService.AddAssignment(onlineExamId, 0.0);
		}

		public void AddAssignment(string[] key, JObject message)
		{
			var weight = 0.0;
			if (message["Item"]["Assessment"] != null)
			{
				weight = double.Parse(message["Item"]["Assessment"].ToString());
			}
			var assignmentId = message["Item"]["Assignment"]["ID"].ToString();
			_studentService.AddAssignment(assignmentId, weight);
		}

		#endregion
	}
}