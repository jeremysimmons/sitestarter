using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
    public interface IVirtualServerConfig : IConfig
    {
		Guid ID {get;set;}
		IUser PrimaryAdministrator {get;set;}
    }
}
