
module("TM WebServices - Admin User (privs needed)");	

var create_User = function(username, callback)
	{
		var password	 = QUnit.randomString();		
		var passwordHash = SHA256(username + password);		
		TM.WebServices.WS_Users.createUser(username, passwordHash, "", "", "", "", callback);					
	};

asyncTest("user_by_Name", function() 	
	{		
		var username 	 = QUnit.randomString();				
						
		var user_by_Name_callback = function(userDetails)
			{
				ok(userDetails, "got user details");
				ok(userDetails.UserName = username);				
				start();
			}
			
		var loggedAsAdmin_callback		= function() 		{  TM.WebServices.WS_Users.user_by_Name (username, user_by_Name_callback ); };				
		var createUser_callback 		= function(userId)	{  QUnit.login_as_Admin					(loggedAsAdmin_callback);	}		

		create_User(username, createUser_callback)				
	});	

asyncTest("user_by_Id", function() 	
	{		
		var username 	 = QUnit.randomString();				
		var _userId		 = undefined;
		
		var user_by_Name_callback = function(userDetails)
			{											
				ok(userDetails, "got user details:" + userDetails);				
				ok(userDetails.UserName == username 		, "UserName was good");
				ok(userDetails.UserID == _userId		, "UserId was good");
				start();
			}
			
		var loggedAsAdmin_callback		= function() 		
			{  				
				ok(_userId , "_userId is set");				
				TM.WebServices.WS_Users.user_by_Id (_userId, user_by_Name_callback ); 
			};				
			
		var createUser_callback 		= function(userId)	
			{  
				_userId = userId;
				QUnit.login_as_Admin(loggedAsAdmin_callback);	
			}		

		create_User(username, createUser_callback)				
	});		
	
asyncTest("Delete user", function() 	
	{		
		var username 	 = QUnit.randomString();				
		
		var fail_deleteUser_callback = function(result)
			{
				ok(result === false, "deleting a bad userid resulted false");
				start();
			}
		var deleteUser_callback = function(result)
			{
				ok(result, "delete was ok");
				TM.WebServices.WS_Users.deleteUser(123451324, fail_deleteUser_callback ); 
				
			}					
		var createUser_callback 		= function(userId)			{ TM.WebServices.WS_Users.deleteUser   	(userId, deleteUser_callback );  }					
		var loggedAsAdmin_callback		= function() 				{ create_User(username, createUser_callback) };				
		
		QUnit.login_as_Admin(loggedAsAdmin_callback);
	});	

	
asyncTest("getUsers", function() 	
	{			
		var callback = function(users)
			{	
				ok(users, "users was defined");
				if (isDefined(users))
				{
					ok(users.length > 0 ,"we got multiple users: " + users.length);
				}				
				start();
			};
			
		var errorHandler = function(msg)
			{				
				ok(msg.responseText, "[errorHandler] found msg.responseText");				
				var responseError = JSON.parse(msg.responseText)				
				equal(responseError.Message, "Request for principal permission failed.","got expected permission error");				
				start();
				ok(false,"User is not allowed to perform this action: Request for principal permission failed.");
				
			}
			
		var loggedInAsAdmin_callback = function() 
			{	
				TM.WebServices.WS_Users.getUsers(callback, errorHandler); 
			};	
		QUnit.login_as_Admin(loggedInAsAdmin_callback);
	});

asyncTest("delete qUnitTest users", function() 		
{
	var deleteUsers = function(usersToDelete)
	{
		//ok("", "usersToDelete:" + usersToDelete);
		if (usersToDelete.length ===0)
			start();
		else
		{
			var userToDelete = usersToDelete.pop();
			ok(userToDelete.UserName, "deleting user: " + userToDelete.UserName);
			TM.WebServices.WS_Users.deleteUser(userToDelete.UserID, 
				function(result)	{
										ok(result, "deleted ok user: " + userToDelete.UserName);
										deleteUsers(usersToDelete);
									});
				
		}				
	}
	
	var users_callback = function(users)
		{			
			ok(users.length > 0 ,"we got multiple users: " + users.length);
			var usersToDelete = []; 
			$.each(users, function() 
				{ 
					if (this.UserName.substr(0,6)=="qunit_" || this.UserName.substr(0,9) === "test_user") 
						{ 
							usersToDelete.push(this)
						}
				}); 
				
			ok(usersToDelete.length > 0 ,"we have users to delete: " + usersToDelete.length);
			deleteUsers(usersToDelete);
		};
		
	var loggedInAsAdmin_callback = function() { TM.WebServices.WS_Users.getUsers(users_callback); };
	QUnit.login_as_Admin(loggedInAsAdmin_callback);
});

	