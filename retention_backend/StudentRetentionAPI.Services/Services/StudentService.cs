using StudentRetentionAPI.Services.Repositories;
using Newtonsoft.Json.Linq;
using System;
using StudentRetentionAPI.Services.Models.Entitys;
using System.Linq;
using System.Threading;

namespace StudentRetentionAPI.Services.Services
{
	/// <summary>
	/// Repo instances at top;
	/// This class will take care of all actions needed for students Get Post to database
	/// and changes made to data from the database.
	/// </summary>
	public class StudentService
	{
		private readonly IUnitOfWork             _uow;
		private readonly IRepository<Student>    _students;
		private readonly IRepository<Enrollment> _enrollments;
		private readonly IRepository<Course>     _courses;
		private readonly IRepository<Assignment> _assignments;
		private readonly IRepository<Grade>      _grades;
		private readonly ConnectToCentris        _connecttocentris;

		/// <summary>
		/// Constructor take in unit of work to sepperate test from real data.
		/// instance set to all database tables needed.
		/// </summary>
		/// <param name="uow"></param>
		public StudentService(IUnitOfWork uow)
		{
			_uow              = uow;
			_connecttocentris = new ConnectToCentris(_uow);
			_students         = uow.GetRepository<Student>();
			_enrollments      = uow.GetRepository<Enrollment>();
			_courses          = uow.GetRepository<Course>();
			_assignments      = uow.GetRepository<Assignment>();
			_grades           = uow.GetRepository<Grade>();
		}

		/// <summary>
		/// This function deletes semesters older than 2 years
		/// should be checked
		/// </summary>
		public void RemoveOldSemesters()
		{
			var year          = (DateTime.Now.Year - 2).ToString();
			var semesterOne   = year + "1";
			var semesterTwo   = year + "2";
			var semesterThree = year + "3";
			RemoveSemester(semesterOne);
			RemoveSemester(semesterTwo);
			RemoveSemester(semesterThree);
		}

		/// <summary>
		/// Removes all courses and all Enrollments from each course from a given semesterID
		/// Future realease might add delete to assignments and grades
		/// </summary>
		/// <param name="semesterToRemove"></param>
		public void RemoveSemester(string semesterToRemove)
		{
			var remove = _courses.All().Where(x => x.Semester == semesterToRemove).ToList();

			if (remove.Count < 1)
			{
				return;
			}
			foreach (var course in remove)
			{
				var getEnrollmentList = _enrollments.All().Where(x => x.CourseID == course.ID).ToList();

				foreach (var enrollment in getEnrollmentList)
				{
					_enrollments.Delete(enrollment);
				}
				_courses.Delete(course);
			}
			_uow.Save();
		}

		/// <summary>
		/// This function starts a new semester gets list of all students and adds them to our database
		/// </summary>
		public void NewSemester()
		{
			var courselist = _connecttocentris.GetCourses();

			foreach (var course in courselist.Children<JObject>())
			{
				AddCourse(course);
			}

			var datalist = _connecttocentris.GetStudentList();

			foreach (var data in datalist.Children<JObject>())
			{
				AddStudent(data);
			}
		}

		/// <summary>
		/// This function adds students to given courses into the Enrollment table
		/// </summary>
		/// <param name="item"></param>
		public void AddEnrollment(JObject item)
		{
			var ssn     = item["SSN"].ToString();
			var student = GetStudentFunc(ssn);

			if (student == null)
			{
				return;
			}

			if (item["Courses"] == null || item["Courses"].Count() < 1)
			{
				return;
			}

			foreach (var data in item["Courses"])
			{
				var courseid         = data.Value<int>();
				var enrollmentexists = _enrollments.All().FirstOrDefault(x => x.StudentID == student.ID && x.CourseID == courseid);

				if (enrollmentexists == null)
				{
					var newenrollment = new Enrollment
					{
						CourseID  = courseid,
						StudentID = student.ID,
					};
					_enrollments.Add(newenrollment);
					_uow.Save();
				}
			}
		}

		/// <summary>
		/// This function deletes a student from our system first all grades then all enrollments and then the student it self.
		/// </summary>
		/// <param name="data"></param>
		public void RemoveStudent(JObject data)
		{
			var ssn            = data["SSN"].ToString();
			var studentexists2 = GetStudentFunc(ssn);

			if (studentexists2 == null)
			{
				return;
			}
			var studentenrollment = _enrollments.All().Where(x => x.StudentID == studentexists2.ID).ToList();
			var studentgrade      = _grades.All().Where(x => x.StudentID == studentexists2.ID).ToList();

			foreach (var s in studentgrade)
			{
				_grades.Delete(s);
			}
			foreach (var s in studentenrollment)
			{
				_enrollments.Delete(s);
			}
			_students.Delete(studentexists2);
			_uow.Save();
		}

		/// <summary>
		/// This function adds a new student to our database.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public Student AddStudent(JObject data)
		{
			var ssn           = data["SSN"].ToString();
			var studentexists = GetStudentFunc(ssn);

			if (studentexists == null)
			{
				var newstudent = new Student
				{
					Name         = data["Name"].ToString(),
					SSN          = ssn,
					DepartmentID = data["DepartmentID"].ToString(),
					MajorID      = data["Major"]["ID"].ToString(),
					MajorName    = data["Major"]["Name"].ToString(),
					RiskFactor   = 33.33,
					LocalStudent = true,
				};
				_students.Add(newstudent);
				_uow.Save();
				AddEnrollment(data);
				return newstudent;
			}
			studentexists.DepartmentID = data["DepartmentID"].ToString();
			studentexists.MajorID      = data["Major"]["ID"].ToString();
			studentexists.MajorName    = data["Major"]["Name"].ToString();
			_uow.Save();
			return studentexists;
		}

		/// <summary>
		/// This function adds a course to a semester
		/// </summary>
		/// <param name="data"></param>
		public void AddCourse(JObject data)
		{
			var courseid     = int.Parse(data["ID"].ToString());
			var semester     = data["Semester"].ToString();
			var courseexists = GetCourseFunc(courseid, semester);

			if (courseexists == null)
			{
				var newCourse = new Course
				{
					ID       = courseid,
					Semester = semester
				};

				_courses.Add(newCourse);
				_uow.Save();
			}
		}

		/// <summary>
		/// This function adds a new student to a specific course.
		/// </summary>
		/// <param name="item"></param>
		public void StudentJoinCourse(JObject item)
		{
			var courseid = int.Parse(item["Item"]["Course"]["InstanceID"].ToString());
			var ssn      = item["Item"]["Student"]["SSN"].ToString();
			var student  = GetStudentFunc(ssn);
			if (student == null)
			{
				return;
			}

			var studentenrollment = _enrollments.All().FirstOrDefault(x => x.StudentID == student.ID && x.CourseID == courseid);
			if (studentenrollment == null)
			{
				var enroll = new Enrollment
				{
					StudentID = student.ID,
					CourseID  = courseid
				};
				_enrollments.Add(enroll);
				_uow.Save();
			}
		}

		/// <summary>
		/// /// This function calls Centris api with a Assignment ID and calls for all grades for assignment
		/// This will only be called when a message arrives from queue saying that grades have been published 
		/// for specific assignment.
		/// </summary>
		/// <param name="courseid"></param>
		/// <param name="assignmentid"></param>
		public void GetGrades(string courseid, string assignmentid)
		{
			var datalist = _connecttocentris.GetGrades(courseid, assignmentid);

			foreach (var item in datalist.Children<JObject>())
			{
				var assid = int.Parse(assignmentid);
				AddGrade(item, assid);
			}
		}

		/// <summary>
		/// This function adds a grade to to Grades table with given assignementID and gets needed information from a JObject
		/// </summary>
		/// <param name="data"></param>
		/// <param name="assignmentid"></param>
		/// <returns></returns>
		public Grade AddGrade(JObject data, int assignmentid)
		{
			var ssn             = data["SSN"].ToString();
			var tempgrade       = new Grade();
			var assignmentgrade = data["Grade"];
			var grade           = Convert.ToDouble(string.IsNullOrEmpty((string) assignmentgrade) ? 0.0 : assignmentgrade);
			var getassignment   = GetAssignmentFunc(assignmentid);
			var latehandin      = data["LateHandinPenalty"].ToString();

			if (getassignment == null)
			{
				getassignment = AddAssignment(assignmentid.ToString(), 0.0);
			}
			var getstudent = GetStudentFunc(ssn);

			if (getassignment != null && getstudent != null)
			{
				tempgrade = GetGradeFunc(getassignment.AssignmentID, getstudent.ID);
			}

			// If grade for assigment of specific student does not exist we add it to grades table
			if (tempgrade == null)
			{
				var newgrade = new Grade
				{
					StudentID       = getstudent.ID,
					AssignmentID    = assignmentid,
					AssignmentGrade = grade,
					LateHandin      = latehandin
				};
				_grades.Add(newgrade);
				_uow.Save();
				return newgrade;
			}
			else
			{
				// If grade exists we go here and update it
				tempgrade.AssignmentGrade = grade;
				_grades.Update(tempgrade);
				_uow.Save();
				return tempgrade;
			}
		}

		/// <summary>
		/// This function Adds a assignment to the database
		/// </summary>
		/// <param name="assignmentid"></param>
		/// <param name="weight"></param>
		/// <returns></returns>
		public Assignment AddAssignment(string assignmentid, double weight)
		{
			var assid         = int.Parse(assignmentid);
			var getassignment = GetAssignmentFunc(assid);

			if (getassignment == null)
			{
				//create new assignment
				var newassignment = new Assignment
				{
					AssignmentID = assid,
					Weight       = weight
				};
				_assignments.Add(newassignment);
				_uow.Save();
				return newassignment;
			}
			else
			{
				//Check if assignment is different...
				//i.e. it's weight has been changed.
				//if so, update weight.
				if (getassignment.Weight != weight)
				{
					getassignment.Weight = weight;
					_uow.Save();
				}
			}
			return getassignment;
		}

		/// <summary>
		/// This function checks if a assignment exists with a given id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Assignment GetAssignmentFunc(int id)
		{
			return _assignments.All().SingleOrDefault(x => x.AssignmentID == id);
		}

		/// <summary>
		/// This Function checks if a student exists with given social secutiry number
		/// </summary>
		/// <param name="ssn"></param>
		/// <returns></returns>
		public Student GetStudentFunc(string ssn)
		{
			var getStudentList = _students.All().Where(x => x.SSN == ssn).ToList();
			Student getStudent = null;

			foreach (var r in getStudentList)
			{
				if (r.SSN == ssn)
				{
					getStudent = r;
				}
			}
			return getStudent;
		}

		/// <summary>
		/// This Function returns a single Enrollment given by specific StudentID and CourseID
		/// </summary>
		/// <param name="studentId"></param>
		/// <param name="courseid"></param>
		/// <returns></returns>
		public Enrollment GetEnrollmentFunc(int studentId, int courseid)
		{
			var getEnrollment = _enrollments.All().SingleOrDefault(x => x.StudentID == studentId && x.CourseID == courseid);
			return getEnrollment;
		}

		/// <summary>
		/// This function checks if a grade already exists with given assignementID and StudentID
		/// </summary>
		/// <param name="assId"></param>
		/// <param name="studentId"></param>
		/// <returns></returns>
		public Grade GetGradeFunc(int assId, int studentId)
		{
			var getGradeList = _grades.All().Where(x => x.StudentID == studentId && x.AssignmentID == assId).ToList();
			Grade getGrade   = null;
			foreach (var r in getGradeList)
			{
				if (r.StudentID == studentId && r.AssignmentID == assId)
				{
					getGrade = r;
				}
			}
			return getGrade;
		}

		/// <summary>
		/// This fuction gets a specific course with given courseid and given semester
		/// </summary>
		/// <param name="courseid"></param>
		/// <param name="semester"></param>
		/// <returns></returns>
		public Course GetCourseFunc(int courseid, string semester)
		{
			var getCourseList = _courses.All().Where(x => x.ID == courseid && x.Semester == semester).ToList();
			Course getCourse  = null;

			foreach (var r in getCourseList)
			{
				if (r.ID == courseid && r.Semester == semester)
				{
					getCourse = r;
				}
			}
			return getCourse;
		}

		/// <summary>
		/// Every day, call this to increment all
		/// student risk factors by one.
		/// </summary>
		public void IncrementAllStudentsByOne()
		{
			var allStudents = _students.All();
			foreach (var s in allStudents)
			{
				if (s.RiskFactor < 99)
				{
					s.RiskFactor += 1;
				}
				else
				{
					s.RiskFactor = 100;
				}
			}
			_uow.Save();
		}
	}
}