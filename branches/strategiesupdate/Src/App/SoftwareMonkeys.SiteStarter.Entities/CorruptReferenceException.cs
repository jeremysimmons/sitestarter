using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Thrown when a corrupt reference has been detected.
	/// </summary>
	public class CorruptReferenceException : Exception
	{
		public CorruptReferenceException() : base("The reference is missing some important data and cannot be used.")
		{
		}
	}
}
