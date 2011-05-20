using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	/// <summary>
	/// Tests the batching functionality.
	/// </summary>
	[TestFixture]
	public class BatchTests : BaseDataTestFixture
	{
		/// <summary>
		/// A value indicating whether the test batch was committed.
		/// </summary>
		bool BatchCommitted = false;
		
		/// <summary>
		/// A value indicating whether the store was committed.
		/// </summary>
		bool StoreCommitted = false;
		
		[SetUp]
		public new void Start()
		{
		//	InitializeMockState();
		//	InitializeMockDiagnostics();
		//	InitializeMockConfiguration();
		//	InitializeMockData();
			BatchCommitted = false;
			StoreCommitted = false;
		}

		[TearDown]
		public new void End()
		{
			BatchCommitted = false;
			StoreCommitted = false;
			
			//DisposeMockData();
			//DisposeMockConfiguration();
		//	DisposeMockDiagnostics();
		//	DisposeMockState();
		}
		
		[Test]
		public void Test_OneBatch_NoOperations()
		{
			Assert.IsFalse(BatchCommitted, "The BatchCommitted flag should start off false.");
			
			using (Batch batch = BatchState.StartBatch())
			{
				batch.Committed += new EventHandler(batch_Committed);
			}
			
			Assert.IsTrue(BatchCommitted, "The BatchCommitted flag wasn't changed to true.");
		}
		
		[Test]
		public void Test_TwoBatches_Nested()
		{
			Assert.IsFalse(BatchCommitted, "The BatchCommitted flag should start off false.");
			
			using (Batch batch = BatchState.StartBatch())
			{
				batch.Committed += new EventHandler(batch_Committed);
				
				using (Batch batch2 = BatchState.StartBatch())
				{
					batch2.Committed += new EventHandler(batch_Committed);
						
				}
				
				Assert.IsFalse(BatchCommitted, "The nested batch committed when it shouldn't have. It should leave it for the outer batch.");
				
			}
			
			Assert.IsTrue(BatchCommitted, "The BatchCommitted flag wasn't changed to true.");
		}
		
		[Test]
		public void Test_OneBatch_TwoOperations()
		{
			// Check the flags to ensure they've been reset
			Assert.IsFalse(BatchCommitted, "The BatchCommitted flag should start off false.");
			Assert.IsFalse(StoreCommitted, "The StoreCommitted flag should start off false.");
			
			// Create mock entities
			TestUser user = new TestUser();
			user.ID = Guid.NewGuid();
			user.FirstName = "First";
			user.LastName = "Last";
			
			TestUser user2 = new TestUser();
			user2.ID = Guid.NewGuid();
			user2.FirstName = "First2";
			user2.LastName = "Last2";
			
			// Start the data operation batch
			using (Batch batch = BatchState.StartBatch())
			{
				// Handle the Committed even of the batch
				batch.Committed += new EventHandler(batch_Committed);
				
				// Ensure that the flags are still set to defaults
				Assert.IsFalse(BatchCommitted, "The BatchCommitted flag should remain false until the batch disposes.");
				Assert.IsFalse(StoreCommitted, "The StoreCommitted flag should remain false until the batch disposes.");
				
				// Get the data store for the mock entities
				IDataStore store = DataAccess.Data.Stores[user.GetType()];
				
				// Handle the committed even of the data store
				store.Committed += new EventHandler(store_Committed);
				
				// Save the first mock user
				DataAccess.Data.Saver.Save(user);
				
				Assert.AreEqual(1, BatchState.Batches.ToArray()[0].DataStores.Count, "Invalid number of data stores found.");
				
				Assert.AreEqual(1, BatchState.Batches.Count, "Invalid number of batches found.");
				
				Assert.IsFalse(BatchCommitted, "#1 The BatchCommitted flag should remain false until the batch disposes, even after saving an entity.");
				Assert.IsFalse(StoreCommitted, "#1 The StoreCommitted flag should remain false until the batch disposes, even after saving an entity.");
				
				DisposeMockData();
				InitializeMockData();
				
				StoreCommitted = false;
				BatchCommitted = false;
				
				store = DataAccess.Data.Stores[user.GetType()];
				
				store.Committed += new EventHandler(store_Committed);
				
				TestUser foundUser = DataAccess.Data.Reader.GetEntity<TestUser>("ID", user.ID);
				
				Assert.IsNull(foundUser, "The user was returned when it shouldn't have been committed yet.");
				
				DataAccess.Data.Saver.Save(user2);
				
				Assert.IsFalse(BatchCommitted, "#2 The BatchCommitted flag should remain false until the batch disposes, even after saving an entity.");
				Assert.IsFalse(StoreCommitted, "#2 The StoreCommitted flag should remain false until the batch disposes, even after saving an entity.");
				
				store = DataAccess.Data.Stores[user.GetType()];
				
				store.Committed += new EventHandler(store_Committed);
				
				
			} // Commit should happen here, when batch disposes
			
			Assert.IsTrue(BatchCommitted, "The BatchCommitted flag wasn't changed to true.");
			Assert.IsTrue(StoreCommitted, "The StoreCommitted flag wasn't changed to true.");
			
			DisposeMockData();
			InitializeMockData();
			
			TestUser foundUser2 = DataAccess.Data.Reader.GetEntity<TestUser>("ID", user2.ID);
			
			Assert.IsNotNull(foundUser2, "The user wasn't returned when it should have been.");
			
			Assert.AreEqual(user2.ID.ToString(), foundUser2.ID.ToString(), "The found user ID doesn't match the expected ID.");
			
			
		}

		void store_Committed(object sender, EventArgs e)
		{
			StoreCommitted = true;
		}

		void batch_Committed(object sender, EventArgs e)
		{
			BatchCommitted = true;
		}
	}
}
