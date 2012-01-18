TM.Gui.CurrentUser = 	
	{		
			userData 			: {}
		,	currentUserName		: null
		,	userRoles 			: []
		,	htmlPage 			: ""				
		, 	autoCheckUser	 	: true
		, 	autoCheckInterval 	: 30 * 1000				// check every 25 sec
		,	loadUserData  		: function()
									{													
										TM.WebServices.WS_Users.currentUser(
											function(data) 
												{			
													var that = TM.Gui.CurrentUser;													
													if (data == null)
														that.userData = {};
													else
													{														
														that.userData = data;
													}
													
													if (that.currentUserName === that.userData.UserName)
														{
															//console.log("SAME user: " + that.currentUserName);
															return;
														}
													that.currentUserName = that.userData.UserName;
													TM.Gui.CurrentUser.loadUserRoles();
												});
									}
		,	loadUserRoles 		: function()
									{												
										TM.WebServices.WS_Users.currentUserRoles(
											function(data) 
												{																	
													TM.Gui.CurrentUser.userRoles = data;	
													TM.Events.onUserDataLoaded();
												});
									}							
		, 	checkUserLoop			: function()
									{
										var that = TM.Gui.CurrentUser;										
										that.loadUserData();
										if (that.autoCheckUser)
											setTimeout(that.checkUserLoop, that.autoCheckInterval);
									}
		, 	logout				: function()		
									{
										var that = TM.Gui.CurrentUser;
										that.userData  = {};
										that.userRoles = [];
										TM.WebServices.WS_Users.logout();	
									}							
		, 	start_checkUserLoop : function()		{	TM.Gui.CurrentUser.autoCheckUser = true; TM.Gui.CurrentUser.checkUserLoop(); }						
		, 	stop_checkUserLoop : function()		{	TM.Gui.CurrentUser.autoCheckUser = false; }						
		, 	get_UserRoles		: function() 		{ 	return TM.Gui.CurrentUser.userRoles;}
		, 	roles				: function() 		{ 	return TM.Gui.CurrentUser.userRoles;} 	
		, 	get_UserData		: function()		{	return TM.Gui.CurrentUser.userData; }
		, 	hasRole				: function(role)  	{ 	return TM.Gui.CurrentUser.userRoles.indexOf(role) > -1; }
		, 	isAdmin				: function()		{	return TM.Gui.CurrentUser.hasRole("Admin"); }	
		,	isEditor			: function()		{	return TM.Gui.CurrentUser.hasRole("EditArticles"); }	
		,	isViewer			: function()		{	return TM.Gui.CurrentUser.hasRole("ReadArticles"); }
		,	userName			: function()		{	return TM.Gui.CurrentUser.userData.UserName; }
		,	loggedIn			: function()		{	return typeof(TM.Gui.CurrentUser.userName()) != "undefined"; }
		,	isUserLoaded		: function()		{	return isDefined(TM.Gui.CurrentUser.userData.UserName); }					
	}
;