using System;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.Generic;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{
	public class ChangedPage
	{
		public Guid PageId { get; set; }
		public DateTime When { get; set;}
		public string UserName { get; set;}
		public Guid SessionID {get; set;}
		public string PageContent { get; set; }
		public string UserComment	{ get; set; }	
		public string UserActivity	{ get; set; }	
	}
	
	
	public class PagesHistory
	{
		public static List<ChangedPage> _pages { get; set;}
		
		public static List<ChangedPage> Pages 
		{ 
			get
			{
				if (_pages.isNull())
				{
					if (DB_File_xml.fileExists())
						_pages = DB_File_xml.load<List<ChangedPage>>();
					else
					{
						_pages = new List<ChangedPage> ();
						savePages();
					}
				}	
				return _pages;
			}
			set
			{
				_pages = value;
			}
		}
		
		public static string dbDir { get; set;}
		
		public static string DB_File_xml
		{
			get	{
					var fileName = "PageHistory.xml";
					return dbDir.pathCombine(fileName);   
				}
		}
				
		
		public PagesHistory()
		{
			dbDir= @"..\_TM_DB".tempDir(false).fullPath();			
		}
		
		public List<ChangedPage> getPages()
		{
			return PagesHistory.Pages;
		}
		
		public static bool savePages()
		{
			lock(_pages)
			{
				try
				{			
					Pages.saveAs(DB_File_xml);
					return true;
				}
				catch(Exception ex)
				{
					"[PagesHistory] error in savePages: {0}".error(ex.Message);
					return false;
				}				
			}		
		}
		
		public static bool removePages(List<ChangedPage> pagesToRemove)
		{
			lock(_pages)
			{
				try
				{			
					foreach(var pageToRemove in pagesToRemove)
						Pages.Remove(pageToRemove);
					savePages();	
					return true;
				}
				catch(Exception ex)
				{
					"[PagesHistory] error in savePages: {0}".error(ex.Message);
					return false;
				}				
			}		
		}
		
		public ChangedPage logPageChange_Random()
		{
			return logPageChange(Guid.NewGuid(), "_a_UnitTest_user", Guid.NewGuid(),  109.randomLetters());
		}
		
		public ChangedPage logPageChange(Guid pageId, String userName, Guid sessionID , String pageContent)
		{
			return logPageChange(pageId, userName, sessionID, pageContent, "","");
		}
		public ChangedPage logPageChange(Guid pageId, String userName, Guid sessionID , String pageContent, string userComment, string userActivity)
		{
			var changedPage = new ChangedPage()
									{
										PageId = pageId,
										When = DateTime.Now,
										UserName = userName,
										SessionID = sessionID,
										PageContent = pageContent,
										UserComment = userComment,
										UserActivity = userActivity, 
									};
			Pages.Add(changedPage);
			savePages();
			return changedPage;
		}
		
		public ChangedPage logPageUserComment(Guid pageId, String userName, Guid sessionID , String userComment)
		{
			return logPageChange(pageId, userName, sessionID, "", userComment,"");
		}
		
		public ChangedPage logPageUserActivity(Guid pageId, String userName, Guid sessionID , String userActivity)
		{
			return logPageChange(pageId, userName, sessionID, "", "",userActivity);
		}
		
		public List<ChangedPage> getPages_by_PageId(Guid pageId)
		{
			return (from changedPage in getPages()
					where changedPage.PageId==pageId
					select changedPage).toList();
		}
		
		public List<ChangedPage> getPages_by_User(string userName)
		{
			return (from changedPage in getPages()
					where changedPage.UserName==userName
					select changedPage).toList();
		}
		
		public List<ChangedPage> getPages_by_SessionID(Guid sessionID)
		{
			return (from changedPage in getPages()
					where changedPage.SessionID==sessionID
					select changedPage).toList();
		}
	}
	
	
}