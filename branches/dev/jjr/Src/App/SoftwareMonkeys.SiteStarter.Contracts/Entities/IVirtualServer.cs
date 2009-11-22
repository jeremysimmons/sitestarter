using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    public interface IVirtualServer : IEntity, IVirtualServerConfig
    {
		Guid ID {get;set;}
		string Name {get;set;}
		Guid PrimaryAdministratorID {get;set;}
		IUser PrimaryAdministrator {get;set;}
    }
}
