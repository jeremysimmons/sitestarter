using System;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Holds the batch state.
	/// </summary>
	public class BatchState
	{
		/// <summary>
		/// Handles the provided data store as part of the batch.
		/// </summary>
		/// <param name="store">The data store being handled as part of the batch.</param>
		public static void Handle(IDataStore store)
		{
			using (LogGroup logGroup = LogGroup.Start("Adding a data store to the batch.", LogLevel.Debug))
			{
				LogWriter.Debug("Batch stack: " + BatchState.Batches.Count.ToString());
				
				if (BatchState.IsRunning)
				{
					Stack<Batch> batches = BatchState.Batches;
					
					// Get the position of the outermost batch
					// The stack is reversed so the last position is the outermost batch
					int outerPosition = batches.Count-1;
					
					// Get the outermost batch and add the data store to it					
					Batch batch = batches.ToArray()[outerPosition];
					
					if (!batch.DataStores.Contains(store))
					{
						LogWriter.Debug("Data store added.");
						batch.DataStores.Add(store);
						
						// Commit the batch stack to state
						Batches = batches;
					}
					else
					{
						LogWriter.Debug("Data store already found. Not added.");
					}
				}
				else
					throw new InvalidOperationException("No batch running. Use Batch.IsRunning to check before calling this method.");
			}
			
		}
			
		/// <summary>
		/// Gets a boolean value indicating whether a batch is currently running.
		/// </summary>
		static public bool IsRunning
		{
			get
			{
				if (StateAccess.IsInitialized && Batches != null)
				return Batches.Count > 0;
				else
					return false;
			}
		}
		
		#region Batches
        
		/// <summary>
		/// Starts a new batch.
		/// </summary>
		/// <returns>The newly started batch.</returns>
		static public Batch StartBatch()
		{
			Batch batch = null;
			using (LogGroup logGroup = LogGroup.Start("Starting a batch of data operations.", LogLevel.Debug))
			{
				batch = new Batch();
				
				LogWriter.Debug("New batch added to the stack.");
				
				Batches.Push(batch);
			}
			
			return batch;
		}
		
		static public string BatchesKey = "BatchState.Batches";
		
		/// <summary>
		/// A stack of all the batches currently in operation.
		/// </summary>
		static public Stack<Batch> Batches
		{
			get {
				if (!State.StateAccess.State.ContainsOperation(BatchesKey)
				   || State.StateAccess.State.GetOperation(BatchesKey) == null)
					State.StateAccess.State.SetOperation(BatchesKey, new Stack<Batch>());
				return (Stack<Batch>)State.StateAccess.State.GetOperation(BatchesKey); }
			set { State.StateAccess.State.SetOperation(BatchesKey, value); }
		}
        #endregion
        
		
		public BatchState()
		{
		}
		
		/// <summary>
		/// Queues the provided entity to be updated when the batch ends.
		/// </summary>
		/// <param name="entity"></param>
		static public void QueueUpdate(IEntity entity)
		{
			if (Batches.Count == 0)
				throw new InvalidOperationException("No batch has been started.");
			
			Batches.ToArray()[0].QueueUpdate(entity);
	}
}
}
