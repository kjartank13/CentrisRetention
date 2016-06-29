namespace StudentRetentionAPI.Services.Models.Entitys
{
	/// <summary>
	/// This class represents A table object from our database
	/// </summary>
	public class Student
	{
		public int ID              { get; set; }

		/// <summary>
		/// Name of the student Example: John Doe Homeson
		/// </summary>
		public string Name         { get; set; }

		/// <summary>
		/// The SSN of the person being added Example: 1234567890
		/// </summary>
		public string SSN          { get; set; }

		/// <summary>
		/// Students DepartmentID Example: 6 each student is part of some Department
		/// </summary>
		public string DepartmentID { get; set; }

		/// <summary>
		/// Students MajorID Example: 76 each student is part of some Major
		/// </summary>
		public string MajorID      { get; set; }

		/// <summary>
		/// Students MajorName Example: BSc í hátækniverkfræði
		/// </summary>
		public string MajorName    { get; set; }

		/// <summary>
		/// Students RiskFactor Example: 33.3
		/// </summary>
		public double RiskFactor   { get; set; }

		/// <summary>
		/// Is the student on camp or distance learning Example: True
		/// </summary>
		public bool LocalStudent   { get; set; }
	}
}
