module("Create Users",
	{
		setup: TM.QUnit.ControlPanel.moduleSetUp
	});

asyncTest("Create testUsers", function()  	
	{	
		var usersCreated = 0;
		var setGroupId = function(userId, groupId)
			{
				TM.WebServices.WS_Users.setUserGroupID(userId, groupId , function()
					{
						ok(true, "for user " + userId + " set group ID to " + groupId);
					})
			};
		var createDefaultUser = function(username, password, groupdId)
			{
				var passwordHash = SHA256(username+ password);
				TM.WebServices.WS_Users.createUser(username,passwordHash,"","","","",function(userID) 
					{			
						ok(true, "created user: " + username);
						if (userID >0)
							setGroupId(userID, groupdId);
						if(++usersCreated ===3)
							start();
					})								
				
			};

		createDefaultUser(TM.QUnit.defaultUser_Admin, TM.QUnit.defaultPassord_Admin, 1);
		createDefaultUser(TM.QUnit.defaultUser_Editor, TM.QUnit.defaultPassord_Editor , 3);
		createDefaultUser(TM.QUnit.defaultUser_Reader, TM.QUnit.defaultPassord_Reader, 2);
	});

asyncTest("User creation", function()  	
	{			
		var testUser1 = "test_user".add_Random() + ",pwd,a,b,1";
		var testUser2 = "test_user".add_Random() + ",pwd,a,b,2";
		var testUser3 = testUser2;
		
		usersCreated = 0;
		var checkUserCreation = function()
			{
				_newUsers = TM.ControlPanel.UserCreation.createUserData;									
				ok(_newUsers.length ===3 , "Created 2 users")
				start();				
			}
			
		var testUserCreation = function()
			{				
				var guiLoaded = isDefined("usersToCreate".$());
				ok(guiLoaded, "Gui element 'usersToCreate' was found");	
				if (guiLoaded)
				{
					var createUserButton = $("#createUsersButton");
					equal(createUserButton.length, 1, "found Create	 button");
					
					var batchUserText = testUser1.line() + testUser2.line() + testUser3.line();				
					"usersToCreate".$().val(batchUserText);
					setTimeout(function() { createUserButton.click() } , 250);									
				}
			}
		var loadUserCreationPage = function()
			{
				 $("a:contains('Create Multiple Users')").click();		 
				 TM.Events.onControlPanelViewLoaded.add_InvokeOnce(testUserCreation);
			};

		TM.Events.onCreateUsers.add_InvokeOnce(checkUserCreation);		

		if (TM.Gui.CurrentUser.isAdmin())
			loadUserCreationPage();
		else
		{
			stop();
			TM.Events.onControlPanelGuiLoaded.add_InvokeOnce(loadUserCreationPage);		
			qunit_ControlPanel_Helper.loginAs_Admin();
		}		
	});		

asyncTest("Delete Temp Users", function()  			
	{
		var removeTempUsers = function()
		{
			var users = jlinq.from(allUserData).starts("UserName","test_user_").select();
			ok(users.length > 0, "There are users to delete");
			if (users.length ===0)
				start();
			else
			{
				var usersDeleted = 0;
				var onDelete = function(result, userName)
					{
						ok(result,"Removed user: " + userName);
						if (++usersDeleted ==  users.length)
						{
							loadUserData();
							start();
						}
					}
				$(users).each(function(index, userData) 
					{					
						deleteUser(userData.UserID, function(data) { onDelete(data.d, userData.UserName);});								
					});	
			   
			}
		}
		var loadUserManagementPage = function()
			{
				ok(	TM.Gui.CurrentUser.isAdmin(), "Current user is Admin");
				 $("#leftMenu_Links a:contains('Manage Users')").click();		 		 
				TM.Events.onControlPanelViewLoaded.add_InvokeOnce(removeTempUsers);
				
			};

		if (TM.Gui.CurrentUser.isAdmin())
			loadUserManagementPage();
		else
		{
			stop();
			TM.Events.onControlPanelGuiLoaded.add_InvokeOnce(loadUserManagementPage);		
			qunit_ControlPanel_Helper.loginAs_Admin();
		}
	});
