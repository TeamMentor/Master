//Helper functions for TM_ControlPanel_* tests

TM.QUnit.ControlPanel = 
	{	
		guiLoaded 		: false,
		moduleSetUp 	: function() 
			{						
				if (TM.QUnit.ControlPanel.guiLoaded)
				{
					start();
					return;
				}
				stop();
				TM.Events.onControlPanelGuiLoaded.add(function() 
					{
						TM.QUnit.ControlPanel.guiLoaded = true;									
						start();
					});
			}
	};

	
var qunit_ControlPanel_Helper = 
	{
/*		openControlPanelGui: function() 
			{
				ok(TM.Gui.Main.homePage,"TM.Gui.Main.homePage function is defined")
				ok(TM.Gui.ShowProgressBar,"TM.Gui.ShowProgressBar function is defined")
				
				TM.Gui.Main.homePage("#Canvas");						
				TM.Events.onHomePageLinksLoaded.add(start);
			},
*/			
		logout: function()
			{
				var logoutButton = $("a:contains('Logout')");	
				if(logoutButton.length == 1)
				{		
					logoutButton.click();
					
					TM.Events.onControlPanelGuiLoaded.add(function() 
						{  							
							start() ;
						});
				}
				else
					start();
			},
		loginAs: function(username, password)
			{
			
				var loginButton = $("a:contains('Login')").click();
				//var logoutButton = $("#topRightMenu a:contains('Logout')");	
				
				equals(1,loginButton.length,"There was one login link");
				//equals(0,logoutButton.length,"There was one no logout button");
				loginButton.click();
				
				TM.Events.onLoginDialogOpen.add(function()
					{
						$("#UsernameBox").val(username)
						$("#PasswordBox").val(password)
						$("button:contains('login')").click();										
					});		
				TM.Events.onUserChange.add(function()	
					{
						TM.Events.onLoginDialogOpen.remove();
						loadControlPanelDefaultPages();
						start();
					});
			}
	}