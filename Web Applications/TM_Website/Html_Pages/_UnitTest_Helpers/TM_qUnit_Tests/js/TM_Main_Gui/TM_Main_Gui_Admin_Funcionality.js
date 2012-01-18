//TM_Main_Gui_Admin_Funcionality.js
module("Admin - Funcionality");

var testUserName = "admin";
var testPassword = "!!tmbeta";

asyncTest("open main GUI", function() 
	{									
		qunit_Gui_Helper.openMainGui(start);
	});

asyncTest("logout if needed", function() 
	{
		qunit_Gui_Helper.logout();
	});
asyncTest("login as admin", function() 	
	{	
		//if (TM.Gui.CurrentUser.isAdmin())
		//	start();
		qunit_Gui_Helper.loginAs(testUserName,testPassword);				
	});
	

	
asyncTest("check top left links", function() 
	{
		equals($("#topRightMenu a:contains('Edit Mode')"	).length, 1, "found Edit Mode link");
		equals($("#topRightMenu a:contains('Logout')"		).length, 1, "found Logout link");
		equals($("#topRightMenu a:contains('Control Panel')").length, 1, "found Control Panel link");
		start();
	});
	
asyncTest("logout as admin", function() 	
	{				
		qunit_Gui_Helper.logout();
		//start();				
	});	