using System;
using System.Data;
using System.Configuration;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    /// <summary>
    /// Defines the interface for a user role in the application.
    /// </summary>
	[DataStore("UserRoles")]
	[Serializable]
    public class UserRole : BaseEntity
    {
        private string name;
	/// <summary>
	/// Gets/sets the name of the role.
	/// </summary>
	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	private string permissions;
	/// <summary>
	/// Gets/sets the permissions available to the role.
	public string Permissions
	{
		get
		{
			return permissions;
		}
		set { permissions = value; }
	}
    }
}