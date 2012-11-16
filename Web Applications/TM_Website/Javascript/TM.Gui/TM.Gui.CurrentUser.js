TM.Gui.CurrentUser = 	
	{		
			userData 			: {}
		,	currentUserName		: null
		,	userRoles 			: []
		,	htmlPage 			: ""				
		, 	autoCheckUser	 	: true
		, 	autoCheckInterval 	: 60    // check every 60 sec (will be incremented by 10 secs on every request
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
													
													TM.WebServices.Config.CSRF_Token = that.userData.CSRF_Token;	// set CSRF token

													if (that.currentUserName === that.userData.UserName)
														{															
															return;
														}
													that.currentUserName = that.userData.UserName;
                                                    TM.Gui.CurrentUser.handleUserPostLoginData();                                            
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
        ,   handleUserPostLoginData : function()
                                    {
										if (isDefined(TM.Gui.CurrentUser.userData))
										{

											var postLoginView = TM.Gui.CurrentUser.userData.PostLoginView;

											if (window.location.hash.length < 2 && typeof(postLoginView) == "string")
											{
												window.location.hash = postLoginView;
											}
										}                    
                                    }						
		, 	checkUserLoop			: function()
									{
										var that = TM.Gui.CurrentUser;										
										that.loadUserData();
										if (that.autoCheckUser)
                                        {
                                            that.autoCheckInterval += 10;
											setTimeout(that.checkUserLoop, that.autoCheckInterval * 1000);
                                        }

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

		,	checkPwdComplexity	: function(password, passwordConfirm, errorDiv)
													{									
															if (password != passwordConfirm)
																$(errorDiv).text("Passwords don't match");
															else if (password.length < 8)
																$(errorDiv).text("Password must be at least 8 characters").fadeIn();
															else if (/^[a-zA-Z0-9]+$/.test(password))	
																$(errorDiv).text("Password must contain an non-letter or non-number character");
															else
																return true;										
															$(errorDiv).fadeIn();
															return false;
													}
	}
;