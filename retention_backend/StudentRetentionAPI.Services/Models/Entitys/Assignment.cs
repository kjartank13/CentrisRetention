namespace StudentRetentionAPI.Services.Models.Entitys
{
	public class Assignment
	{
		/// <summary>
		/// ID of the assignment in the database
		/// </summary>
		public int ID           { get; set; }

		/// <summary>
		/// ID of this assignment in Centris
		/// </summary>
		public int AssignmentID { get; set; }

		/// <summary>
		/// Percentage weight of this assignment towards
		/// a course's final grade.
		/// </summary>
		public double Weight    { get; set; }
	}
}