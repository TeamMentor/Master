module("TM Global Objects");


test("Check top level variables", function() {
	ok(TM, 		 "TM")
	ok(TM.Gui, 	 "TM.Gui");
	ok(TM.QUnit, "TM.QUnit");
	ok(TM.Debug, "TM.Debug");
	ok(TM.Debug, "TM.Debug");
	ok(TM.Events,"TM.Events");  	
});

test("Check 2nd level objects", function() {  
	ok(TM.WebServices			, "TM.WebServices");  
	ok(TM.WebServices.Data		, "TM.WebServices.Data");  
	ok(TM.WebServices.Config 	, "TM.WebServices.Config");  
	ok(TM.WebServices.Helper	, "TM.WebServices.Helper");    
	ok(TM.WebServices.WS_Utils	, "TM.WebServices.WS_Utils");  
	ok(TM.WebServices.WS_Data	, "TM.WebServices.WS_Data");  	
	
	ok(TM.Events.Gui			, "TM.Events.Gui");  
	
	ok(TM.Gui.Main				, "TM.Gui.Main");  
	
});




/* put these on a special Events NUnit test
  ok(TM.Events.onGuiObjectsLoaded);
	ok(TM.Events.onFolderStructureLoaded);
	ok(TM.Events.onDisplayDataTable);
*/
