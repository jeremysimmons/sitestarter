using System;
using SoftwareMonkeys.SiteStarter.Data;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to initialize the reaction state and make reactions available for use.
	/// </summary>
	public class ReactionInitializer
	{
		private ReactionFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create reaction file names/paths.
		/// </summary>
		public ReactionFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new ReactionFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		private ReactionSaver saver;
		/// <summary>
		/// Gets/sets the reaction saver used to save reactions to file.
		/// </summary>
		public ReactionSaver Saver
		{
			get {
				if (saver == null)
				{
					saver = new ReactionSaver();
				}
				return saver; }
			set { saver = value; }
		}
		
		private ReactionScanner scanner;
		/// <summary>
		/// Gets/sets the reaction scanner used to find available reactions in the existing assemblies.
		/// </summary>
		public ReactionScanner Scanner
		{
			get {
				if (scanner == null)
				{
					scanner = new ReactionScanner();
				}
				return scanner; }
			set { scanner = value; }
		}
		
		private ReactionLoader loader;
		/// <summary>
		/// Gets/sets the reaction loader used to find available reactions in the existing assemblies.
		/// </summary>
		public ReactionLoader Loader
		{
			get {
				if (loader == null)
				{
					loader = new ReactionLoader();
				}
				return loader; }
			set { loader = value; }
		}
		
		/// <summary>
		/// Gets a value indicating whether the reactions info has been cached.
		/// </summary>
		public bool IsCached
		{
			get {
				bool isCached = ReactionsInfoExists();
				return isCached; }
		}
		
		public ReactionInitializer()
		{
		}
		
		
		/// <summary>
		/// Initializes the reactions and loads all reactions to state.
		/// </summary>
		/// <param name="reactions">The reactions to initialize.</param>
		public void Initialize(ReactionInfo[] reactions)
		{
			ReactionState.Reactions = new ReactionStateNameValueCollection(reactions);
		}
		
		/// <summary>
		/// Initializes the reactions and loads all reactions to state.
		/// </summary>
		public void Initialize()
		{
			Initialize(false);
		}
		
		/// <summary>
		/// Initializes the reactions and loads all reactions to state. Note: Skips initialization if already initialized.
		/// </summary>
		/// <param name="includeTestReactions"></param>
		public void Initialize(bool includeTestReactions)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Initializing the business reactions."))
			{
				ReactionInfo[] reactions = new ReactionInfo[]{};
				if (!ReactionState.IsInitialized)
				{
					if (IsCached)
					{
						LogWriter.Debug("Is cached. Loading from XML.");
						
						reactions = LoadReactions();
					}
					else
					{
						LogWriter.Debug("Is not cached. Scanning from type attributes.");
						
						reactions = FindReactions(includeTestReactions);
						Saver.SaveToFile(reactions);
					}
					
					Initialize(reactions);
				}
				else
					LogWriter.Debug("Already initialized.");
			}
		}
		
		/// <summary>
		/// Loads the available reactions from file.
		/// </summary>
		/// <returns>The loaded from the reactions mappings directory.</returns>
		public ReactionInfo[] LoadReactions()
		{
			return Loader.LoadInfoFromFile();
		}
		
		/// <summary>
		/// Finds all the reactions available to the application.
		/// </summary>
		/// <param name="includeTestReactions"></param>
		/// <returns>An array of the available reactions.</returns>
		public ReactionInfo[] FindReactions(bool includeTestReactions)
		{
			return Scanner.FindReactions(includeTestReactions);
		}
		
		/// <summary>
		/// Checks whether the reactions info file has been created.
		/// </summary>
		/// <returns>A value indicating whether the reactions info file was found.</returns>
		public bool ReactionsInfoExists()
		{
			string path = FileNamer.ReactionsInfoFilePath;
			
			return File.Exists(path);
		}
	}
}
