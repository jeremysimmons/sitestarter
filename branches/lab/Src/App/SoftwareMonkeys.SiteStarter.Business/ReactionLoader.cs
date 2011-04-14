using System;
using System.IO;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to load reactions from file.
	/// </summary>
	public class ReactionLoader
	{
		private ReactionFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create file paths for reactions.
		/// </summary>
		public ReactionFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new ReactionFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		private string reactionsDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the directory containing the reaction files.
		/// </summary>
		public string ReactionsDirectoryPath
		{
			get {
				if (reactionsDirectoryPath == null || reactionsDirectoryPath == String.Empty)
				{
					if (FileNamer == null)
						throw new InvalidOperationException("FileNamer is not set.");
					
					reactionsDirectoryPath = FileNamer.ReactionsDirectoryPath;
				}
				return reactionsDirectoryPath; }
			set { reactionsDirectoryPath = value; }
		}
		
		public ReactionLoader()
		{
		}
		
		/// <summary>
		/// Loads all the reactions found in the reactions directory.
		/// </summary>
		/// <returns>An array of the the reactions found in the directory.</returns>
		public ReactionInfo[] LoadFromDirectory()
		{
			ReactionInfoCollection reactions = new ReactionInfoCollection();
			
			using (LogGroup logGroup = LogGroup.Start("Loading the reactions from the XML files.", NLog.LogLevel.Debug))
			{
				foreach (string file in Directory.GetFiles(ReactionsDirectoryPath))
				{
					LogWriter.Debug("File: " + file);
					
					reactions.Add(LoadFromFile(file));
				}
			}
			
			return reactions.ToArray();
		}
		
		/// <summary>
		/// Loads the reaction from the specified path.
		/// </summary>
		/// <param name="reactionPath">The full path to the reaction to load.</param>
		/// <returns>The reaction deserialized from the specified file path.</returns>
		public ReactionInfo LoadFromFile(string reactionPath)
		{
			ReactionInfo info = null;
			
			using (LogGroup logGroup = LogGroup.Start("Loading the reaction from the specified path.", NLog.LogLevel.Debug))
			{
				if (!File.Exists(reactionPath))
					throw new ArgumentException("The specified file does not exist.");
				
				LogWriter.Debug("Path: " + reactionPath);
				
				
				using (StreamReader reader = new StreamReader(File.OpenRead(reactionPath)))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(ReactionInfo));
					
					info = (ReactionInfo)serializer.Deserialize(reader);
					
					reader.Close();
				}
			}
			
			return info;
		}
	}
}
