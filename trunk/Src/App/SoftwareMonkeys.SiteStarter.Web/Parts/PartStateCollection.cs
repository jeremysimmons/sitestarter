using System;
using SoftwareMonkeys.SiteStarter.State;


namespace SoftwareMonkeys.SiteStarter.Web.Parts
{
	/// <summary>
	/// Holds the state of all available projections.
	/// </summary>
	public class PartStateCollection : StateNameValueCollection<PartInfo>
	{
		/// <summary>
		/// Gets/sets the projection for the specifid action and type.
		/// </summary>
		public PartInfo this[string action, string type]
		{
			get { return GetPart(action, type, PartFormat.Html); }
			set { SetPart(action, type, PartFormat.Html, value); }
		}
		
		/// <summary>
		/// Gets/sets the projection for the specifid action and type.
		/// </summary>
		public PartInfo this[string action, Type type]
		{
			get { return GetPart(action, type.Name, PartFormat.Html); }
			set { SetPart(action, type.Name, PartFormat.Html, value); }
		}
		
		/// <summary>
		/// Gets/sets the projection for the specifid action and type.
		/// </summary>
		public PartInfo this[string action, string type, PartFormat format]
		{
			get { return GetPart(action, type, format); }
			set { SetPart(action, type, format, value); }
		}
		
		/// <summary>
		/// Gets/sets the projection for the specifid action and type.
		/// </summary>
		public PartInfo this[string action, Type type, PartFormat format]
		{
			get { return GetPart(action, type.Name, format); }
			set { SetPart(action, type.Name, format, value); }
		}
		
		
		
		public PartStateCollection() : base(StateScope.Application, "Web.Parts")
		{
		}
		
		public PartStateCollection(PartInfo[] strategies) : base(StateScope.Application, "Web.Parts")
		{
			foreach (PartInfo projection in strategies)
			{
				SetPart(projection.Action, projection.TypeName, projection.Format, projection);
			}
		}
		
		/// <summary>
		/// Adds the provided projection info to the collection.
		/// </summary>
		/// <param name="projection">The projection info to add to the collection.</param>
		public void Add(PartInfo projection)
		{
			if (projection == null)
				throw new ArgumentNullException("projection");
			
			string key = GetPartKey(projection.Action, projection.TypeName, projection.Format);
			
			this[key] = projection;
		}
		
		// TODO: Remove if not needed
		/*/// <summary>
		/// Adds the info of the provided projection to the collection.
		/// </summary>
		/// <param name="projection">The projection info to add to the collection.</param>
		public void Add(IPart projection)
		{
			if (projection == null)
				throw new ArgumentNullException("projection");
			
			Add(new PartInfo(projection));
		}*/
		
		
		/// <summary>
		/// Checks whether a projection exists with the provided key.
		/// </summary>
		/// <param name="key">The key of the projection to check for.</param>
		/// <returns>A value indicating whether the projection exists.</returns>
		public bool PartExists(string key)
		{
			return StateValueExists(key);
		}
		
		/// <summary>
		/// Retrieves the projection with the provided action and type.
		/// </summary>
		/// <param name="action">The action that the projection performs.</param>
		/// <param name="typeName">The type of entity involved in the projection</param>
		/// <param name="format">The output format of the desired projection.</param>
		/// <returns>The projection matching the provided action and type.</returns>
		public PartInfo GetPart(string action, string typeName, PartFormat format)
		{
			PartLocator locator = new PartLocator(this);
			
			PartInfo foundPart = locator.Locate(action, typeName, format);
			
			if (foundPart == null)
				throw new PartNotFoundException(action, typeName, format);
			
			
			return foundPart;
		}

		/// <summary>
		/// Sets the projection with the provided action and type.
		/// </summary>
		/// <param name="action">The action that the projection performs.</param>
		/// <param name="type">The type of entity involved in the projection</param>
		/// <param name="projection">The projection that corresponds with the specified action and type.</param>
		/// <param name="format">The output format of the projection.</param>
		public void SetPart(string action, string type, PartFormat format, PartInfo projection)
		{
			this[GetPartKey(action, type, format)] = projection;
		}

		/// <summary>
		/// Retrieves the key for the specifid action and type.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="type"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public string GetPartKey(string action, string type, PartFormat format)
		{
			string fullKey = action + "_" + type + "_" + format.ToString();
			
			return fullKey;
		}
	}
}
