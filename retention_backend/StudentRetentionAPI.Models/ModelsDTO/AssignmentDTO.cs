namespace StudentRetentionAPI.Models.ModelsDTO
{
	public class AssignmentDTO
	{
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