using System;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// A mock reaction used during testing which occurrs when an entity is saved.
	/// </summary>
	[Reaction("Save", "IEntity")]
	public class MockSaveEntityReaction : BaseSaveReaction
	{
		public MockSaveEntityReaction()
		{
		}
	}
}
