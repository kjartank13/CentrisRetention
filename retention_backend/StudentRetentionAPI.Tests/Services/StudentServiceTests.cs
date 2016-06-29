using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using StudentRetentionAPI.Services.Models.Entitys;
using StudentRetentionAPI.Services.Services;
using StudentRetentionAPI.Tests.MockObjects;

namespace StudentRetentionAPI.Tests.Services
{
	[TestClass]
	public class StudentServiceTests
	{
		private StudentService                  _service;
		private MockUnitOfWork<MockDataContext> _mockUow;
		private MockFactory                     _mockFactory;

		[TestInitialize]
		public void Setup()
		{
			_mockUow     = new MockUnitOfWork<MockDataContext>();
			_mockFactory = new MockFactory();
			_mockUow.SetRepositoryData(_mockFactory.GetMockData<Student>());
			_mockUow.SetRepositoryData(_mockFactory.GetMockData<Course>());
			_mockUow.SetRepositoryData(_mockFactory.GetMockData<Enrollment>());
			_mockUow.SetRepositoryData(_mockFactory.GetMockData<Grade>());
			_service = new StudentService(_mockUow);
		}

		#region Student Tests

		[TestMethod]
		public void AddAlreadyExistStudent()
		{
			// Arrange
			dynamic TestObj      = new JObject();
			TestObj.Name         = "Óttar Helgi Einarsson";
			TestObj.SSN          = MockFactory.SSN_OTTAR;
			TestObj.DepartmentID = "1";
			TestObj.Major        = new JObject();
			TestObj.Major.ID     = MockFactory.MAJOR_ID_BSC_COMP_SCI;
			TestObj.Major.Name   = "Meistaranám í tölvunarfræði";

			// Act:
			var oldList = _mockFactory.GetMockData<Student>().Count;
			// list should not change since student already exists
			_service.AddStudent(TestObj);
			var newList = _mockFactory.GetMockData<Student>().Count;
			// Assert:
			Assert.AreEqual(oldList, newList);
		}

		[TestMethod]
		public void AddNewStudent()
		{
			// Arrange:
			dynamic TestObj      = new JObject();
			TestObj.Name         = "Kevin Freyr Leósson";
			TestObj.SSN          = MockFactory.SSN_NON_STUDENT_1;
			TestObj.DepartmentID = "69";
			TestObj.Major        = new JObject();
			TestObj.Major.ID     = MockFactory.MAJOR_ID_BSC_COMP_SCI;
			TestObj.Major.Name   = "BSc í tölvunarfræði";

			// Act:
			var oldList = _mockFactory.GetMockData<Student>().Count;
			_service.AddStudent(TestObj);
			var newList = _mockFactory.GetMockData<Student>().Count;
			// Assert:	
			Assert.AreEqual(oldList, newList - 1);
		}

		[TestMethod]
		public void DeleteStudent()
		{
			// Arrange:
			dynamic TestObj      = new JObject();
			TestObj.Name         = "Óttar Helgi Einarsson";
			TestObj.SSN          = MockFactory.SSN_OTTAR;
			TestObj.DepartmentID = "1";
			TestObj.Major        = new JObject();
			TestObj.Major.ID     = MockFactory.MAJOR_ID_BSC_COMP_SCI;
			TestObj.Major.Name   = "Meistaranám í tölvunarfræði";
			var oldList          = _mockFactory.GetMockData<Student>().Count;
			// Act:
			_service.RemoveStudent(TestObj);
			var studentExists = _service.GetStudentFunc(MockFactory.SSN_OTTAR);
			var newList       = _mockFactory.GetMockData<Student>().Count;
			// Assert:	
			Assert.AreEqual(oldList, newList + 1);
			Assert.IsNull(studentExists);
		}

		[TestMethod]
		public void DeleteStudentThatNotExists()
		{
			// Arrange:
			dynamic TestObj      = new JObject();
			TestObj.Name         = "Kevin Freyr Leósson";
			TestObj.SSN          = MockFactory.SSN_NON_STUDENT_1;
			TestObj.DepartmentID = "69";
			TestObj.Major        = new JObject();
			TestObj.Major.ID     = MockFactory.MAJOR_ID_BSC_COMP_SCI;
			TestObj.Major.Name   = "BSc í tölvunarfræði";
			var oldList          = _mockFactory.GetMockData<Student>().Count;
			// Act:
			_service.RemoveStudent(TestObj);
			var studentExists = _service.GetStudentFunc(MockFactory.SSN_NON_STUDENT_1);
			var newList       = _mockFactory.GetMockData<Student>().Count;
			// Assert:	
			Assert.AreEqual(oldList, newList);
			Assert.IsNull(studentExists);
		}

		#endregion

		#region Enrollment Tests

		[TestMethod]
		public void AddEnrollmentsValidStudentWithCourses()
		{
			// Arrange:
			dynamic TestObj      = new JObject();
			TestObj.Name         = "Óttar Helgi Einarsson";
			TestObj.SSN          = MockFactory.SSN_OTTAR;
			TestObj.DepartmentID = "1";
			TestObj.Courses      = new JArray(
				20437,
				20436,
				20435
				);
			// Act:
			// 4 enrollments exist this student is valid and is enrolling in 3 valid courses
			// enrollment count should increase by 3
			var oldList = _mockFactory.GetMockData<Enrollment>().Count;

			_service.AddEnrollment(TestObj);

			var newList = _mockFactory.GetMockData<Enrollment>().Count;
			// Assert:	
			Assert.AreEqual(oldList, newList - 3);
		}

		[TestMethod]
		public void AddEnrollmentsValidStudentNoCourses()
		{
			// Arrange:
			dynamic TestObj      = new JObject();
			TestObj.Name         = "Kevin Freyr Leósson";
			TestObj.SSN          = MockFactory.SSN_NON_STUDENT_1;
			TestObj.DepartmentID = "69";
			TestObj.Major        = new JObject();
			TestObj.Major.ID     = MockFactory.MAJOR_ID_BSC_COMP_SCI;
			TestObj.Major.Name   = "BSc í tölvunarfræði";
			TestObj.Courses      = new JArray();

			// Act:
			// We need to add a student first with no courses addstudent will call AddEnrollment
			var studentCountBeforeAdd    = _mockFactory.GetMockData<Student>().Count;
			var enrollmentCountBeforeAdd = _mockFactory.GetMockData<Enrollment>().Count;
			_service.AddStudent(TestObj);
			// Assert:	
			// Student Count should be +1  and enrollment not changed
			var studentCountAfterAdd    = _mockFactory.GetMockData<Student>().Count;
			var enrollmentCountAfterAdd = _mockFactory.GetMockData<Enrollment>().Count;

			Assert.AreEqual(studentCountBeforeAdd, studentCountAfterAdd - 1);
			Assert.AreEqual(enrollmentCountBeforeAdd, enrollmentCountAfterAdd);
		}

		#endregion

		#region Semester Tests

		[TestMethod]
		public void RemoveSemesterThatDoesNotExist()
		{
			// Arrange:
			// there are 4 courses in the database
			var semestertoremove = MockFactory.SEMESTER_NOT_VALID;
			// Act: log how many courses are
			var oldCourseCount   = _mockFactory.GetMockData<Course>().Count;
			_service.RemoveSemester(semestertoremove);
			// semester id is non_valid so no courses should be removed
			var newCourseCount = _mockFactory.GetMockData<Course>().Count;
			// Assert:	should be the same since nothing should be deleted
			Assert.AreEqual(oldCourseCount, newCourseCount);
		}

		[TestMethod]
		public void RemoveSemesterThatExists()
		{
			// Arrange:
			var semestertoremove = MockFactory.SEMESTER_VALID;
			var oldCourseCount   = _mockFactory.GetMockData<Course>().Count;
			// Act: Call removesemester should remove all courses linked to SEMESTER_VALID id
			_service.RemoveSemester(semestertoremove);
			// All courses should be out of the database now
			var newCourseCount = _mockFactory.GetMockData<Course>().Count;
			// Assert:	
			Assert.AreEqual(oldCourseCount, newCourseCount + 4);
		}

		[TestMethod]
		public void RemoveOldSemestersTest()
		{
			// Arrange:
			var semesterOlderThen2Years = false;
			var year                    = (DateTime.Now.Year - 2).ToString();
			var semesterOne             = year + "1";
			var semesterTwo             = year + "2";
			var semesterThree           = year + "3";
			// Act:
			_service.RemoveOldSemesters();
			var newCourseList = _mockFactory.GetMockData<Course>();

			foreach (var item in newCourseList)
			{
				if (item.Semester == semesterOne || item.Semester == semesterTwo || item.Semester == semesterThree)
				{
					semesterOlderThen2Years = true;
				}
			}
			// Assert:	
			Assert.IsFalse(semesterOlderThen2Years);
		}

		[TestMethod]
		public void NewSemesterTest()
		{
			// Arrange: Get Course and Student Count before new semester
			var oldCourseCount  = _mockFactory.GetMockData<Course>().Count;
			var oldStudentCount = _mockFactory.GetMockData<Student>().Count;

			// Act: Call Newsemester that will ask Centris for all students and Courses for next semester
			_service.NewSemester();

			// Both lists should change and have more items in them so we assert notequal
			var newCourseCount  = _mockFactory.GetMockData<Course>().Count;
			var newStudentCount = _mockFactory.GetMockData<Student>().Count;
			// Assert:	should be the same since nothing should be deleted

			Assert.AreNotEqual(oldCourseCount, newCourseCount);
			Assert.AreNotEqual(oldStudentCount, newStudentCount);
		}

		#endregion

		#region Course Tests

		[TestMethod]
		public void AddCourseTest()
		{
			// Arrange
			dynamic TestObj  = new JObject();
			TestObj.ID       = 500;
			TestObj.Semester = MockFactory.SEMESTER_VALID;
			// Act: get list of courses before add
			var oldList = _mockFactory.GetMockData<Course>().Count;
			// list should not change since student already exists
			_service.AddCourse(TestObj);
			var wasTheCourseAdded = _service.GetCourseFunc(500, MockFactory.SEMESTER_VALID);
			var newList = _mockFactory.GetMockData<Course>().Count;
			// Assert: if the list got 1 bigger and that the course was found in mockdb
			Assert.AreEqual(oldList, newList - 1);
			Assert.IsNotNull(wasTheCourseAdded);
		}

		[TestMethod]
		public void StudentJoinCourseTest()
		{
			// Arrange build the object like the function gets from messageservice
			dynamic TestObj                = new JObject();
			TestObj.Item                   = new JObject();
			TestObj.Item.Course            = new JObject();
			TestObj.Item.Student           = new JObject();
			TestObj.Item.Course.InstanceID = MockFactory.COURSE_INST_ID_TOLH_2010;
			TestObj.Item.Student.SSN       = MockFactory.SSN_OTTAR;
			// Act: 
			_service.StudentJoinCourse(TestObj);
			//Lookup by the courseID and StudentID in this case 48 since we use Ottar from MockFactory
			var wasTheEnrollmentAdded = _service.GetEnrollmentFunc(48, MockFactory.COURSE_INST_ID_TOLH_2010);
			// Assert:
			Assert.IsNotNull(wasTheEnrollmentAdded);
		}

		#endregion

		#region Grades Testing

		[TestMethod]
		public void AddGradeTest()
		{
			// Arrange: Hardcoded assignmentid and built object like the function would normally get
			dynamic TestObj           = new JObject();
			TestObj.StudentID         = 48;
			TestObj.SSN               = MockFactory.SSN_OTTAR;
			TestObj.AssignmentGrade   = 9.5;
			TestObj.LateHandinPenalty = "True";
			var assignmentid          = 3333;
			// Act: Get all the grades and add them
			_service.AddGrade(TestObj, assignmentid);
			var result = _service.GetGradeFunc(3333, 48);
			// Assert: Check if the grade gets made
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetGradesTest()
		{
			// TODO: dependency injection to test API calls
		}

		#endregion

		#region Assignment Testing

		[TestMethod]
		public void AddAssignmentTesting()
		{
			// Arrange: Hardcoded assignmentid and weight of the assignment
			var assignmentid = "666";
			var weight       = 0.3;
			// Act: Get all the grades and add them
			_service.AddAssignment(assignmentid, weight);
			var result = _service.GetAssignmentFunc(666);
			// Assert: Check if the grade gets made
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void UpdateAssignmentTesting()
		{
			// Arrange: Hardcoded assignmentid and weight of the assignment
			var assignmentid = "1111";
			var weight       = 0.3;
			// Act: Get all the grades and add them
			_service.AddAssignment(assignmentid, weight);
			var result = _service.GetAssignmentFunc(1111);
			// Assert: weight should have been changed from 15 to 0.3
			Assert.AreEqual(weight, result.Weight);
		}

		#endregion

		#region DataBase Calling Functions Tested

		[TestMethod]
		public void GetStudentFuncTest()
		{
			// Arrange: 
			var ssn    = MockFactory.SSN_OTTAR;
			// Act:  There is a student with this ssn so he should be found
			var result = _service.GetStudentFunc(ssn);
			// Assert:
			Assert.AreEqual(ssn, result.SSN);
		}

		[TestMethod]
		public void GetEnrollmentFuncTest()
		{
			// Arrange: 
			var courseId  = MockFactory.COURSE_INST_ID_PROG_2010;
			var studentId = 11;
			// Act:  There is a enrollment with given info so it should be found.
			var result    = _service.GetEnrollmentFunc(studentId, courseId);
			// Assert:
			Assert.AreEqual(result.StudentID, 11);
		}

		[TestMethod]
		public void GetGradeFuncTest()
		{
			// Arrange: 
			var assignmentId = 38344;
			var studentId    = 48;
			// Act:  There is a Grade with given info so it should be found.
			var result       = _service.GetGradeFunc(assignmentId, studentId);
			// Assert:
			Assert.AreEqual(result.ID, 1);
		}

		[TestMethod]
		public void GetCourseFuncTest()
		{
			// Arrange: 
			var courseId = MockFactory.COURSE_INST_ID_TOLH_2010;
			var semester = MockFactory.SEMESTER_VALID;
			// Act:  There is a Grade with given info so it should be found.
			var result   = _service.GetCourseFunc(courseId, semester);
			// Assert:
			Assert.AreEqual(result.ID, 20434);
		}

		#endregion
	}
}