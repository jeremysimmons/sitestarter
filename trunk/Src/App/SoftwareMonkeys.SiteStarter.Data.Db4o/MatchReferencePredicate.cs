using System;
using Db4objects.Db4o.Query;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
	/// <summary>
	/// 
	/// </summary>
	public class MatchReferencePredicate : Predicate
	{
		Guid ReferencedEntityID = Guid.Empty;
		Type ReferencedEntityType;
		
		string PropertyName = String.Empty;
		string MirrorPropertyName = String.Empty;
		
		public MatchReferencePredicate(DataProvider provider, Type entityType, string propertyName, Type referencedEntityType, Guid referencedEntityID)
		{
			ReferencedEntityID = referencedEntityID;
			PropertyName = propertyName;
			ReferencedEntityType = referencedEntityType;
		}
		
		public MatchReferencePredicate(DataProvider provider, Type entityType, string propertyName, Type referencedEntityType, Guid referencedEntityID, string mirrorPropertyName)
		{
			ReferencedEntityID = referencedEntityID;
			PropertyName = propertyName;
			ReferencedEntityType = referencedEntityType;
			MirrorPropertyName = mirrorPropertyName;
		}
		
		public bool Match(IEntity entity)
		{
			bool doesMatch = true;
			
			//using (LogGroup logGroup = LogGroup.StartDebug("Querying entity."))
			//{
				// Catch and log errors otherwise they won't get caught as this is executed within db4o
				try
				{
			//		LogWriter.Debug("Checking type " + entity.GetType().ToString());
			//		LogWriter.Debug("Entity ID: " + entity.ID);
			//		LogWriter.Debug("Property name: " + PropertyName);
					
					string mirrorPropertyName = MirrorPropertyName;
					
					// If no mirror property name was specified by the calling code then detect it
					if (mirrorPropertyName == null || mirrorPropertyName == String.Empty)
					{
						if (PropertyName != String.Empty)
							mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(entity, PropertyName);
						else
							mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyNameReverse(entity, PropertyName, ReferencedEntityType);
					}
					
			//		LogWriter.Debug("Mirror property name: " + mirrorPropertyName);
					
					bool referenceFound = false;
					
					if (ReferencedEntityID != Guid.Empty)
						referenceFound = DataAccess.Data.Referencer.MatchReference(entity.GetType(), entity.ID, PropertyName, ReferencedEntityType, ReferencedEntityID, mirrorPropertyName);
					else
						referenceFound = DataAccess.Data.Referencer.MatchReference(entity.GetType(), entity.ID, PropertyName, ReferencedEntityType, mirrorPropertyName);
					
			//		LogWriter.Debug("Reference found: " + referenceFound);
					
					// If a reference entity ID is specified then it matches if a reference is found
					if (ReferencedEntityID != Guid.Empty)
					{
			//			LogWriter.Debug("Referenced entity ID is specified. Will match if a reference is found.");
						doesMatch = referenceFound;
					}
					// Otherwise it matches if NO references are found, because the calling code wants to match an entity with no found references matching the one specified
					else
					{
			//			LogWriter.Debug("Referenced entity ID is empty. Will match if no reference is found.");
						doesMatch = !referenceFound;
					}
					
				}
				catch (Exception ex)
				{
			//		LogWriter.Error(ex);
					throw ex;
				}
			//	LogWriter.Debug("Matches: " + doesMatch);
			//}
			return doesMatch;
		}
	}
}
