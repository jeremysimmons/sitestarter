using System;
using System.Data;
using System.Configuration;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    /// <summary>
    /// Defines the interface for a user role in the application.
    /// </summary>
    public struct UserPermission
    {
        private string verb;
	/// <summary>
	/// Gets/sets the verb that the permission applies to.
	/// </summary>
	public string Verb
	{
		get
		{
			return verb;
		}
		set
		{
			verb = value;
		}
	}

	private Type entityType;
	/// <summary>
	/// Gets/sets the entity type that the permission applies to.
	public Type EntityType
	{
		get
		{
			return entityType;
		}
		set { entityType = value; }
	}
    }
}