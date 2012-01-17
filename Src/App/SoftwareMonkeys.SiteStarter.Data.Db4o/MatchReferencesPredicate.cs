using System;
using Db4objects.Db4o.Query;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
	/// <summary>
	/// 
	/// </summary>
	public class MatchReferencesPredicate : Predicate
	{
		string PropertyName = String.Empty;
		Guid[] ReferencedEntityIDs = new Guid[]{};
		Type ReferencedEntityType;
		
		public MatchReferencesPredicate(DataProvider provider, Type entityType, string propertyName, Type referencedEntityType, Guid[] referencedEntityIDs)
		{
			PropertyName = propertyName;
			ReferencedEntityIDs = referencedEntityIDs;
			ReferencedEntityType = referencedEntityType;
		}
		
		public bool Match(IEntity entity)
		{
			bool doesMatch = true;
			
			//using (LogGroup logGroup2 = LogGroup.Start("Querying entity.", NLog.LogLevel.Debug))
			//{

			//LogWriter.Debug("Checking type " + e.GetType().ToString());
			//LogWriter.Debug("Entity ID: " + e.ID);
			try
			{
				string mirrorPropertyName = String.Empty;
				if (PropertyName != String.Empty)
					mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(entity, PropertyName);
				else
					mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyNameReverse(entity, PropertyName, ReferencedEntityType);
				
				bool foundReference = false;
				foreach (Guid id in ReferencedEntityIDs)
				{
					if (DataAccess.Data.Referencer.MatchReference(entity.GetType(), entity.ID, PropertyName, ReferencedEntityType, id, mirrorPropertyName))
						foundReference = true;
				}
				
				// If references are provided then it matches if found
				if (ReferencedEntityIDs.Length > 0)
					doesMatch = foundReference;
				// Otherwise the calling code is trying to get entities where NO reference exists, therefore it matches when no reference is found
				else
					doesMatch = !foundReference;
			}
			catch (Exception ex)
			{
				LogWriter.Error(ex);
				throw ex;
			}
			//LogWriter.Debug("Matches: " + matches);
			//}
			return doesMatch;
		}
	}
}
