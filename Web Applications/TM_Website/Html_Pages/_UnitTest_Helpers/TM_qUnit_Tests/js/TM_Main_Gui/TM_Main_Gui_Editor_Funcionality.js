//TM_Main_Gui_Editor_Funcionality.js
module("Editor - Funcionality");

var testUserName = "editor";
var testPassword = TM.QUnit.defaultPassord_Editor;

asyncTest("open main GUI", function() 
	{								
		qunit_Gui_Helper.openMainGui(start);
	});
	
asyncTest("logout if needed", function() 
	{
		qunit_Gui_Helper.logout();
	});
asyncTest("login as editor", function() 	
	{			
		qunit_Gui_Helper.loginAs(testUserName,testPassword);				
	});
	

	
/*asyncTest("login as editor", function() 	
	{	
		if (TM.Gui.CurrentUser.isEditor())
			start();
		else
		{						
			//stop();
			stop();
			qunit_Gui_Helper.logout();	
			
			qunit_Gui_Helper.loginAs(testUserName,testPassword);		
		}
	});
	
*/
	
asyncTest("check top left links", function() 
	{
		equals($("#topRightMenu a:contains('Edit Mode')"	).length, 1, "found Edit Mode link");
		equals($("#topRightMenu a:contains('Logout')"		).length, 1, "found Logout link");
		equals($("#topRightMenu a:contains('Control Panel')").length, 0, "dind't Control Panel link");
		start();
	});

asyncTest("open Edit Mode", function() 
	{					
		var editModeLink = $("#topRightMenu a:contains('Edit Mode')");		
		TM.Events.onHomePageLinksLoaded.add(function()
			{
				TM.Events.onHomePageLinksLoaded = function() { }
				start();
			});
			
		editModeLink.click();				
	});
	
asyncTest("logout as editor", function() 	
	{				
	//	TM.Events.onHomePageLinksLoaded = function() { }		
		start();
		//qunit_Gui_Helper.logout();		
	});	