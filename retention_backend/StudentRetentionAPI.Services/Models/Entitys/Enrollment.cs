namespace StudentRetentionAPI.Services.Models.Entitys
{
	public class Enrollment
	{
		/// <summary>
		/// ID of the Enrollment
		/// </summary>
		public int ID        { get; set; }

		/// <summary>
		/// Id of the course that student is enrolling in
		/// </summary>
		public int CourseID  { get; set; }

		/// <summary>
		/// Id of the student that is enrolling in a course
		/// </summary>
		public int StudentID { get; set; }
	}
}