using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Tests;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	[TestFixture]
	public class DataActivatorTests : BaseDataTestFixture
	{
		public DataActivatorTests()
		{
		}
		
		
		[Test]
		public void Test_Activate()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the Active function.", NLog.LogLevel.Debug))
			{
				
				TestUser user = new TestUser();
				Guid userID = user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				TestRole role = new TestRole();
				Guid roleID = role.ID = Guid.NewGuid();
				role.Name = "Test Role";
				
				
				user.Roles = Collection<TestRole>.Add(user.Roles, role);
				
				DataAccess.Data.Saver.Save(user);
				
				DataAccess.Data.Saver.Save(role);
				
				user = null;
				role = null;
				
				
				TestUser user2 = DataAccess.Data.Reader.GetEntity<TestUser>("ID", userID);
				
				DataAccess.Data.Activator.Activate(user2);
				
				
				
				Assert.IsNotNull(user2.Roles, "The user.Roles property is null.");
				
				if (user2.Roles != null)
				{
					Assert.AreEqual(1, user2.Roles.Length, "Wrong number of roles found.");
					
					Assert.AreEqual(roleID, user2.Roles[0].ID, "ID of referenced role after activating doesn't match the original.");
				}
			}
		}
		
		[Test]
		public void Test_Activate_SingleReference()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the activate function with a single reference.", NLog.LogLevel.Debug))
			{
				TestArticle article = new TestArticle();
				Guid articleID = article.ID = Guid.NewGuid();
				article.Title = "Test";
				
				TestArticlePage page = new TestArticlePage();
				Guid pageID = page.ID = Guid.NewGuid();
				page.Title = "Test Page";
				
				article.Pages = new TestArticlePage[] {page};
				//user.Roles = Collection<TestRole>.Add(user.Roles, role);
				
				DataAccess.Data.Saver.Save(page);
				
				DataAccess.Data.Saver.Save(article);
				
				page = null;
				article = null;
				
				
				TestArticlePage page2 = DataAccess.Data.Reader.GetEntity<TestArticlePage>("ID", pageID);
				
				DataAccess.Data.Activator.Activate(page2, "Article");
				
				
				
				Assert.IsNotNull(page2.Article, "The page2.Article property is null.");
				
				if (page2.Article != null)
				{
					
					Assert.AreEqual(articleID.ToString(), page2.Article.ID.ToString(), "ID of referenced article after activating doesn't match the original.");
				}
			}
		}
		
		
		[Test]
		public void Test_Activate_2References()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the Activate function with 2 references.", NLog.LogLevel.Debug))
			{				
				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				Guid articleID = article.ID;
				article.Title = "Test";
				
				TestArticlePage page = new TestArticlePage();
				page.ID = Guid.NewGuid();
				Guid pageID = page.ID;
				page.Title = "Test Page";
				
				TestArticlePage page2 = new TestArticlePage();
				page2.ID = Guid.NewGuid();
				Guid page2ID = page2.ID;
				page2.Title = "Test Page 2";
				
				article.Pages = new TestArticlePage[] {page, page2};
				//user.Roles = Collection<TestRole>.Add(user.Roles, role);
				
				DataAccess.Data.Saver.Save(page);
				DataAccess.Data.Saver.Save(page2);
				
				DataAccess.Data.Saver.Save(article);
				
				//  TODO: Check if needed
				//page = null;
				//page2 = null;
				//article = null;
				
				EntityReferenceCollection references = DataAccess.Data.Referencer.GetReferences(article.GetType().Name, page.GetType().Name);
				
				Assert.IsNotNull(references, "references == null");
				
				Assert.AreEqual(2, references.Count, "Invalid number of references found.");
				
				
				TestArticle foundArticle = DataAccess.Data.Reader.GetEntity<TestArticle>("ID", articleID);
				
				Assert.IsNotNull(foundArticle, "The foundArticle variable is null.");
				
				DataAccess.Data.Activator.Activate(foundArticle, "Pages");
				
				
				
				Assert.IsNotNull(foundArticle.Pages, "The article.Pages property is null.");
				Assert.AreEqual(2, foundArticle.Pages.Length, "article.Pages.Length != 2.");
				
				if (foundArticle.Pages != null && foundArticle.Pages.Length == 2)
				{
					TestArticlePage foundPage1 = foundArticle.Pages[0];
					TestArticlePage foundPage2 = foundArticle.Pages[1];
					
					//if (foundPage2.ID.ToString() == pageID.ToString())
					//{
					//	TestArticlePage tmp = foundPage2;
					//	foundPage2 = foundPage1;
					//	foundPage1 = tmp;
					//}
					
					// Use an array and IndexOf function so the match will work in any order
					Guid[] pageIDs = new Guid[] {pageID, page2ID};
					
					Assert.IsTrue(Array.IndexOf(pageIDs, foundPage1.ID) > -1, "First found page has the wrong ID.");
					Assert.IsTrue(Array.IndexOf(pageIDs, foundPage2.ID) > -1, "Second found page has the wrong ID.");
					
					//Assert.AreEqual(pageID.ToString() + "---" + page2ID.ToString(), foundPage1.ID.ToString() + "---" + foundPage2.ID.ToString(), "IDs of the reference don't match.");
					
				}
			}
			
			
		}
		
		
		[Test]
		public void Test_Activate_2References_Async_Converging()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the Activate function with 2 asynchronous, converging references.", NLog.LogLevel.Debug))
			{
				
				
				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				Guid articleID = article.ID;
				article.Title = "Test";
				
				TestUser user = new TestUser();
				user.ID = Guid.NewGuid();
				Guid userID = user.ID;
				user.FirstName = "First";
				user.LastName = "Last";
				user.Username ="Username";
				user.Password = "pass";
				
				/*TestArticlePage page = new TestArticlePage();
				page.ID = Guid.NewGuid();
				Guid pageID = page.ID;
				page.Title = "Test Page";
				
				TestArticlePage page2 = new TestArticlePage();
				page2.ID = Guid.NewGuid();
				Guid page2ID = page2.ID;
				page2.Title = "Test Page 2";
				*/
				//article.Pages = new TestArticlePage[] {page, page2};
				//user.Roles = Collection<TestRole>.Add(user.Roles, role);
				
				article.Editors = new TestUser[] {user};
				
				//DataAccess.Data.Saver.Save(page);
				//DataAccess.Data.Saver.Save(page2);
				
				DataAccess.Data.Saver.Save(user);
				DataAccess.Data.Saver.Save(article);
				
				//  TODO: Check if needed
				//page = null;
				//page2 = null;
				//article = null;
				
				EntityReferenceCollection references = DataAccess.Data.Referencer.GetReferences(article.GetType().Name, user.GetType().Name);
				
				Assert.IsNotNull(references, "references == null");
				
				Assert.AreEqual(1, references.Count, "Invalid number of references found.");
				
				
				TestArticle foundArticle = DataAccess.Data.Reader.GetEntity<TestArticle>("ID", articleID);
				
				Assert.IsNotNull(foundArticle, "The foundArticle variable is null.");
				
				DataAccess.Data.Activator.Activate(foundArticle, "Editors");
				DataAccess.Data.Activator.Activate(foundArticle, "Authors");
				
				
				
				Assert.IsNotNull(foundArticle.Editors, "The article.Editors property is null.");
				Assert.AreEqual(1, foundArticle.Editors.Length, "article.Editors.Length != 1.");
				
				if (foundArticle.Editors != null && foundArticle.Editors.Length == 1)
				{
					TestUser foundUser1 = foundArticle.Editors[0];
					
					// Use an array and IndexOf function so the match will work in any order
					Guid[] userIDs = new Guid[] {userID};
					
					Assert.AreEqual(userID.ToString(), foundUser1.ID.ToString(), "The IDs of the user in Editors property doesn't match expected.");
					
				}
				
				Assert.AreEqual(0, foundArticle.Authors.Length, "Authors found when they shouldn't be. The two properties must be getting mixed up.");
				
			}
			
			
		}
		
		
		[Test]
		public void Test_Activate_ReverseReference()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the activate function with a reverse reference.", NLog.LogLevel.Debug))
			{
				TestUser user = new TestUser();
				Guid userID = user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				TestRole role = new TestRole();
				role.ID = Guid.NewGuid();
				Guid roleID = role.ID;
				role.Name = "Test Role";
				
				
				user.Roles = Collection<TestRole>.Add(user.Roles, role);
				
				DataAccess.Data.Saver.Save(user);
				
				DataAccess.Data.Saver.Save(role);
				
				user = null;
				role = null;
				
				
				TestRole role2 = DataAccess.Data.Reader.GetEntity<TestRole>("ID", roleID);
				
				DataAccess.Data.Activator.Activate(role2);
				
				
				
				Assert.IsNotNull(role2.Users, "The role2.Users property is null.");
				
				if (role2.Users != null)
				{
					Assert.AreEqual(1, role2.Users.Length, "Wrong number of users found.");
					
					Assert.AreEqual(userID, role2.Users[0].ID, "ID of referenced user after activating doesn't match the original.");
				}
			}
		}
		

	}
}
