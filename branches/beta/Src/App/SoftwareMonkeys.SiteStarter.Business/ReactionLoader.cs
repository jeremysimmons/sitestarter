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
		
		public ReactionInfo[] Reactions;
		
		public ReactionLoader()
		{
		}
		
		/// <summary>
		/// Loads all the reactions found in the reactions directory.
		/// </summary>
		/// <returns>An array of the the reactions found.</returns>
		public ReactionInfo[] LoadInfoFromFile()
		{
			return LoadInfoFromFile(false);
		}
		
		/// <summary>
		/// Loads all the reactions found in the reactions file.
		/// </summary>
		/// <param name="includeDisabled"></param>
		/// <returns>An array of the the reactions found.</returns>
		public ReactionInfo[] LoadInfoFromFile(bool includeDisabled)
		{
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.StartDebug("Loading the reactions from the XML file."))
			//{
			if (Reactions == null)
			{
				List<ReactionInfo> validReactions = new List<ReactionInfo>();
				
				ReactionInfo[] reactions = new ReactionInfo[]{};
				
				using (StreamReader reader = new StreamReader(File.OpenRead(FileNamer.ReactionsInfoFilePath)))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(ReactionInfo[]));
					reactions = (ReactionInfo[])serializer.Deserialize(reader);
				}
				
				foreach (ReactionInfo reaction in reactions)
					if (reaction.Enabled || includeDisabled)
						validReactions.Add(reaction);
				
				Reactions = validReactions.ToArray();
			}
			//}
			return Reactions;
		}
	}
}
