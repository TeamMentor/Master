//Helper functions for TM_Main_Gui_* tests

var qunit_Gui_Helper = 
	{
		openMainGui: function(callback) 
			{						
				if (TM.HomePageLinks == "Loaded")
				{									
					callback();
					return;
				}
				TM.Gui.Main.homePage("#Canvas");						
				
				TM.Events.onLibraryTreeLoaded.add(function()
					{
						TM.Events.onLibraryTreeLoaded.remove();
						callback();						
					})
			},
		logout: function()
			{					
				var logoutButton = $("#topRightMenu a:contains('Logout')");	
				if(logoutButton.length == 1)
				{												
					TM.Events.onHomePageLinksLoaded.add(function() 
						{  
							TM.Events.onHomePageLinksLoaded.remove();
							start() ;
						});						
					logoutButton.click();	
				}
				else
					start();
			},
		loginAs: function(username, password)
			{
				var loginButton = $("#topRightMenu a:contains('Login')");
				var logoutButton = $("#topRightMenu a:contains('Logout')");	
				
				equals(1,loginButton.length,"There was one login link");
				equals(0,logoutButton.length,"There was one no logout button");
				$("#topRightMenu a:contains('Login')").click();
				
				TM.Events.onLoginDialogOpen.add(function()
					{
						$("#UsernameBox").val(username)
						$("#PasswordBox").val(password)
						$("button:contains('login')").click();				
					});	
					
				TM.Events.onHomePageLinksLoaded.add(function() 
						{  
							TM.Events.onHomePageLinksLoaded.remove()
							start() ;
						});					
			}
	}