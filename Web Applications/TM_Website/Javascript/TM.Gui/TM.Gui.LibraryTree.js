
//$.jstree.defaults.themes.url = "/javascript/jQuery.jsTree/themes/default/style.css";

TM.Gui.LibraryTree = 
	{	
		// variables
		version 	: 1 , 
		plugins		:  [ "themes", "json_data" , "ui", "crrm", "contextmenu"] ,	  			 //  ,  "hotkeys"	
		treeData 	: undefined,
		targetDiv	: undefined,		
		jsTree   	: undefined,			
		
		// events
		onTreeCreated: 		function() { } ,
		onTreeLoaded: 		function() { } ,
		onSelectedNode:		function() { } ,
		
		
		// methods
		open: 				function(_targetDiv)
								{									
									this.targetDiv = _targetDiv
									return this;
								},
								
		create_EmptyTree:	function()
								{
									this.jsTree = undefined;
									this.treeData = { data: [] }; 									
									this.create_Tree();
								},
								
		create_TreeUsingJSON: function()								
								{															
									var that = this;
									var startTime = new Date();
									this.jsTree = undefined;
									
									this.onTreeCreated = function() 
										{											
											TM.Debug.TimeSpan_Gui_LibraryTree_CreatedTreeFromJsonData = startTime.toNow();		
											that.onTreeLoaded();
											TM.Events.onLibraryTreeLoaded();											
										};
										
									this.loadJsonData(function() 
										{ 														
											that.create_Tree();
										} );
								},
		
		create_Tree:		function()
								{															
									var options = { 
													  json_data 	: this.treeData,
													  plugins 		: //(TM.Gui.editMode)  
																		//? this.plugins.concat("dnd")
																		//: 
																		this.plugins, 	
													  themes		: { url: '/javascript/jQuery.jsTree/themes/default/style.css'},
													  contextmenu 	: { items: TM.Gui.LibraryTree.createContextMenu },
													  ui 			: { "select_limit" : 1 }
													  //,
/*													  "dnd"			: { "drag_check": function() { alert('drag check') } ,
																		"drag_finish": function() { alert('drag finish') }, 
																		"drop_finish": function() { alert('drop finish') } 
																	  }*/
												  };

									_options = options;									
									var that = this;
								
									$(this.targetDiv).bind("loaded.jstree", function (event, data) 
														{																	
															that.jsTree = data.inst;
															that.onTreeCreated();
														});
									
									//create tree
									$(this.targetDiv).jstree(options)
													 .delegate("a", "click", this.setSelectedId)
													 .delegate("a", "click", function() { that.onClick() } );
									
									//bind renamema nd create events
									$(this.targetDiv)
										  .bind("rename.jstree", 		TM.Gui.LibraryTree.onRename)
										  .bind("create.jstree", 		TM.Gui.LibraryTree.onCreate)						 
										  
									//setup drag&drop actions and icons 
									TM.Gui.LibraryTree.dropActions.setUp();
									
								},
								
		setSelectedId:		function (event)	
								{									
									//_event = event;									
									//var node = $(event.srcElement);	// doesn't work in FF
									var node = $(event.target);									
									TM.Gui.LibraryTree.selectNode(node);																		
								},
								
		onClick :			function (event, data) 		{ },
								
		loadJsonData:		function(callback)
								{					
									var that = this;									
									TM.WebServices.WS_Data.getJsTreeWithFolders(
										function(data)
											{														
												that.treeData = data;												
												 callback();
											});
								},
								
		create_Tree_FromWsData: 	function()
										{	
											this.create_EmptyTree();
											this.onTreeCreated = this.add_LibrariesFromWsData;
										},
											
		add_LibrariesFromWsData: 	function()
										{
											var startTime = new Date();
											var that = this;
											$.each(TM.WebServices.Data.AllLibraries, function() 
													{ 
														that.add_LibraryFromWsData(this);  
													});		
											TM.Debug.TimeSpan_Gui_LibraryTree_CreatedTreeFromWsData = startTime.toNow();		
											this.onTreeLoaded();
											console.log("add_LibrariesFromWsData");
											console.log("add_LibrariesFromWsData");
										},
										
		add_LibraryFromWsData:		function(library)
										{
											var libraryNode = this.add_Library(library.name);
											libraryNode.attr('id', library.id);
											libraryNode.attr('type', library.__type);
											this.add_FoldersFromWsData	(libraryNode, library.subFolders);
											this.add_ViewsFromWsData	(libraryNode, library.views);
										},
										
		add_FoldersFromWsData:		function(rootNode, folders)
										{					
											var that = this;
											$.each(folders, function() 
													{ 
														var folderNode = rootNode.add_Folder(this.name);
														folderNode.attr('id', this.id);
														folderNode.attr('type', this.__type);
														
														that.add_FoldersFromWsData	(folderNode, this.subFolders);
														that.add_ViewsFromWsData	(folderNode, this.views);
													});		
										},
		add_ViewsFromWsData:		function(rootNode, views)
										{											
											$.each(views, function() 
													{ 
														var folderNode = rootNode.add_View(this.caption);
														folderNode.attr('id', this.id);
														folderNode.attr('type', this.__type);
														//that.add_LibraryFromWsData(this);  
													});		
										}
		
								
	}

//****************
//nodes manipulation methods
//****************

TM.Gui.LibraryTree.nodes = function() 
	{
		return $(TM.Gui.LibraryTree.targetDiv + " ul li");
	}
	
TM.Gui.LibraryTree.title = function(node, value)
	{
		if(isDefined(value))
		{
			TM.Gui.LibraryTree.jsTree.set_text(node, value);
			return node;
		}
		else
			return TM.Gui.LibraryTree.jsTree.get_text(node);		
	}

TM.Gui.LibraryTree.firstNode = function() 	
	{
		return TM.Gui.LibraryTree.nodes().first();
	}
	
TM.Gui.LibraryTree.selectNode = function(node) 		
	{							
		_node = node;		
		if (isDefined(node))
		{			
			selectedNodeId = jQuery(node).attr('id'); 			
			if (isUndefined(selectedNodeId) || selectedNodeId==="")
				selectedNodeId = jQuery(node).parent().attr('id'); 		
			if (isDefined(selectedNodeId))
			{			
				if(selectedNodeId != TM.Gui.selectedNodeId)
				{					
					TM.Gui.selectedNodeId = selectedNodeId;		
					TM.Gui.selectedNodeData = $.data[selectedNodeId];							
					TM.Gui.LibraryTree.jsTree.select_node(node);
					TM.Gui.LibraryTree.selectedNode = node;
					TM.Gui.LibraryTree.onSelectedNode(node);
				}
			}
			else				
				console.log("selectedNodeId was not defined");
		}
		return node;
	}
	
TM.Gui.LibraryTree.openNode = function(node) 		
	{
		TM.Gui.LibraryTree.jsTree.open_node(node);
		return node;
	}	
	
TM.Gui.LibraryTree.selectFirstNode = function() 		
	{
		var firstNode = TM.Gui.LibraryTree.firstNode();
		TM.Gui.LibraryTree.selectNode(firstNode);
		return firstNode;
	}
//firstNode = $("#libraryJsTree ul li a").first();
//				libraryTree.jsTree.select_node(firstNode);	
	

//*********************	
//node creation methods
//*********************

TM.Gui.LibraryTree.add_Node = function(targetNode, title, icon, callback, skip_rename)
	{
		if (isUndefined(skip_rename))
			skip_rename = true;
		var newNode = TM.Gui.LibraryTree.jsTree
							.create(
										targetNode, 
										"last",
										{  data: { 
													title:title , 
													icon:  icon
												 }
										} , 
										callback , 
										skip_rename
									);
		newNode.title = function(value) { return TM.Gui.LibraryTree.title(newNode, value) };	
		return newNode;
	}
//These methods make no connection to the database (i.e. are purely GUI driven)
TM.Gui.LibraryTree.add_Library = function(title, callback, skip_rename)
	{		
		var libraryNode = TM.Gui.LibraryTree.add_Node("",title, '/Images/SingleLibrary.png' , callback, skip_rename)
		libraryNode.add_Folder = function(title, callback, skip_rename) 
			{ 
				return TM.Gui.LibraryTree.add_Folder(libraryNode, title, callback, skip_rename);
			};
		libraryNode.add_View = function(title, callback, skip_rename) 
			{ 
				return TM.Gui.LibraryTree.add_View(libraryNode, title, callback, skip_rename);
			};	
			
		//libraryNode.title = function(value) { return TM.Gui.LibraryTree.title(libraryNode, value) };
		
		return libraryNode;
	}
	
TM.Gui.LibraryTree.add_Folder = function(targetNode, title, callback, skip_rename)
	{
		var folderNode = TM.Gui.LibraryTree.add_Node(targetNode,title, '/Images/FolderIcon.png' , callback, skip_rename)
		folderNode.add_Folder = function(title, callback, skip_rename) 
			{ 
				return TM.Gui.LibraryTree.add_Folder(folderNode, title, callback, skip_rename);
			};
		folderNode.add_View = function(title, callback, skip_rename) 
			{ 
				return TM.Gui.LibraryTree.add_View(folderNode, title, callback, skip_rename);
			};	
		return folderNode;
	}

TM.Gui.LibraryTree.add_View = function(targetNode, title, callback, skip_rename)
	{
		return TM.Gui.LibraryTree.add_Node(targetNode,title, '/Images/ViewIcon.png' , callback, skip_rename)
	}

//These methods are the ones that make commits to the database and the GUI
TM.Gui.LibraryTree.add_Library_to_Database = function(title, callback, skip_rename)	
	{
		var libraryNode = TM.Gui.LibraryTree.add_Library(title, callback, skip_rename);
		libraryNode.hide();		
		TM.WebServices.WS_Libraries.add_Library(
			title, function(libraryV3) 
				{ 						
					if (libraryV3 != null)		
					{					
						libraryV3.type = "Library";
						libraryNode.fadeIn();
						TM.WebServices.Data.AllLibraries.push(libraryV3);						
						$.data[libraryV3.libraryId] = libraryV3;
						libraryNode.attr("id", libraryV3.libraryId);
						
						TM.Gui.Dialog.alertUser('Library Created');
					}					
					else
						TM.Gui.Dialog.alertUser("It was not possible to create the library called : " + title, 'Library creation error'  );					
					TM.Events.onNewLibrary(libraryV3);						
				}); 
		return libraryNode;
	}

TM.Gui.LibraryTree.remove_Library_from_Database = function(libraryIdOrName)
	{
		var libraryId = libraryIdOrName;
		var libraryV3 = $.data[libraryId];
		if (isUndefined(libraryV3))
		{			
			libraryV3 = TM.WebServices.Data.library(libraryIdOrName);	
			if (isUndefined(libraryV3))
				TM.Gui.Dialog.showUserMessage("In remove_Library_from_Database, could not find the library to remove:" + libraryIdOrName);					
			libraryId = libraryV3.libraryId;
		}		
		
		var libraryNode = $("#" + libraryId);  
		
		if (libraryNode.length != 1 || isUndefined(libraryV3))
		{	
			_libraryNode = libraryNode;
			_libraryV3 = libraryV3;
			TM.Gui.Dialog.showUserMessage("something is wrong, the objects required to remove the library are not available: " + libraryId);								
			TM.Events.onRemovedLibrary(false,libraryV3);	
			return libraryId;
		}
		else	
			TM.WebServices.WS_Libraries.remove_Library(
				libraryId, function(result) 
					{ 											
						if (result)		
						{											
							libraryNode.remove();
							TM.WebServices.Data.AllLibraries.pop(libraryV3);
							delete $.data[libraryV3.libraryId];
							TM.Gui.Dialog.alertUser('Library Deleted');
						}					
						else
							TM.Gui.Dialog.showUserMessage("it was not possible to remove the library:" + libraryId);												
						TM.Events.onRemovedLibrary(result,libraryV3);	
					}); 		
	}
	
TM.Gui.LibraryTree.remove_Folder_from_Database = function(libraryId, folderId)
	{		
		var folderNode = $("#" + folderId);  
		var folderV3 = $.data[folderId];				
		TM.WebServices.WS_Libraries.remove_Folder(
				libraryId, folderId,  function(result) 
					{ 											
						if (result)		
						{											
							folderNode.remove();							
							TM.WebServices.Data.AllFolders.pop(folderV3);
							delete  $.data[folderId];
							TM.Gui.Dialog.alertUser('Folder Removed');
						}					
						else
							TM.Gui.Dialog.showUserMessage("it was not possible to remove the folder:" + folderId);												
						TM.Events.onRemovedFolder(result,folderV3);	
					}); 		
	}	
	
TM.Gui.LibraryTree.remove_View_from_Database = function(libraryId, viewId)
	{		
		var viewNode = $("#" + viewId);  
		var viewV3 = $.data[viewV3];	
		TM.WebServices.WS_Libraries.remove_View(
				libraryId, viewId,  function(result) 
					{ 											
						if (result)		
						{											
							viewNode.remove();							
							TM.WebServices.Data.AllViews.pop(viewV3);
							delete  $.data[viewV3];		
							TM.Gui.Dialog.alertUser('View Removed');
						}					
						else
							TM.Gui.Dialog.showUserMessage("it was not possible to remove the view:" + viewId);												
						TM.Events.onRemovedView(result, viewV3);	
					}); 		
	}		

TM.Gui.LibraryTree.add_Folder_to_Database = function(libraryId, folderId, title, callback, skip_rename)	
	{				
		var targetNode = isDefined(folderId)
							? $("#" + folderId)
							: $("#" + libraryId);
							
		var folderNode = TM.Gui.LibraryTree.add_Folder(targetNode,title,callback, skip_rename);
		
		folderNode.hide();		
		TM.WebServices.WS_Libraries.add_Folder(
			libraryId, 
			folderId,
			title, 
			function(folderV3) 
				{ 						
					if (folderV3 != null)		
					{					
						folderV3.type = "Folder";
						folderNode.fadeIn();
						TM.WebServices.Data.AllFolders.push(folderV3);
						$.data[folderV3.folderId]= folderV3;
						$.data[folderV3.folderId].parentId = folderId;
						folderNode.attr("id", folderV3.folderId);
						TM.Gui.Dialog.alertUser('Folder Added');
					}					
					else
						TM.Gui.Dialog.showUserMessage("it was not possible to create the folder:" + title);					
					TM.Events.onNewFolder(folderV3);										}); 

		return folderNode;		
	}	

TM.Gui.LibraryTree.add_View_to_Database = function(libraryId, folderId, viewName, callback, skip_rename)	
	{				
		var targetNode = isDefined(folderId)
							? $("#" + folderId)
							: $("#" + libraryId);
		
		var viewNode = TM.Gui.LibraryTree.add_View(targetNode, viewName, callback, skip_rename);
		viewNode.hide();		
		TM.WebServices.WS_Libraries.add_View(
			libraryId, 
			folderId,
			viewName, 
			function(viewV3) 
				{ 						
					if (viewV3 != null)		
					{					
						viewV3.type = "View";
						viewV3.name = viewV3.caption;
						viewNode.fadeIn();
						TM.WebServices.Data.AllViews.push(viewV3);
						$.data[viewV3.viewId]= viewV3;
						viewNode.attr("id", viewV3.viewId);
						TM.Gui.LibraryTree.setDraggableOptionsForView(viewNode, viewV3);						
						TM.Gui.Dialog.alertUser('View added: {0}'.format(viewV3.caption));
						_viewNode = viewNode
						
						
						// this is a work around the current issue of not being able to drag into created views
						TM.Gui.LibraryTree.jsTree.deselect_all()
						TM.Gui.LibraryTree.selectNode(_viewNode)
						TM.Events.onLibraryTreeSelected();
					}					
					//else
					//	TM.Gui.Dialog.showUserMessage("it was not possible to create the view:" + viewName);					
					TM.Events.onNewView(viewV3);						
				}); 

		return viewNode;		
	}		

TM.Gui.LibraryTree.rename_Library_to_Database = function(libraryId, newName)
	{
		var libraryNode = $("#"+libraryId);		
		//libraryNode.hide();		
		TM.WebServices.WS_Libraries.rename_Library(libraryId, newName 
			  , function(result) 
				{ 	
					var libraryV3 =$.data[libraryId];
					if (result != null)		
					{			
						TM.Gui.LibraryTree.title(libraryNode, newName);
						libraryNode.hide().fadeIn();						
						libraryV3.name = newName;	
						TM.Gui.Dialog.alertUser('Library Renamed');
					}					
					//else
					//	TM.Gui.Dialog.showUserMessage("it was not possible to rename the library:" + title);					
					TM.Events.onRenamedLibrary(libraryV3);						
				},
				function(error)
				{
					_libraryId = libraryId;
					TM.Gui.LibraryTree.jsTree.rename_node(libraryNode,$.data[_libraryId].name)
					TM.WebServices.Helper.defaultErrorHandler(error);
				}); 
		return libraryNode;
	}

TM.Gui.LibraryTree.rename_Folder_to_Database = function(libraryId, folderId, newName)
	{
		var folderNode = $("#"+folderId);
		folderNode.hide();		
		TM.WebServices.WS_Libraries.rename_Folder(libraryId, folderId, newName,
			  function(result) 
				{ 	
					var folderV3 =$.data[folderId];
					if (result != null)		
					{			
						TM.Gui.LibraryTree.title(folderNode, newName);
						folderNode.fadeIn();						
						folderV3.name = newName;	
						TM.Gui.Dialog.alertUser('Folder Renamed');
					}					
					//else
					//	TM.Gui.Dialog.showUserMessage("it was not possible to rename the library:" + title);					
					TM.Events.onRenamedFolder(folderV3);						
				}); 
		return folderNode;
	}

TM.Gui.LibraryTree.rename_View_to_Database = function(libraryId, folderId, viewId, newName)
	{
		var viewNode = $("#"+viewId);
		viewNode.hide();		
		TM.WebServices.WS_Libraries.rename_View(libraryId, folderId, viewId, newName,
			   function(result) 
				{ 	
					var viewV3 =$.data[viewId];
					if (result != null)		
					{			
						TM.Gui.LibraryTree.title(viewNode, newName);
						viewNode.fadeIn();						
						viewV3.caption = newName;	
						TM.Gui.Dialog.alertUser("View renamed to: {0}".format(viewV3.caption));
					}					
					//else
					//	TM.Gui.Dialog.showUserMessage("it was not possible to rename the library:" + title);					
					TM.Events.onRenamedView(viewV3);						
				}); 
		return viewNode;
	}	

	
//**********************	
//Admin/Edit mode 
//**********************
//Drag & Drop

TM.Gui.LibraryTree.setDraggableOptionsForView = function(node, nodeData)
	{
		$(node).draggable(
						{
							helper:'clone',
							cursorAt: {left: -20, top: -20},
							start : function() { TM.Gui.draggedData = nodeData ; TM.dragMode = 'view'; }
						})  
	}
	

// Create Context menu
TM.Gui.LibraryTree.createContextMenu = function(node)
{					
	updatedNodeId = false;
	createMode = "";
	var items = {};
	
	if (TM.Gui.CurrentUser.isEditor() === false)
		return items;
	if (TM.Gui.editMode === false)
	{
		items = 
			{	
				showEditMode: 	{ label: "Open Edit Mode",  action: TM.Gui.showEditMode }
			}
		return items;
	}
	contextMenuIdValue = $.data[node.attr('id')];	
	
	if (typeof(contextMenuIdValue) == "undefined")
		return items;
	var nodeType = contextMenuIdValue.__type;	
	
	
	var addLibrary = function()
		{			
			TM.Gui.LibraryTree.add_Library_to_Database("New_Library" + "".add_Random().slice(0,5), undefined, false);				
		}
		
	var removeLibrary = function(parentNode)
		{			
			var nodeId = parentNode.id();
			var nodeData = $.data[nodeId];
			var description = "library: '{0}'".format(nodeData.name);
			TM.Gui.Dialog.deleteConfirmation(description, function() 
				{ 				
					var nodeId = parentNode.id();
					TM.Gui.LibraryTree.remove_Library_from_Database(nodeId)
				});
		}			
	
	var addFolder = function(parentNode)
		{				
			var nodeId = parentNode.id();
			var nodeData = $.data[nodeId];			
			var libraryId = nodeData.libraryId;
			var folderId = nodeData.folderId;						
			TM.Gui.LibraryTree.add_Folder_to_Database(libraryId, folderId, "new folder", undefined, false);			
		}
		
	var removeFolder = function(parentNode)
		{						
			var nodeId = parentNode.id();
			var nodeData = $.data[nodeId];			
			var libraryId = nodeData.libraryId;
			var folderId = nodeData.folderId;			
			
			//confirm before deletion 
			var description = "folder: '{0}'".format(nodeData.name);						
			TM.Gui.Dialog.deleteConfirmation(description, function() 
				{ 
					TM.Gui.LibraryTree.remove_Folder_from_Database(libraryId,folderId)
				} );
			
		}				
		
	var addView = function(parentNode)
		{				
			var nodeId = parentNode.id();
			var nodeData = $.data[nodeId];
			var libraryId = nodeData.libraryId;
			var folderId = nodeData.folderId;						
			TM.Gui.LibraryTree.add_View_to_Database(libraryId, folderId, "new view", undefined, false);						
		}	
		
	var removeView = function(parentNode)
		{						
			var nodeId = parentNode.id();
			var nodeData = $.data[nodeId];			
			var libraryId = nodeData.libraryId;
			//var folderId = nodeData.folderId;	
			var viewId = nodeData.viewId;						
			var description = "view: '{0}'".format(nodeData.caption);
			TM.Gui.Dialog.deleteConfirmation(description, function() 
				{ 
					TM.Gui.LibraryTree.remove_View_from_Database(libraryId, viewId)
				});
		}				


	if (nodeType == "SecurityInnovation.TeamMentor.WebClient.Library_V3")		
		items = 
			{	
				createFolderItem: 	{ label: "Add View",  action: addView },
				createViewItem: 	{ label: "Add Folder",  action: addFolder },
				//createViewItem: 	{ label: "Add Folder",  action: function (obj) 	{ createMode = "Folder" ; this.create(obj); } },
				//createFolderItem: 	{ label: "Add View",  action: function (obj) 	{ createMode = "View" ;   this.create(obj); } },
				createGuidanceItem: { label: "Add Guidance Item",  action: TM.Gui.LibraryTree.newGuidanceItem },//function (obj) { newGuidanceItem(); } }, //this.create(obj); } },
				renameItem: { label: "Rename Library",  action: function (obj) { this.rename(obj); } },					
				deteteItem: { label: "Delete Library",  action: removeLibrary},     //function (obj) { this.remove(obj); } },
				separatorItem: { label: "-----------",  action: function (obj) { } },
				newLibItem: { label: "New Library",  action: function (obj) { addLibrary(); } }
				//reloadItem: { label: "Reload Data",  action: function (obj) { refreshLibraryView(); } }													
			}
	else if (nodeType == "SecurityInnovation.TeamMentor.WebClient.Folder_V3")		
		items = 
			{				
				createViewItem: 	{ label: "Add View",  action: addView },
				createFolderItem: 	{ label: "Add Folder",  action: addFolder },
				renameItem: { label: "Rename Folder",  action: function (obj) { this.rename(obj); } },
				deteteItem: { label: "Delete Folder",  action: removeFolder }				
			}
	else if (nodeType == "SecurityInnovation.TeamMentor.WebClient.View_V3")
		items = 
			{				
				createItem: { label: "Add Guidance Item",  action:  TM.Gui.LibraryTree.newGuidanceItem },//function (obj) { newGuidanceItem(); } }, //this.create(obj); } },
				renameItem: { label: "Rename View",  action: function (obj) { this.rename(obj); } },
				deteteItem: { label: "Delete View",  action: removeView }				
			}
			
	/*else if (nodeType == "guidanceItem")				
		items = 
			{									
				renameItem: { label: "Rename Guidance Item",  action: function (obj) { this.rename(obj); } },
				deteteItem: { label: "Delete Guidance Item",  action: function (obj) { this.remove(obj); } }
			}*/
	else
	{
		showUserMessage('not supported nodeType: ' + nodeType);
	}
	items.separator2Item = { label: "-----------",  action: function (obj) { } };
	items.userMode = { label: "Exit Edit Mode",  action: TM.Gui.showUserMode};
	
	return items;
}

TM.Gui.LibraryTree.dropActions = 
	{	
			dropOk				: undefined
		,	dropNotOk			: undefined
		,	iconOffset_Top		: 0
		,	iconOffset_Left		: -20
		, 	currentTargetNode	: undefined
		,	setUp				: function()
									{
										var that = TM.Gui.LibraryTree.dropActions;
										that.dropOk = $("<div>").attr('id','dropOk').appendTo('body').absolute().height(18).width(18).zIndex(100)
										that.dropOk.css("background" , "url('/javascript/jQuery.jsTree/themes/default/d.png') -2px -53px no-repeat !important;" ).width(18).height(18)
										
										that.dropNotOk = $("<div>").attr('id','dropNotOk').appendTo('body').absolute().height(18).width(18).zIndex(100)
										that.dropNotOk.css("background" , "url('/javascript/jQuery.jsTree/themes/default/d.png') -18px -53px no-repeat !important;" ).width(18).height(18)
										
										that.hide_DropIcons();
									}
									
		, 	hide_DropIcons		: function()						
									{
										var that = TM.Gui.LibraryTree.dropActions;
										that.dropOk.hide();
										that.dropNotOk.hide();
									}
		, 	show_DropIcon		: function(targetNode, dropIcon)
									{
										
										if(isDefined(targetNode))
										{
											var that = TM.Gui.LibraryTree.dropActions;
											that.hide_DropIcons();
											dropIcon.show();
											dropIcon.top ($(targetNode).offset().top + that.iconOffset_Top)
											dropIcon.left($(targetNode).offset().left + that.iconOffset_Left)
										}
									}
		, 	show_DropOk			: function(targetNode)
									{								
										var that = TM.Gui.LibraryTree.dropActions;
										that.show_DropIcon(targetNode, that.dropOk)	
									}
		, 	show_DropNotOk		: function(targetNode)
									{								
										var that = TM.Gui.LibraryTree.dropActions;
										that.show_DropIcon(targetNode, that.dropNotOk)	
										
										//open the node after 1 second (if still the same)
										that.currentTargetNode = targetNode
										setTimeout(function()
											{
												if(that.currentTargetNode == targetNode)											
												{													
													TM.Gui.LibraryTree.jsTree.open_node(targetNode);
												}											
											}, 1000);
									}
									
	}





// RENAME event
TM.Gui.LibraryTree.onRename = function(event, data)
{		
	var rename_newData = data.rslt.new_name;
	//_targetNodeData = eval("( {0} )".format(data.rslt.obj.attr("id")));
	
	var targetNodeData = $.data[data.rslt.obj.attr("id")];	
	if (isDefined(targetNodeData))
	{		
		if (targetNodeData.__type == "SecurityInnovation.TeamMentor.WebClient.Library_V3")
		{			
			TM.Gui.LibraryTree.rename_Library_to_Database(targetNodeData.libraryId, rename_newData);			
		}		
		if (targetNodeData.__type == "SecurityInnovation.TeamMentor.WebClient.Folder_V3")
		{			
			TM.Gui.LibraryTree.rename_Folder_to_Database(targetNodeData.libraryId, targetNodeData.folderId, rename_newData);			
		}
		if (targetNodeData.__type == "SecurityInnovation.TeamMentor.WebClient.View_V3")
		{			
			TM.Gui.LibraryTree.rename_View_to_Database(targetNodeData.libraryId, targetNodeData.folderId, targetNodeData.viewId, rename_newData);			
		}				
	}	
}

// CREATE event
TM.Gui.LibraryTree.onCreate = function(event, data)
{			
	data.rslt.new_name = data.rslt.name;		// to make it compatible with onCreate		
	TM.Gui.LibraryTree.onRename(event, data);   // the node is already there so we only have to deal with the rename		
}
	
// NEW GUIDANCE ITEM

TM.Gui.LibraryTree.newGuidanceItem = function()
{            

	var closeNewGuidanceItemDialog = function()
	{		
		"ui-dialog-titlebar-close".$().click();				
	}     
	
	var createNewGuidanceItem = function()
	  { 				
			var title = "New Guidance Item";
			var htmlContent = "";
			createGuidanceItem(title,htmlContent, contextMenuIdValue.libraryId, 
				function(data) 
					{						
						var newGuidanceItemId = data.d;
						if (typeof(contextMenuIdValue.viewId) != "undefined")
						{
							
							var viewId = contextMenuIdValue.viewId;							
							addGuidanceItemToView(viewId, newGuidanceItemId, 	
								function() {													
												$.data[viewId].guidanceItems.push(newGuidanceItemId);
										   } );
														
						}
						
						$.data[newGuidanceItemId] = {};						
						$.data[newGuidanceItemId].guidanceItemId = newGuidanceItemId;
						$.data[newGuidanceItemId].libraryId 	 = contextMenuIdValue.libraryId;
						$.data[newGuidanceItemId].title 		 = title;
						$.data[newGuidanceItemId].technology 	 = "";
						$.data[newGuidanceItemId].phase 	 	 = "";
						$.data[newGuidanceItemId].type 	 		 = "";
						$.data[newGuidanceItemId].category 		 = "";
						$.data[contextMenuIdValue.libraryId].guidanceItems.push(newGuidanceItemId);
												
						openGuidanceItemEditor(newGuidanceItemId);
					});
	  } ;			  	  	
	  createNewGuidanceItem();
}	

