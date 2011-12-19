using System;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Entities;
using System.IO;
using System.Resources;
using System.Collection.Generic;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to dynamically access language entries.
	/// </summary>
	public static class DynamicLanguage
	{
		static public List<ResourceSet> ResourceSets = new List<ResourceSet>();
		
		/// <summary>
		/// Retrieves the text entry with the specified key from the assembly containing the provided relative type.
		/// </summary>
		/// <param name="relativeType">An object type in the assembly containing the language file.</param>
		/// <param name="key">The key of the language entry.</param>
		/// <returns>The text matching the provided key from the assembly that contains the relative type.</returns>
		public static string GetText(Type relativeType, string key)
		{
			if (relativeType == null)
				throw new ArgumentNullException("relativeType");
			
			if (key == null || key == String.Empty)
				throw new ArgumentNullException("key");
			
			ResourceSet resources = GetResourceSet(relativeType, key);
			
			if (resources == null)
				throw new Exception("No resource set found for key '" + key + "'.");
			
			string output = resources.GetString(key);
			
			if (output == null || output == String.Empty)
				throw new ArgumentException("No language entries found with the key '" + key + "'.");
			
			return output;
		}
		
		/// <summary>
		/// Retrieves the text entry with the specified key.
		/// </summary>
		/// <param name="key">The key of the language entry.</param>
		/// <returns>The text matching the provided key from the assembly.</returns>
		public static string GetText(string key)
		{
			if (key == null || key == String.Empty)
				throw new ArgumentNullException("key");
			
			ResourceSet resources = GetResourceSet(key);
			
			if (resources == null)
				throw new Exception("No resource set found for key '" + key + "'.");
			
			string output = resources.GetString(key);
			
			if (output == null || output == String.Empty)
				throw new ArgumentException("No language entries found with the key '" + key + "'.");
			
			return key;
		}
		
		/// <summary>
		/// Retrieves the text entry with the specified key.
		/// </summary>
		/// <param name="key">The key of the language entry.</param>
		/// <param name="typeName">The short name of the type that the text applies to.</param>
		/// <returns>The text matching the provided key from the assembly.</returns>
		public static string GetEntityText(string key, string typeName)
		{
			Type type = EntityState.GetType(typeName);
			
			string text = GetText(type, key);
			string entityName = GetText(type, typeName);
			
			text = text.Replace("${entity}", entityName.ToLower());
			text = text.Replace("${Entity}", entityName);
			
			return text;
		}
		
		/// <summary>
		/// Retrieves the assembly containing the resource file with the specified key, and if possible, relative to the provided type.
		/// </summary>
		/// <param name="key">The key of the target entry in the target resource set.</param>
		/// <returns>The resource set containing the target entry from the assembly containing the provided relative type.</returns>
		public static ResourceSet GetResourceSet(string key)
		{
			return GetResourceSet(null, key);
		}
		
		/// <summary>
		/// Retrieves the assembly containing the resource file with the specified key, and if possible, relative to the provided type.
		/// </summary>
		/// <param name="relativeType">A type in the assembly containing the target resource set.</param>
		/// <param name="key">The key of the target entry in the target resource set.</param>
		/// <returns>The resource set containing the target entry from the assembly containing the provided relative type.</returns>
		public static ResourceSet GetResourceSet(Type relativeType, string key)
		{
			ResourceSet resources = GetResourceSetFromProperty(relativeType, key);
			
			if (resources == null)
			{
				// Try getting the relative set
				if (relativeType != null)
					resources = GetRelativeResourceSet(relativeType, key);
				
				// Try getting the App_GlobalResources set
				if (!ResourceSetMatches(resources, key))
				{
					Assembly a = Assembly.Load("App_GlobalResources");
					
					resources = GetResourceSetFromAssembly(a, key);
				}
				
				// Try getting any other set that can be found
				if (!ResourceSetMatches(resources, key))
				{
					string binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
					
					foreach (string assemblyFile in Directory.GetFiles(binDirectory, "*.dll"))
					{
						Assembly a = Assembly.LoadFile(assemblyFile);
						
						resources = GetResourceSetFromAssembly(a, key);
					}
				}
				
				ResourceSets.Add(resources);
			}
			
			return resources;
		}
		
		static public ResourceSet GetResourceSetFromProperty(Type relativeType, string key)
		{
			foreach (ResourceSet rs in ResourceSets)
				if (ResourceSetMatches(rs, key))
					return rs;
			
			return null;
		}
		
		/// <summary>
		/// Retrieves the resource set containing an entry with the specified key and from the assembly containing the provided relative type.
		/// </summary>
		/// <param name="relativeType">A type in the assembly containing the target resource set.</param>
		/// <param name="key">The key of the target entry in the target resource set.</param>
		/// <returns>The resource set matching the specified key, from the assembly containing the provided type.</returns>
		public static ResourceSet GetRelativeResourceSet(Type relativeType, string key)
		{
			Assembly assembly = Assembly.GetAssembly(relativeType);
			
			ResourceSet resources = GetResourceSetFromAssembly(assembly, key);
			
			if (ResourceSetMatches(resources, key))
				return resources;
			else
				return null;
		}
		
		/// <summary>
		/// Checks whether the provided resource set contains an entry matching the provided key.
		/// </summary>
		/// <param name="resources"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool ResourceSetMatches(ResourceSet resources, string key)
		{
			if (resources == null)
				return false;
			
			string output = resources.GetString(key);
			
			return (output != null && output != String.Empty);
		}
		
		/// <summary>
		/// Retrieves the resource set containing the specified key from the provided assembly.
		/// </summary>
		/// <param name="assembly">The assembly containing the target resource set.</param>
		/// <param name="key">The key of the target entry.</param>
		/// <returns>The resource set from the provided assembly containing the provided key.</returns>
		public static ResourceSet GetResourceSetFromAssembly(Assembly assembly, string key)
		{
			
			string[] resourceNames = assembly.GetManifestResourceNames();
			
			ResourceSet resources = null;
			
			foreach (string resourceName in resourceNames)
			{
				string text = String.Empty;
				
				using (Stream stream = assembly.GetManifestResourceStream(resourceName))
				{
					resources = new ResourceSet(stream);
					
					stream.Close();
					
				}
				
				text = resources.GetString(key);
				
				if (ResourceSetMatches(resources, key))
				{
					return resources;
				}
			}
			
			return resources;
		}

	}
}
