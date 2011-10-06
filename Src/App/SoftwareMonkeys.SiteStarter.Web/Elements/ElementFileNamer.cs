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
		private string elementsDirectoryPath;
		/// <summary>
		/// Gets the path to the directory containing the element files.
		/// </summary>
		public virtual string ElementsDirectoryPath
		{
			get {
				if (elementsDirectoryPath == null || elementsDirectoryPath == String.Empty)
				{
					if (StateAccess.IsInitialized)
						elementsDirectoryPath = StateAccess.State.PhysicalApplicationPath
							+ Path.DirectorySeparatorChar + "Elements";
				}
				return elementsDirectoryPath;
			}
			set { elementsDirectoryPath = value; }
		}
		
		private string elementsInfoDirectoryPath;
		/// <summary>
		/// Gets the path to the directory containing serialized element information.
		/// </summary>
		public virtual string ElementsInfoDirectoryPath
		{
			get {
				if (elementsInfoDirectoryPath == null || elementsInfoDirectoryPath == String.Empty)
				{
					if (StateAccess.IsInitialized)
						elementsInfoDirectoryPath = StateAccess.State.PhysicalApplicationPath
							+ Path.DirectorySeparatorChar + "App_Data"
							+ Path.DirectorySeparatorChar + "Elements";
				}
				return elementsInfoDirectoryPath;
			}
			set { elementsInfoDirectoryPath = value; }
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
		
		/// <summary>
		/// Creates the file name for the serialized info for the provided element.
		/// </summary>
		/// <param name="element">The element to create the file name for.</param>
		/// <returns>The full file name for the serialized info for the provided element.</returns>
		public virtual string CreateInfoFileName(ElementInfo element)
		{			
			if (element == null)
				throw new ArgumentNullException("element");
			
			// Use the element name
			string name = element.Name;
			
			// If the type and action are specified then use them as the name instead
			if (element.TypeName != String.Empty && element.Action != String.Empty)
			{
				name = element.TypeName + "-" + element.Action;
			}
			
			name = name + ".element";
			
			return name;
		}
		
		/// <summary>
		/// Creates the full file path for the serialized info for the provided element.
		/// </summary>
		/// <param name="element">The element to create the file path for.</param>
		/// <returns>The full file path for the serialized info for the provided element.</returns>
		public string CreateInfoFilePath(ElementInfo element)
		{
			return ElementsInfoDirectoryPath + Path.DirectorySeparatorChar + CreateInfoFileName(element);
		}
	}
}
