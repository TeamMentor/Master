module("Fetching data from Webservices");

asyncTest("Check that WebServices are working ok", function() 
	{
		TM.WebServices.WS_Utils.getTime(
			function(data){										
							ok(typeof(data) === "string");
							start();							
						   });
	});
	
asyncTest("TM.WebServices.WS_Data.getGUIObjects", function() 
	{
		TM.WebServices.WS_Data.getGUIObjects(
			function(data){
							ok(TM.Debug.TimeSpan_WebService_getGUIObjects);
							ok(TM.WebServices.Data.GuiObjects);
							ok(TM.WebServices.Data.GuiObjects.UniqueStrings.length >=0, "no Unique Strings");
							ok(TM.WebServices.Data.GuiObjects.GuidanceItemsMappings.length >=0, "no Unique Strings");
							start();
						   });
	});

asyncTest("TM.WebServices.WS_Data.getFolderStructure", function() 
	{
		TM.WebServices.WS_Data.getFolderStructure(
			function(data){
							ok(TM.Debug.TimeSpan_WebService_getFolderStructure)
							ok(TM.WebServices.Data.folderStructure);
							ok(TM.WebServices.Data.folderStructure.length >=0, "no Libraries");
							ok(TM.WebServices.Data.folderStructure[0].libraryId);
							ok(TM.WebServices.Data.folderStructure[0].name)
							ok(TM.WebServices.Data.folderStructure[0].views)
//							ok(TM.WebServices.Data.folderStructure[0].subFolders.length>0, "no sub folders")
							start();
						   });
	});	

	
test("TM.WebServices Data Performance - OK ", function() 
	{
		stop();
		TM.WebServices.WS_Data.getGUIObjects(
			function(data)  {
								var seconds = 			TM.Debug.TimeSpan_WebService_getGUIObjects.getSeconds();
								var milliseconds = 		TM.Debug.TimeSpan_WebService_getGUIObjects.getMilliseconds();
								ok(seconds < 5 , 		"TimeSpan_WebService_getGUIObjects (ok < 5) - it was: " + seconds  + " s");
								//ok(milliseconds < 500 , "TimeSpan_WebService_getGUIObjects (ok < 500) - it was: " + milliseconds + " ms" );
								start();
							});									
	
		stop();
		TM.WebServices.WS_Data.getFolderStructure(
			function(data)  {
								var seconds = 			TM.Debug.TimeSpan_WebService_getFolderStructure.getSeconds();
								var milliseconds = 		TM.Debug.TimeSpan_WebService_getFolderStructure.getMilliseconds();
								ok(seconds < 5, 		"TimeSpan_WebService_getFolderStructure (ok < 5) - it was: " + seconds + " s");
								//ok(milliseconds < 999 , "TimeSpan_WebService_getFolderStructure (ok < 999) - it was: " + milliseconds + " ms");
								start();
							});
		
	});
	
