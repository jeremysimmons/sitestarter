using System;
using Db4objects.Db4o.Query;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
	/// <summary>
	/// 
	/// </summary>
	public class MatchReferencesPredicate : Predicate
	{
		EntityReferenceCollection References;
		string PropertyName = String.Empty;
		
		public MatchReferencesPredicate(DataProvider provider, Type entityType, string propertyName, Type referencedEntityType, EntityReferenceCollection references)
		{
			References = references;
			PropertyName = propertyName;
		}
		
		public bool Match(IEntity entity)
		{
			bool doesMatch = true;
			
			//using (LogGroup logGroup2 = LogGroup.Start("Querying entity.", NLog.LogLevel.Debug))
			//{

			//LogWriter.Debug("Checking type " + e.GetType().ToString());
			//LogWriter.Debug("Entity ID: " + e.ID);
			
			bool foundReference = References.Includes(entity.ID, PropertyName);
			
			// If references are provided then it matches if found
			if (References.Count > 0)
				doesMatch = foundReference;
			// Otherwise the calling code is trying to get entities where NO reference exists, therefore it matches when no reference is found
			else
				doesMatch = !foundReference;
			
			//LogWriter.Debug("Matches: " + matches);
			//}
			return doesMatch;
		}
	}
}
