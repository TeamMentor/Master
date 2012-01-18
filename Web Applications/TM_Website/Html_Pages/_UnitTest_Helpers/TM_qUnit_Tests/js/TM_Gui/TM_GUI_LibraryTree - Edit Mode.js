//Ensure TM GuiObject are loaded

module("TM.Gui.LibraryTree - Edit Mode",
	{
		setup: 	function()
				{
					stop();
					TM.Gui.CurrentUser.loadUserData(start);
				}
	});

var loadDataAndCreateTree = function()
	{
		$("#Canvas").html("<div id='LibraryTreeWithLiveData'>empty tree</div>");	
		var libraryTree = TM.Gui.LibraryTree.open("#LibraryTreeWithLiveData");			
		TM.Events.onFolderStructureLoaded.add(function()
			{
				libraryTree.create_TreeUsingJSON();			
			});
		return libraryTree;
	};

asyncTest("login As Admin", function() 
	{		
		var checkLogin = function()
		{						
			ok(TM.Gui.CurrentUser.isAdmin(), "Current User isAdmin");
			start();
		}
		QUnit.login_as_Admin(
			function(sessionID)
				{		
					notEqual(sessionID, TM.Const.emptyGuid, "valid SessionID: " + sessionID);				
					TM.Gui.CurrentUser.loadUserData(checkLogin);						
				});
	});
	
asyncTest("Open Library in Edit Mode", function() 
{		
	var libraryTree = TM.Gui.LibraryTree;	
	TM.Gui.editMode = true;
	libraryTree.onTreeLoaded = function()
		{
			ok(TM.Gui.editMode, "edit mode was enabled");
			ok(libraryTree.jsTree.data.contextmenu, "contextmenu plugin was loaded");
			start();
		}	
	loadDataAndCreateTree();
});

asyncTest("Test adding new GuidanceItems", function() 
	{
//		ok("","Test adding new GuidanceItems");
		var randomText = QUnit.randomString();
		
		start();
	});