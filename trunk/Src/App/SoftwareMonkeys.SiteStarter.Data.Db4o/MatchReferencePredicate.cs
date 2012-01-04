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
				
				bool foundReference = DataAccess.Data.Referencer.MatchReference(entity.GetType(), entity.ID, PropertyName, ReferencedEntityType, ReferencedEntityID, mirrorPropertyName);
				
				// If a referenced entity ID is specified then entities match if a reference exists
				if (ReferencedEntityID != Guid.Empty)
					doesMatch = foundReference;
				// Otherwise the calling code is trying to get entities where NO reference exists, therefore it matches when no reference is found
				else
					doesMatch = !foundReference;
				
				//		LogWriter.Debug("Matches: " + doesMatch);
			}
			catch (Exception ex)
			{
				LogWriter.Error(ex);
				throw ex;
			}
			//}
			return doesMatch;
		}
	}
}
