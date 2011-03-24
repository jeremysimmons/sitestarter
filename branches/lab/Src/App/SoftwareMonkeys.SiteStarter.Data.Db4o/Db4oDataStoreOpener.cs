﻿using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
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
				IConfiguration configuration = Db4oFactory.NewConfiguration();
				
				configuration.ActivationDepth(2);
				configuration.UpdateDepth(0);
				
				configuration.ObjectClass(typeof(IEntity)).ObjectField("id").Indexed(true);
				
				configuration.ObjectClass(typeof(EntityIDReference)).ObjectField("property1Name").Indexed(true);
				configuration.ObjectClass(typeof(EntityIDReference)).ObjectField("type1Name").Indexed(true);
				configuration.ObjectClass(typeof(EntityIDReference)).ObjectField("entity1ID").Indexed(true);
				configuration.ObjectClass(typeof(EntityIDReference)).ObjectField("property2Name").Indexed(true);
				configuration.ObjectClass(typeof(EntityIDReference)).ObjectField("type2Name").Indexed(true);
				configuration.ObjectClass(typeof(EntityIDReference)).ObjectField("entity2ID").Indexed(true);
				
				//configuration.AutomaticShutDown(false);
				
				LogWriter.Debug("Full file name: " + yapFilePath);
				
				if (!Directory.Exists(Path.GetDirectoryName(yapFilePath)))
					Directory.CreateDirectory(Path.GetDirectoryName(yapFilePath));
				
				IObjectServer server = null;
				
				try
				{
					server = Db4oFactory.OpenServer(configuration, yapFilePath, 0);
				}
				catch (Exception ex)
				{
					if (maxRetries == 0)
						LogWriter.Error(ex);
					
					server = TryOpenServer(yapFilePath, maxRetries-1);
				}
				
				return server;
			}
		}
	}
}
