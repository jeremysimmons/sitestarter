using System;
using System.Xml;
using System.Xml.Serialization;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// The base of all configuration components.
	/// </summary>
	public class BaseConfig : IConfig
	{
		private string name;
        /// <summary>
        /// Gets/sets the name of the configuration object/file.
        /// </summary>
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }
        
        private IConfigSaver saver;
        /// <summary>
        /// Gets/sets the saver used to save the configuration component.
        /// </summary>
        [XmlIgnore] // Leave out of XML
        public IConfigSaver Saver
        {
        	get { return saver; }
        	set { saver = value; }
        }
        
        private string filePath = String.Empty;
        /// <summary>
        /// Gets/sets the path to the configuration file.
        /// </summary>
        public string FilePath
        {
        	get { return filePath; }
        	set { filePath = value; }
        }
        
        /// <summary>
        /// Saves the configuration component to file.
        /// </summary>
        public void Save()
        {
        	if (Saver == null)
        		throw new InvalidOperationException("Cannot save when the saver has not been initialized to the Saver property.");
        	
        	Saver.Save(this);
        }
        
        
    	private string pathVariation;
        /// <summary>
        /// Gets/sets the variation applied to the config file path (eg. staging, local, etc.).
        /// </summary>
        public string PathVariation
        {
        	get { return pathVariation; }
        	set { pathVariation = value; }
        }
	}
}
