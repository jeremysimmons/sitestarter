/*
 * Created by SharpDevelop.
 * User: Jose
 * Date: 10/05/2011
 * Time: 10:23 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Represents a command; a combination of a verb/action and a type name.
	/// </summary>
	public class CommandInfo
	{
		private string action;
		/// <summary>
		/// Gets/sets the action part of the command.
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private string typeName;
		/// <summary>
		/// Gets/sets the name of the type involved in the command.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		public CommandInfo()
		{
		}
		
		public CommandInfo(string action, string typeName)
		{
			Action = action;
			TypeName = typeName;
		}
		
		/// <summary>
		/// Extracts the type name and action from the command and sets the values to the corresponding properties.
		/// </summary>
		/// <param name="command">The command string; either "[Action] [TypeName]" or "[TypeName] [Action]".</param>
		public CommandInfo(string command)
		{
			string[] parts = command.Split(' ');
			
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
