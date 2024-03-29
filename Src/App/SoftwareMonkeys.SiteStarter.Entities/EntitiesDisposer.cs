﻿using System;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Used to remove/dispose entities from the system.
	/// </summary>
	public class EntitiesDisposer
	{
		
		private EntityFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create entity file names/paths.
		/// </summary>
		public EntityFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new EntityFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		public EntitiesDisposer()
		{
		}
		
		/// <summary>
		/// Disposes the entities found by the scanner.
		/// </summary>
		public void Dispose()
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Disposing the entities."))
			{
				EntityInfo[] entities = new EntityInfo[]{};
				
				Dispose(EntityState.Entities.ToArray());
			}
		}
		
		/// <summary>
		/// Disposes the provided entities.
		/// </summary>
		public void Dispose(EntityInfo[] entities)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Disposing the entities."))
			{
				foreach (EntityInfo entity in entities)
				{
					EntityState.Entities.Remove(
						EntityState.Entities[entity.TypeName]
					);
				}
				
				File.Delete(FileNamer.EntitiesInfoFilePath);
			}
		}
	}
}
