using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Creates strategies for various scenarios.
	/// </summary>
	public static class StrategyFactory
	{
		/// <summary>
		/// Creates a default entity retriever strategy.
		/// </summary>
		/// <returns>A default entity retriever strategy.</returns>
		static public RetrieverStrategy<T> NewRetrieverStrategy<T>()
			where T : IEntity
		{
			RetrieverStrategy<T> strategy = new RetrieverStrategy<T>();
			
			return strategy;
		}
	}
}
