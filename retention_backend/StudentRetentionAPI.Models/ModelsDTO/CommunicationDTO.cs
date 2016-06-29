namespace StudentRetentionAPI.Models.ModelsDTO
{
	public class CommunicationDTO
	{
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
