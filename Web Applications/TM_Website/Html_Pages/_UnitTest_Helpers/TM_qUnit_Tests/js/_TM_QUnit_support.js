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
		TM.WebServices.WS_Users.login("admin", TM.QUnit.defaultPassord_Admin ,callback);
	};	

QUnit.config.reorder = false;	 //always exectute from the start