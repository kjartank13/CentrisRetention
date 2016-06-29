using System;
using System.ComponentModel.DataAnnotations;

namespace StudentRetentionAPI.Services.Models.Entitys
{
	public class StudentHistory
	{
		/// <summary>
		/// ID for the database
		/// </summary>
		[Key]
		public int ID             { get; set; }

		/// <summary>
		/// Id of the Student Example: 14
		/// </summary>
		public int StudentID      { get; set; }

		/// <summary>
		/// Date of the event happening!
		/// Should be of the form Date.Today, NOT Date.Now
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
		/// The day's most relevant risk factor change
		/// </summary>
		public double MaxFactor   { get; set; }
	}
}