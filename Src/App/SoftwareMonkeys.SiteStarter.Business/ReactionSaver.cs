using System;
using System.IO;
using SoftwareMonkeys.SiteStarter.Data;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to save reaction components to file.
	/// </summary>
	public class ReactionSaver
	{
		private ReactionFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create file names/paths.
		/// </summary>
		public ReactionFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new ReactionFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		public ReactionSaver()
		{
		}
		
		/// <summary>
		/// Saves the provided reactions to file.
		/// </summary>
		/// <param name="reactions">An array of the reactions to save to file.</param>
		public void SaveToFile(ReactionInfo[] reactions)
		{
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.StartDebug("Saving the provided reactions to XML file."))
			//{
			string path = FileNamer.ReactionsInfoFilePath;
			
			//LogWriter.Debug("Path : " + path);
			
			if (!Directory.Exists(Path.GetDirectoryName(path)))
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			
			using (StreamWriter writer = File.CreateText(path))
			{
				XmlSerializer serializer = new XmlSerializer(reactions.GetType());
				serializer.Serialize(writer, reactions);
				writer.Close();
			}
			//}
		}
	}
}
