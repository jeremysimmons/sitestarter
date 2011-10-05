using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Description of ICommandManager.
	/// </summary>
	public interface ICommandManager
	{
		bool CommandExists(string action, string type, Type[] parameters);
		object Execute(string action, string type, object[] parameters);
		bool TryExecute(string action, string type, object[] parameters, out object returnValue);
	}
}
