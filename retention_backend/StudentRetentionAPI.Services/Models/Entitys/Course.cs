namespace StudentRetentionAPI.Services.Models.Entitys
{
	public class Course
	{
		/// <summary>
		/// ID of the course Example: 14562
		/// </summary>
		public int ID          { get; set; }

		/// <summary>
		/// What semester is this course thought in Example: 20161
		/// 2016 is the year thenfollowed  1 = spring, 2 = summer or 3 = Fall
		/// </summary>
		public string Semester { get; set; }
	}
}