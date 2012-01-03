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
		public EntityReferenceCollection References = null;
		Guid ReferencedEntityID = Guid.Empty;
		
		string PropertyName = String.Empty;
		
		public MatchReferencePredicate(DataProvider provider, Type entityType, string propertyName, Type referencedEntityType, Guid referencedEntityID)
		{
			ReferencedEntityID = referencedEntityID;
			PropertyName = propertyName;
			
			// Load the references all in one go, to avoid individual loads
			References = provider.Referencer.GetReferences(referencedEntityType, referencedEntityID, entityType, false);
			
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
					
					bool foundReference = References.Includes(entity.ID, PropertyName);
					
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
