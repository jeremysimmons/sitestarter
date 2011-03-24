using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Defines the interface of all sub entities throughout the system. Example: The items of an order, or the pages or an article.
	/// </summary>
	public interface ISubEntity : IEntity
	{
		IEntity Parent { get;set; }
		string ParentTypeName { get;}
		string ParentPropertyName { get; }
		int Number { get;set; }
		string NumberPropertyName { get; }
		string ItemsPropertyName { get; }
	}
}
