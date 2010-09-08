using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Holds the state of the business strategies.
	/// </summary>
	static public class StrategyState
	{
		/// <summary>
		/// Gets a value indicating whether the strategy state has been initialized.
		/// </summary>
		static public bool IsInitialized
		{
			get { return strategies != null && strategies.Count > 0; }
		}
		
		static private StrategyStateNameValueCollection strategies;
		/// <summary>
		/// Gets/sets a collection of all the available strategies which are held in state.
		/// </summary>
		static public StrategyStateNameValueCollection Strategies
		{
			get {
				if (!IsInitialized)
					throw new InvalidOperationException("The business strategy state has not been initialized.");
				
				if (strategies == null)
					strategies = new StrategyStateNameValueCollection();
				return strategies;  }
			set { strategies = value; }
		}
	}
}
