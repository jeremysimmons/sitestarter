using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Web.Configuration;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.IO;
using System.Web;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Web.Data
{
	public class DataProviderInitializer : IDataProviderInitializer
	{
		//Initialization related variables and logic
		private bool isInitialized = false;
		private Exception initializationException;

		private object initializationLock = new object();

		public DataProviderInitializer()
		{
			//Initialize();
		}

		public DataProvider Initialize()
		{
			using (LogGroup logGroup = LogGroup.Start("Initializing data provider", NLog.LogLevel.Debug))
			{
				if (StateAccess.IsInitialized && Config.IsInitialized
				    && !isInitialized)
				{
					
					try
					{
						//Get the feature's configuration info
						DataProviderConfiguration qc =
							(DataProviderConfiguration)ConfigurationManager.GetSection("DataProvider");
						
						if (qc.DefaultProvider == null || qc.Providers == null || qc.Providers.Count < 1)
							throw new ProviderException("You must specify a valid default provider.");
						
						//Instantiate the providers
						providerCollection = new DataProviderCollection();
						ProvidersHelper.InstantiateProviders(qc.Providers, providerCollection, typeof(DataProvider));
						providerCollection.SetReadOnly();
						defaultProvider = providerCollection[qc.DefaultProvider];
						
						if (defaultProvider == null)
						{
							throw new ConfigurationErrorsException(
								"You must specify a default provider for the feature.",
								qc.ElementInformation.Properties["defaultProvider"].Source,
								qc.ElementInformation.Properties["defaultProvider"].LineNumber);
						}
						
						
						string dataDirectoryPath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath) + Path.DirectorySeparatorChar + "App_Data";
						defaultProvider.Exporter.ExportDirectoryPath = dataDirectoryPath + Path.DirectorySeparatorChar + "Exported";
						defaultProvider.Importer.ImportableDirectoryPath = dataDirectoryPath + Path.DirectorySeparatorChar + "Legacy";
						defaultProvider.Importer.ImportedDirectoryPath = dataDirectoryPath + Path.DirectorySeparatorChar + "Imported";
						defaultProvider.Schema.SchemaCommandDirectoryPath = dataDirectoryPath + Path.DirectorySeparatorChar + "Schema";
					}
					catch (Exception ex)
					{
						LogWriter.Error(ex.ToString());
						
						initializationException = ex;
						isInitialized = true;
						throw ex;
					}
					
					isInitialized = true; //error-free initialization
				}
				
			}
			return defaultProvider;
		}

		//Public feature API
		private DataProvider defaultProvider;
		private DataProviderCollection providerCollection;

		public DataProvider Provider
		{
			get
			{
				return defaultProvider;
			}
		}

		public DataProviderCollection Providers
		{
			get
			{
				return providerCollection;
			}
		}

		/* public static string DoWork()
        {
            return Provider.DoWork();
        }*/
	}
}
