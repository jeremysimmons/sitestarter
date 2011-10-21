using System;
using Db4objects.Db4o.Query;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
	/// <summary>
	/// 
	/// </summary>
	public class MatchFilterGroupPredicate : Predicate
	{
		IDataFilterGroup FilterGroup = null;
		
		public MatchFilterGroupPredicate(IDataFilterGroup filterGroup)
		{
			FilterGroup = filterGroup;
		}
		
		public bool Match(IEntity entity)
		{
			bool doesMatch = true;
			
			//using (LogGroup logGroup2 = LogGroup.Start("Querying entity.", NLog.LogLevel.Debug))
			//{

			//LogWriter.Debug("Checking type " + e.GetType().ToString());
			//LogWriter.Debug("Entity ID: " + e.ID);
			
			doesMatch = FilterGroup.IsMatch(entity);
			
			//LogWriter.Debug("Matches: " + matches);
			//}
			return doesMatch;
		}
	}
}
