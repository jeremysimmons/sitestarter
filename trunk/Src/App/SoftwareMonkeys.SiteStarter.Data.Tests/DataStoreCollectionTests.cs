using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	/// <summary>
	/// Description of DataStoreCollectionTests.
	/// </summary>
	[TestFixture]
	public class DataStoreCollectionTests : BaseDataTestFixture
	{
		[Test]
		public void Test_this_VirtualServerState()
		{
			
			Guid virtualServerID1 = Guid.NewGuid();
			Guid virtualServerID2 = Guid.NewGuid();
		
			
			DataStoreCollection collection = new DataStoreCollection();
			
			IDataStore dataStore1 = DataAccess.Data.InitializeDataStore("TestStore1");
			
			collection.Add(dataStore1);
			
			IDataStore dataStore2 = DataAccess.Data.InitializeDataStore("TestStore2");
			
			collection.Add(dataStore2);
			
			
			
			IDataStore dataStore3 = DataAccess.Data.InitializeDataStore(virtualServerID1.ToString(), "TestStore3");
			
			collection.Add(dataStore3);
			
			
			IDataStore dataStore4 = DataAccess.Data.InitializeDataStore(virtualServerID2.ToString(), "TestStore4");
			
			collection.Add(dataStore4);
			
			
			IDataStore store1 = DataAccess.Data.Stores["TestStore1"];
			
			Assert.IsNotNull(store1, "TestStore1 not found.");
			
			IDataStore store3 = DataAccess.Data.Stores["TestStore3"];
			
			Assert.IsNotNull(store3, "TestStore3 was found when it shouldn't have been because no virtual server is currently selected but TestStore3 is within a virtual server.");
			
			VirtualServerState.VirtualServerID = virtualServerID1.ToString();
			
			
			IDataStore store2 = DataAccess.Data.Stores["TestStore2"];
			
			Assert.IsNotNull(store2, "TestStore2 was found when it shouldn't have been because a virtual server is currently selected but TestStore2 is not within a virtual server.");
			
			
			IDataStore store4 = DataAccess.Data.Stores["TestStore4"];
			
			Assert.IsNotNull(store4, "TestStore4 not found. The virtual server is selected so it should be found.");
			
		}
		
		[Test]
		public void Test_GetByName()
		{
			DataStoreCollection collection = new DataStoreCollection();
			
			IDataStore dataStore1 = DataAccess.Data.InitializeDataStore("TestStore1");
			
			collection.Add(dataStore1);
			
			IDataStore dataStore2 = DataAccess.Data.InitializeDataStore("TestStore2");
			
			collection.Add(dataStore2);
			
			
			
			IDataStore foundStore1 = collection.GetByName("TestStore1");
			
			Assert.IsNotNull(foundStore1, "The first store wasn't found when searching by short store name.");
			
			IDataStore foundStore2 = collection.GetByName("TestStore2");
			
			Assert.IsNotNull(foundStore2, "The second store wasn't found when searching by short store name.");
			
			
		}
		
		[Test]
		public void Test_GetByName_VirtualServer()
		{
			DataStoreCollection collection = new DataStoreCollection();
			
			Guid virtualServerID1 = Guid.NewGuid();
			
			IDataStore dataStore3 = DataAccess.Data.InitializeDataStore(virtualServerID1.ToString(), "TestStore3");
			
			collection.Add(dataStore3);
			
			Guid virtualServerID2 = Guid.NewGuid();
			
			IDataStore dataStore4 = DataAccess.Data.InitializeDataStore(virtualServerID2.ToString(), "TestStore4");
			
			collection.Add(dataStore4);
			
			
			
			
			IDataStore foundStore3 = collection.GetByName(virtualServerID1.ToString(), "TestStore3");
			
			Assert.IsNotNull(foundStore3, "The third store wasn't found when searching by long store name including virtual server ID.");
			
			IDataStore foundStore4 = collection.GetByName(virtualServerID2.ToString(), "TestStore4");
			
			Assert.IsNotNull(foundStore4, "The fourth store wasn't found when searching by long store name including virtual server ID.");
			
			
			IDataStore notFoundStore3 = collection.GetByName("TestStore3");
			
			Assert.IsNull(notFoundStore3, "The third store was found when it shouldn't have been because it's within a virtual server.");
			
			IDataStore notFoundStore4 = collection.GetByName("TestStore4");
			
			Assert.IsNull(notFoundStore4, "The fourth store was found when it shouldn't have been because it's within a virtual server.");
			
			
			
		}
	}
}
