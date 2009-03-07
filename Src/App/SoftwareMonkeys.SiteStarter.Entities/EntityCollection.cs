using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Holds a collection of entities.
	/// </summary>
	[Serializable]
	public class Collection<E> : CollectionBase, IEnumerable
       where E : SoftwareMonkeys.SiteStarter.Entities.BaseEntity
	{
		/// <summary>
		/// Gets/sets the entity at the specified position in the collection.
		/// </summary>
		public E this[int index]
		{
			get { return (E)List[index]; }
			set { List[index] = value; }
		}

        /// <summary>
        /// Gets/sets the entity in the collection with the provided ID.
        /// </summary>
        public E this[Guid id]
        {
            get {
                foreach (E entity in this)
                    if (entity.ID == id)
                        return entity;
                return null;
            }
            set
            {
                bool found = false;
                for (int i = 0; i < Count; i++)
                {
                    if (this[i].ID == id)
                    {
                        this[i] = value;
                        found = true;
                    }
                }
                if (!found)
                    Add(value);
            }
        }

        /*/// <summary>
        /// Gets the type of E in the collection.
        /// </summary>
        public Type EType
        {
            get {

            }
        }*/

            /// <summary>
            /// Empty constructor.
            /// </summary>
            public Collection()
		{}

		/// <summary>
		/// Adds the provided entity to the collection.
		/// </summary>
		/// <param name="entity">The entity to add to the collection.</param>
		public Collection (E entity)
		{
            if (entity != null)
			    Add(entity);
		}

        /// <summary>
        /// Adds the provided entity to the collection.
        /// </summary>
        /// <param name="entity">The entity to add to the collection.</param>
        public Collection(object entity)
        {
            if (entity != null && entity is E)
                Add((E)entity);
        }

		/// <summary>
		/// Adds the provided entities to the collection.
		/// </summary>
		/// <param name="entities">Entities to add to the collection.</param>
		public Collection(object[] entities)
		{
			if (entities != null)
			{
				foreach (object entity in entities)
				{
					Add((E)entity);
				}
			}
		}

        /// <summary>
        /// Adds the provided entities to the collection.
        /// </summary>
        /// <param name="entities">Entities to add to the collection.</param>
        public Collection(Collection<E> entities)
        {
            if (entities != null)
            {
                foreach (E entity in entities)
                {
                    Add((E)entity);
                }
            }
        }

        /// <summary>
        /// Adds the provided entities to the collection.
        /// </summary>
        /// <param name="entities">Entities to add to the collection.</param>
        public Collection(IEnumerable list)
        {
            if (list != null)
            {
                foreach (object entity in list)
                {
                    Add((E)entity);
                }
            }
        }

		/// <summary>
		/// Adds the provided entity to the collection.
		/// </summary>
		/// <param name="entity">The entity to add to the collection.</param>
		public void Add(E entity)
		{
			if (entity != null)
				List.Add(entity);
		}

        /// <summary>
        /// Adds the provided entities to the collection.
        /// </summary>
        /// <param name="entity">The entity to add to the collection.</param>
        public void Add(IEnumerable entities)
        {
            if (entities != null)
            {
                foreach (object obj in entities)
                    if (obj is E)
                        List.Add((E)obj);
            }
        }

        /// <summary>
        /// Adds the provided entities to the collection.
        /// </summary>
        /// <param name="entity">The entity to add to the collection.</param>
        public void Add(BaseEntity[] entities)
        {
            if (entities != null)
            {
                foreach (object obj in entities)
                    if (obj is E)
                        List.Add((E)obj);
            }
        }

        /// <summary>
        /// Removes the provided entity from the collection.
        /// </summary>
        /// <param name="entity">The entity to remove from the collection.</param>
        public void Remove(E entity)
        {
            if (entity != null && List.Contains(entity))
                List.Remove(entity);
        }

		/// <summary>
		/// Checks whether a entity with the specified ID is in the collection.
		/// </summary>
		/// <param name="entityID">The ID of the entity to check for.</param>
		/// <returns>A boolean value indicating whether the entity is in the collection.</returns>
		public bool Contains(Guid entityID)
		{
			foreach (E entity in this)
				if (entity.ID == entityID)
					return true;
			return false;
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
			foreach (E entity in this)
			{
                PropertyInfo idProperty = entity.GetType().GetProperty("ID");
                if (idProperty != null)
                {
                    Guid id = (Guid)idProperty.GetValue(entity, (object[])null);
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
        public bool Contains(E entity)
        {
            return List.Contains(entity);
        }

        /// <summary>
        /// Converts the collection to an array of E objects.
        /// </summary>
        /// <returns>An array of the E objects in the collection.</returns>
        public E[] ToArray(Type type)
        {
            ArrayList list = new ArrayList();
            foreach (E entity in this)
                list.Add(entity);
            return (E[])list.ToArray(type);
        }

        /// <summary>
        /// Converts the collection to an array of E objects.
        /// </summary>
        /// <returns>An array of the E objects in the collection.</returns>
        public E[] ToArray()
        {
            ArrayList list = new ArrayList();
            foreach (E entity in this)
                list.Add(entity);
            return (E[])list.ToArray(typeof(E));
        }

        // TODO: Check if needed
        /*
        /// <summary>
        /// Converts the collection to an array of objects.
        /// </summary>
        /// <returns>An array of the objects in the collection.</returns>
        public object[] ToObjectArray()
        {
            ArrayList list = new ArrayList();
            foreach (E entity in this)
                list.Add(entity);
            return (object[])list.ToArray(typeof(object));
        }*/

        // TODO: Check if needed
        /*/// <summary>
        /// Casts the provided value back to a Collection&lt;E&gt; type then to an array.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static public E[] CastToArray(object value)
        {
            Collection<E> newCollection = new Collection<E>();
            if (value is Collection<E>)
            {
                Type type = ((Collection<E>)value).EType;
                foreach (object obj in (IEnumerable)value)
                {
                    if (obj is E)
                        newCollection.Add((E)obj);
                }
                return newCollection.ToArray(type);
            }
            else
                throw new ArgumentException("The provided object must be of type Collection<E> or a collection using a derivitive of the E class.");

            
        }*/

        // TODO: Check if needed
        /*/// <summary>
        /// Casts the provided value back to a Collection&lt;E&gt; type then to an array of objects.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static public object[] CastToObjectArray(object value)
        {
            Collection<E> newCollection = new Collection<E>();
            if (value is Collection<E>)
            {
                foreach (object obj in (IEnumerable)value)
                {
                    if (obj is E)
                        newCollection.Add((E)obj);
                }
            }
            else
                throw new ArgumentException("The provided object must be of type Collection<E> or a collection using a derivitive of the E class.");

            return newCollection.ToObjectArray();
        }*/

        #region Conversion operators
        /*public static implicit operator Collection<E>(E[] entities)
        {
            return new Collection<E>(entities);
        }

        public static implicit operator E[](Collection<E> entities)
        {
            return entities.ToArray();
        }*/

        /*public static implicit operator Collection<E>(Object[] entities)
        {
            return new Collection<E>(entities);
        }

        public static implicit operator Object[](Collection<E> entities)
        {
            return entities.ToObjectArray();
        }*/

       /* public static implicit operator Collection<E>(object collection)
        {
            if (collection is Collection<E>)
            {
                return new Collection<E>((IEnumerable)collection);
            }
            throw new Exception("Cannot convert from object to Collection<E>.");
        }

        public static implicit operator object(Collection<E> entities)
        {
            return (object)entities;
        }*/

      /*  public static implicit operator Collection<E>(Collection<E> entities)
        {
            return new Collection<E>(entities);
        }*/
        #endregion

        /// <summary>
        /// Sorts the collection as specified.
        /// </summary>
        /// <param name="propertyName">The name of the property being sorted by.</param>
        /// <param name="direction">The sort direction.</param>
        public void Sort(string propertyName, SortDirection direction)
        {
            if (Count > 0 && InnerList[0] != null)
            {
                Type type = InnerList[0].GetType();
                this.InnerList.Sort(new DynamicComparer(type, propertyName, direction));
            }
        }

        /// <summary>
        /// Sorts the collection as specified.
        /// </summary>
        /// <param name="sort">The sort command to use.</param>
        public void Sort(string sort)
        {
            if (sort != null && sort != String.Empty)
            {
                if (sort.IndexOf(SortDirection.Ascending.ToString()) > -1)
                    Sort(sort.Replace(SortDirection.Ascending.ToString(), ""), SortDirection.Ascending);
                else if (sort.IndexOf(SortDirection.Descending.ToString()) > -1)
                    Sort(sort.Replace(SortDirection.Descending.ToString(), ""), SortDirection.Descending);
            }
        }

        /// <summary>
        /// Retrieves all entities within this collection with an ID that matches one of the IDs provided.
        /// </summary>
        /// <param name="ids">The IDs to check for in this collection.</param>
        /// <returns>A collection of entities with the specified IDs.</returns>
        public Collection<E> GetByIDs(Guid[] ids)
        {
            // Get the inner type of the collection
          /*  Type innerType = null; // TODO: Remove code
            if (List.Count > 0)
                innerType = List[0].GetType();
            else
                throw new Exception("The type of objects in this collection are not supported. They must be derived from E.");
            */
            // Create the specific collection type
            System.Type specificType = typeof(Collection<>).MakeGenericType(new System.Type[] { typeof(E) });
            Collection<E> entities = (Collection<E>)Activator.CreateInstance(specificType);

            foreach (E entity in this)
            {
                if (Array.IndexOf(ids, entity.ID) > -1)
                    entities.Add(entity);
            }

            return entities;
        }

        /// <summary>
        /// Provides a way to dynamically create comparers for any property of any object.
        /// </summary>
        public class DynamicComparer : IComparer
        {
            private Type objectType;
            /// <summary>
            /// Gets/sets the type of object being compared.
            /// </summary>
            public Type ObjectType
            {
                get { return objectType; }
                set { objectType = value; }
            }

            private string propertyName;
            /// <summary>
            /// Gets/sets the name of the property to compare.
            /// </summary>
            public string PropertyName
            {
                get { return propertyName; }
                set { propertyName = value; }
            }

            private SortDirection sortDirection;
            /// <summary>
            /// Gets/sets the sort direction.
            /// </summary>
            public SortDirection SortDirection
            {
                get { return sortDirection; }
                set { sortDirection = value; }
            }

            /// <summary>
            /// Initializes settings of the comparer.
            /// </summary>
            /// <param name="objectType">The type of object being compared.</param>
            /// <param name="propertyName">The name of the property being compared.</param>
            /// <param name="sortDirection">The sort direction.</param>
            public DynamicComparer(Type objectType, string propertyName, SortDirection sortDirection)
            {
                ObjectType = objectType;
                PropertyName = propertyName;
                SortDirection = sortDirection;
            }

            /// <summary>
            /// Compares the specified property of the specified object type.
            /// </summary>
            /// <param name="a">The first object to compare.</param>
            /// <param name="b">The second object to compare.</param>
            /// <returns>The comparison value.</returns>
            public int Compare(Object a, Object b)
            {
                PropertyInfo property = ObjectType.GetProperty(PropertyName);

                if (property == null)
                    throw new MissingMemberException(objectType.ToString(), PropertyName);

                object t = property.GetValue(a, (object[])null);

                IComparable c1 = (IComparable)property.GetValue(a, null);
                IComparable c2 = (IComparable)property.GetValue(b, null);

                if (SortDirection == SortDirection.Ascending)
                {
                    if (c1 is Enum && c2 is Enum)
                        return ((int)c1).CompareTo((int)c2);

                    return c1.CompareTo(c2);
                }
                else
                {
                    if (c1 is Enum && c2 is Enum)
                        return ((int)c2).CompareTo((int)c1);

                    return c2.CompareTo(c1);
                }
            }
        }

        /// <summary>
        /// Retrieves all entities within this collection with an ID that matches one of the IDs provided.
        /// </summary>
        /// <param name="id">The IDs to check for in this collection.</param>
        /// <returns>A collection of entities with the specified IDs.</returns>
        static public E GetByID(E[] entities, Guid id)
        {
            // Create the specific collection type
            System.Type specificType = typeof(Collection<>).MakeGenericType(new System.Type[] { typeof(E) });
          
            foreach (E entity in entities)
            {
                if (entity != null)
                {
                    if (entity.ID == id)
                        return entity;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves all entities within this collection with an ID that matches one of the IDs provided.
        /// </summary>
        /// <param name="ids">The IDs to check for in this collection.</param>
        /// <returns>A collection of entities with the specified IDs.</returns>
        static public E[] GetByIDs(E[] entities, Guid[] ids)
        {
            ArrayList found = new ArrayList();

            foreach (E entity in entities)
            {
                if (Array.IndexOf(ids, entity.ID) > -1)
                    found.Add(entity);
            }

            return (E[])found.ToArray(typeof(E));
        }

        /// <summary>
        /// Gets the IDs of all entities in the collection.
        /// </summary>
        /// <param name="entities">The entities to get the IDs of.</param>
        /// <returns>An array of entity IDs.</returns>
        static public Guid[] GetIDs(E[] entities)
        {
            if (entities == null || entities.Length == 0)
                return new Guid[] { };

            // Create an ID collection
            ArrayList ids = new ArrayList();

            // Loop through all the entities in the collection and get their IDs
            foreach (E entity in entities)
            {
                if (entity != null)
                {
                    PropertyInfo idProperty = entity.GetType().GetProperty("ID");
                    if (idProperty != null)
                    {
                        Guid id = (Guid)idProperty.GetValue(entity, (object[])null);
                        ids.Add(id);
                    }
                }
            }

            // Return the IDs
            return (Guid[])ids.ToArray(typeof(Guid));
        }

        /// <summary>
        /// Sorts the collection as specified.
        /// </summary>
        /// <param name="entities">The entities to sort.</param>
        /// <param name="propertyName">The name of the property being sorted by.</param>
        /// <param name="direction">The sort direction.</param>
        static public E[] Sort(E[] entities, string propertyName, SortDirection direction)
        {
            Collection<E> list = new Collection<E>(entities);
            list.Sort(propertyName, direction);
            return (E[])list.ToArray(typeof(E));
        }

        /// <summary>
        /// Sorts the collection as specified.
        /// </summary>
        /// <param name="entities">The entities to sort.</param>
        /// <param name="sortExpression">The sort expression to use to sort the entities.</param>
        static public E[] Sort(E[] entities, string sortExpression)
        {
            if (sortExpression != null && sortExpression != String.Empty)
            {
                if (sortExpression.IndexOf(SortDirection.Ascending.ToString()) > -1)
                    return Sort(entities, sortExpression.Replace(SortDirection.Ascending.ToString(), ""), SortDirection.Ascending);
                else if (sortExpression.IndexOf(SortDirection.Descending.ToString()) > -1)
                    return Sort(entities, sortExpression.Replace(SortDirection.Descending.ToString(), ""), SortDirection.Descending);
            }
            return entities;
        }

        /// <summary>
        /// Adds the provided item to the provided array.
        /// </summary>
        /// <param name="collection">The array of items.</param>
        /// <param name="newItem">The new item to add to the array.</param>
        /// <returns>An array containing the all the provided items.</returns>
        static public E[] Add(E[] items, E newItem)
        {
            List<E> list = new List<E>(items);
            if (Collection<E>.GetByID(items, newItem.ID) != null)
            {
                list.Add(newItem);
            }
            return (E[])list.ToArray();
        }

	static public Array CastArray(BaseEntity[] entities)
	{
		Collection<E> list = new Collection<E>();
		list.Add(entities);
		return list.ToArray();
	}

	}
}
