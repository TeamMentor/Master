using System;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
//O2File:../UtilMethods.cs 

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{
	[Serializable]
	public class RequestData
	{
		[XmlAttribute] public string Url 			{ get; set; }
		[XmlAttribute] public DateTime Date		{ get; set; }
		[XmlAttribute] public string UserName		{ get; set; }
		[XmlAttribute] public Guid SessionID		{ get; set; }
		[XmlAttribute] public string PostData		{ get; set; }			
	}
	
	public class ActivityDB
	{
		public static List<RequestData> _pastRequests { get; set;}
		public static int dbFileIndex;
		
		public static string dbDir;

		public static bool DontSave = false;
		public static bool DontLog = false;
		public static int MaxMillisecondsToSaveIn = 500;
		public static int SaveToDiskSleepValue = 2000;
		static Semaphore waitForSave = new Semaphore(1, 1);
		static bool triggerSaveThread = true;
		
		
	    static ActivityDB()
		{
		    dbDir= @"..\_TM_DB\ActivityTracking".tempDir(false)
											    .fullPath()
												.createDir();			
		}
		
		public static string DB_File_xml
		{
			get	{				
					var fileName = "{0}_-_{1}.xml".format(DateTime.Now
																  .ToLongDateString()
																  .safeFileName(),
														  dbFileIndex);
					var dbFile = dbDir.pathCombine(fileName);   
					if (dbFile.fileExists())
					{
						dbFileIndex++;
						return DB_File_xml;
					}
					return dbFile;
				}
		}
		
		public static List<RequestData> PastRequests
		{
			get {					
					if (_pastRequests.isNull())
						_pastRequests = new List<RequestData>();				
					return _pastRequests;
				}
				
			set {
					_pastRequests = value;
				}
		}
		
		public static RequestData LogRequest(RequestData requestData)
		{						
			if (requestData.notNull())		
			{						
				waitForSave.WaitOne();
				PastRequests.Add(requestData);
				waitForSave.Release();
				SaveToDisk();
			}
			else
				"in logRequest, requestData was null".error();
			return requestData;
		}
		
		
		public static void SaveToDisk()
		{		
			if (DontSave)
				return;			
			if (triggerSaveThread)
			{				
				triggerSaveThread = false;
				O2Thread.mtaThread(
					()=>{										
							PastRequests.sleep(SaveToDiskSleepValue);														
							waitForSave.WaitOne();
							try
							{									
								var atStart = DateTime.Now;
								"saving PastRequests to disk: {0}".info(DB_File_xml);
								
								var savedItemsCount = PastRequests.size();
								PastRequests.saveAs(DB_File_xml);
								var saveDuration = DateTime.Now - atStart;
								"save took: {0}".debug(saveDuration.TotalSeconds);
								if (saveDuration.Milliseconds > MaxMillisecondsToSaveIn)
								{										
									//"MaxMilisecondsSecondsToSaveIn reached, increasing dbFileIndex :{0} == {1}".error(
									//	savedItemsCount, PastRequests.size());
									PastRequests.Clear();
									dbFileIndex++;
								}		
								
							}
							catch(Exception ex)
							{
								"[ActivityDB] Error in SaveToDisk: {0}".error(ex.Message);
							}
							triggerSaveThread = true;
							waitForSave.Release();							
						});
			}
		}
				
		
		public static List<string> getTodaysFiles()
		{
			"[ActivityDB] getTodaysFiles".info();
			return dbDir.files("*.xml");
		}
		
		public static bool deleteTodaysFiles()
		{
			"[ActivityDB] deleteTodaysFiles".info();	
			var filesToDelete = getTodaysFiles();
			foreach(var file in filesToDelete)
				Files.deleteFile(file);
			foreach(var file in filesToDelete)
				if (file.fileExists())
					return false;
			return true;
		}
		
		public static List<RequestData> getTodaysRequestData()
		{
			var todaysRequestData = new List<RequestData>();
			foreach(var file in getTodaysFiles())
			{
				var requestData = file.load<List<RequestData>>();
				if (requestData.notNull())
					todaysRequestData.AddRange(requestData);
			}
			return todaysRequestData;
		}
	}
	
	public class ActivityTracking
	{
		public RequestData GetRequestData()
		{
			return GetRequestData(Guid.Empty, "");
		}
		
		public RequestData GetRequestData(Guid sessionID, string userName)
		{
			try
			{
				var httpContext = HttpContextFactory.Current;
				var postData = httpContext.GetPostDataAsString();
				var url = httpContext.Request.Url;
				var requestData = new RequestData()
									{
										Url = url.notNull() ? url.str() : "",
										Date = DateTime.Now,
										UserName = userName,
										SessionID = sessionID,
										PostData = postData
									};
				return requestData;			
			}
			catch(Exception ex)
			{				
				ex.log();
				return null;
			}			
		}
		
		public RequestData GetLastRequest()
		{
			if (PastRequests.size() < 2)
				return null;
			return PastRequests[PastRequests.size() -1];	// so that we don't get the entry for GetLastRequest			
		}
				
		public void LogRequest()
		{
			LogRequest(Guid.Empty, "");
		}
		
		public RequestData LogRequest(Guid sessionID, string userName)
		{		
			if (ActivityDB.DontLog)
				return null;
			var requestData = GetRequestData(sessionID, userName);
			return LogRequest(requestData);
		}
		
		public RequestData LogRequest(RequestData requestData)
		{
			return ActivityDB.LogRequest(requestData);
		}			
		
		public List<RequestData> PastRequests
		{
			get {
					return ActivityDB.PastRequests;
				}						
		}	
		
		//public List<RequestData pastRequests(int count)
		//{
			
		//}
	}
	
	public static class ActivityTracking_ExtensionMethods
	{
		public static ActivityTracking clearPastRequestsObject(this ActivityTracking activityTracking)
		{
			activityTracking.PastRequests.Clear();
			return activityTracking;
		}
		// Random data
		public static RequestData randomRequestData(this ActivityTracking activityTracking)
		{
			return activityTracking.randomRequestData(false);
		}
		public static RequestData randomRequestData(this ActivityTracking activityTracking, bool createLargeRequest)
		{
			var requestData = new RequestData() 
									{ 
										Url = "/{0}.asmx/{1}".format(10.randomLetters(),10.randomLetters()) ,
										Date = DateTime.Now,
										SessionID = Guid.NewGuid(),
										UserName = 10.randomLetters(), 
										PostData = (createLargeRequest)
														? "{{ {0} : '{1}' + '{2}' }}".format(50.randomLetters(), 500.randomLetters(), 5000.randomLetters())
														: "{{ {0} : '{1}'  }}".format(5.randomLetters(), 30.randomLetters())
									};
												
			return requestData;
		}
		
		public static RequestData logRandomRequestData(this ActivityTracking activityTracking)
		{
			return activityTracking.logRandomRequestData(1)[0];
		}
		
		public static List<RequestData> logRandomRequestData(this ActivityTracking activityTracking, int count)
		{
			return activityTracking.logRandomRequestData(count, false);
		}
		public static List<RequestData> logRandomRequestData(this ActivityTracking activityTracking, int count, bool createLargeRequest)
		{
			var requestsLogged = new List<RequestData>();
			for(var i=0 ; i < count; i++)
			{
				var requestData = activityTracking.randomRequestData(createLargeRequest); 
				requestsLogged.Add(activityTracking.LogRequest(requestData));
			}
			return requestsLogged;
		}
		
		
	}
}