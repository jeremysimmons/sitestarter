using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
	[Serializable]
    public class EntityOne : BaseTestEntity
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private Guid[] twoIDs;
        public Guid[] TwoIDs
        {
            get { return twoIDs; }
            set { twoIDs = value; }
        }
        
        private EntityTwo singleReferenceProperty;
        [Reference]
        public EntityTwo SingleReferenceProperty
        {
        	get { return singleReferenceProperty; }
        	set { singleReferenceProperty = value; }
        }
        
        public T DoSomething<T>(T input)
        	where T : IEntity
        {
        	return input;
        }
    }
}
