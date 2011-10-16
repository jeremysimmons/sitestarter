using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Used to generate file names for entities to be used during export/import of data to/from XML.
	/// </summary>
	public class EntityFileNamer
	{
		private string baseDirectoryPath;
		/// <summary>
		/// Gets the base directory containing the XML files (each in their own sub directory).
		/// </summary>
		public string BaseDirectoryPath
		{
			get { return baseDirectoryPath; }
		}
		
		private IEntity entity;
		/// <summary>
		/// Gets the entity that the file name is for.
		/// </summary>
		public IEntity Entity
		{
			get { return entity; }
		}
		
		/// <summary>
		/// Sets the entity that the file name is being generated for.
		/// </summary>
		/// <param name="entity">The entity to generate the file name for.</param>
		/// <param name="baseDirectory">The base directory containing the entity XML files (each in their own sub directory).</param>
		public EntityFileNamer(IEntity entity, string baseDirectory)
		{
			this.entity = entity;
			this.baseDirectoryPath = baseDirectory;
		}
		
		/// <summary>
		/// Creates the file path for the entity.
		/// </summary>
		/// <returns>The full file path for the entity</returns>
		public string CreateFilePath()
		{
			if (EntitiesUtilities.IsReference(entity.GetType()))
			{
				return CreateReferencePath();
			}
			else
			{
				return CreateEntityPath();
			}
		}
		
		/// <summary>
		/// Creates the full file path for the reference.
		/// </summary>
		/// <returns>The full path for the provided reference.</returns>
		protected string CreateReferencePath()
		{
			string referenceFileName = String.Empty;
			using (LogGroup logGroup = LogGroup.Start("Creating the path to the specified reference file.", LogLevel.Debug))
			{
				EntityIDReference reference = (EntityIDReference)entity;
				
				string type1Name = reference.Type1Name;
				string type2Name = reference.Type2Name;
				Guid entity1ID = reference.Entity1ID;
				Guid entity2ID = reference.Entity2ID;
				string property1Name = reference.Property1Name;
				string property2Name = reference.Property2Name;
				
				LogWriter.Debug("Type 1 name: " + type1Name);
				LogWriter.Debug("Type 2 name: " + type2Name);
				LogWriter.Debug("Entity 1 ID: " + entity1ID);
				LogWriter.Debug("Entity 2 ID: " + entity2ID);
				LogWriter.Debug("Property 1 name: " + property1Name);
				LogWriter.Debug("Property 2 name: " + property2Name);

				// Sort the names alphabetically
				string[] typeNames = new String[] { type1Name, type2Name };
				Array.Sort(typeNames);

				// If the type name order changed then switch the other variables too
				if (type1Name != typeNames[0])
				{
					LogWriter.Debug("Switched order of entity type names.");

					type1Name = typeNames[0];
					type2Name = typeNames[1];

					// Switch the property names
					string y = property1Name;
					string x = property2Name;

					property1Name = x;
					property2Name = y;

					// Switch the entity names
					Guid a = entity1ID;
					Guid b = entity2ID;

					entity1ID = b;
					entity2ID = a;

					LogWriter.Debug("Type 1 name: " + type1Name);
					LogWriter.Debug("Type 2 name: " + type2Name);
					LogWriter.Debug("Entity 1 ID: " + entity1ID);
					LogWriter.Debug("Entity 2 ID: " + entity2ID);
					LogWriter.Debug("Property 1 name: " + property1Name);
					LogWriter.Debug("Property 2 name: " + property2Name);
				}

				string typeName = typeNames[0] + "-" + typeNames[1];

				referenceFileName = baseDirectoryPath + Path.DirectorySeparatorChar;
				if (Path.GetDirectoryName(baseDirectoryPath) != type1Name + "-" + type2Name)
					referenceFileName += typeName + Path.DirectorySeparatorChar;
				
				referenceFileName += reference.ID.ToString() + ".xml";
				
				// TODO: Remove if not needed
				//referenceFileName += entity1ID + "-" + property1Name + "---" + entity2ID + "-" + property2Name + ".xml";

				LogWriter.Debug("Reference file name: " + referenceFileName);
			}
			return referenceFileName;
		}
		
		
		/// <summary>
		/// Creates the file name for the entity in the specified export directory.
		/// </summary>
		/// <returns>The full path for the provided entity.</returns>
		protected string CreateEntityPath()
		{
			Guid id = entity.ID;

			string fullPath = baseDirectoryPath + Path.DirectorySeparatorChar +
				entity.GetType().FullName + Path.DirectorySeparatorChar
				+ id + ".xml";
			
			return fullPath;
		}
	}
}
