//Ensure TM GuiObject are loaded

module("TM.Gui.LibraryTree - Edit Mode",
	{
		setup: 	function()
				{		
					if (TM.Gui.CurrentUser.isAdmin())
					{						
						start();			
						return;
					}					
					TM.Events.onUserDataLoaded.add_InvokeOnce(function() 
						{ 							
							start();			
						});					
					stop();					
                    TM.Gui.CurrentUser.loadUserData();
				}
	});

var loadDataAndCreateTree = function()
	{
		$("#Canvas").html("<div id='LibraryTreeWithLiveData'>loading tree</div>");	
		var libraryTree = TM.Gui.LibraryTree.open("#LibraryTreeWithLiveData");			
		TM.Events.onFolderStructureMapped.add(function()
			{
				libraryTree.create_TreeUsingJSON();			
			});
		TM.Events.onFolderStructureLoaded();
		return libraryTree;
	};

asyncTest("login As Admin", function() 
	{					
		var checkLogin = function()
		{						
			ok(TM.Gui.CurrentUser.isAdmin(), "Current User isAdmin");
			start();
		}
		if (TM.Gui.CurrentUser.isAdmin())
		{
			checkLogin()
		}
		else
			QUnit.login_as_Admin(function(sessionID)
				{		
					notEqual(sessionID, TM.Const.emptyGuid, "valid SessionID: " + sessionID);				
					TM.Gui.CurrentUser.loadUserData();						
					TM.Events.onUserDataLoaded.add(checkLogin);
				});
	});
	
asyncTest("Open Library in Edit Mode", function() 
{	

	var removeEventsSet = function()
	{
		TM.Events.onLibraryTreeLoaded.remove();
		TM.Events.onFolderStructureMapped.remove();						
	};	    
	var libraryTree = TM.Gui.LibraryTree;	
	TM.Gui.editMode = true;
	TM.Events.onLibraryTreeLoaded.add(function()
		{
			ok(TM.Gui.editMode, "edit mode was enabled");
			ok(libraryTree.jsTree.data.contextmenu, "contextmenu plugin was loaded");
			removeEventsSet();
			start();
		});
	loadDataAndCreateTree();
});

/*asyncTest("Test adding new GuidanceItems (TODO)", function() 
	{
//		ok("","Test adding new GuidanceItems");
		var randomText = QUnit.randomString();
		
		start();
	});*/