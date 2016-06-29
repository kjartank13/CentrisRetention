namespace StudentRetentionAPI.Models.ModelsDTO
{
	/// <summary>
	/// This class represents a Get request on a course from our database
	/// </summary>
	public class CourseDTO
	{
		/// <summary>
		/// ID of the course Example: 20163
		/// </summary>
		public int CourseID    { get; set; }

		/// <summary>
		/// What semester is this course thought in Example: 20161
		/// 2016 is the year thenfollowed  1 = spring, 2 = summer or 3 = Fall
		/// </summary>
		public string Semester { get; set; }
	}
}