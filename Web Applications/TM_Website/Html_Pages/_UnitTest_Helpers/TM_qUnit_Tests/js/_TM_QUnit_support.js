QUnit.done = function(result)
	{		
		TM.QUnit.lastExecution = result;
		UnitTest_Helper_QUnitExecutionCompleted = true;				
	};	
		
QUnit.randomString = function() 
	{  		
		return  "qunit_" + Math.ceil(Math.random() * 100000);		
	};	
	
QUnit.login_as_Admin = function(callback)
	{				
		var onLogin = function()
			{
				TM.Events.onUserDataLoaded.add_InvokeOnce(function() 
					{ 							
						callback();		
					});										
				TM.Gui.CurrentUser.loadUserData();
			}
		if (TM.Gui.CurrentUser.isAdmin())
			callback();			
		else
			TM.WebServices.WS_Users.login(TM.QUnit.defaultUser_Admin, TM.QUnit.defaultPassord_Admin ,onLogin);
		
	};	

QUnit.config.reorder = false;	 //always exectute from the start