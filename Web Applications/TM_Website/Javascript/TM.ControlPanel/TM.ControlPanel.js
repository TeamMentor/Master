TM.ControlPanel = 
	{
			baseFolder			: "/Html_Pages/ControlPanel/Views/"
		,	defaultTargetDiv	: "#controlPanel_Center"
		
		,	open_Page			: function(targetDiv, page, callback)
								{
									if(isUndefined(targetDiv))
										targetDiv = TM.ControlPanel.defaultTargetDiv;
									loadPage(targetDiv, TM.ControlPanel.baseFolder + page, callback);
								}
								
		,	open_LinksPage		: function(targetDiv)		{	TM.ControlPanel.open_Page(targetDiv, "Admin_Links_LeftMenu.html"		);	}
		,	open_WelcomeScreen	: function(targetDiv)		{	TM.ControlPanel.open_Page(targetDiv, "Welcome_Screen.html"				);  }
		,	open_Banner			: function(targetDiv)		{	TM.ControlPanel.open_Page(targetDiv, "Top_Banner.html"					);	}
		
		,	open_MyAccount		: function(targetDiv)		{	TM.ControlPanel.open_Page(targetDiv, "../ManageUsers/EditUser.html"		, function() { editCurrentUser() });	}
		,	open_ManageUsers	: function(targetDiv)		{	TM.ControlPanel.open_Page(targetDiv, "../ManageUsers/View_Current_Users.html"			);	}
		,	open_CreateUsers 	: function(targetDiv)		{	TM.ControlPanel.open_Page(targetDiv, "../ManageUsers/CreateUsers.html"	);	}
		,	open_AdminTasks		: function(targetDiv)		{	TM.ControlPanel.open_Page(targetDiv, "Admin_Tasks.html"					);	}			
				
		
		,	open_WebServices	: function(targetDiv)		{	TM.ControlPanel.open_Page(targetDiv, "../WebServices/TM_Webservices.html"	);	}

		,	open_QUnitTests		: function(targetDiv)		{	TM.ControlPanel.open_Page(targetDiv, "TeamMentor_QUnitTests.html"		);	}
		,	open_GitHubSync		: function(targetDiv)		{	TM.ControlPanel.open_Page(targetDiv, "GitHub_Sync.html"					);	}
		,	open_InstallLibrary	: function(targetDiv)		{	TM.ControlPanel.open_Page(targetDiv, "Install_Libraries.html"			);	}
		//,	open_InstallLibrary	: function(targetDiv)		{	TM.ControlPanel.open_Page(targetDiv, "File_Upload.html"				);	}
		
		,	open_AdminActions	: function(targetDiv)		{	window.open(TM.ControlPanel.baseFolder + "../UIAutomation/AdminActions.html", "_blank" , "top=200, left=700,height=200, width=400, toolbar=no,scrollbars=no,  menubar=no, location=no"		);	}
		,	open_UserActions	: function(targetDiv)		{	window.open(TM.ControlPanel.baseFolder + "../UIAutomation/UserActions.html" , "_blank" , "top=200, left=700,height=500, width=400, toolbar=no,scrollbars=no,  menubar=no, location=no"		);	}
		,	refresh				: function() 				{ }									




        ,   WebServices         : {}
	}