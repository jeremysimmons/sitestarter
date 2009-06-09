using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data.Tests.Entities
{
    [DataStore("Testing1")]
    public class EntityOne : BaseEntity
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
    }
}
