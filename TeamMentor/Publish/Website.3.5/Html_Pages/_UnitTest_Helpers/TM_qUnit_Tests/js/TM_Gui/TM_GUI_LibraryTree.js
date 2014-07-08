//Ensure TM GuiObject are loaded

module("TM.Gui.LibraryTree (without webservices)");

asyncTest("Check LibraryTree Object", function() 
{		
	ok(TM.Gui, "TM.GUI was set");
	var libraryTree = TM.Gui.LibraryTree;
	ok(libraryTree, "TM.Gui.LibraryTree was there");	
	equal(libraryTree.version , 1 , "the expected version for this QUnit tests");
	equal(typeof(libraryTree.open), "function", "open function was there");
	start();
});
 
asyncTest("Create Empty LibraryTree", function() 
{		
	$("#Canvas").html("<div id='EmptyLibraryTree'>empty tree</div>");	
	var libraryTree = TM.Gui.LibraryTree.open("#EmptyLibraryTree");
	ok($(libraryTree.targetDiv).length > 0 , "targetDiv was set");
	ok(libraryTree.onTreeCreated, "onTreeCreated was there");
	equal(typeof(libraryTree.onTreeCreated), "function", "onTreeCreated was a function");	
	libraryTree.onTreeCreated = function()
		{
			ok(TM.Gui.LibraryTree.jsTree, "TM.Gui.LibraryTree.jsTree created ok");					
			start();
		}	
	libraryTree.create_EmptyTree();		
});  

asyncTest("Create Test LibraryTree from Scratch (direct methods)", function() 
{		
	$("#Canvas").html("<div id='TestLibraryTree'><ul></ul></div>");	
	var libraryTree = TM.Gui.LibraryTree.open("#TestLibraryTree");
	ok($(libraryTree.targetDiv).length > 0 , "targetDiv was set");
	
	var createLibraryStructure = function()
		{
			var   libName 		= "_test library";
			var   libraryNode 	= libraryTree.add_Library(libName);
			ok   (libraryNode   , "libraryNode created ok");					
			equal($.trim(libraryNode. text()), libName, "libname matched");
			
			var   folderName 	= "_test folder";
			var   folderNode 	= libraryTree.add_Folder(libraryNode, folderName);
			ok   (folderNode  	, "folderNode created ok");					
			equal($.trim(folderNode. text()), folderName, "folderName matched");
			
			var   view1Name 	= "_test view 1 (inside library)";
			var   view1Node 	=  libraryTree.add_View(libraryNode, view1Name);
			ok	 (folderNode  	, "view1Node created ok");					
			equal($.trim(view1Node. text()), view1Name, "view1Name matched");
			
			var   view2Name 	= "_test view 2 (inside folder)";
			var   view1Node 	= libraryTree.add_View(folderNode, view2Name);
			ok   (view1Node   	, "folderName created ok");					
			equal($.trim(view1Node. text()), view2Name, "view2Name matched");
			
			var nodes = libraryTree.nodes()
			equal(nodes.length, 4, "There were 4 nodes");
			
			var   subFolderName = "a sub folder";
			var   subFolderNode = libraryTree.add_Folder(nodes[2], subFolderName);
			ok   (subFolderNode , "subFolderNode created ok");					
			equal($.trim(subFolderNode.text()), subFolderName, "subFolderName matched");
			
			equal(libraryTree.title(folderNode) , libraryTree.title(folderNode), "subFolderNode and respective  folderNode title matched");
			start();
		};
	
	libraryTree.onTreeCreated = createLibraryStructure;
		
	libraryTree.create_EmptyTree();		
});       

asyncTest("Create Test LibraryTree from Scratch (ExtensionMethods)", function() 
{		
	$("#Canvas").html("<div id='TestLibraryTree'><ul></ul></div>");	
	var libraryTree = TM.Gui.LibraryTree.open("#TestLibraryTree");
	ok($(libraryTree.targetDiv).length > 0 , "targetDiv was set");
	
	var createLibraryStructure = function()
		{
			var libName 		= "_test library";
			libraryNode 	= libraryTree.add_Library(libName).title(libName + " - renamed");
			libraryNode.add_View  ("a view").title("a view - renamed");
			libraryNode.add_Folder("a folder").title("a folder - renamed")
					   .add_Folder("a sub folder").title("a sub folder - renamed")
					   .add_View  ("a view inside the subfolder").title("a view inside the subfolder" + " - renamed");;
			start();
		}
	libraryTree.onTreeCreated = createLibraryStructure;		
	libraryTree.create_EmptyTree();		
});       
