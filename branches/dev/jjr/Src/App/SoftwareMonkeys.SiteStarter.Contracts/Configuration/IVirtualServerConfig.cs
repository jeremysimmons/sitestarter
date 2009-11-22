using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
    public interface IVirtualServerConfig : IConfig
    {
		Guid ID {get;set;}
		Guid PrimaryAdministratorID {get;set;}
    }
}
