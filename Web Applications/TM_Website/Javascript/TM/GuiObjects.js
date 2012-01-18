 //Client Side data processing

TM.WebServices.Data.extractGuiObjects = function(callback)
	{						
		var startTime1 = new Date();
		TM.WebServices.Data.GuidanceItemsIDs = [];
		
		function extractMapping(rawMapping) 
		{
			var uniqueStrings = TM.WebServices.Data.GuiObjects.UniqueStrings;
			var indexes = rawMapping.split(',')
			var mapping = {
								guidanceItemId: uniqueStrings[indexes[0]],
								libraryId: uniqueStrings[indexes[1]],
								title: uniqueStrings[indexes[2]], 
								technology: uniqueStrings[indexes[3]], 
								phase: uniqueStrings[indexes[4]],
								type: uniqueStrings[indexes[5]],
								category: uniqueStrings[indexes[6]]
						  }; 
			$.data[mapping.guidanceItemId] = mapping;
			TM.WebServices.Data.GuidanceItemsIDs.push(mapping.guidanceItemId);				
			return mapping; 
		};
		
		function extractAllMappings() 
		{
			var startTime2 = new Date();
			var guidanceItemsMappings = TM.WebServices.Data.GuiObjects.GuidanceItemsMappings;
			$.each(guidanceItemsMappings, function() {	extractMapping(this); });
			
			TM.WebServices.Data.ExtractedAllMappings = true;
			TM.Debug.TimeSpan_ExtractAllMappings  = new Date(new Date() - startTime1);
			TM.Debug.TimeSpan_CalculateAllMappings = new Date(new Date() - startTime2);			
			if(typeof(callback) !="undefined")
				callback();	
		}		
		TM.WebServices.WS_Data.getGUIObjects(extractAllMappings);
		
	}
		
TM.WebServices.Data.extractFolderStructure = function(callback)
	{						
		var startTime1 = new Date();			
		var mapFolderStructure = function()
		{								
			//console.log("in mapFolderStructure");
			TM.Events.raiseProcessBarNextValue("Mapping Library Structure");	
			
			var mapView = function(view)
				{      			
				   allViews.push(view)			   
				}

			var mapViews = function(views, parentFolder)
				{
				   if(typeof(views) != "undefined")
				   {  
						$.each(views, function() {
													  this.id 	 = this.viewId;
													  this.type  = "View";
													  this.name  = this.caption;
													  mapView(this);
													  
													  var guidanceItems = (this.guidanceItems.length  === 0)
																			? []
																			: this.guidanceItems.toString().split(",");
													  $.data[this.viewId] = this;//guidanceItems;
													  														
													  parentFolder.guidanceItems = parentFolder.guidanceItems.concat(guidanceItems)
													  													 
													  //get unique list  
													  parentFolder.guidanceItems = jlinq.from(parentFolder.guidanceItems).distinct();
													  
												  } )            
				   }
				}

			var mapFolder = function(folder)
				{				   
				   allFolders.push(folder);
				   folder.guidanceItems = [];				   
				   mapViews(folder.views, folder);				   
				   mapFolders(folder.subFolders, folder);				   
				}

			var mapFolders = function(folders, parentFolder)
				{
					 if(typeof(folders) != "undefined")
						$.each(folders, function() {
													   this.id 	 = this.folderId;
													   this.type = "Folder"; 
													   mapFolder(this, parentFolder);
													   var guidanceItems = (this.guidanceItems.length  === 0)
																			? []
																			: this.guidanceItems.toString().split(",");
													   $.data[this.folderId] = this;//guidanceItems
													   parentFolder.guidanceItems = parentFolder.guidanceItems.concat(guidanceItems)
													   //get unique list  
													   parentFolder.guidanceItems = jlinq.from(parentFolder.guidanceItems).distinct();
													   													   
													   this.parentId = parentFolder.id;													   
												   } );
				}
			
			var startTime2 	 = new Date();		
			var allViews 	 = []
			var allFolders 	 = []
			var allLibraries = [];			
			
			
			var setGlobalVariablesAndContinue = function()
				{
					//console.log("in setGlobalVariablesAndContinue()");
					TM.WebServices.Data.AllViews 	 = allViews;
					TM.WebServices.Data.AllFolders 	 = allFolders
					TM.WebServices.Data.AllLibraries = allLibraries
					
					TM.WebServices.Data.FolderStructure = true;
					TM.Debug.TimeSpan_ExtractFolderStructure = new Date(new Date()   - startTime1);
					TM.Debug.TimeSpan_CalculateFolderStructure = new Date(new Date() - startTime2);
					
					
					//console.log("after mapFolderStructure");
					
					if(typeof(callback) !="undefined")
						callback();		
				}
			
					
			var mapLibrary = function(libraryStructure)
			{		
				//console.log("Mapping library: " + libraryStructure.name);
				TM.Events.raiseProcessBarNextValue("Mapping Library:" + libraryStructure.name);			
				var rootNode 	= libraryStructure;
				rootNode.id 	= libraryStructure.libraryId;
				rootNode.type 	= "Library"; 
								
				allLibraries.push(rootNode)				
				mapFolders (rootNode.subFolders,rootNode );
				mapViews(rootNode.views,rootNode);						
				$.data[libraryStructure.libraryId]= libraryStructure; 
				
				//console.log(TM.WebServices.Data.folderStructure.length + "  === " + allLibraries.length);
				if(TM.WebServices.Data.folderStructure.length  === allLibraries.length)
				{
					//console.log(">>>>>");   
					setGlobalVariablesAndContinue();
					//console.log("<<<<<");
				}
			}		
			
			if (TM.WebServices.Data.folderStructure.length===0)
				setGlobalVariablesAndContinue();
			else	
				$.each(TM.WebServices.Data.folderStructure, function()	
					{					
						var that = this;
						setTimeout(function() { mapLibrary(that) } , 1);
					})			
													
				
				
			
		}
		
		/*TM.WebServices.Data.extractGuiObjects(
			TM.WebServices.WS_Data.getFolderStructure(
				mapFolderStructure));*/
		//timing bug		
		
		//console.log("Before extractGuiObjects");
		
		TM.WebServices.Data.extractGuiObjects(
			function() {
							//console.log("before getFolderStructure");
							TM.Events.raiseProcessBarNextValue("Downloading Library Structure");			
							TM.WebServices.WS_Data.getFolderStructure(mapFolderStructure) 
						} );
		
	}
	
//Extra Methods to access TM.WebServices.Data objects

TM.WebServices.Data.library = function(nameOrId)
	{
		return jLinq.from(TM.WebServices.Data.AllLibraries)					
					.equals("name", nameOrId)
					.orEquals("libraryId",nameOrId)
					.first();
	}

TM.WebServices.Data.folder = function(nameOrId)
	{
		return jLinq.from(TM.WebServices.Data.AllFolders)
					.equals("name", nameOrId)
					.orEquals("folderId",nameOrId)
					.first();
	}

TM.WebServices.Data.view = function(nameOrId)
	{
		return jLinq.from(TM.WebServices.Data.AllViews)
					.equals("caption", nameOrId)
					.orEquals("viewId",nameOrId)
					.first();
	}	
	
//Get Data previously processed
	
TM.WebServices.Data.getGuidanceItemsForGuids = function(guids)
	{	
		if (typeof(guids) == "undefined")
			guids = TM.WebServices.Data.GuidanceItemsIDs;		
		var guidanceItems = [];
		$.each(guids, function(index,guid) 
						{						
							var guidanceItem = $.data[guid];
							if (typeof(guidanceItem)!="undefined")
								guidanceItems.push(guidanceItem);						
						});	
		return 	guidanceItems;		
	}	

TM.WebServices.Data.getGuidanceItems_For_DataTable = function(guidanceItemsIds)
   {   		
		var columns = ["Index","Title", "Technology", "Phase", "Type","Category", "Guid"];//, "LibraryId"];
		
		var getTableColumns = function()
		{				
			var tableColumns = [];
			var add_Columns = function(titles)
				{						
					$.each(titles,function() { tableColumns.push({sTitle:this}) })
				}
							
			add_Columns(columns);
			
			tableColumns[0].sWidth="5%";
			//tableColumns[1].sWidth=400;
			tableColumns[1].sWidth="50%";
			tableColumns[3].sWidth="15%";
			tableColumns[4].sWidth="15%";
			tableColumns[5].sWidth="15%";			
			return tableColumns;
		}
			
		var getTableData = function(guids)
		{
			var data = [];							
			$.each(guids, function(index,value) 
				{ 						
					var guidandeItem = $.data[value];
					if (typeof(guidandeItem) == "undefined")
					{
						//console.log("ERROR FOR: " + value);
					}
					else
						data.push([
									index,									
									guidandeItem.title,
									guidandeItem.technology,
									guidandeItem.phase,
									guidandeItem.type,
									guidandeItem.category,
									guidandeItem.guidanceItemId, //value variable has the same value									
//									guidandeItem.libraryId									
								  ]) ; 						
				});			
			return data;				
		}									
	
		var dataTableData = { 
								bPaginate : false,
								bInfo : false,
								bSort : true,
								bDeferRender: true,
								bProcessing: true,
								aoColumns  : getTableColumns()							
							 };
		dataTableData.aaData  = getTableData(guidanceItemsIds);				
		return dataTableData;
	}
	
TM.WebServices.Data.getGuidanceItemsInGuid_For_DataTable = function(guid_for_LibraryFolderOrView)
	{		
		_guid_for_LibraryFolderOrView = guid_for_LibraryFolderOrView;
		var dataTableData = {};
		if (typeof(guid_for_LibraryFolderOrView) != "undefined")
		{		
			var targetObject = $.data[guid_for_LibraryFolderOrView];
			if (typeof(targetObject)!= "undefined")
			{
				TM.WebServices.Data.targetObjectGuidanceItemsIds = targetObject.guidanceItems;
				dataTableData = TM.WebServices.Data.getGuidanceItems_For_DataTable(targetObject.guidanceItems);				
				dataTableData.targetObject = targetObject;
			}
			else
				dataTableData = TM.WebServices.Data.getGuidanceItems_For_DataTable([])			
				
			/*dataTableData.targetObject = $.data[guid_for_LibraryFolderOrView];
			if (typeof(dataTableData.targetObject)!= "undefined")
				dataTableData.aaData = getTableData(dataTableData.targetObject.guidanceItems);				
			*/	
		}
		else
		{
			//dataTableData = TM.WebServices.Data.getGuidanceItems_For_DataTable(TM.WebServices.Data.GuidanceItemsIDs)			
			dataTableData = TM.WebServices.Data.getGuidanceItems_For_DataTable([])			
		}
		
		TM.WebServices.Data.lastDataTableData    = dataTableData;
		return dataTableData;
   }
   
   
   
   
   
//Extension methods:
   
 Array.prototype.unique = function()
	{
		var arrVal = this;
		var uniqueArr = [];
		for (var i = arrVal.length; i--; ) 
		{
			var val = arrVal[i];
			if ($.inArray(val, uniqueArr) === -1) 
			{
				uniqueArr.unshift(val);
			}
		}
		return uniqueArr; 
	};
	
Date.prototype.secondsAndMiliSeconds = function	()
	{
		return this.getSeconds() + "s " + this.getMilliseconds() + "ms" ; 
	};
	
Date.prototype.toNow_SecondsAndMiliSeconds = function	()
	{
		return new Date(new Date() - this).secondsAndMiliSeconds();  
	};	
	
Date.prototype.toNow = function	()	
	{
		return new Date(new Date() - this);
	}