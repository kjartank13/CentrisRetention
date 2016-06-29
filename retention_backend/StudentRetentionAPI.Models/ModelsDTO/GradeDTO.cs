namespace StudentRetentionAPI.Models.ModelsDTO
{
	public class GradeDTO
	{
		/// <summary>
		/// ID of grade example: 1
		/// </summary>
		public int ID                 { get; set; }

		/// <summary>
		/// Student ID example: 14
		/// </summary>
		public int StudentID          { get; set; }

		/// <summary>
		/// ID of Assignment Example: 3
		/// </summary>
		public int AssignmentID       { get; set; }

		/// <summary>
		/// Students Grade for the Assignment Example: 9,95
		/// </summary>
		public double AssignmentGrade { get; set; }
	}
}