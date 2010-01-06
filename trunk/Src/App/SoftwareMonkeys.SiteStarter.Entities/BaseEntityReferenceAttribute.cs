using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    public abstract class BaseEntityReferenceAttribute : Attribute
    {
        private bool cascadeSave;
        /// <summary>
        /// Gets/sets a boolean value indicating whether save operations should cascade through to the assigned entities.
        /// </summary>
        public bool CascadeSave
        {
            get { return cascadeSave; }
            set { cascadeSave = value; }
        }

        private bool cascadeDelete;
        /// <summary>
        /// Gets/sets a boolean value indicating whether delete operations should cascade through to the assigned entities.
        /// </summary>
        public bool CascadeDelete
        {
            get { return cascadeDelete; }
            set { cascadeDelete = value; }
        }

        private bool cascadeUpdate;
        /// <summary>
        /// Gets/sets a boolean value indicating whether update operations should cascade through to the assigned entities.
        /// </summary>
        public bool CascadeUpdate
        {
            get { return cascadeUpdate; }
            set { cascadeUpdate = value; }
        }

        private bool excludeFromDataStore;
        /// <summary>
        /// Gets/sets a boolean value indicating whether the references and entities should be left out of the data store.
        /// </summary>
        public bool ExcludeFromDataStore
        {
            get { return excludeFromDataStore; }
            set { excludeFromDataStore = value; }
        }

        private string idsPropertyName = String.Empty;
        /// <summary>
        /// Gets/sets the name of the IDs property that corresponds with this reference.
        /// </summary>
        public string IDsPropertyName
        {
            get { return idsPropertyName; }
            set { idsPropertyName = value; }
        }

        private string entitiesPropertyName = String.Empty;
        /// <summary>
        /// Gets/sets the name of the entities property that corresponds with this reference.
        /// </summary>
        public string EntitiesPropertyName
        {
            get { return entitiesPropertyName; }
            set { entitiesPropertyName = value; }
        }

        private string mirrorName = String.Empty;
        /// <summary>
        /// Gets/sets the name of the opposite property on the opposite entity.
        /// </summary>
        public string MirrorName
        {
            get { return mirrorName; }
            set { mirrorName = value; }
        }
        
        /*private Type mirrorType;
        /// <summary>
        /// Gets/sets the type of the mirror propery.
        /// </summary>
        public Type MirrorType
        {
            get { return mirrorType; }
            set { mirrorType = value; }
        }*/
        
                
        private string referenceTypeName = String.Empty;
        /// <summary>
        /// Gets/sets the short name of the mirror type.
        /// </summary>
        public string ReferenceTypeName
        {
            get { return referenceTypeName; }
            set { referenceTypeName = value; }
        }
        
        private Type entitiesPropertyType;
        /// <summary>
        /// Gets/sets the entities property type.
        /// </summary>
        public Type EntitiesPropertyType
        {
            get { return entitiesPropertyType; }
            set { entitiesPropertyType = value; }
        }

        public BaseEntityReferenceAttribute()
        {

        }

        public BaseEntityReferenceAttribute(bool excludeFromDateStore, bool cascadeSave, bool cascadeUpdate, bool cascadeDelete, string idsPropertyName, string mirrorName)
        {
            ExcludeFromDataStore = excludeFromDataStore;
            CascadeSave = cascadeSave;
            CascadeUpdate = cascadeUpdate;
            CascadeDelete = cascadeDelete;
            IDsPropertyName = idsPropertyName;
            MirrorName = mirrorName;
        }
    }
}
