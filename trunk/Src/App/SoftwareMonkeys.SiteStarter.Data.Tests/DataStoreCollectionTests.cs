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
		
	}
}
