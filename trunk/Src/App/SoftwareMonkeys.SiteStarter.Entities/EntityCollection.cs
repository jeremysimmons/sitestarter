using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Holds a collection of entities.
	/// </summary>
	[Serializable]
	public class Collection<E> : List<E>, ICollection<E>, IEnumerable<E>, IEnumerable
		where E : SoftwareMonkeys.SiteStarter.Entities.IEntity
	{
		public Guid[] IDs
		{
			get { return GetIDs(); }
			set { SetIDs(value); }
		}
		
		/// <summary>
		/// Gets/sets the entity at the specified position in the collection.
		/// </summary>
		public new E this[int index]
		{
			get { return (E)base[index]; }
			set { base[index] = value; }
		}

		/// <summary>
		/// Gets/sets the entity in the collection with the provided ID.
		/// </summary>
		public E this[Guid id]
		{
			get
			{
				foreach (E entity in this)
					if (entity.ID == id)
						return entity;
				return default(E);
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
		{ }

		/// <summary>
		/// Adds the provided entity to the collection.
		/// </summary>
		/// <param name="entity">The entity to add to the collection.</param>
		public Collection(E entity)
		{
			if (entity != null)
				Add((E)entity);
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
					if (entity.GetType().FullName == typeof(E).FullName
					    || typeof(E).IsAssignableFrom(entity.GetType()))
						Add((E)entity);
					else
						throw new NotSupportedException("Invalid type: Expected " + typeof(E).ToString() + " but was " + entity.GetType() + ".");
				}
			}
		}
		
		/// <summary>
		/// Adds the provided entities to the collection.
		/// </summary>
		/// <param name="data">Entities to add to the collection.</param>
		public Collection(object data)
		{
			if (data != null)
			{
				if (data is IEnumerable)
					AddRange((IEnumerable)data);
				else
					throw new ArgumentException("Invalid data argument: " + data.GetType().ToString());
			}
		}

		/// <summary>
		/// Adds the provided entities to the collection.
		/// </summary>
		/// <param name="entity">The entity to add to the collection.</param>
		public new void AddRange(IEnumerable<E> entities)
		{
			if (entities != null)
			{
				foreach (object obj in entities)
				{
					if (obj != null)
						if (obj is E)
							base.Add((E)obj);
				}
			}
		}
		
		/// <summary>
		/// Adds the provided entities to the collection.
		/// </summary>
		/// <param name="entity">The entity to add to the collection.</param>
		public void AddRange(IEnumerable entities)
		{
			if (entities != null)
			{
				foreach (object obj in entities)
				{
					if (obj != null)
					{
						if (obj is E)
							base.Add((E)obj);
						else
							throw new ArgumentException("Invalid item type: " + obj.ToString());
					}
				}
			}
		}

		/// <summary>
		/// Removes the provided entity from the collection.
		/// </summary>
		/// <param name="entity">The entity to remove from the collection.</param>
		public new void Remove(E entity)
		{
			if (entity != null && base.Contains(entity))
				base.Remove(entity);
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
		/// Sets the IDs of all entities in the collection.
		/// </summary>
		public void SetIDs(Guid[] ids)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Setting entity IDs on the collection."))
			{
				LogWriter.Debug("The following IDs were passed:");
				if (ids != null && ids.Length > 0)
				{
					foreach (Guid id in ids)
					{
						LogWriter.Debug(id.ToString());
					}
				}
				else
				{
					LogWriter.Debug("None");
				}
				
				// Remove obsolete entities
				for (int i = 0; i < this.Count; i ++)
				{
					if (Array.IndexOf(ids, this[i].ID) == -1)
					{
						LogWriter.Debug("Removing obsolete enitity: " + this[i].ID.ToString());
						RemoveAt(i);
						i--;
					}
				}

				// Add new entity pointers
				for (int i = 0; i < ids.Length; i ++)
				{
					if (!Contains(ids[i]))
					{
						LogWriter.Debug("Adding enitity: " + ids[i].ToString());
						E pointer = (E)Activator.CreateInstance(typeof(E), new object[]{ids[i]});
						
						Add(pointer);
					}
				}
				
				LogWriter.Debug("The following IDs are now in the collection:");
				if (ids != null && ids.Length > 0)
				{
					foreach (E e in this)
					{
						LogWriter.Debug(e.ID.ToString());
					}
				}
				else
				{
					LogWriter.Debug("None");
				}
			}
		}

		/// <summary>
		/// Checks whether the provided entity instance is found in the collection.
		/// </summary>
		/// <param name="entity">The entity to look for in the application.</param>
		/// <returns>A boolean value indicating whether the entity was found in the collection.</returns>
		public new bool Contains(E entity)
		{
			return base.Contains(entity);
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
		public new E[] ToArray()
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
			if (Count > 0 && this[0] != null)
			{
				Type type = this[0].GetType();
				base.Sort(new DynamicComparer<E>(type, propertyName, direction));
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
		
		public Collection<E> GetPage(int pageIndex, int pageSize)
		{
			Collection<E> page = new Collection<E>();
			
			for (int i = 0; i < Count; i++)
			{
				if (new PagingLocation(pageIndex, pageSize).IsInPage(i))
					page.Add(this[i]);
			}
			
			return page;
		}

		/// <summary>
		/// Retrieves all entities within this collection with an ID that matches one of the IDs provided.
		/// </summary>
		/// <param name="id">The IDs to check for in this collection.</param>
		/// <returns>A collection of entities with the specified IDs.</returns>
		static public E GetByID(E[] entities, Guid id)
		{
			if (entities == null)
				throw new ArgumentNullException("entities");
			
			if (id == Guid.Empty)
				throw new ArgumentException("id", "The provided ID cannot be Guid.Empty.");
			
			foreach (E entity in entities)
			{
				if (entity != null)
				{
					if (entity.ID == id)
						return entity;
				}
			}

			return default(E);
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
			List<Guid> ids = new List<Guid>();

			// Loop through all the entities in the collection and get their IDs
			foreach (object entity in entities)
			{
				if (entity != null)
				{
					if (entity is IEntity)
					{
						
						ids.Add(((IEntity)entity).ID);
					}
					else
						throw new NotSupportedException("Invalid type: "+ entity.GetType());
				}
			}

			// Return the IDs
			return (Guid[])ids.ToArray();
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
			// Only add it if the item isn't already in the list (ie. GetByID(...) == null)
			if (Collection<E>.GetByID(items, newItem.ID) == null)
			{
				list.Add(newItem);
			}
			return (E[])list.ToArray();
		}
		
		
		/// <summary>
		/// Adds the provided item to the provided array.
		/// </summary>
		/// <param name="collection">The array of items.</param>
		/// <param name="newItem">The index of the item to remove..</param>
		/// <returns>An array containing the all the provided items.</returns>
		static public E[] RemoveAt(E[] items, int index)
		{
			List<E> list = new List<E>(items);
			if (items.Length > index)
			{
				list.RemoveAt(index);
			}
			return (E[])list.ToArray();
		}
		
		/// <summary>
		/// Removes the provided item from the provided array.
		/// </summary>
		/// <param name="collection">The array of items.</param>
		/// <param name="newItem">The index of the item to remove..</param>
		/// <returns>An array containing the all the provided items.</returns>
		static public E[] Remove(E[] items, E item)
		{
			Collection<E> list = new Collection<E>(items);
			list.Remove(list[item.ID]);
			
			return (E[])list.ToArray();
		}

		/*static public E[] ConvertAll(IEntity[] entities)
        {
			if (entities == null || entities.Length == 0)
				return new E[] {};
		    return (E[])Array.ConvertAll<IEntity, E>(entities, new Converter<IEntity, E>(IEntity_Convert));
        }*/
		
		static public E[] ConvertAll(object entities)
		{
			if (entities == null)
				return new E[] {};
			
			Collection<E> collection = new Collection<E>();
			
			if (entities is IEnumerable)
			{
				foreach (object obj in (IEnumerable)entities)
				{
					collection.Add((E)obj);
				}
			}
			else
				throw new NotSupportedException("Type note supported:" + entities.GetType().ToString());
			
			return (E[])collection.ToArray();
			//return (E[])Array.ConvertAll<object, E>((object[])entities, new Converter<object, E>(object_Convert));
		}
		
		static public E[] ConvertAll(Array entities, Type type)
		{
			if (entities == null)
				return new E[] {};
			
			ArrayList collection = new ArrayList();
			
			foreach (object obj in entities)
			{
				if (type.IsAssignableFrom(obj.GetType()))
					collection.Add((E)obj);
				else
					throw new InvalidCastException("Cannot cast type '" + obj.GetType().ToString() + "' to type '" + type.ToString() + "'.");
			}
			
			return (E[])collection.ToArray(type);
			//return (E[])Array.ConvertAll<object, E>((object[])entities, new Converter<object, E>(object_Convert));
		}

		/* static public E IEntity_Convert(IEntity entity)
        {
		//if (entity == null)
		//	return null;
		//else
	            return (E)entity;
        }*/
		
		static public E object_Convert(object entity)
		{
			//if (entity == null)
			//	return null;
			//else
			if (entity is E)
				return (E)entity;
			else
				throw new InvalidOperationException("The type '" + entity.GetType().ToString() + "' is incompatible with type '" + typeof(E).ToString() + "'.");
		}
	}
}
