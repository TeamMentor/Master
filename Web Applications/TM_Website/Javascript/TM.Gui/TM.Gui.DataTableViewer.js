TM.Gui.DataTableViewer = 
	{
			applyCss				: function()
										{
											$("#nowShowingText").css(
												{ 
														"text-align"	: "center"
													,	"font-weight"	: "bold" 
												});
										}
										
		,	set_Title				: function(text)
										{
											$("#nowShowingText").html(text);
										}
										
		, 	addCheckBoxesToDataTable : function(dataTable)
										{
											if(TM.Gui.editMode)
											{			
												$.each(dataTable.aaData, function() 
													{   
														this[0] = "<input type='checkbox' class='GuidanceItemCheckBox' style='text-align: center'/>"
													});
											}											
										}					
	}

	
TM.Gui.DataTableViewer.setDragAndDropOptions = function()
	{	
		// for main Table  (when dragging guidance items into views)
			
		$("#guidanceItemsTable tr").draggable(
			{ 
				//helper:'clone', 
				helper: getDragHelperElement,
				revert: false , 
				cursor: "arrow" , 
				revertDuration: 250, 			
				cursorAt: {left: -20, top: -20},
				zIndex: 10,
				appendTo: 'body', 
				start : function() { TM.dragMode = 'guidanceItem';}
				 //containment: 'DOM',
				 //zIndex: 1500
				// addClasses: false
				//opacity: 0.35
			});
			
		$("#guidanceItemsTable input").draggable(
			{ 
				helper:'clone', 
				//helper: getDragHelperElement,
				revert: false , 
				cursor: "arrow" , 
				revertDuration: 250, 			
				cursorAt: {left: -20, top: -20},
				zIndex: 10,
				appendTo: 'body', 
				start : function() { TM.dragMode = 'guidanceItem';}
				 //containment: 'DOM',
				 //zIndex: 1500
				// addClasses: false
				//opacity: 0.35
			});	

		//for library Tree when dragging views
		$(".LibraryTree a").each(function() 
			{ 
				if (typeof($(this).parent().attr('id')) != 'undefined')
				{
					var id = $(this).parent().attr('id');
					var nodeData = $.data[id];				      
					//if( $(this).parent().attr('id').indexOf("type: 'view'") != -1) 
					if (nodeData.__type === "SecurityInnovation.TeamMentor.WebClient.View_V3")
					{ 			
						TM.Gui.LibraryTree.setDraggableOptionsForView(this, nodeData);
						//$(this)
					} 
					/*if( $(this).parent().attr('id').indexOf("type: 'folder'") != -1) 
					{ 
						$(this).draggable(
							{
								helper:'clone',
								cursorAt: {left: -20, top: -20},
								start : function() { TM.dragMode = 'folder'; }
							})  
					}*/
				}
			});
		
		var addGuidanceItemsToParentObject = function(parentId, guidanceIds)
		{
			parentData = $.data[parentId];
			if(isDefined(parentData) && parentData.__type == "SecurityInnovation.TeamMentor.WebClient.Folder_V3")
			{			
				parentData.guidanceItems  = parentData.guidanceItems.concat(guidanceIds);			
				addGuidanceItemsToParentObject(parentData.parentId, guidanceIds);
			}
		}
		
		var addGuidanceItemsToLocalViewObject = function(viewId, guidanceIds)
		{		
			var viewData = $.data[viewId];
			if(isDefined(viewData))
			{
				viewData.guidanceItems  = viewData.guidanceItems.concat(guidanceIds);			
				var parentId = (viewData.folderId === TM.Const.emptyGuid)
									? viewData.libraryId
									: viewData.folderId;
				addGuidanceItemsToParentObject(parentId, guidanceIds);
			}		
		}
		
		var modeViewToFolderOrLibrary = function(viewId, targetId)
		{
			var sourceNode = $("#" + viewId);
			var targetNode = $("#" + targetId);
			sourceNode.fadeOut();
			TM.Gui.LibraryTree.jsTree.move_node(sourceNode, targetNode);
			sourceNode.fadeIn();
		}
		
		var libraryTree_OnOut = function( event, ui ) 
			{							
				TM.Gui.LibraryTree.dropActions.hide_DropIcons()	
			}
			
		var libraryTree_OnOver = function(event, nodeData, dragMode)
		{	
			var targetNode = event.target;
							
			if (isUndefined(nodeData))
			{
				console.log("ERROR: nodeData not defined");
				return;
			}
			
			TM.Gui.LibraryTree.dropActions.show_DropNotOk(targetNode)
			
			if (TM.Gui.selectedNodeData.libraryId == nodeData.libraryId)
			{			
				if (dragMode == 'guidanceItem')
				{								
					if (nodeData.__type=="SecurityInnovation.TeamMentor.WebClient.View_V3")
						TM.Gui.LibraryTree.dropActions.show_DropOk(targetNode)					
										
				}
				else if (dragMode == 'view')// || dragMode == 'folder')
				{				
					if (nodeData.__type=="SecurityInnovation.TeamMentor.WebClient.Folder_V3" || 
						nodeData.__type=="SecurityInnovation.TeamMentor.WebClient.Library_V3")
					{
						TM.Gui.LibraryTree.dropActions.show_DropOk(targetNode)
					}					
				}			
			}
		}
		
		var libraryTree_OnDrop = function(event, nodeData, dragMode)
		{		
			if (isUndefined(nodeData))
			{
				console.log("ERROR: nodeData not defined");
				return;
			}
			
			if (dragMode == 'view' )
			{						
				if (TM.Gui.selectedNodeData.libraryId == nodeData.libraryId)
				{
					if (nodeData.__type=="SecurityInnovation.TeamMentor.WebClient.Folder_V3")
						moveViewToFolder(TM.Gui.draggedData.viewId, nodeData.folderId,
							function() {	
											modeViewToFolderOrLibrary(TM.Gui.draggedData.viewId, nodeData.folderId);									
											
											TM.Gui.Dialog.alertUser('Moved view into folder');
											
										});
					if (nodeData.__type=="SecurityInnovation.TeamMentor.WebClient.Library_V3")
						moveViewToFolder(TM.Gui.draggedData.viewId, nodeData.libraryId,
							function() {	
											modeViewToFolderOrLibrary(TM.Gui.draggedData.viewId, nodeData.libraryId);									
											
											TM.Gui.Dialog.alertUser('Moved view into library');
											
										});
				}
				return;			
			}
			
			if (nodeData.__type=="SecurityInnovation.TeamMentor.WebClient.View_V3" && 
				TM.Gui.selectedNodeData.libraryId == nodeData.libraryId)				
			{						
				addGuidanceItemToViews(nodeData.viewId, selectedGuidanceIds, 
					function(result) 
						{
							if (result)
							{
								addGuidanceItemsToLocalViewObject(nodeData.viewId, selectedGuidanceIds)
								TM.Gui.Dialog.alertUser('Added {0} Guidance Items to view: {1}'.format(selectedGuidanceIds.length, nodeData.caption) );
							}
							
						} );						
			}
		}	
			
		$(".LibraryTree a").droppable( 
			{
				tolerance : 'pointer',
				drop: function( event, ui ) 
					{				
						libraryTree_OnOut();
						var droppedNodeData = $.data[$(event.target.parentNode).attr('id')];					
						TM.Gui.LibraryTree.droppedNodeData = droppedNodeData;
						libraryTree_OnDrop(event, droppedNodeData, TM.dragMode);									
					}  ,
				out: libraryTree_OnOut  ,
				over: function( event, ui ) 
					{ 									
						setTimeout(function()
							{
								var overNodeData = $.data[$(event.target.parentNode).attr('id')];	
								TM.Gui.LibraryTree.overNodeData = overNodeData;
								libraryTree_OnOver(event, overNodeData, TM.dragMode)					
							});
					}  
			} )	
	}	
