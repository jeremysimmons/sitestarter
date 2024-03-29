﻿using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class DataCounter : DataAdapter, IDataCounter
	{		
		public abstract int CountEntities<T>()
			where T : IEntity;
				
		public abstract int CountEntities(Type type, Guid[] ids);

		public abstract int CountEntities<T>(Guid[] ids)
			where T : IEntity;

        public abstract int CountEntities();

		public abstract int CountEntities(Type type);

		public abstract int CountEntities(Type type, Dictionary<string, object> parameters);

		/// <summary>
		/// Counts all the entities of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns>The total number of entities found.</returns>
		public abstract int CountEntities<T>(Dictionary<string, object> parameters)
			where T : IEntity;
		
		/// <summary>
		/// Counts all the entities of the specified type from the data store.
		/// </summary>
		/// <param name="group">The group of filters to apply to the query.</param>
		/// <returns>The total number of entities found.</returns>
		public abstract int CountEntities(IDataFilterGroup group);
		
		public abstract int CountEntities(Type type, IDataFilterGroup filterGroup);
		
		public abstract int CountEntities<T>(IDataFilterGroup filterGroup)
			where T : IEntity;
				
		public abstract int CountEntitiesWithReference(Type entityType, Guid entityID, string propertyName, Type referencedEntityType, string mirrorPropertyName);
			
		public abstract int CountEntities(Type type, string propertyName, object propertyValue);
		
		public abstract int CountEntities<T>(string propertyName, object propertyValue)
			where T : IEntity;
		
		
		/// <summary>
		/// Counts all the entities of the specified type from the data store.
		/// </summary>
		/// <param name="filter">The filter to apply to the query.</param>
		/// <returns>The total number of entities found.</returns>
		public abstract int CountEntities(IDataFilter filter);

	}
}
