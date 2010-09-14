using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the base class used by all business strategy components.
	/// </summary>
	[Strategy(false)]
	public class BaseStrategy : IStrategy
	{
		private string typeName = String.Empty;
		/// <summary>
		/// Gets/sets the name of the type of entity being handled by this strategy.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public BaseStrategy()
		{
		}
		
		/// <summary>
		/// Sets the name of the type involved in the strategy.
		/// </summary>
		/// <param name="typeName"></param>
		public BaseStrategy(string typeName)
		{
			TypeName = typeName;
		}
		
		public void CheckTypeName()
		{
			if (typeName == null || typeName == String.Empty)
				throw new InvalidOperationException("The TypeName property has not been set.");
			
			if (typeName == "IEntity")
				throw new InvalidOperationException("The TypeName property cannot be set to 'IEntity'. A specific type must be specified.");
			
			if (typeName == "IUniqueEntity")
				throw new InvalidOperationException("The TypeName property cannot be set to 'IUniqueEntity'. A specific type must be specified.");
		}
	}
}
