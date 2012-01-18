//TM_Main_Gui_Editor_Funcionality.js
module("Viewer - Funcionality");

var testUserName = "reader";
var testPassword = TM.QUnit.defaultPassord_Reader;

asyncTest("open main GUI", function() 
	{								
		qunit_Gui_Helper.openMainGui(start);		
	});

asyncTest("logout if needed", function() 
	{
		qunit_Gui_Helper.logout();
	});
asyncTest("login as viewer", function() 	
	{			
		qunit_Gui_Helper.loginAs(testUserName,testPassword);				
	});
	
/*asyncTest("login as viewer", function() 	
	{	
		if (TM.Gui.CurrentUser.isViewer())		
		{
			qunit_Gui_Helper.logout(function() 
				{ 					
					qunit_Gui_Helper.loginAs(testUserName,testPassword);
				});					
		}	
		else
		{			
			qunit_Gui_Helper.loginAs(testUserName,testPassword);
		}
	});
*/	

	
asyncTest("check top left links", function() 
	{
		equals($("#topRightMenu a:contains('Logout')"		).length, 1, "found Logout link");
		equals($("#topRightMenu a:contains('Edit Mode')"	).length, 0, "found Edit Mode link");		
		equals($("#topRightMenu a:contains('Control Panel')").length, 0, "didn't Control Panel link");
		start();
	});

asyncTest("logout as viewer", function() 	
	{				
		//start();
		qunit_Gui_Helper.logout(start);		
	});	