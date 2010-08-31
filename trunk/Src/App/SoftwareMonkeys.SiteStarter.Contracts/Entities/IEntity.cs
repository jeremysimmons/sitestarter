using System;
using System.Data;
using System.Configuration;
using System.Web;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    /// <summary>
    /// Defines the interface for all entities.
    /// </summary>
    public interface IEntity
    {
    	string UniqueKey {get;}
        Guid ID { get;set; }
        string ShortTypeName { get; }
        void Strip();
        IEntity Clone();
    }
}