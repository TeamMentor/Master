//Main GUI tests

module("Anonymous - Check Components Load");

asyncTest("open main GUI", function() 
	{					
		ok(TM.Gui.Main.homePage,"TM.Gui.Main.homePage function is defined")
		ok(TM.Gui.ShowProgressBar,"TM.Gui.ShowProgressBar function is defined")
		qunit_Gui_Helper.openMainGui(start);
	});

asyncTest("Check if main panels where loaded", function() 
	{
		equals($(".LibraryTree"			).length, 1, "found library tree");
		equals($("#pivotPanels"			).length, 1, "found pivot panels");
		equals($("#guidanceItems"		).length, 1, "found guidance items");
		equals($("#selectedGuidanceItem").length, 1, "found selected guidance item");
		start();
	});	

asyncTest("logout if needed", function() 	
	{
		qunit_Gui_Helper.logout();
	});
	
asyncTest("check if we have owasp library loaded", function()
	{		
		ok(isUndefined(TM.WebServices.Data.library("OWASP__AAAA")), "didn't find library OWASP__AAAA");
		var owasp_Library 	  = TM.WebServices.Data.library("OWASP")	
		var a01Injection_View = TM.WebServices.Data.view("A01: Injection");
		ok(owasp_Library		, "found library OWASP ");
		ok(a01Injection_View, "found view a01Injection_View");
		equals(a01Injection_View.libraryId,  owasp_Library.id, "view libraryId matches")
		var viewIdNode = $("#" + a01Injection_View.viewId);
		equal(viewIdNode.length, 1, "found viewId node");
		start();
	});

	