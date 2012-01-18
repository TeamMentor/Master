module("Create Users",
	{
		setup: TM.QUnit.ControlPanel.moduleSetUp
	});

		
asyncTest("User creation", function() 	
	{	
		var testUser1 = "test_user".add_Random() + ",pwd,a,b,1";
		var testUser2 = "test_user".add_Random() + ",pwd,a,b,2";
		var testUser3 = testUser2;
		
		var checkUserCreation = function()
			{
				_newUsers = TM.ControlPanel.UserCreation.createUserData;
				//alert(JSON.stringify(_newUsers));
				start();
			}
			
		var testUserCreation = function()
			{
				TM.Events.onControlPanelViewLoaded.remove();
				var createUserButton = $("#createUsersButton");
				equal(createUserButton.length, 1, "found Create button");
								
				var batchUserText = testUser1.line() + testUser2.line() + testUser3.line();				
				"usersToCreate".$().val(batchUserText);
				setTimeout(function() { createUserButton.click() } , 250);
				
				TM.Events.onCreateUsers.add(checkUserCreation);
			}
		var loadUserCreationPage = function()
			{
				 $("a:contains('Create Multiple Users')").click();		 
				 TM.Events.onControlPanelViewLoaded.add(testUserCreation);
			};
		loadUserCreationPage();
	});		
