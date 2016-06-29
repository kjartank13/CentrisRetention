using System.Collections.Generic;
using StudentRetentionAPI.Services.Models.Entitys;
using System;

namespace StudentRetentionAPI.Tests.MockObjects
{
	/// <summary>
	/// This Class returns mockdata for uow and testing purpose
	/// </summary>
	class MockFactory : BaseMockFactory
	{
		#region CONSTANTS

		// A few constants which will be used througout the 
		// test data. Note: we should only declare a constant
		// if it is used in more than one place, such as if
		// it is a primary key (and hence used as a foreign
		// key in another entity).
		public const string SSN_LOA_JOHANNSDOTTIR  = "1903863419";
		public const string SSN_SNORRI_DANIELSSON  = "2403902139";
		public const string SSN_DANIEL_SIGURGEIRS  = "1203735289";
		public const string SSN_GUDRUN_SIF_HILMARS = "1708882519";
		public const string SSN_ASLAUG_SOLLILJA    = "1901902939";
		public const string SSN_TOMAS_MAGNUSSON    = "0905903249";
		public const string SSN_MAGGA_SESSELJA     = "1309862429";
		public const string SSN_BJARKI_SORENS      = "0805903269";
		public const string SSN_JON_JONSSON        = "1809802519";
		public const string SSN_EINAR_ALEXANDER    = "1309872149";
		public const string SSN_THORDIS_JONA       = "2802922309";
		public const string SSN_JOHANN_BRYNJAR     = "1804912289";
		public const string SSN_JAKOB_THORDAR      = "1301922979";
		public const string SSN_OFFICE_MANAGER     = "2104842809";
		public const string SSN_OTTAR              = "2304854539";
		public const string SSN_KJARTAN            = "1610913249";
		public const string SSN_KEVIN              = "0703892319";
		public const string SSN_INGOLFUR           = "2810872159";
		public const string SSN_NON_STUDENT_1      = "1701842519";

		public const string SEMESTER_VALID     = "20161";
		public const string SEMESTER_NOT_VALID = "9999999";

		public const string COURSE_TEMPL_ID_TOLH = "T-107-TOLH";
		public const string COURSE_TEMPL_ID_INTO = "T-109-INTO";
		public const string COURSE_TEMPL_ID_VERK = "T-110-VERK";
		public const string COURSE_TEMPL_ID_PROG = "T-111-PROG";
		public const string COURSE_TEMPL_ID_STR1 = "T-117-STR1";
		public const string COURSE_TEMPL_ID_STY1 = "T-215-STY1";

		public const int COURSE_INST_ID_TOLH_2010 = 20434;
		public const int COURSE_INST_ID_INTO_2010 = 20435;
		public const int COURSE_INST_ID_VERK_2010 = 20436;
		public const int COURSE_INST_ID_PROG_2010 = 20437;
		public const int COURSE_INST_ID_STR1_2010 = 20438;
		public const int COURSE_INST_ID_INTO_2011 = 22363;
		public const int COURSE_INST_ID_EXCH_2012 = 22971;
		public const int COURSE_INST_INVALID      = 99999;

		public const string MAJOR_ID_BSC_COMP_SCI  = "12";
		public const string MAJOR_ID_BSC_SOFTW_ENG = "236";
		public const string MAJOR_ID_BSC_SPORT_SCI = "62";
		public const string MAJOR_ID_BSC_BUSINESS  = "127";

		public const int MAJOR_TYPE_UNDERGRADUATE = 1;
		public const int MAJOR_TYPE_GRADUATE      = 2;

		public const int DEPARTMENT_ID_COMP_SCI = 1;
		public const int DEPARTMENT_ID_BUSINESS = 2;
		public const int DEPARTMENT_ID_EXEC_ED  = 3;
		public const int DEPARTMENT_ID_SCI_ENG  = 6;

		#endregion

		/// <summary>
		/// Constructor That sets all the data
		/// </summary>
		public MockFactory()
		{
			#region COURSE SETUP

			var courseList = new List<Course>
			{
				new Course
				{
					ID       = COURSE_INST_ID_TOLH_2010, // 20434
					Semester = SEMESTER_VALID
				},
				new Course
				{
					ID       = COURSE_INST_ID_INTO_2010, // 20435
					Semester = SEMESTER_VALID
				},
				new Course
				{
					ID       = COURSE_INST_ID_VERK_2010, // 20436
					Semester = SEMESTER_VALID
				},
				new Course
				{
					ID       = COURSE_INST_ID_PROG_2010, // 20437
					Semester = SEMESTER_VALID
				}
			};
			_repositories.Add(typeof(Course), courseList);

			#endregion

			#region ENROLLMENT SETUP

			var enrollmentList = new List<Enrollment>
			{
				new Enrollment
				{
					ID        = 1,
					CourseID  = COURSE_INST_ID_PROG_2010,
					StudentID = 11
				},
				new Enrollment
				{
					ID        = 2,
					CourseID  = COURSE_INST_ID_STR1_2010,
					StudentID = 12
				},
				new Enrollment
				{
					ID        = 3,
					CourseID  = COURSE_INST_ID_INTO_2011,
					StudentID = 13
				},
				new Enrollment
				{
					ID        = 4,
					CourseID  = COURSE_INST_ID_EXCH_2012,
					StudentID = 14
				}
			};
			_repositories.Add(typeof(Enrollment), enrollmentList);

			#endregion

			#region STUDENT SETUP

			var studentList = new List<Student>
			{
				new Student
				{
					ID           = 1,
					Name         = "Ingólfur Rúnar Jónsson",
					SSN          = "2810872159",
					DepartmentID = "6",
					MajorID      = "74",
					MajorName    = "BSc í hátækniverkfræði",
					RiskFactor   = 33.33,
					LocalStudent = true
				},
				new Student
				{
					ID           = 2,
					Name         = "Kevin Freyr Leósson",
					SSN          = "0703892319",
					DepartmentID = "1",
					MajorID      = "12",
					MajorName    = "BSc í tölvunarfræði",
					RiskFactor   = 33.33,
					LocalStudent = true
				},
				new Student
				{
					ID           = 3,
					Name         = "Kjartan Valur Kjartansson",
					SSN          = "1610913249",
					DepartmentID = "1",
					MajorID      = "3",
					MajorName    = "Diplóma í kerfisfræði",
					RiskFactor   = 33.33,
					LocalStudent = true
				},
				new Student
				{
					ID           = 48,
					Name         = "Óttar Helgi Einarsson",
					SSN          = SSN_OTTAR,
					DepartmentID = "1",
					MajorID      = MAJOR_ID_BSC_COMP_SCI,
					MajorName    = "Meistaranám í tölvunarfræði",
					RiskFactor   = 33.33,
					LocalStudent = true
				}
			};
			_repositories.Add(typeof(Student), studentList);

			#endregion

			#region STUDENT HISTORY SETUP

			var studentHistoryList = new List<StudentHistory>
			{
				new StudentHistory
				{
					ID          = 1,
					StudentID   = 2,
					Date        = DateTime.Today,
					RiskFactor  = 33.33,
					DeltaReason = "Student handed assignment in.",
					MaxFactor   = 2,
				},
				new StudentHistory
				{
					ID          = 2,
					StudentID   = 11,
					Date        = DateTime.Today,
					RiskFactor  = 33.33,
					DeltaReason = "Student handed assignment in.",
					MaxFactor   = 3,
				}
			};
			_repositories.Add(typeof(StudentHistory), studentHistoryList);

			#endregion

			#region GRADE SETUP

			var gradeList = new List<Grade>
			{
				new Grade
				{
					ID              = 1,
					StudentID       = 48,
					AssignmentID    = 38344,
					AssignmentGrade = 6.5,
					LateHandin      = "false"
				},
				new Grade
				{
					ID              = 2,
					StudentID       = 2,
					AssignmentID    = 38344,
					AssignmentGrade = 6.5,
					LateHandin      = "false"
				},
				new Grade
				{
					ID              = 3,
					StudentID       = 3,
					AssignmentID    = 38344,
					AssignmentGrade = 6.5,
					LateHandin      = "false"
				},
				new Grade
				{
					ID              = 4,
					StudentID       = 3,
					AssignmentID    = 38346,
					AssignmentGrade = 9.5,
					LateHandin      = "True"
				}
			};
			_repositories.Add(typeof(Grade), gradeList);

			#endregion

			#region ASSIGNMENT SETUP

			var assignmentList = new List<Assignment>
			{
				new Assignment
				{
					ID           = 1,
					AssignmentID = 1111,
					Weight       = 15
				},
				new Assignment
				{
					ID           = 2,
					AssignmentID = 2222,
					Weight       = 1
				},
				new Assignment
				{
					ID           = 3,
					AssignmentID = 3333,
					Weight       = 2
				},
				new Assignment
				{
					ID           = 4,
					AssignmentID = 38344,
					Weight       = 200
				}
			};
			_repositories.Add(typeof(Assignment), assignmentList);

			#endregion
		}
	}
}