namespace StudentRetentionAPI.Models.ModelsDTO
{
	/// <summary>
	/// This class is the connection table for Student and Course
	/// </summary>
	public class EnrollmentDTO
	{
		/// <summary>
		/// Connection ID
		/// </summary>
		public int EnrollmentID { get; set; }

		/// <summary>
		/// CourseID from Course table
		/// </summary>
		public int CourseID     { get; set; }

		/// <summary>
		/// Student ID from student table
		/// </summary>
		public int StudentID    { get; set; }
	}
}