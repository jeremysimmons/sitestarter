using System;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Represents a command; a combination of an action and a type name.
	/// </summary>
	public class CommandInfo : ICommandInfo
	{
		private string action = String.Empty;
		/// <summary>
		/// Gets/sets the action part of the command.
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private string typeName = String.Empty;
		/// <summary>
		/// Gets/sets the name of the type involved in the command.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private string[] aliasActions = new String[] {};
		/// <summary>
		/// Gets/sets the aliases for the actions. For example "Save" is an alias of "Create" and "Update" is an alias of "Edit".
		/// </summary>
		public string[] AliasActions
		{
			get { return aliasActions; }
			set { aliasActions = value; }
		}
		
		/// <summary>
		/// Gets a list of all actions including the primary as well as all aliases.
		/// </summary>
		public string[] AllActions
		{
			get {
				List<string> list = new List<string>();
				if (Action != String.Empty)
					list.Add(Action);
				if (AliasActions.Length > 0)
					list.AddRange(AliasActions);
				return list.ToArray();
			}
		}
		
		
		public CommandInfo()
		{
		}
		
		public CommandInfo(string action, string typeName)
		{
			Action = action;
			TypeName = typeName;
		}
		
		public CommandInfo(string action, string typeName, params string[] aliasActions)
		{
			Action = action;
			TypeName = typeName;
			AliasActions = aliasActions;
		}
		
		/// <summary>
		/// Extracts the type name and action from the command and sets the values to the corresponding properties.
		/// </summary>
		/// <param name="command">The command string; either "[Action] [TypeName]" or "[TypeName] [Action]".</param>
		public CommandInfo(string command)
		{
			string[] parts = command.Replace(" ", "-").Split('-');
			
			if (EntityState.IsType(parts[0]))
			{
				TypeName = parts[0];
				Action = parts[1];
			}
			else
			{
				TypeName = parts[1];
				Action = parts[0];
			}
		}
	}
}
