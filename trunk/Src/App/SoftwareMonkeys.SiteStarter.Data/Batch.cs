﻿using System;
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
		private List<IDataStore> dataStores;
		/// <summary>
		/// The data stores affected during the batch.
		/// </summary>
		public List<IDataStore> DataStores
		{
			get {
				if (dataStores == null)
					dataStores = new List<IDataStore>();
				return dataStores; }
			set { dataStores = value; }
		}
		
		private bool isCommitted;
		/// <summary>
		/// Gets/sets a boolean value indicating whether the batch has been committed.
		/// </summary>
		public bool IsCommitted
		{
			get { return isCommitted; }
			set { isCommitted = value; }
		}
		
		/// <summary>
		/// Fired when the batch gets committed.
		/// </summary>
		public event EventHandler Committed;
		
		/// <summary>
		/// Raises the committed event.
		/// </summary>
		public virtual void RaiseCommitted()
		{
			if (Committed != null)
				Committed(this, EventArgs.Empty);
		}
		
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public Batch()
		{
		}
		
		/// <summary>
		/// Commits the batch and all data stores within it.
		/// </summary>
		public void Commit()
		{
			using (LogGroup logGroup = LogGroup.Start("Committing data stores in batch.",NLog.LogLevel.Debug))
			{
				if (DataAccess.IsInitialized)
				{
					LogWriter.Debug("# of stores: " + DataStores.Count.ToString());
					
					// If the batch has ended then commit
					if (!BatchState.IsRunning)
					{
						foreach (IDataStore dataStore in DataStores)
						{
							if (!dataStore.IsClosed)
							{
								LogWriter.Debug("Committing store: " + dataStore.Name);
								dataStore.Commit(true);
							}
							else
								LogWriter.Error("Can't commit. Store is closed: " + dataStore.Name);
						}
						
						// Mark the batch as committed
						IsCommitted = true;
						
						// Raise the committed event
						RaiseCommitted();
						
						LogWriter.Debug("Finished.");
						
					}
					else
					{
						LogWriter.Debug("Outer batch still running. Skipping commit.");
						
					}
				}
			}
		}
		
		/*public void Handle(IDataStore store)
		{
			if (!DataStores.Contains(store))
				DataStores.Add(store);
		}*/
		
		/// <summary>
		/// Commits and disposes the batch.
		/// </summary>
		/// <param name="disposing">A value indicating whether the component is actually disposing.</param>
		protected override void Dispose(bool disposing)
		{
			using (LogGroup logGroup = LogGroup.Start("Disposing the batch and finishing operations.", NLog.LogLevel.Debug))
			{
				
				// Get rid of this batch and all others within it from the stack
				while (BatchState.Batches.Contains(this))
				{
					LogWriter.Debug("This batch is in the stack. Removing top item.");
					
					BatchState.Batches.Pop();
				}
				
				Commit();
			}
			
			base.Dispose(disposing);
		}
	}
}
