using System;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to remove/dispose reactions from the system.
	/// </summary>
	public class ReactionsDisposer
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
		
		public ReactionsDisposer()
		{
		}
		
		/// <summary>
		/// Disposes the reactions found by the scanner.
		/// </summary>
		public void Dispose()
		{
			using (LogGroup logGroup = LogGroup.Start("Disposing the reactions.", NLog.LogLevel.Debug))
			{
				ReactionInfo[] reactions = new ReactionInfo[]{};
				
				Dispose(ReactionState.Reactions.ToArray());
			}
		}
		
		/// <summary>
		/// Disposes the provided reactions.
		/// </summary>
		public void Dispose(ReactionInfo[] reactions)
		{
			using (LogGroup logGroup = LogGroup.Start("Disposing the reactions.", NLog.LogLevel.Debug))
			{
				foreach (ReactionInfo reaction in reactions)
				{
					ReactionState.Reactions.Remove(
						ReactionState.Reactions[reaction.TypeName]
					);
					
				}
			}
		}
		
	}
}
