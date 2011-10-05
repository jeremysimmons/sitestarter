using System;

namespace SoftwareMonkeys.SiteStarter.State
{
	/// <summary>
	/// 
	/// </summary>
	public class StateInitializer
	{
		public StateInitializer()
		{
		}
		
		static public void Initialize()
		{
			StateAccess.State = new StateProvider();
			StateAccess.State.Initialize();
		}
		
		static public void Initialize(string physicalApplicationPath)
		{
			StateAccess.State = new StateProvider();
			StateAccess.State.Initialize();
			StateAccess.State.PhysicalApplicationPath = physicalApplicationPath;
		}
	}
}
