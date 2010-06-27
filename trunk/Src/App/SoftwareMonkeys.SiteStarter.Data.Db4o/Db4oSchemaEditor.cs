using System;
using System.IO;
using Db4objects.Db4o;
using System.Xml;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;
using Db4objects.Db4o.Config;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
	/// <summary>
	/// Description of Db4oSchemaEditor.
	/// </summary>
	public class Db4oSchemaEditor : ISchemaEditor
	{
		private string[] storeNames = new String[] {};
		
		private string[] messages = new String[] {};
		public string[] Messages
		{
			get { return messages; }
			set { messages = value; }
		}
		
		private IConfiguration db4oConfiguration;
		public IConfiguration Db4oConfiguration
		{
			get
			{
				if (db4oConfiguration == null)
					db4oConfiguration = Db4oFactory.NewConfiguration();
				return db4oConfiguration;
			}
		}
		
		public void RenameType(string originalType, string newType)
		{
			//if (DataAccess.IsInitialized)
			//	throw new InvalidOperationException("Suspend the data stores before altering the schema.");
			
			Db4oConfiguration.ObjectClass(originalType).Rename(newType);
		
		}
		
		public void RenameProperty(string typeName, string originalProperty, string newProperty)
		{
			//if (DataAccess.IsInitialized)
			//	throw new InvalidOperationException("Suspend the data stores before altering the schema.");
			
			Type type = EntitiesUtilities.GetType(typeName);
			
			Db4oConfiguration
				.ObjectClass(type)
				.ObjectField(
					EntitiesUtilities.GetFieldName(type, originalProperty))
				.Rename(
					EntitiesUtilities.GetFieldName(type, newProperty));
		}
		
		public void RenameProperty(XmlNode node)
		{
			string typeName = node.Attributes["TypeName"].Value;
			string originalName = node.Attributes["OriginalName"].Value;
			string newName = node.Attributes["NewName"].Value;
			
			RenameProperty(typeName, originalName, newName);
		}
		
		public void RenameType(XmlNode node)
		{
			string originalName = node.Attributes["OriginalName"].Value;
			string newName = node.Attributes["NewName"].Value;
			
			RenameType(originalName, newName);
		}
		
		public string[] Suspend()
		{
			List<string> names = new List<string>();
			for (int i = 0; i < DataAccess.Data.Stores.Count;i++)
			{
				IDataStore store = DataAccess.Data.Stores[i];
				names.Add(store.Name);
				//store.Close(); // Dispose calls Close as well
				store.Dispose();
				DataAccess.Data.Stores.Remove(store);
				i--;
			}
			
			this.storeNames = names.ToArray();
			
			return names.ToArray();
		}
		
		public void Resume()
		{
			// Open and then close the stores (necessary to apply updates)
			Refresh(storeNames);
			
			// Open stores again
			DataAccess.Initialize();
		}
		
		public void Refresh(string[] storeNames)
		{
			// Initialize then suspend again (necessary for db4o schema to change)
			DataAccess.Initialize();
			
			foreach (String storeName in storeNames)
			{
				if (storeName == string.Empty)
					throw new Exception("storeName == String.Empty");
				
				// Load the store into memory for the refresh
				//IDataStore store = DataAccess.Data.Stores[storeName];
				IDataStore store = Db4oDataStoreFactory.InitializeDataStore(storeName);
				store.Dispose();
			}
			
			// Now stores can be loaded (outside this function) and should reflect changes
		}
		
		public void ApplySchema(string applicationPath)
		{
			Suspend();
			
			ApplySchemaDirectory(applicationPath + Path.DirectorySeparatorChar + "Schema");
			
			Resume();
		}
		
		public void ApplySchemaDirectory(string dir)
		{
			foreach (string d in Directory.GetDirectories(dir))
			{
				ApplySchemaDirectory(d);
			}
			
			foreach (string file in Directory.GetFiles(dir, "*.xml"))
			{
				ApplySchemaFile(file);
			}
		}
		
		public void ApplySchemaFile(string file)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(file);
			
			foreach (XmlNode node in doc.DocumentElement.ChildNodes)
			{
				ApplySchemaNode(node);
			}
		}
		
		public void ApplySchemaNode(XmlNode node)
		{
			switch (node.Name)
			{
				case "RenameProperty":
					RenameProperty(node);
					break;
				case "RenameType":
					RenameType(node);
					break;
			}
		}
		
	}
}
