using System;
using SoftwareMonkeys.SiteStarter.State;


namespace SoftwareMonkeys.SiteStarter.Web.Parts
{
	/// <summary>
	/// Holds the state of all available parts.
	/// </summary>
	public class PartStateCollection : StateNameValueCollection<PartInfo>
	{
		/// <summary>
		/// Gets/sets the part for the specifid action and type.
		/// </summary>
		public PartInfo this[string action, string type]
		{
			get { return GetPart(action, type); }
			set { SetPart(action, type, value); }
		}
		
		/// <summary>
		/// Gets/sets the part for the specifid action and type.
		/// </summary>
		public PartInfo this[string action, Type type]
		{
			get { return GetPart(action, type.Name); }
			set { SetPart(action, type.Name, value); }
		}
		
		public PartStateCollection() : base(StateScope.Application, "Web.Parts")
		{
		}
		
		public PartStateCollection(PartInfo[] strategies) : base(StateScope.Application, "Web.Parts")
		{
			foreach (PartInfo part in strategies)
			{
				SetPart(part.Action, part.TypeName, part);
			}
		}
		
		/// <summary>
		/// Adds the provided part info to the collection.
		/// </summary>
		/// <param name="part">The part info to add to the collection.</param>
		public void Add(PartInfo part)
		{
			if (part == null)
				throw new ArgumentNullException("part");
			
			string key = GetPartKey(part.Action, part.TypeName);
			
			base[key] = part;
		}
		
		/// <summary>
		/// Checks whether a part exists with the provided key.
		/// </summary>
		/// <param name="key">The key of the part to check for.</param>
		/// <returns>A value indicating whether the part exists.</returns>
		public bool PartExists(string key)
		{
			return StateValueExists(key);
		}
		
		/// <summary>
		/// Retrieves the part with the provided action and type.
		/// </summary>
		/// <param name="action">The action that the part performs.</param>
		/// <param name="typeName">The type of entity involved in the part</param>
		/// <returns>The part matching the provided action and type.</returns>
		public PartInfo GetPart(string action, string typeName)
		{
			PartLocator locator = new PartLocator(this);
			
			PartInfo foundPart = locator.Locate(action, typeName);
			
			if (foundPart == null)
				throw new PartNotFoundException(action, typeName);
			
			
			return foundPart;
		}
		
		
		/// <summary>
		/// Retrieves the part with the provided action and type.
		/// </summary>
		/// <param name="action">The action that the part performs.</param>
		/// <returns>The part matching the provided action and type.</returns>
		public PartInfo GetPart(string action)
		{
			PartLocator locator = new PartLocator(this);
			
			PartInfo foundPart = locator.Locate(action);
			
			if (foundPart == null)
				throw new PartNotFoundException(action);
			
			
			return foundPart;
		}

		/// <summary>
		/// Sets the part with the provided action and type.
		/// </summary>
		/// <param name="action">The action that the part performs.</param>
		/// <param name="type">The type of entity involved in the part</param>
		/// <param name="part">The part that corresponds with the specified action and type.</param>
		public void SetPart(string action, string type, PartInfo part)
		{
			base[GetPartKey(action, type)] = part;
		}
		
		/// <summary>
		/// Sets the part with the provided action and type.
		/// </summary>
		/// <param name="action">The action that the part performs.</param>
		/// <param name="part">The part that corresponds with the specified action and type.</param>
		public void SetPart(string action, PartInfo part)
		{
			base[GetPartKey(action)] = part;
		}

		/// <summary>
		/// Retrieves the key for the specifid action and type.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public string GetPartKey(string action, string type)
		{
			if (type == String.Empty)
			{
				return GetPartKey(action);
			}
			else
			{
				string fullKey = String.Empty;
				
				if (action != String.Empty)
					fullKey += action + "-";
				
				fullKey += type;
				
				return fullKey;
			}
		}
		
		/// <summary>
		/// Retrieves the key for the specifid action.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		public string GetPartKey(string action)
		{
			string fullKey = action;
			
			return fullKey;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		public string GetPartKey(PartInfo info)
		{
			if (info.Action != String.Empty
			    && info.TypeName != String.Empty)
			{
				return GetPartKey(info.Action, info.TypeName);
			}
			else
			{
				return GetPartKey(info.Action);
			}
		}
	}
}
