TM.Debug.logLoadedPages = true
var testUserName = "admin";
var testPassword = TM.QUnit.defaultPassord_Admin;
var pauseOnPopups = false;

module("Open Views",
	{
		setup: TM.QUnit.ControlPanel.moduleSetUp
	});
	
asyncTest("Check if main panels where loaded", function() 
	{			 		
		//var checkPanels = function()
		//	{						
				ok($("#controlPanel_Center").html(), "Welcome to TeamMentor's Control Panel_", "welcome message was ok");										
				ok($("#leftMenu_Links a").length > 0 , "there were links on the left menu");						
				start();	
//			};		
		
	});	
	
asyncTest("Login as Admin", function() 	
	{			
		if (TM.Gui.CurrentUser.isAdmin())
			start();
		else
		{						
			qunit_ControlPanel_Helper.loginAs(testUserName,testPassword);					
		}
	});
	
asyncTest("My Account", function() 	
	{	
		 $("#leftMenu_Links a:contains('My Account')").click();		 		 
		 TM.Events.onControlPanelViewLoaded.add(function() 
			{
				var userEditWindow = TM.ControlPanel.userEditModalWindow
				ok(userEditWindow, "found TM.ControlPanel.userEditModalWindow");
				ok(isDefined(userEditWindow.d.data), "found userEditModalWindow.d.data");						
				var closePopup = function()	{
												userEditWindow.close();	
												start();
											};
				if (pauseOnPopups)			
					setTimeout(closePopup, 1000);	// so that we can see it
				else
					closePopup();				
				
			});;
	});	
	
asyncTest("Manage Users", function() 	
	{	
		 $("#leftMenu_Links a:contains('Manage Users')").click();		 		 
		 TM.Events.onControlPanelViewLoaded.add(start);
	});	
	
asyncTest("Create Multiple Users", function() 	
	{	
		 $("a:contains('Create Multiple Users')").click();		 
		 TM.Events.onControlPanelViewLoaded.add(start);
	});		

 
	
asyncTest("WebServices", function() 	
	{	
		 TM.ControlPanel.open_WebServices();
		 TM.Events.onControlPanelViewLoaded.add(start);
	});		
	
asyncTest("AdminTasks", function() 	
	{	
		 TM.ControlPanel.open_AdminTasks();
		 TM.Events.onControlPanelViewLoaded.add(start);
	});		

asyncTest("TeamMentor QUnitTests", function() 	
	{	
		 TM.ControlPanel.open_QUnitTests();
		 TM.Events.onControlPanelViewLoaded.add(start);
	});		
	

	
/*asyncTest("logout	", function() 	
	{
		qunit_ControlPanel_Helper.logout();
	});
*/