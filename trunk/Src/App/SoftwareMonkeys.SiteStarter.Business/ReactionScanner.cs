using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used for scanning assemblies to look for usable business strategies.
	/// </summary>
	public class ReactionScanner
	{
		private string[] assemblyPaths;
		/// <summary>
		/// Gets/sets the paths of the assemblies containing strategies.
		/// </summary>
		public string[] AssemblyPaths
		{
			get {
				if (assemblyPaths == null || assemblyPaths.Length == 0)
					assemblyPaths = GetAssemblyPaths();
				return assemblyPaths; }
			set { assemblyPaths = value; }
		}
		
		private string binDirectoryPath = String.Empty;
		/// <summary>
		/// Gets/sets the path to the directory containing the assemblies.
		/// </summary>
		public string BinDirectoryPath
		{
			get {
				if (binDirectoryPath == String.Empty)
				{
					if (StateAccess.IsInitialized)
						binDirectoryPath = StateAccess.State.PhysicalApplicationPath + Path.DirectorySeparatorChar + "bin";
				}
				return binDirectoryPath; }
			set { binDirectoryPath = value; }
		}
		
		public ReactionScanner()
		{
		}
		
		/// <summary>
		/// Retrieves a list of all available assembly paths.
		/// </summary>
		/// <returns>An array of full file paths to the available assemblies.</returns>
		public string[] GetAssemblyPaths()
		{
			List<string> list = new List<string>();
						
			foreach (string file in Directory.GetFiles(BinDirectoryPath, "*.dll"))
			{
				list.Add(file);
			}
			
			return list.ToArray();
		}
		
		
		/// <summary>
		/// Finds all the strategies in the available assemblies.
		/// </summary>
		/// <returns>An array of info about the strategies found.</returns>
		public ReactionInfo[] FindReactions()
		{
			ReactionInfoCollection strategies = new ReactionInfoCollection();
			
			//using (LogGroup logGroup = LogGroup.Start("Finding strategies by scanning the attributes of the available type.", NLog.LogLevel.Debug))
			//{
			foreach (string assemblyPath in AssemblyPaths)
			{
				Assembly assembly = Assembly.LoadFrom(assemblyPath);
				
				if (ContainsReactions(assembly))
				{
					foreach (Type type in assembly.GetTypes())
					{
						if (IsReaction(type))
						{
							//LogWriter.Debug("Found reaction type: " + type.ToString());
							
							ReactionInfo reactionInfo = new ReactionInfo(type);
							
							if (reactionInfo.TypeName != null && reactionInfo.TypeName != String.Empty
							    && reactionInfo.Action != null && reactionInfo.Action != String.Empty)
							{
								//LogWriter.Debug("Found match.");
								
								//LogWriter.Debug("Type name: " + reactionInfo.TypeName);
								//LogWriter.Debug("Action: " + reactionInfo.Action);
								
								strategies.Add(reactionInfo);
							}
						}
					}
				}
			}
			//}
			
			return strategies.ToArray();
		}
		
		/// <summary>
		/// Checks whether the provided assembly contains reaction types by looking for AssemblyContainsReactionsAttribute.
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public bool ContainsReactions(Assembly assembly)
		{
			bool doesContainReactions = false;
			
			doesContainReactions = assembly.GetCustomAttributes(typeof(AssemblyContainsReactionsAttribute), true).Length > 0;
			
			return doesContainReactions;
		}
		
		/// <summary>
		/// Checks whether the provided type is a reaction.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool IsReaction(Type type)
		{
			bool matchesInterface = false;
			bool isNotInterface = false;
			bool isNotAbstract = false;
			bool hasAttribute = false;
			
			//using (LogGroup logGroup = LogGroup.Start("Checks whether the provided type is a reaction.", NLog.LogLevel.Debug))
			//{
			//	LogWriter.Debug("Type: " + type.ToString());
			
			matchesInterface = typeof(IReaction).IsAssignableFrom(type)
				|| type.GetInterface("IReaction") != null;
			
			isNotInterface = !type.IsInterface;
			
			isNotAbstract = (!type.IsAbstract);
			
			hasAttribute = type.GetCustomAttributes(typeof(ReactionAttribute), true).Length > 0;

			//	LogWriter.Debug("Matches interface: " + matchesInterface);
			//	LogWriter.Debug("Is not reaction interface: " + isNotInterface);
			//}
			
			return matchesInterface
				&& isNotInterface
				&& isNotAbstract
				&& hasAttribute;
		}
	}
}
