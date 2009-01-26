using System;
using System.Collections;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
	/// <summary>
	/// Holds a collection of entities.
	/// </summary>
	[Serializable]
	public class EntityFormItemCollection : CollectionBase
	{
		/// <summary>
		/// Gets/sets the entity at the specified position in the collection.
		/// </summary>
		public EntityFormItem this[int index]
		{
            get { return (EntityFormItem)List[index]; }
			set { List[index] = value; }
		}

		/// <summary>
		/// Empty constructor.
		/// </summary>
		public EntityFormItemCollection()
		{}

		/// <summary>
		/// Adds the provided entity to the collection.
		/// </summary>
		/// <param name="entity">The entity to add to the collection.</param>
        public EntityFormItemCollection(EntityFormItem entity)
		{
			Add(entity);
		}

		/// <summary>
		/// Adds the provided entities to the collection.
		/// </summary>
		/// <param name="entities">Entities to add to the collection.</param>
        public EntityFormItemCollection(object[] entities)
		{
			if (entities != null)
			{
				foreach (object entity in entities)
				{
                    Add((EntityFormItem)entity);
				}
			}
		}

		/// <summary>
		/// Adds the provided entity to the collection.
		/// </summary>
		/// <param name="entity">The entity to add to the collection.</param>
        public void Add(EntityFormItem entity)
		{
			if (entity != null)
				List.Add(entity);
		}

		/// <summary>
		/// Gets the IDs of all entities in the collection.
		/// </summary>
		/// <returns>An array of entity IDs.</returns>
		public Guid[] GetIDs()
		{
			// Create an ID collection
			ArrayList ids = new ArrayList();

			// Loop through all the entities in the collection and get their IDs
            foreach (EntityFormItem entity in this)
			{
                PropertyInfo idProperty = entity.GetType().GetProperty("ID");
                if (idProperty != null)
                {
                    Guid id = (Guid)idProperty.GetValue((object)entity, (object[])null);
                    ids.Add(id);
                }
			}

			// Return the IDs
			return (Guid[])ids.ToArray(typeof(Guid));
		}

        /// <summary>
        /// Checks whether the provided entity instance is found in the collection.
        /// </summary>
        /// <param name="entity">The entity to look for in the application.</param>
        /// <returns>A boolean value indicating whether the entity was found in the collection.</returns>
        public bool Contains(EntityFormItem entity)
        {
            return List.Contains(entity);
        }
	}
}
