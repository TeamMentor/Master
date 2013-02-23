
//$.jstree.defaults.themes.url = "/javascript/jQuery.jsTree/themes/default/style.css";

window.TM.Gui.LibraryTree =    {
        // variables
        version     : 1 ,
        plugins     : [ "themes", "json_data" , "ui", "crrm", "contextmenu"] , //  ,  "hotkeys"
        treeData    : undefined,
        targetDiv   : undefined,
        jsTree      : undefined,
        
        // events
        onTreeCreated:      function() { } ,
        onTreeLoaded:       function() { } ,
        onSelectedNode:     function() { } ,
        
        
        // methods
        open:                       function(_targetDiv)    {
                                                                this.targetDiv = _targetDiv;
                                                                return this;
                                                            },
        create_EmptyTree:           function()              {
                                                                this.jsTree = undefined;
                                                                this.treeData = { data: [] };
                                                                this.create_Tree();
                                                            },
        create_TreeUsingJSON:       function()
                                {
                                    var that = this,
                                        startTime = new Date();
                                    this.jsTree = undefined;
                                    
                                    this.onTreeCreated = function() 
                                        {
                                            window.TM.Debug.TimeSpan_Gui_LibraryTree_CreatedTreeFromJsonData = startTime.toNow();
                                            that.onTreeLoaded();
                                            window.TM.Events.onLibraryTreeLoaded();
                                        };
                                        
                                    this.loadJsonData(function() 
                                        {
                                            that.create_Tree();
                                        } );
                                },
        
        create_Tree:                function()
                                {															
                                    var options = { 
                                                      json_data     : this.treeData,
                                                      plugins       : this.plugins,
                                                      themes		: { url: '/javascript/jQuery.jsTree/themes/default/style.css'},
                                                      contextmenu   : { items: window.TM.Gui.LibraryTree.createContextMenu },
                                                      ui            : { "select_limit" : 1 }
                                                      //,
/*													  "dnd"			: { "drag_check": function() { alert('drag check') } ,
                                                                        "drag_finish": function() { alert('drag finish') }, 
                                                                        "drop_finish": function() { alert('drop finish') } 
                                                                      }*/
                                        },
                                        that    = this;
                                
                                    $(this.targetDiv).bind("loaded.jstree", function (event, data)  {
                                                                                                        that.jsTree = data.inst;
                                                                                                        that.onTreeCreated();
                                                                                                    });
                                    
                                    //create tree
                                    $(this.targetDiv).jstree(options)
                                                     .delegate("a", "click", this.setSelectedId)
                                                     .delegate("a", "click", function() { that.onClick(); } );
                                    
                                    //bind renamema nd create events
                                    $(this.targetDiv)
                                          .bind("rename.jstree",        window.TM.Gui.LibraryTree.onRename)
                                          .bind("create.jstree",        window.TM.Gui.LibraryTree.onCreate);
                                          
                                    //setup drag&drop actions and icons 
                                    window.TM.Gui.LibraryTree.dropActions.setUp();
                                    
                                },
                                
        setSelectedId:              function (event)
                                {									
                                    //_event = event;									
                                    //var node = $(event.srcElement);	// doesn't work in FF
                                    var node = $(event.target);									
                                    window.TM.Gui.LibraryTree.selectNode(node);
                                },
                                
        onClick :                   function (event, data)      { },
                                
        loadJsonData:               function(callback) {
                                                            var that = this;
                                                            window.TM.WebServices.WS_Data.getJsTreeWithFolders(
                                                                function(data)
                                                                    {
                                                                        that.treeData = data;
                                                                         callback();
                                                                    });
                                                        },
                                            
        add_LibrariesFromWsData:    function()
                                        {
                                            var startTime = new Date(),
                                                that = this;
                                            $.each(window.TM.WebServices.Data.AllLibraries, function()
                                                    { 
                                                        that.add_LibraryFromWsData(this);  
                                                    });		
                                            window.TM.Debug.TimeSpan_Gui_LibraryTree_CreatedTreeFromWsData = startTime.toNow();
                                            this.onTreeLoaded();
                                        },
                                        
        add_LibraryFromWsData:      function(library)
                                        {
                                            var libraryNode = this.add_Library(library.name);
                                            libraryNode.attr('id', library.id);
                                            libraryNode.attr('type', library.__type);
                                            this.add_FoldersFromWsData	(libraryNode, library.subFolders);
                                            this.add_ViewsFromWsData	(libraryNode, library.views);
                                        },
                                        
        add_FoldersFromWsData:      function(rootNode, folders)
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
        add_ViewsFromWsData:        function(rootNode, views)
                                        {											
                                            $.each(views, function() 
                                                    { 
                                                        var folderNode = rootNode.add_View(this.caption);
                                                        folderNode.attr('id', this.id);
                                                        folderNode.attr('type', this.__type);
                                                        //that.add_LibraryFromWsData(this);  
                                                    });		
                                        }
        
                                
    };

//*: Show Tree
window.TM.Gui.LibraryTree.showTree = function()    {
        var applyJsTreeCssPatches = function()  {
                                                    $('.jstree-default.jstree-focused').css('background-color','#FFFFFF');
                                                },
            libraryTree           = window.TM.Gui.LibraryTree.open("#libraryJsTree");

        $("#libraryJsTree").html('...loading tree...');

        libraryTree.onTreeLoaded = function()
            {
                applyJsTreeCssPatches();

                $(libraryTree.targetDiv).delegate("a", "click",
                    function (event, data)
                        {
                            window.TM.Events.onLibraryTreeSelected();
                        });

                window.TM.Gui.LibraryTree.selectNode_ById(window.TM.Gui.Main.Panels.initialId);
            };
        libraryTree.create_TreeUsingJSON();
    };

//*: nodes manipulation methods
window.TM.Gui.LibraryTree.nodes             = function()            {
        return $(window.TM.Gui.LibraryTree.targetDiv + " ul li");
    };
window.TM.Gui.LibraryTree.title             = function(node, value) {
        if(value !== undefined) //isDefined(value))
        {
            window.TM.Gui.LibraryTree.jsTree.set_text(node, value);
            return node;
        }
        return window.TM.Gui.LibraryTree.jsTree.get_text(node);
    };
window.TM.Gui.LibraryTree.firstNode         = function()            {
        return window.TM.Gui.LibraryTree.nodes().first();
    };
window.TM.Gui.LibraryTree.selectNode        = function(node)        {
        //alert(TM.Gui.LibraryTree.selectedNode === node)
        
        if (node !==undefined) // isDefined(node))
        {					
            var selectedNodeId = $(node).attr('id');
            if (selectedNodeId === undefined || selectedNodeId==="") //isUndefined(selectedNodeId) || selectedNodeId==="")
            {
                selectedNodeId = $(node).parent().attr('id');
            }
            if (selectedNodeId !== undefined) //isDefined(selectedNodeId))
            {			
                if(selectedNodeId !== window.TM.Gui.selectedNodeId)
                {
                    window.TM.Gui.LibraryTree.jsTree.deselect_all();
                    window.TM.Gui.selectedNodeId = selectedNodeId;
                    window.TM.Gui.selectedNodeData = $.data[selectedNodeId];
                    window.TM.Gui.LibraryTree.jsTree.select_node(node);
                    window.TM.Gui.LibraryTree.selectedNode = node;
                    window.TM.Gui.LibraryTree.onSelectedNode(node);
                }
            }
            else
            {
                window.console.log("selectedNodeId was not defined");
            }
        }
        return node;
    };
window.TM.Gui.LibraryTree.selectNode_ById   = function(guid)        {
                                                                    if (guid===undefined) // isDefined(guid) === false)
                                                                    {
                                                                        return;
                                                                    }
                                                                    try
                                                                    {
                                                                        var firstNode,
                                                                            nodeToSelect = $(window.TM.Gui.LibraryTree.targetDiv + " ul li[id='" + window.htmlEscape(guid) + "']");
                                                                        if (nodeToSelect.size() === 1)
                                                                        {
                                                                            window.TM.Gui.LibraryTree.selectNode(nodeToSelect);
                                                                        }
                                                                        else
                                                                        {
                                                                            nodeToSelect = $(window.TM.Gui.LibraryTree.targetDiv + " ul li a:contains('" + window.htmlEscape(guid)+ "')");
                                                                            if (nodeToSelect.size() === 1)
                                                                            {
                                                                                window.TM.Gui.LibraryTree.selectNode(nodeToSelect);
                                                                            }
                                                                            else
                                                                            {
                                                                                firstNode = window.TM.Gui.LibraryTree.selectFirstNode();
                                                                                window.TM.Gui.LibraryTree.openNode(firstNode);
                                                                            }
                                                                        }
                                                                        window.TM.Events.onLibraryTreeSelected();

                                                                    }
                                                                    catch (e)
                                                                    {
                                                                        window.TM.Gui.Dialog.alertUser(e.message, "in window.TM.Gui.LibraryTree.selectNode_ById");
                                                                    }

                                                                };
window.TM.Gui.LibraryTree.selectNode_ByName = function(name)        {
                                                                    var guid = window.TM.WebServices.Data.id_ByName(name);
                                                                    window.TM.Gui.LibraryTree.selectNode_ById(guid);
                                                                };
window.TM.Gui.LibraryTree.openNode          = function(node)        {
                                                                    window.TM.Gui.LibraryTree.jsTree.open_node(node);
                                                                    return node;
                                                                };
window.TM.Gui.LibraryTree.selectFirstNode   = function()            {
                                                                    var firstNode = window.TM.Gui.LibraryTree.firstNode();
                                                                    window.TM.Gui.LibraryTree.selectNode(firstNode);
                                                                    return firstNode;
                                                                };

//*: Node creation methods: These methods make no connection to the database (i.e. are purely GUI driven)
window.TM.Gui.LibraryTree.add_Node          = function(targetNode, title, icon, callback, skip_rename)      {
        if (skip_rename === undefined) //isUndefined(skip_rename))
        {
            skip_rename = true;
        }
        var newNode = window.TM.Gui.LibraryTree.jsTree
                            .create(
                                        targetNode, 
                                        "last",
                                        {
                                            data: {
                                                    title:title , 
                                                    icon:  icon
                                                 }
                                        } , 
                                        callback , 
                                        skip_rename
                                    );
        newNode.title = function(value) { return window.TM.Gui.LibraryTree.title(newNode, value); };
        return newNode;
    };
window.TM.Gui.LibraryTree.add_Library       = function(title, callback, skip_rename)                        {
        var libraryNode = window.TM.Gui.LibraryTree.add_Node("",title, '/Images/SingleLibrary.png' , callback, skip_rename);
        libraryNode.add_Folder = function(title, callback, skip_rename) 
            { 
                return window.TM.Gui.LibraryTree.add_Folder(libraryNode, title, callback, skip_rename);
            };
        libraryNode.add_View = function(title, callback, skip_rename) 
            { 
                return window.TM.Gui.LibraryTree.add_View(libraryNode, title, callback, skip_rename);
            };	
            
        //libraryNode.title = function(value) { return window.TM.Gui.LibraryTree.title(libraryNode, value) };
        
        return libraryNode;
    };
window.TM.Gui.LibraryTree.add_Folder        = function(targetNode, title, callback, skip_rename)            {
        var folderNode = window.TM.Gui.LibraryTree.add_Node(targetNode,title, '/Images/FolderIcon.png' , callback, skip_rename);
        folderNode.add_Folder = function(title, callback, skip_rename) 
            { 
                return window.TM.Gui.LibraryTree.add_Folder(folderNode, title, callback, skip_rename);
            };
        folderNode.add_View = function(title, callback, skip_rename) 
            { 
                return window.TM.Gui.LibraryTree.add_View(folderNode, title, callback, skip_rename);
            };	
        return folderNode;
    };
window.TM.Gui.LibraryTree.add_View          = function(targetNode, title, callback, skip_rename)            {
        return window.TM.Gui.LibraryTree.add_Node(targetNode,title, '/Images/ViewIcon.png' , callback, skip_rename);
    };

//*: Node creation methods: These methods are the ones that make commits to the database and the GUI
window.TM.Gui.LibraryTree.add_Library_to_Database       = function(title, callback, skip_rename)            {
        var libraryNode = window.TM.Gui.LibraryTree.add_Library(title, callback, skip_rename);
        libraryNode.hide();

        window.TM.WebServices.WS_Libraries.add_Library(
            title,
            function(libraryV3) {
                                    if (libraryV3 !== null)
                                    {
                                        libraryV3.type = "Library";
                                        libraryNode.fadeIn();
                                        window.TM.WebServices.Data.AllLibraries.push(libraryV3);
                                        $.data[libraryV3.libraryId] = libraryV3;
                                        libraryNode.attr("id", libraryV3.libraryId);

                                        window.TM.Gui.Dialog.alertUser('Library Created');
                                    }
                                    else
                                    {
                                        window.TM.Gui.Dialog.alertUser("It was not possible to create the library called : " + title, 'Library creation error'  );
                                    }
                                    window.TM.Gui.LibraryTree.lastLibraryCreated = libraryV3;
                                    window.TM.Events.onNewLibrary();
                                });
        return libraryNode;
    };
window.TM.Gui.LibraryTree.remove_Library_from_Database  = function(libraryIdOrName)                         {
        var libraryId = libraryIdOrName,
            libraryV3 = $.data[libraryId],
            libraryNode;

        if (libraryV3 === undefined)    //isUndefined(libraryV3))
        {			
            libraryV3 = window.TM.WebServices.Data.library(libraryIdOrName);
            if (libraryV3 === undefined) // isUndefined(libraryV3))
            {
                window.TM.Gui.Dialog.showUserMessage("In remove_Library_from_Database, could not find the library to remove:" + libraryIdOrName);
            }
            libraryId = libraryV3.libraryId;
        }		
        
        libraryNode = $("#" + libraryId);
        
        if (libraryNode.length !== 1 || libraryV3 === undefined) //isUndefined(libraryV3))
        {
            window.TM.Gui.Dialog.showUserMessage("something is wrong, the objects required to remove the library are not available: " + libraryId);
        }
        else
        {
            window.TM.WebServices.WS_Libraries.remove_Library(
                libraryId, function(result) 
                    {
                        if (result)
                        {											
                            libraryNode.remove();
                            window.TM.WebServices.Data.AllLibraries.pop(libraryV3);
                            delete $.data[libraryV3.libraryId];
                            window.TM.Gui.Dialog.alertUser('Library Deleted');
                            window.TM.Gui.LibraryTree.lastLibraryRemoved = libraryV3;
                            window.TM.Events.onRemovedLibrary();
                        }
                        else
                        {
                            window.TM.Gui.Dialog.showUserMessage("it was not possible to remove the library:" + libraryId);
                        }
                        
                    });
        }
    };
window.TM.Gui.LibraryTree.remove_Folder_from_Database   = function(libraryId, folderId)                     {
                                                                                            var folderNode = $("#" + folderId),
                                                                                                folderV3 = $.data[folderId];
                                                                                            window.TM.WebServices.WS_Libraries.remove_Folder(
                                                                                                    libraryId, folderId,  function(result)
                                                                                                        {
                                                                                                            if (result)
                                                                                                            {
                                                                                                                folderNode.remove();
                                                                                                                window.TM.WebServices.Data.AllFolders.pop();
                                                                                                                delete  $.data[folderId];
                                                                                                                window.TM.Gui.Dialog.alertUser('Folder Removed');

                                                                                                                window.TM.Gui.LibraryTree.lastFolderRemoved = folderV3;
                                                                                                                window.TM.Events.onRemovedFolder();
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                window.TM.Gui.Dialog.showUserMessage("it was not possible to remove the folder:" + folderId);
                                                                                                            }

                                                                                                        });
                                                                                        };
window.TM.Gui.LibraryTree.remove_View_from_Database     = function(libraryId, viewId)                       {
        var viewNode = $("#" + viewId),
            viewV3 = $.data[viewId];
        window.TM.WebServices.WS_Libraries.remove_View(
                libraryId, viewId,  function(result) 
                    {
                        if (result)
                        {											
                            viewNode.remove();							
                            window.TM.WebServices.Data.AllViews.pop(viewV3);
                            delete  $.data[viewV3];		
                            window.TM.Gui.Dialog.alertUser('View Removed');
                            window.TM.Gui.LibraryTree.lastViewRemoved = viewV3;
                            window.TM.Events.onRemovedView();
                        }
                        else
                        {
                            window.TM.Gui.Dialog.showUserMessage("it was not possible to remove the view:" + viewId);
                        }
                    });
    };
window.TM.Gui.LibraryTree.add_Folder_to_Database        = function(libraryId, folderId, title, callback, skip_rename)       {
        var targetNode = folderId !== undefined //isDefined(folderId)
                            ? $("#" + folderId)
                            : $("#" + libraryId),
                            
            folderNode = window.TM.Gui.LibraryTree.add_Folder(targetNode,title,callback, skip_rename);
        
        folderNode.hide();
        window.TM.WebServices.WS_Libraries.add_Folder(
            libraryId, 
            folderId,
            title, 
            function(folderV3) 
                {
                    if (folderV3 !== null)
                    {					
                        folderV3.type = "Folder";
                        folderNode.fadeIn();
                        window.TM.WebServices.Data.AllFolders.push(folderV3);
                        $.data[folderV3.folderId]= folderV3;
                        $.data[folderV3.folderId].parentId = folderId;
                        folderNode.attr("id", folderV3.folderId);
                        window.TM.Gui.Dialog.alertUser('Folder Added');
                    }
                    else
                    {
                        window.TM.Gui.Dialog.showUserMessage("it was not possible to create the folder:" + title);
                    }
                    window.TM.Gui.LibraryTree.lastFolderCreated = folderV3;
                    window.TM.Events.onNewFolder();
                }); 

        return folderNode;
    };
window.TM.Gui.LibraryTree.add_View_to_Database          = function(libraryId, folderId, viewName, callback, skip_rename)    {
        var targetNode = folderId !== undefined //isDefined(folderId)
                            ? $("#" + folderId)
                            : $("#" + libraryId),
            viewNode = window.TM.Gui.LibraryTree.add_View(targetNode, viewName, callback, skip_rename);

        viewNode.hide();
        window.TM.WebServices.WS_Libraries.add_View(
            libraryId, 
            folderId,
            viewName, 
            function(viewV3) 
                {
                    if (viewV3 !== null)
                    {
                        viewV3.type = "View";
                        viewV3.name = viewV3.caption;
                        viewNode.fadeIn();
                        window.TM.WebServices.Data.AllViews.push(viewV3);
                        $.data[viewV3.viewId]= viewV3;
                        viewNode.attr("id", viewV3.viewId);
                        window.TM.Gui.LibraryTree.setDraggableOptionsForView(viewNode, viewV3);
                        window.TM.Gui.Dialog.alertUser('View added: {0}'.format(viewV3.caption));
                        
                        
                        // this is a work around the current issue of not being able to drag into created views
                        window.TM.Gui.LibraryTree.jsTree.deselect_all();
                        window.TM.Gui.LibraryTree.selectNode(viewNode);
                        window.TM.Events.onLibraryTreeSelected();
                    }
                    //else
                    //	TM.Gui.Dialog.showUserMessage("it was not possible to create the view:" + viewName);					
                    window.TM.Gui.LibraryTree.lastViewCreated = viewV3;
                    window.TM.Events.onNewView();
                }); 

        return viewNode;
    };
window.TM.Gui.LibraryTree.rename_Library_to_Database    = function(libraryId, newName)                      {
        var libraryNode = $("#"+libraryId);
        //libraryNode.hide();
        window.TM.WebServices.WS_Libraries.rename_Library(libraryId, newName
              , function(result) {
                                    var libraryV3 =$.data[libraryId];
                                    if (result !== null)
                                    {
                                        window.TM.Gui.LibraryTree.title(libraryNode, newName);
                                        libraryNode.hide().fadeIn();
                                        libraryV3.name = newName;
                                        window.TM.Gui.Dialog.alertUser('Library Renamed');
                                    }
                                    //else
                                    //	TM.Gui.Dialog.showUserMessage("it was not possible to rename the library:" + title);
                                    window.TM.Gui.LibraryTree.lastLibraryRenamed = libraryV3;
                                    window.TM.Events.onRenamedLibrary();
                                },
                function(error)
                {
                    window.TM.Gui.LibraryTree.jsTree.rename_node(libraryNode,$.data[libraryId].name);
                    window.TM.WebServices.Helper.defaultErrorHandler(error);
                }); 
        return libraryNode;
    };
window.TM.Gui.LibraryTree.rename_Folder_to_Database     = function(libraryId, folderId, newName)            {
        var folderNode = $("#"+folderId);
        folderNode.hide();
        window.TM.WebServices.WS_Libraries.rename_Folder(libraryId, folderId, newName,
              function(result) 
                {
                    var folderV3 =$.data[folderId];
                    if (result !== null)
                    {			
                        window.TM.Gui.LibraryTree.title(folderNode, newName);
                        folderNode.fadeIn();						
                        folderV3.name = newName;	
                        window.TM.Gui.Dialog.alertUser('Folder Renamed');
                    }
                    //else
                    //	TM.Gui.Dialog.showUserMessage("it was not possible to rename the library:" + title);					
                    window.TM.Gui.LibraryTree.lastFolderRenamed = folderV3;
                    window.TM.Events.onRenamedFolder();
                }); 
        return folderNode;
    };
window.TM.Gui.LibraryTree.rename_View_to_Database       = function(libraryId, folderId, viewId, newName)    {
        var viewNode = $("#"+viewId);
        viewNode.hide();
        window.TM.WebServices.WS_Libraries.rename_View(libraryId, folderId, viewId, newName,
               function(result) 
                {
                    var viewV3 =$.data[viewId];
                    if (result !== null)
                    {			
                        window.TM.Gui.LibraryTree.title(viewNode, newName);
                        viewNode.fadeIn();						
                        viewV3.caption = newName;	
                        window.TM.Gui.Dialog.alertUser("View renamed to: {0}".format(viewV3.caption));
                    }
                    //else
                    //	TM.Gui.Dialog.showUserMessage("it was not possible to rename the library:" + title);					
                    window.TM.Gui.LibraryTree.lastViewRenamed = viewV3;
                    window.TM.Events.onRenamedView();
                }); 
        return viewNode;
    };
    
//*:    Admin/Edit mode

//Drag & Drop
window.TM.Gui.LibraryTree.setDraggableOptionsForView      = function(node, nodeData)    {
        $(node).draggable(
                        {
                            helper:'clone',
                            cursorAt: {left: -20, top: -20},
                            start : function() {
                                                    window.TM.Gui.draggedData = nodeData ;
                                                    window.TM.dragMode = 'view';
                                                }
                        });
    };
window.TM.Gui.LibraryTree.dropActions =                     {
                                                                dropOk              : undefined,
                                                                dropNotOk           : undefined,
                                                                iconOffset_Top      : 0,
                                                                iconOffset_Left     : -20,
                                                                currentTargetNode   : undefined
                                                            };
window.TM.Gui.LibraryTree.dropActions.setUp               = function()                        {

        var that = window.TM.Gui.LibraryTree.dropActions;
        that.dropOk = $("<div>").attr('id','dropOk').appendTo('body').absolute().height(18).width(18).zIndex(100);
        that.dropOk.css("background" , "url('/javascript/jQuery.jsTree/themes/default/d.png') -2px -53px no-repeat" ).width(18).height(18);

        that.dropNotOk = $("<div>").attr('id','dropNotOk').appendTo('body').absolute().height(18).width(18).zIndex(100);
        that.dropNotOk.css("background" , "url('/javascript/jQuery.jsTree/themes/default/d.png') -18px -53px no-repeat" ).width(18).height(18)

        that.hide_DropIcons();
    };
window.TM.Gui.LibraryTree.dropActions.hide_DropIcons      = function()                        {
  //{
        var that = window.TM.Gui.LibraryTree.dropActions;
        that.dropOk.hide();
        that.dropNotOk.hide();
    };
window.TM.Gui.LibraryTree.dropActions.show_DropOk         = function(targetNode)              {
  //{
        var that = window.TM.Gui.LibraryTree.dropActions;
        that.show_DropIcon(targetNode, that.dropOk);
    };
window.TM.Gui.LibraryTree.dropActions.show_DropNotOk      = function(targetNode)              {
 //{
        var that = window.TM.Gui.LibraryTree.dropActions;
        that.show_DropIcon(targetNode, that.dropNotOk);

        //open the node after 1 second (if still the same)
        that.currentTargetNode = targetNode;
        setTimeout(function()
        {
            if(that.currentTargetNode === targetNode)
            {
                window.TM.Gui.LibraryTree.jsTree.open_node(targetNode);
            }
        }, 1000);
    };
window.TM.Gui.LibraryTree.dropActions.show_DropIcon       = function(targetNode, dropIcon)    {
 //{
        if(targetNode !== undefined) //isDefined(targetNode))
        {
            var that = window.TM.Gui.LibraryTree.dropActions;
            that.hide_DropIcons();
            dropIcon.show();
            dropIcon.top ($(targetNode).offset().top + that.iconOffset_Top);
            dropIcon.left($(targetNode).offset().left + that.iconOffset_Left);
        }
    };
// Context menu
window.TM.Gui.LibraryTree.contextMenuIdValue = null;
window.TM.Gui.LibraryTree.createContextMenu = function(node)    {
    var updatedNodeId = false,
        createMode = "",
        items = {},
        nodeType,
        addLibrary, removeLibrary, addFolder, removeFolder, addView, removeView;

    if (window.TM.Gui.CurrentUser.isEditor() === false)
    {
        items =
        {            
            showDirectLink:         { label: "Show Direct Link",            action: window.TM.Gui.LibraryTree.showDirectLink } ,
            showDirectLinkForDevs:  { label: "Show Reading View Link",      action: window.TM.Gui.LibraryTree.showDirectLinkForDevs }
        };
        return items;
    }
    if (window.TM.Gui.editMode === false)
    {
        items =
        {
            showEditMode:           { label: "Open Edit Mode",              action: window.TM.Gui.showEditMode } ,
            showDirectLink:         { label: "Show Direct Link",            action: window.TM.Gui.LibraryTree.showDirectLink } ,
            showDirectLinkForDevs:  { label: "Show Reading View Link",      action: window.TM.Gui.LibraryTree.showDirectLinkForDevs }
        };
        return items;
    }
    window.TM.Gui.LibraryTree.contextMenuIdValue = $.data[node.attr('id')];

    if (window.TM.Gui.LibraryTree.contextMenuIdValue === undefined)
    {
        return items;
    }
    nodeType = window.TM.Gui.LibraryTree.contextMenuIdValue.__type;

    addLibrary = function()
    {
        window.TM.Gui.LibraryTree.add_Library_to_Database("New_Library" + "".add_Random().slice(0,5), undefined, false);
    };

    removeLibrary = function(parentNode)
    {
        var nodeId = parentNode.id(),
            nodeData = $.data[nodeId],
            description = "library: '{0}'".format(nodeData.name);
        window.TM.Gui.Dialog.deleteConfirmation(description, function()
        {
            var nodeId = parentNode.id();
            window.TM.Gui.LibraryTree.remove_Library_from_Database(nodeId)
        });
    };

    addFolder = function(parentNode)
    {
        var nodeId = parentNode.id(),
            nodeData = $.data[nodeId],
            libraryId = nodeData.libraryId,
            folderId = nodeData.folderId;
        window.TM.Gui.LibraryTree.add_Folder_to_Database(libraryId, folderId, "new folder", undefined, false);
    };

    removeFolder = function(parentNode)
    {
        var nodeId = parentNode.id(),
            nodeData = $.data[nodeId],
            libraryId = nodeData.libraryId,
            folderId = nodeData.folderId,
            description = "folder: '{0}'".format(nodeData.name);
        //confirm before deletion
        window.TM.Gui.Dialog.deleteConfirmation(description, function()
        {
            window.TM.Gui.LibraryTree.remove_Folder_from_Database(libraryId,folderId)
        } );

    };

    addView = function(parentNode)
    {
        var nodeId = parentNode.id(),
            nodeData = $.data[nodeId],
            libraryId = nodeData.libraryId,
            folderId = nodeData.folderId;
        window.TM.Gui.LibraryTree.add_View_to_Database(libraryId, folderId, "new view", undefined, false);
    };

    removeView = function(parentNode)
    {
        var nodeId = parentNode.id(),
            nodeData = $.data[nodeId],
            libraryId = nodeData.libraryId,
            viewId = nodeData.viewId,
            description = "view: '{0}'".format(nodeData.caption);
        window.TM.Gui.Dialog.deleteConfirmation(description, function()
        {
            window.TM.Gui.LibraryTree.remove_View_from_Database(libraryId, viewId);
            window.TM.Events.onUserDeleted();
        });
    };


    if (nodeType === "TeamMentor.CoreLib.Library_V3")
    {
        items = {
            createFolderItem:   { label: "Add View",  action: addView },
            createViewItem:     { label: "Add Folder",  action: addFolder },
            //createViewItem:   { label: "Add Folder",  action: function (obj)  { createMode = "Folder" ; this.create(obj); } },
            //createFolderItem: { label: "Add View",  action: function (obj)    { createMode = "View" ;   this.create(obj); } },
            createGuidanceItem: { label: "Add Guidance Item",  action: window.TM.Gui.LibraryTree.newGuidanceItem },//function (obj) { newGuidanceItem(); } }, //this.create(obj); } },
            renameItem: { label: "Rename Library",  action: function (obj) { this.rename(obj); } },
            deteteItem: { label: "Delete Library",  action: removeLibrary},     //function (obj) { this.remove(obj); } },
            separatorItem: { label: "-----------",  action: function (obj) { } },
            newLibItem: { label: "New Library",  action: function (obj) { addLibrary(); } }
            //reloadItem: { label: "Reload Data",  action: function (obj) { refreshLibraryView(); } }
        };
    }
    else if (nodeType === "TeamMentor.CoreLib.Folder_V3")
    {
        items = {
            createViewItem      : { label: "Add View"       ,  action: addView },
            createFolderItem    : { label: "Add Folder"     ,  action: addFolder },
            renameItem          : { label: "Rename Folder"  ,  action: function (obj) { this.rename(obj); } },
            deteteItem          : { label: "Delete Folder"  ,  action: removeFolder }
        };
    }
    else if (nodeType === "TeamMentor.CoreLib.View_V3")
    {
        items = {
            createItem: { label: "Add Guidance Item",  action:  window.TM.Gui.LibraryTree.newGuidanceItem },//function (obj) { newGuidanceItem(); } }, //this.create(obj); } },
            renameItem: { label: "Rename View",  action: function (obj) { this.rename(obj); } },
            deteteItem: { label: "Delete View",  action: removeView }
        };
    }
    /*else if (nodeType == "guidanceItem")
     items =
     {
     renameItem: { label: "Rename Guidance Item",  action: function (obj) { this.rename(obj); } },
     deteteItem: { label: "Delete Guidance Item",  action: function (obj) { this.remove(obj); } }
     }*/
    else
    {
        window.TM.Gui.Dialog.alertUser('not supported nodeType: ' + nodeType);
    }
    items.separator2Item = { label: "-----------",  action: function (obj) { } };
    items.userMode = { label: "Exit Edit Mode",  action: window.TM.Gui.showUserMode};

    return items;
};
window.TM.Gui.LibraryTree.onRename = function(event, data)  {
    var rename_newData = data.rslt.new_name;
    //_targetNodeData = eval("( {0} )".format(data.rslt.obj.attr("id")));
    
    var targetNodeData = $.data[data.rslt.obj.attr("id")];
    if (targetNodeData !== undefined) //isDefined(targetNodeData))
    {
        if (targetNodeData.__type == "TeamMentor.CoreLib.Library_V3")
        {
            window.TM.Gui.LibraryTree.rename_Library_to_Database(targetNodeData.libraryId, rename_newData);
        }
        if (targetNodeData.__type == "TeamMentor.CoreLib.Folder_V3")
        {
            window.TM.Gui.LibraryTree.rename_Folder_to_Database(targetNodeData.libraryId, targetNodeData.folderId, rename_newData);
        }
        if (targetNodeData.__type == "TeamMentor.CoreLib.View_V3")
        {
            window.TM.Gui.LibraryTree.rename_View_to_Database(targetNodeData.libraryId, targetNodeData.folderId, targetNodeData.viewId, rename_newData);
        }
    }
};
window.TM.Gui.LibraryTree.onCreate = function(event, data)  {
                                                                data.rslt.new_name = data.rslt.name;                // to make it compatible with onCreate
                                                                window.TM.Gui.LibraryTree.onRename(event, data);   // the node is already there so we only have to deal with the rename
                                                            };
window.TM.Gui.LibraryTree.newGuidanceItem = function()      {
                                                                var closeNewGuidanceItemDialog, createNewGuidanceItem;

                                                                closeNewGuidanceItemDialog = function()
                                                                                                {
                                                                                                    "ui-dialog-titlebar-close".$().click();
                                                                                                };
                                                                createNewGuidanceItem       = function()
                                                                                                {
                                                                                                    var title = "New Guidance Item",
                                                                                                        htmlContent = "";
                                                                                                    createGuidanceItem(title,htmlContent, window.TM.Gui.LibraryTree.contextMenuIdValue.libraryId,
                                                                                                        function(data)
                                                                                                            {
                                                                                                                var newGuidanceItemId = data.d;
                                                                                                                if (typeof(window.TM.Gui.LibraryTree.contextMenuIdValue.viewId) != "undefined")
                                                                                                                {

                                                                                                                    var viewId = window.TM.Gui.LibraryTree.contextMenuIdValue.viewId;
                                                                                                                    addGuidanceItemToView(viewId, newGuidanceItemId,
                                                                                                                        function() {
                                                                                                                                        $.data[viewId].guidanceItems.push(newGuidanceItemId);
                                                                                                                                   } );

                                                                                                                }

                                                                                                                $.data[newGuidanceItemId] = {};
                                                                                                                $.data[newGuidanceItemId].guidanceItemId = newGuidanceItemId;
                                                                                                                $.data[newGuidanceItemId].libraryId      = window.TM.Gui.LibraryTree.contextMenuIdValue.libraryId;
                                                                                                                $.data[newGuidanceItemId].title          = title;
                                                                                                                $.data[newGuidanceItemId].technology     = "";
                                                                                                                $.data[newGuidanceItemId].phase          = "";
                                                                                                                $.data[newGuidanceItemId].type           = "";
                                                                                                                $.data[newGuidanceItemId].category       = "";
                                                                                                                $.data[window.TM.Gui.LibraryTree.contextMenuIdValue.libraryId].guidanceItems.push(newGuidanceItemId);

                                                                                                                window.TM.Gui.DataTableViewer.selectedRowTarget = null;
                                                                                                                window.TM.Gui.DataTableViewer.selectedRowIndex = -1;

                                                                                                                editGuidanceItemInNewWindow(newGuidanceItemId);
                                                                                                            });
                                                                                                  } ;
                                                                  createNewGuidanceItem();
                                                            };
window.TM.Gui.LibraryTree.showDirectLink = function()       {
        window.location.hash = "#load:" + window.TM.Gui.selectedNodeId;
        window.TM.Gui.AppliedFilters.currentFilters = [];
        window.TM.Gui.Dialog.alertUser("The page's hash tag was set to the direct link: " + window.location.hash);
    };
window.TM.Gui.LibraryTree.showDirectLinkForDevs = function()       {
        window.location.hash = "#load:" + window.TM.Gui.selectedNodeId + "&showFilters:false&showTree:false&centerGuidanceItems:true";
        window.TM.Gui.AppliedFilters.currentFilters = [];
        window.TM.Gui.Dialog.alertUser("The page's hash tag was set to the Reading View link");
    };
