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
		private string reactionsDirectoryPath;
		/// <summary>
		/// Gets the full path to the directory containing reaction mappings.
		/// </summary>
		public string ReactionsDirectoryPath
		{
			get {
				if (reactionsDirectoryPath == null || reactionsDirectoryPath == String.Empty)
				{
					if (FileNamer != null)
						reactionsDirectoryPath = FileNamer.ReactionsInfoDirectoryPath;
					
				}
				return reactionsDirectoryPath;
			}
			set { reactionsDirectoryPath = value; }
		}
		
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
		/// Saves the provided reaction to the reactions mappings directory.
		/// </summary>
		/// <param name="reaction">The reaction to save to file.</param>
		public void SaveToFile(ReactionInfo reaction)
		{
			using (LogGroup logGroup = LogGroup.Start("Saving the provided reaction to file.", NLog.LogLevel.Debug))
			{
				string path = FileNamer.CreateInfoFilePath(reaction);
				
				LogWriter.Debug("Path : " + path);
				
				if (!Directory.Exists(Path.GetDirectoryName(path)))
					Directory.CreateDirectory(Path.GetDirectoryName(path));
				
				using (StreamWriter writer = File.CreateText(path))
				{
					XmlSerializer serializer = new XmlSerializer(reaction.GetType());
					serializer.Serialize(writer, reaction);
					writer.Close();
				}
			}
		}
		
		/// <summary>
		/// Saves the provided reactions to file.
		/// </summary>
		/// <param name="reactions">An array of the reactions to save to file.</param>
		public void SaveToFile(ReactionInfo[] reactions)
		{
			using (LogGroup logGroup = LogGroup.Start("Saving the provided reactions to XML files.", NLog.LogLevel.Debug))
			{
				foreach (ReactionInfo reaction in reactions)
				{
					SaveToFile(reaction);
				}
			}
		}
	}
}
