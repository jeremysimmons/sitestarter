/*
 * Created by SharpDevelop.
 * User: J
 * Date: 21/10/2011
 * Time: 10:10 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
		public Guid[] ReferencedEntityIDs = new Guid[]{};
		Guid ReferencedEntityID = Guid.Empty;
		
		public MatchReferencesPredicate(DataProvider provider, Type entityType, string propertyName, Type referencedEntityType, string mirrorPropertyName, Guid[] referencedEntityIDs)
		{
			//ReferencedEntityID = referencedEntityID;
			
			ReferencedEntityIDs = referencedEntityIDs;
			
			// Load the references all in one go, to avoid individual loads
			//EntityReferenceCollection references = provider.Referencer.GetReferences(referencedEntityType, referencedEntityID, mirrorPropertyName, entityType, false);
			
			//EntityIDs = references.GetEntityIDs(referencedEntityID);
		}
		
		public bool Match(IEntity entity)
		{
			bool doesMatch = true;
			
			//using (LogGroup logGroup2 = LogGroup.Start("Querying entity.", NLog.LogLevel.Debug))
			//{

			//LogWriter.Debug("Checking type " + e.GetType().ToString());
			//LogWriter.Debug("Entity ID: " + e.ID);
			
			bool foundReference = Array.IndexOf(ReferencedEntityIDs, entity.ID) > -1;
			
			// If referenced IDs are provided then it matches if found
			if (ReferencedEntityIDs.Length > 0)
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
