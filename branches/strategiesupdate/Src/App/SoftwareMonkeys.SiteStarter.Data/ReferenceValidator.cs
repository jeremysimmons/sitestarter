using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of ReferenceValidator.
	/// </summary>
	public class ReferenceValidator
	{
		public ReferenceValidator()
		{
		}
		
		
		public void CheckForCircularReference<T>(T[] entities)
			where T : IEntity
		{
			if (ContainsCircularReference<T>(entities))
				throw new InvalidOperationException("The provided entities contain a circular reference.");
		}
		
		public void CheckForCircularReference<T>(T entity)
			where T : IEntity
		{
			if (ContainsCircularReference<T>(new T[] {entity}))
				throw new InvalidOperationException("The provided entity contains a circular reference.");
		}
		
		public bool ContainsCircularReference<T>(T[] entities)
			where T : IEntity
		{
			bool does = false;
			foreach (IEntity entity in entities)
			{
				Stack stack = new Stack();
				stack.Push(entity);
				
				if (ContainsCircularReference_StepDown<T>(entities, stack))
					does = true;
				
				stack.Pop();
				
				if (does)
					return true;
			}
			
			return does;
		}
		
		public bool ContainsCircularReference_StepDown<T>(T[] originalList, Stack stack)
			where T : IEntity
		{
			bool does = false;
			
			T foundEntity = (T)stack.Peek();
			T entity = foundEntity;//Collection<T>.GetByID(originalList, foundEntity.ID);
			
			// TODO: Check if entity should be activated
			
			foreach (PropertyInfo property in entity.GetType().GetProperties())
			{
				if (EntitiesUtilities.IsReference(entity.GetType(), property))
				{
					if (EntitiesUtilities.GetReferencedEntities(entity, property).Length == 0)
						DataAccess.Data.Activator.Activate(entity, property.Name);
					
					foreach (IEntity referencedEntity in EntitiesUtilities.GetReferencedEntities(entity, entity.GetType().GetProperty(property.Name)))
					{
						// If the referenced entity is already in the stack then it's a circular reference
						if (stack != null && stack.Count > 0)
						{
							foreach (IEntity e in stack.ToArray())
							{
								if (e.ID == referencedEntity.ID)
									return true;
							}
							
							stack.Push(referencedEntity);
							
							if (ContainsCircularReference_StepDown<T>(originalList, stack))
								does = true;
							
							stack.Pop();
							
							if (does)
								return true;
						}
					}
				}
			}
			return does;
		}
	}
}
