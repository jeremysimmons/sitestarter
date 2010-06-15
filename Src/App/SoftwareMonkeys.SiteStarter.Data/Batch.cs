using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of Batch.
	/// </summary>
	public class Batch : Component
	{
		public List<IDataStore> DataStores = new List<IDataStore>();
		
		static public bool IsRunning
		{
			get
			{
				return BatchStack.Count > 0;
			}
		}
		
		public Batch()
		{
		}
		
		public void Commit()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Committing data stores in batch.",NLog.LogLevel.Debug))
			{
				AppLogger.Debug("# of stores: " + DataStores.Count.ToString());
				
				foreach (IDataStore dataStore in DataStores)
				{
					AppLogger.Debug("Committing store: " + dataStore.Name);
					dataStore.Commit(true);
				}
				
				// Get rid of this batch and all others within it from the stack
				while (BatchStack.Contains(this))
				{
					AppLogger.Debug("This batch is in the stack. Removing top item.");
					
					BatchStack.Pop();
				}
				
				AppLogger.Debug("Commit complete.");
				
			}
		}
		
		/*public void Handle(IDataStore store)
		{
			if (!DataStores.Contains(store))
				DataStores.Add(store);
		}*/
		
		public static void Handle(IDataStore store)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Adding a data store to the batch.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Batch stack: " + BatchStack.Count.ToString());
				
				if (BatchStack.Count > 0)
				{
					// Get the outermost batch and add the data store to it
					Batch batch = BatchStack.ToArray()[0];
					
					if (!batch.DataStores.Contains(store))
					{
						AppLogger.Debug("Data store added.");
						batch.DataStores.Add(store);
					}
					else
						AppLogger.Debug("Data store already found. Not added.");
				}
				else
					throw new InvalidOperationException("No batch running. Use Batch.IsRunning to check before calling this method.");
			}
		}
		
		protected override void Dispose(bool disposing)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Disposing the batch and finishing operations.", NLog.LogLevel.Debug))
			{
				Commit();
			}
			
			base.Dispose(disposing);
		}
		
		static public Batch StartBatch()
		{
			Batch batch = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Starting a batch of data operations.", NLog.LogLevel.Debug))
			{
				batch = new Batch();
				
				AppLogger.Debug("New batch added to the stack.");
				
				BatchStack.Push(batch);
			}
			
			return batch;
		}
		
		static public Stack<Batch> BatchStack = new Stack<Batch>();
	}
}
