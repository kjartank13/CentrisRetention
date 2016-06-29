using StudentRetentionAPI.Services.Services;
using StudentRetentionAPI.Tests.MockObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StudentRetentionAPI.Services.Models.Entitys;
using System.Collections.Generic;

namespace StudentRetentionAPI.Tests.Services
{
	[TestClass]
	public class AssessmentServiceTests
	{
		private AssessmentService               _assessmentService;
		private StudentService                  _studentService;
		private MockUnitOfWork<MockDataContext> _mockUow;
		private MockFactory                     _mockFactory;

		/// <summary>
		/// It should be noted that app.cfg in tests holds constants for test purpose only
		/// They are all set to 1 but might have other values when the project is runned thoose values
		/// are implemented in app.cfg in the listener project.
		/// THEESE VALUES ARE USED TO CAHNGE THE RISKFACTOR OF STUDENTS
		///<add key = "AsessmentView" value="1" />
		///<add key = "LateHandin" value="1" />
		///<add key = "Comment" value="1" />
		///<add key = "Handin" value="1" />
		/// </summary>
		[TestInitialize]
		public void Setup()
		{
			_mockUow     = new MockUnitOfWork<MockDataContext>();
			_mockFactory = new MockFactory();
			_mockUow.SetRepositoryData(_mockFactory.GetMockData<Student>());
			_mockUow.SetRepositoryData(_mockFactory.GetMockData<Course>());
			_mockUow.SetRepositoryData(_mockFactory.GetMockData<Enrollment>());
			_mockUow.SetRepositoryData(_mockFactory.GetMockData<Grade>());
			_mockUow.SetRepositoryData(_mockFactory.GetMockData<Assignment>());
			_mockUow.SetRepositoryData(_mockFactory.GetMockData<StudentHistory>());
			_assessmentService = new AssessmentService(_mockUow);
			_studentService    = new StudentService(_mockUow);
		}

		/// <summary>
		/// This method checks if the RiskFactor gets updated for sure
		/// </summary>
		[TestMethod]
		public void AssesGradeTest()
		{
			// Arrange: This assignment is real so we will get grades from centris
			// Also it has weight on it so assessment% should change on some students
			var assignmentId = "38344";
			var reason       = "Assignment handin with alot of Weight";
			var ssn          = MockFactory.SSN_OTTAR;
			var studentCheck = _studentService.GetStudentFunc(ssn);
			var risk         = studentCheck.RiskFactor;
			// Act:
			_assessmentService.AssessGrade(assignmentId, reason);
			var studentCheck2 = _studentService.GetStudentFunc(ssn);
			// Assert:
			Assert.AreNotEqual(risk, studentCheck2.RiskFactor);
			Assert.IsFalse(studentCheck2.RiskFactor > 100 && studentCheck2.RiskFactor < 0);
		}

		/// <summary>
		/// This method checks if the RiskFactor gets updated for sure
		/// </summary>
		[TestMethod]
		public void AssesViewTest()
		{
			// Arrange: This function takes in ssn
			var ssn          = MockFactory.SSN_OTTAR;
			var studentCheck = _studentService.GetStudentFunc(ssn);
			var risk         = studentCheck.RiskFactor;
			// Act:
			_assessmentService.AssessView(ssn);
			var studentCheck2 = _studentService.GetStudentFunc(ssn);
			// Assert: Riskfactor should have lowered by 1 since View is set to value 1 .. app.cfg
			Assert.AreEqual(risk, studentCheck2.RiskFactor + 1);
			Assert.IsFalse(studentCheck2.RiskFactor > 100 && studentCheck2.RiskFactor < 0);
		}

		/// <summary>
		/// This method checks if the RiskFactor gets updated for sure
		/// </summary>
		[TestMethod]
		public void AssesCommentTest()
		{
			// Arrange: This function takes in ssn
			var ssn          = MockFactory.SSN_OTTAR;
			var studentCheck = _studentService.GetStudentFunc(ssn);
			var risk         = studentCheck.RiskFactor;
			// Act:
			_assessmentService.AssessView(ssn);
			var studentCheck2 = _studentService.GetStudentFunc(ssn);
			// Assert: Riskfactor should have lowered by 1 since Comment is set to value 1 .. app.cfg
			Assert.AreEqual(risk, studentCheck2.RiskFactor + 1);
			Assert.IsFalse(studentCheck2.RiskFactor > 100 && studentCheck2.RiskFactor < 0);
		}

		/// <summary>
		/// Make sure all students that are part of handin get theyre riskfactor changed
		/// </summary>
		[TestMethod]
		public void AssesHandinTest()
		{
			// Arrange: This function list of members in a handin, bool lateHandin and string reason
			var startingRiskfactor = 33.33;
			List<string> members   = new List<string>();
			var lateHandin         = true;
			var reason             = "totally testing this out";
			members.Add(MockFactory.SSN_OTTAR);
			members.Add(MockFactory.SSN_KJARTAN);
			members.Add(MockFactory.SSN_KEVIN);

			// Act: All students have RiskFactor = 33.33 after running Handin func
			// The riskfactor should lower down  for 3 of them since its good to handin
			// how ever 1 student SSN_INGOLFUR should stay at 33.33
			_assessmentService.AssessHandin(members, lateHandin, reason);
			var student1Check = _studentService.GetStudentFunc(MockFactory.SSN_OTTAR);
			var student2Check = _studentService.GetStudentFunc(MockFactory.SSN_KJARTAN);
			var student3Check = _studentService.GetStudentFunc(MockFactory.SSN_KEVIN);
			var student4Check = _studentService.GetStudentFunc(MockFactory.SSN_INGOLFUR);
			// Assert: Riskfactor should have lowered by 1 since Comment is set to value 1 .. app.cfg
			Assert.IsTrue(student1Check.RiskFactor < startingRiskfactor);
			Assert.IsTrue(student2Check.RiskFactor < startingRiskfactor);
			Assert.IsTrue(student3Check.RiskFactor < startingRiskfactor);
			Assert.IsFalse(student4Check.RiskFactor < startingRiskfactor);
		}

		/// <summary>
		/// This method checks if the Studenthistory will be made if history does not exist
		/// </summary>
		[TestMethod]
		public void AddStudentHistoryTest()
		{
			// Arrange:
			var student      = _studentService.GetStudentFunc(MockFactory.SSN_OTTAR);
			var factorChange = 2.22;
			var reason       = "Testing this lovely function";
			// Act: Function should make a new studentHistory since no history exists on SSN_OTTAR
			// there are only 2 hostorys so count of entity should give us 3 after execution
			_assessmentService.UpdateStudentHistory(student, factorChange, reason);

			var countHistories = _mockFactory.GetMockData<StudentHistory>().Count;
			// Assert: 
			Assert.AreEqual(3, countHistories);
		}

		/// <summary>
		/// This method checks if the Studenthistory will be made if history does not exist
		/// </summary>
		[TestMethod]
		public void UpdateStudentHistoryTest()
		{
			// Arrange:
			var student      = _studentService.GetStudentFunc(MockFactory.SSN_KEVIN);
			var factorChange = 2.22;
			var reason       = "Testing this lovely function";
			// Act: Function should make a new studentHistory since no history exists on SSN_OTTAR
			// there are only 2 hostorys so count of entity should give us 2 after execution since the student should 
			//be updated
			_assessmentService.UpdateStudentHistory(student, factorChange, reason);

			var countHistories = _mockFactory.GetMockData<StudentHistory>().Count;
			// Assert: 
			Assert.AreEqual(2, countHistories);
		}
	}
}
