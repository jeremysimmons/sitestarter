using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.CS;
using Db4objects.Db4o.CS.Config;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.IO;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
	/// <summary>
	/// Used to open a data store along with the server and container.
	/// </summary>
	public class Db4oDataStoreOpener
	{
		public IObjectServer TryOpenServer(string yapFilePath, int maxRetries)
		{
			using (LogGroup logGroup = LogGroup.Start("Trying to open a data server.", NLog.LogLevel.Debug))
			{
				IServerConfiguration configuration = Db4oClientServer.NewServerConfiguration();
								
				configuration.Common.ObjectClass(typeof(IEntity)).ObjectField("id").Indexed(true);
				
				configuration.Common.ObjectClass(typeof(EntityReference)).ObjectField("property1Name").Indexed(true);
				configuration.Common.ObjectClass(typeof(EntityReference)).ObjectField("type1Name").Indexed(true);
				configuration.Common.ObjectClass(typeof(EntityReference)).ObjectField("entity1ID").Indexed(true);
				configuration.Common.ObjectClass(typeof(EntityReference)).ObjectField("property2Name").Indexed(true);
				configuration.Common.ObjectClass(typeof(EntityReference)).ObjectField("type2Name").Indexed(true);
				configuration.Common.ObjectClass(typeof(EntityReference)).ObjectField("entity2ID").Indexed(true);
				
				LogWriter.Debug("Full file name: " + yapFilePath);
				
				if (!Directory.Exists(Path.GetDirectoryName(yapFilePath)))
					Directory.CreateDirectory(Path.GetDirectoryName(yapFilePath));
				
				IObjectServer server = null;
				
				try
				{
					server = Db4oClientServer.OpenServer(configuration, yapFilePath, 0);
				}
				catch (Exception ex)
				{
					if (maxRetries == 0)
						LogWriter.Error(ex);
					
					if (maxRetries > 0)
						server = TryOpenServer(yapFilePath, maxRetries-1);
				}
				
				return server;
			}
		}
	}
}
