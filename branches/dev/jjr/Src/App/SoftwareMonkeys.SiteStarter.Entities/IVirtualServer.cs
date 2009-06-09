using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    public interface IVirtualServer : IEntity
    {
		Guid ID {get;set;}
		Guid PrimaryAdministratorID {get;set;}
    }
}
