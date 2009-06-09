using System;
using System.Data;
using System.Configuration;
using System.Web;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    /// <summary>
    /// Defines the interface for all entities.
    /// </summary>
    public interface IEntity
    {
        Guid ID { get;set;}
    }
}