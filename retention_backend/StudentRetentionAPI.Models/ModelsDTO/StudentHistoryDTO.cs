using System;

namespace StudentRetentionAPI.Models.ModelsDTO
{
	public class StudenthistoryDTO
	{
		/// <summary>
		/// Id of the Student Example: 14
		/// </summary>
		public int StudentID      { get; set; }

		/// <summary>
		/// Date of the event happening!
		/// </summary>
		public DateTime Date      { get; set; }

		/// <summary>
		/// Students Riskfactor at given day
		/// </summary>
		public double RiskFactor  { get; set; }

		/// <summary>
		/// Object containing all events that happened that day on that student
		/// </summary>
		public string DeltaReason { get; set; }

		/// <summary>
		/// The most relevant change to the student's risk factor today
		/// </summary>
		public double MaxFactor   { get; set; }
	}
}