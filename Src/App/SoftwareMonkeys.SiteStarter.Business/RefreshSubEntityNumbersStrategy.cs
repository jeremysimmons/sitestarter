using System;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to refresh the numbers of sub items.
	/// </summary>
	public class RefreshSubEntityNumbersStrategy : BaseStrategy
	{
		/// <summary>
		/// Refreshes the numbers of the sub entities on the specified items property of the provided parent entity.
		/// </summary>
		/// <param name="parentEntity">The parent entity containing the sub items.</param>
		/// <param name="itemsPropertyName">The name of the property containing the sub items.</param>
		public void Refresh(IEntity parentEntity, string itemsPropertyName)
		{
			if (parentEntity == null)
				throw new ArgumentNullException("parentEntity");
			
			if (itemsPropertyName == null)
				throw new ArgumentNullException("itemsPropertyName");
			
			if (itemsPropertyName == String.Empty)
				throw new ArgumentException("An items property name must be provided.", "itemsPropertyName");
			
			ActivateStrategy.New(parentEntity).Activate(parentEntity, itemsPropertyName);
			
			PropertyInfo property = parentEntity.GetType().GetProperty(itemsPropertyName);
			
			if (property == null)
				throw new ArgumentException("No property with the name '" + itemsPropertyName + "' found on the type '" + parentEntity.ShortTypeName + "'.");
			
			ISubEntity[] items = (ISubEntity[])Collection<ISubEntity>.ConvertAll(property.GetValue(parentEntity, null));
			
			string numberPropertyName = String.Empty;
			
			if (items.Length > 0)
			{
				numberPropertyName = items[0].NumberPropertyName;
				
				items = Collection<ISubEntity>.Sort(items, numberPropertyName + "Ascending");
				
				Refresh(items, true);
			}
		}
		
		public virtual void Refresh(ISubEntity[] items, bool updateModified)
		{
			Collection<ISubEntity> updated = new Collection<ISubEntity>();
			
			for (int i = 0; i < items.Length; i++)
			{
				int originalNumber = items[i].Number;
				
				items[i].Number = i+1; // +1 to change index (0 based) to number (1 based)
				
				if (originalNumber != items[i].Number)
				{
					updated.Add(items[i]);
				}
			}
			
			foreach (ISubEntity item in updated)
			{
				ActivateStrategy.New(item, RequireAuthorisation).Activate(item);
				
				UpdateStrategy.New(item, RequireAuthorisation).Update(item);
			}
		}
		
		static public RefreshSubEntityNumbersStrategy New()
		{
			return new RefreshSubEntityNumbersStrategy();
		}
		
		static public RefreshSubEntityNumbersStrategy New(bool requireAuthorisation)
		{
			RefreshSubEntityNumbersStrategy strategy = new RefreshSubEntityNumbersStrategy();
			
			strategy.RequireAuthorisation = requireAuthorisation;
			
			return strategy;
		}
	}
}
