using System.ComponentModel.DataAnnotations;

namespace StudentRetentionAPI.Services.Models.Entitys
{
	public class Communication
	{
		/// <summary>
		/// ID for the database
		/// </summary>
		[Key]
		public int ID         { get; set; }

		/// <summary>
		/// Social security number of the student
		/// </summary>
		public string SSN     { get; set; }

		/// <summary>
		/// The date of the message
		/// </summary>
		public string Date    { get; set; }

		/// <summary>
		/// Logging of the message sent to the student
		/// </summary>
		public string Message { get; set; }
	}
}