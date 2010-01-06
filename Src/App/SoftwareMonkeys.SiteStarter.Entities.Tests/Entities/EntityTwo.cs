using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests.Entities
{
	[Serializable]
   	[DataStore("Testing")]
    public class EntityTwo : BaseEntity
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private Guid[] oneIDs;
        public Guid[] OneIDs
        {
            get { return oneIDs; }
            set { oneIDs = value; }
        }
    }
}
