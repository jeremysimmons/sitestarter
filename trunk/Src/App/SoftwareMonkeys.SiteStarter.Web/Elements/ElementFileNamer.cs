using System;
using SoftwareMonkeys.SiteStarter.Data;
using System.IO;
using SoftwareMonkeys.SiteStarter.IO;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	/// Used to create file names/paths for elements.
	/// </summary>
	public class ElementFileNamer
	{
		private string elementsInfoFilePath;
		/// <summary>
		/// Gets the path to the file containing the element info.
		/// </summary>
		public virtual string ElementsInfoFilePath
		{
			get {
				if (elementsInfoFilePath == null || elementsInfoFilePath == String.Empty)
				{
					if (StateAccess.IsInitialized)
						elementsInfoFilePath = StateAccess.State.PhysicalApplicationPath
							+ Path.DirectorySeparatorChar + "App_Data"
							+ Path.DirectorySeparatorChar + "Elements.xml";
				}
				return elementsInfoFilePath;
			}
			set { elementsInfoFilePath = value; }
		}
		
		private IFileMapper fileMapper;
		/// <summary>
		/// Gets/sets the file mapper used for mapping the full path of relative files.
		/// </summary>
		public IFileMapper FileMapper
		{
			get {
				if (fileMapper == null)
					fileMapper = new FileMapper();
				return fileMapper; }
			set { fileMapper = value; }
		}
		
		public ElementFileNamer()
		{
		}
	}
}
