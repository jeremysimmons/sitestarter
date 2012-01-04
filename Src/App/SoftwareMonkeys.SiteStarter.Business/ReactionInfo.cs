using System;
using System.Xml;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Holds information about a reaction and can be serialized as a reference to a particular reaction.
	/// </summary>
	[XmlRoot("Reaction")]
	[XmlType("Reaction")]
	public class ReactionInfo
	{
		private string key;
		/// <summary>
		/// Gets/sets the key that separates the reaction from others that are similar.
		/// </summary>
		public string Key
		{
			get { return key; }
			set { key = value; }
		}
		
		private string action;
		/// <summary>
		/// Gets/sets the action that the reaction is responsible for carrying out in relation to an entity of the specified type.
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private string typeName;
		/// <summary>
		/// Gets/sets the name of the type that is involved in the reaction.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private string reactionType;
		/// <summary>
		/// Gets/sets the full string representation of the reaction object type that corresponds with the Actions and TypeName.
		/// </summary>
		public string ReactionType
		{
			get { return reactionType; }
			set { reactionType = value; }
		}
		
		private bool enabled = true;
		public bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
		}
		
		private ReactionCreator creator;
		/// <summary>
		/// Gets the reaction creator.
		/// </summary>
		[XmlIgnore]
		public ReactionCreator Creator
		{
			get {
				if (creator == null)
				{
					creator = new ReactionCreator();
				}
				return creator;
			}
			set { creator = value; }
		}
		
		public ReactionInfo()
		{
		}
		
		/// <summary>
		/// Sets the action and the name of the type involved in the reaction.
		/// </summary>
		/// <param name="action">The action being performed by the reaction.</param>
		/// <param name="typeName">The name of the type involved in the reaction.</param>
		/// <param name="reactionType">The full string representation of the reaction object type that corresponds with the provided actions and type name.</param>
		/// <param name="key">The key used to distingish the reaction from others that are similar.</param>
		public ReactionInfo(string action, string typeName, string reactionType, string key)
		{
			Action = action;
			TypeName = typeName;
			ReactionType = reactionType;
			Key = key;
		}
		
		/// <summary>
		/// Sets the action and type name indicated by the Reaction attribute on the provided type.
		/// </summary>
		/// <param name="type">The reaction type with the Reaction attribute to get the reaction information from.</param>
		public ReactionInfo(Type type)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Analyzing the provided type to extract the info."))
			{
				ReactionAttribute attribute = null;
				foreach (Attribute a in type.GetCustomAttributes(true))
				{
					if (a is ReactionAttribute)
					{
						attribute = (ReactionAttribute)a;
						break;
					}
				}
				
				if (attribute == null)
					throw new ArgumentException("Can't find ReactionAttribute on type: " + type.FullName);
				
				Action = attribute.Action;
				TypeName = attribute.TypeName;
				ReactionType = type.FullName + ", " + type.Assembly.GetName().Name;
				
				LogWriter.Debug("Action: " + Action);
				LogWriter.Debug("Type name: " + TypeName);
				LogWriter.Debug("Reaction type: " + ReactionType);
			}
		}
		
		/// <summary>
		/// Sets the action and tye name indicated by the Reaction attribute on the type of the provided reaction.
		/// </summary>
		/// <param name="reaction">The reaction containing the Reaction attribute.</param>
		public ReactionInfo(IReaction reaction) : this(reaction.GetType())
		{}
		
		/// <summary>
		/// Creates a new instance of the corresponding reaction for use by the system.
		/// </summary>
		/// <returns>An instance of the corresponding reaction.</returns>
		public IReaction New()
		{
			IReaction reaction = null;

			using (LogGroup logGroup = LogGroup.Start("Creating a new reaction."))
			{
				LogWriter.Debug("Type name: " + TypeName);
				
				LogWriter.Debug("Action: " + Action);
				
				LogWriter.Debug("Key: " + Key);
			
				reaction = Creator.CreateReaction(this);	
			}

			return reaction;
		}
		
		/// <summary>
		/// Creates a new instance of the corresponding reaction for use by the system.
		/// </summary>
		/// <returns>An instance of the corresponding reaction, cast to the specified type.</returns>
		public T New<T>()
			where T : IReaction
		{
			return New<T>(TypeName);
		}
		
		/// <summary>
		/// Creates a new instance of the corresponding reaction for use by the system.
		/// </summary>
		/// <returns>An instance of the corresponding reaction, cast to the specified type.</returns>
		public T New<T>(string entityTypeName)
			where T : IReaction
		{
			T reaction = default(T);
			
			using (LogGroup logGroup = LogGroup.StartDebug("Creating a new reaction for the type '" + entityTypeName + "' and action '" + Action + "'."))
			{
				LogWriter.Debug("Entity type name: " + entityTypeName);
				
				LogWriter.Debug("Reaction type: " + typeof(T).FullName);
				
				reaction = (T)New();
				reaction.TypeName = entityTypeName;
			}
			
			return reaction;
		}
		
	}
}
