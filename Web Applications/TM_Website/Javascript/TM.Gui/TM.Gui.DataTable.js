TM.Gui.DataTable.tableOptions = 
    {
            maxBeforePaginate 	: ($.browser.msie) ? 700 : 2000
            
        ,	bRetrieve 			: true
        ,	bDeferRender	 	: true
        ,	bProcessing 		: true
        ,	bInfo 				: false		
        
        ,	bAutoWidth			: true
        ,   iDisplayLength 		: 500		
            
        ,  	sScrollY			: 100
        , 	sScrollX			: "100%"
        ,   sScrollYInner		: "100%"
        ,   sScrollXInner		: "100%"
        
        ,	bSort 				: true
        , 	aaSorting			: [[ 1, "asc" ]]
        //,	bScrollCollapse	: true			
    };

TM.Gui.DataTable.showEditOptions = function()
    {
        return TM.Gui.editMode && TM.Gui.CurrentUser.isEditor();
    }

TM.Gui.DataTable.reDraw = function () 
    {
        if (isDefined(TM.Gui.DataTable.currentDataTable)) {
            var dataTableYOffset = (TM.Gui.editMode === true) ? 85 : 70;
            $(".dataTables_scrollBody").height($("#gui_CenterCenter").height() - dataTableYOffset);
            TM.Gui.DataTable.currentDataTable.fnAdjustColumnSizing();
            TM.Gui.DataTable.currentDataTable.fnDraw();
        }
    }
    
TM.Gui.DataTable.selectRow = function(index)
    {		
        if (isUndefined(index))
            index = 1;
        $("#guidanceItemsTable tr td").eq(index).mousedown()					
    }

TM.Gui.DataTable.displayCurrentDataTable_onUserChange = function()
    {
        //if (TM.Gui.CurrentUser.loggedIn() === false && 
        //if(TM.Gui.DataTable.showEditOptions())
        if(TM.Gui.editMode)
            TM.Gui.DataTable.displayCurrentDataTable();
    }
    
TM.Gui.DataTable.displayCurrentDataTable = function()
        {					
            var dataTable = TM.WebServices.Data.filteredDataTable.aaData.length > 0
                                ? TM.WebServices.Data.filteredDataTable
                                : TM.WebServices.Data.lastDataTableData
                        
            TM.Gui.DataTable.displayDataTable(dataTable);
        }

TM.Gui.DataTable.displayDataTable = (function(dataTableToShow)
        {						
            if(isUndefined(dataTableToShow))
                return;						
            
            //$('#selectedGuidanceItem').html('');  // clear the 'loading' image
            $('#legend').html('');
            
            TM.Gui.DataTableViewer.addCheckBoxesToDataTable(dataTableToShow);
            //load table
            TM.Gui.DataTable.createDataTableWithGuidanceItems('guidanceItems', dataTableToShow);					
            
            /*setTimeout(function()
                { 					
                    TM.Gui.DataTable.createDataTableWithGuidanceItems('guidanceItems', dataTableToShow);					
                } , 20);			
                */
            TM.Gui.DataTableViewer.applyCss();	
        });		

TM.Gui.DataTable.loadDataTable = function(dataToLoad)
{			
    if (typeof(dataToLoad) == "undefined" || typeof(dataToLoad.aaData)=="undefined")
    {			
        return;
    }		
    TM.Gui.DataTable.displayDataTable(dataToLoad);					
}		

TM.loadingData = false;
TM.abortTableDataLoad = true;


TM.Gui.DataTable.createDataTableWithGuidanceItems = function (targetDiv, dataTableData, recreateTable) {
    //_dataTableData = dataTableData

    if (typeof (targetDiv.$()) == "undefined" || dataTableData.aoColumns.length === 0)
        return;

    if ($("#guidanceItemsTable").length == 0); // || recreateTable)
    {
        targetDiv.$().html('<br/>' +
                            '<div id="guidanceItemsTableButtons"/>' +
                            '<table cellpadding=\"10\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"guidanceItemsTable\"></table><br><br/>');

        //options	

        //newDataTable.aaSorting = [ [1,'asc'] ];	
        var tableOptions = TM.Gui.DataTable.tableOptions;
        var dataTableYOffset = (TM.Gui.editMode === true) ? 85 : 70;
        tableOptions.sScrollY = $("#gui_CenterCenter").height() - dataTableYOffset;
        tableOptions.bPaginate = (dataTableData.aaData.length > tableOptions.maxBeforePaginate);
        tableOptions.aoColumns = dataTableData.aoColumns;
        tableOptions.aaData = {};

        //TM.Gui.Dialog.alertUser('dataTable - createDataTableWithGuidanceItems with {0} items'.format(dataTableData.aaData.length));
        TM.Gui.DataTable.currentDataTable = $('#guidanceItemsTable').dataTable(tableOptions);

        TM.Gui.DataTable.currentDataTable.fnSetColumnVis(0, TM.Gui.editMode);
        TM.Gui.DataTable.currentDataTable.fnSetColumnVis(6);

        TM.Gui.DataTable.loadDataIntoDataTable_Step4(); 	//CSS fixes		

        jsTree_Configure_SetRowSelected_forGuidanceItems();
    }

    TM.Gui.DataTable.addDataTableButtons();
    TM.Gui.DataTable.loadingNode = TM.Gui.DataTable.currentDataTable.fnAddData(["", "Loading Data....", "", "", "", "", ""]);
    setTimeout(function () { TM.Gui.DataTable.loadDataIntoDataTable_Step1(dataTableData) }, 25);
}

TM.Gui.DataTable.loadDataIntoDataTable_Step1 = function(dataTableData)
    {			
        if (isUndefined(TM.Gui.DataTable.currentDataTable))	
            return;
        var _currentDataTable = TM.Gui.DataTable.currentDataTable;
        if (TM.loadingData)
        {
            TM.abortTableDataLoad = true;
            setTimeout(function() 
                {                    
                    TM.Gui.DataTable.loadDataIntoDataTable_Step1(dataTableData);
                }, 200);;            
            return;
        }
        TM.loadingData = true;
        TM.abortTableDataLoad = false;			
        TM.Gui.lastDataLoaded = dataTableData;
        lastDataLoad = dataTableData;
        TM.abortTableDataLoad = false;			//current object
                
        _currentDataTable.fnClearTable(); 	
        
        TM.Gui.DataTableViewer.selectedRowTarget = null;
        TM.Gui.DataTableViewer.selectedRowIndex = -1;

        var itemsToAdd = dataTableData.aaData.slice() ; 	// create shalow copy of data to load
        var totalItemstoLoad = itemsToAdd.length;
        
        var dataTableLoadComplete = function()
            {
                TM.DataTableLoaded = "TM: DataTable Loaded";				
                if (lastDataLoad.aaData.length === 0)
                    TM.Gui.DataTable.raiseEventForEmptyTable();
                else
                {					
                    var restCommand = location.pathname.toLowerCase().split("/");			//handle the case where the user as issue an REST command to open an specific article
                    var mapping = restCommand.pop();
                    var command = restCommand.pop();
                    if (command == "open")					
                        TM.Gui.GuidanceItemViewer.showGuidanceFromMapping(mapping)
                    else
                        TM.Gui.DataTable.selectRow(1);										// default to selecting the first row
                }
                TM.Events.onDataTableDisplayed();
            };
        
        if (itemsToAdd.length < 500)
        {
            _currentDataTable.fnAddData(itemsToAdd);
            //$("#nowShowingLabel").html("Showing " + totalItemstoLoad + " items");            
            TM.Gui.DataTableViewer.set_Title("Showing " + totalItemstoLoad + " items (out of " + totalItemstoLoad  + ")");
            dataTableLoadComplete();
        }
        else
        {				
            var loadItems = function()  
                {								
                    if(TM.abortTableDataLoad)																				
                    {
                        _currentDataTable.fnClearTable(); 
                        _currentDataTable.fnAddData(["","Refreshing....","","","","",""]);                                             
                        return;										
                    }										
                    //$("#nowShowingLabel").html(
                    setTimeout(function() 
                                {
                                    TM.Gui.DataTableViewer.set_Title("Loaded " + (totalItemstoLoad - itemsToAdd.length) + " out of " + totalItemstoLoad);
                                }, 10);
                    try
                    {
                         var slice = itemsToAdd.splice(0,500)                                         
                            _currentDataTable.fnAddData(slice  );
                                         
                            if (itemsToAdd.length > 0)
                                setTimeout(loadItems, 25);
                            else
                            {
                                //$("#nowShowingLabel").html(
                                TM.Gui.DataTableViewer.set_Title("Showing " + totalItemstoLoad + " items");											 
                                             
                                dataTableLoadComplete();
                            }
                    } catch(e)
                    {
                            console.log(e);
                    } 
                }                         
            loadItems();
        };		
        setTimeout(function() { TM.Gui.DataTable.loadDataIntoDataTable_Step3()}, 25 ) ;
    }

TM.Gui.DataTable.loadDataIntoDataTable_Step3 = function()
    {
        TM.loadingData = false;
        setTimeout(function() { TM.Gui.DataTable.loadDataIntoDataTable_Step4()}, 25 ) ;
    }
    
TM.Gui.DataTable.loadDataIntoDataTable_Step4 = function()
    {
        //cssFixes
        $('#guidanceItemsTable').css('width', '100%');
        $(".GuidanceItemCheckBox").parent().attr('style','text-align: center');	
        $('#guidanceItemsTable').css('font-size', '13')
        jQuery('#guidanceItemsTable_filter').css('display','none');
        jQuery('.dataTables_wrapper').css('clear','none');
        jQuery('.display').css('clear','none');
        jQuery('table.display').css('clear','none') ;
        jQuery('#guidanceItemsTable_length').remove();
        if (TM.Gui.DataTable.showEditOptions())
            TM.Gui.DataTableViewer.setDragAndDropOptions();		                
    }	
    
TM.Gui.DataTable.addDataTableButtons = function()
    {
        if (TM.Gui.DataTable.showEditOptions())
        {				
            "guidanceItemsTableButtons".$().html(//"<div id='guidanceItemsTableButtons'>" +
                                            "<button id='button_selectAll'>Select All</button>" +
                                            "<button id='button_deselectAll'>Deselect All</button>"  +
                                            //"<button id='button_newGuidanceItem'>New Guidance Item</button>" + 										
                                            "<button id='button_RemoveGuidanceItemsFromView'>Remove Guidance Items from View</button>" + 
                                            "<button id='button_DeleteGuidanceItemsFromLibrary'>Delete Guidance Items from Library</button>" + 
                                            "<span id=guidanceTableEditorHelperText></span>" + 
                                            "</div>" + 
                                            "<br/>"); 
                                        
            //"button_newGuidanceItem".$().click(newGuidanceItem );								
            "button_selectAll".$().click(function() 
                { 				
                    "#guidanceItemsTable input".$().each(function() { $(this).attr('checked','true') }); 
                    "#guidanceItemsTable input".$()[0].click();		// this will trigger the event that populates the selectedGuidanceIds
                    "#guidanceItemsTable input".$()[0].click();		// reselect it
                } );							
            "button_deselectAll".$().click(function() 	
                {
                    "#guidanceItemsTable input".$().each(function() { $(this).removeAttr('checked') }); 
                    "#guidanceItemsTable input".$()[0].click();		// this will trigger the event that populates the selectedGuidanceIds
                    "#guidanceItemsTable input".$()[0].click();		// reselect it
                } );
            "button_RemoveGuidanceItemsFromView".$().click(function() 
                {					
                    removeGuidanceItemsFromView(TM.Gui.selectedNodeData.viewId, selectedGuidanceIds, 
                        function(result) 
                            {
                                if (result.d)
                                {
                                    //_viewId = TM.Gui.selectedNodeData.viewId;
                                    //_selectedGuidanceIds = selectedGuidanceIds;
                                    TM.Gui.Dialog.alertUser("GuidanceItems successfully removed from view, please refresh view")
                                }
                                else
                                    TM.Gui.Dialog.alertUser("There was an error performing this action")
                            });
                });
        
            "button_DeleteGuidanceItemsFromLibrary".$().click(function()
                {				
                    var deleteGuidanceItems = function() 
                        {
                            TM.WebServices.WS_Libraries.remove_GuidanceItems(selectedGuidanceIds, 
                                function(result) 
                                    {
                                        if (result)
                                        {								
                                            TM.Gui.Dialog.alertUser("GuidanceItems successfully deleted, please refresh browser")
                                        }
                                        else
                                            TM.Gui.Dialog.alertUser("There was an error performing this action")
                                    });	
                        };
                   
                    if (selectedGuidanceIds.length > 0){
                        var description = "{0} {1}?".format(selectedGuidanceIds.length, selectedGuidanceIds.length>1 ? "guidance items" :"guidance item");
                        TM.Gui.Dialog.deleteConfirmation(description,deleteGuidanceItems);
                    }
                             });
            
            "guidanceTableEditorHelperText".$().css('font-size','10pt')
                                               .relative()
                                               .left(20)		
                                           
            //hide it for now since it is not implemented
            //if(typeof(TM.Gui.selectedNodeData.viewId) != "undefined" || typeof(TM.Gui.selectedNodeData.folderId) != "undefined" )	
            if(TM.Gui.selectedNodeData !== undefined)
            {
                if(TM.Gui.selectedNodeData.__type != "TeamMentor.CoreLib.Library_V3")	 
                    "button_DeleteGuidanceItemsFromLibrary".$().hide();
                //if (typeof(TM.Gui.selectedNodeData) != "undefined")
                if(TM.Gui.selectedNodeData.__type != "TeamMentor.CoreLib.View_V3")
                        "button_RemoveGuidanceItemsFromView".$().hide();
             }       
            "button".$().button(); 
            "button".$().css("font-size",'10px');
        
            if ($.browser.msie)	//TODO: figure out why this is happening in IE
            {
                $("#guidanceItemsTableButtons .ui-button-text").width('100%');
                "button_selectAll".$().width(100);
                "button_deselectAll".$().width(100);			
                "button_RemoveGuidanceItemsFromView".$().width(250);
                "button_DeleteGuidanceItemsFromLibrary".$().width(250);			
                "guidanceTableEditorHelperText".$().width(300);
            }
        
        
            /*"<button id='button_selectAll'>Select All</button>" +
                                            "<button id=''>Deselect All</button>"  +
                                            //"<button id=''>New Guidance Item</button>" + 										
                                            "<button id=''>Remove Guidance Items from View</button>" + 
                                            "<button id=''>Delete Guidance Items from Library</button>" + 
                                            "<span id=></span>"
            */
        }
    };

TM.Gui.DataTable.raiseEventForEmptyTable = function()
    {
        TM.Gui.selectedGuidanceId = TM.Const.emptyGuid;
        TM.Gui.selectedGuidanceTitle = "No article available.";
        TM.Events.onShowGuidanceItem();
    }