
    
//from http://datatables.net/plug-ins/api#fnGetFilteredData

$.fn.dataTableExt.oApi.fnGetFilteredData = function ( oSettings ) {
    var a = [];
    for ( var i=0, iLen=oSettings.aiDisplay.length ; i<iLen ; i++ ) {
        a.push(oSettings.aoData[ oSettings.aiDisplay[i] ]._aData);
    }
    return a;
}


//extra methods (not in http://datatables.net/plug-ins/api#fnGetFilteredData)


//fnColumnData
$.fn.dataTableExt.oApi.fnColumnData  = function ( oSettings, columnIndex )
{
    var dataInTable = this.fnGetData();
    return getColumnFromArray(dataInTable, columnIndex);
}

//fnColumnDataDistinct
$.fn.dataTableExt.oApi.fnColumnDataDistinct  = function ( oSettings, columnIndex )
{
    return jLinq.from(this.fnColumnData(columnIndex)).distinct()
}

$.fn.dataTableExt.oApi.fnGetFilteredColumnDataDistinct = function ( oSettings, columnIndex ) 
{
    return getDistinctColumnFromArray(this.fnGetFilteredData(), columnIndex);
}

$.fn.dataTableExt.oApi.fnGetFilteredColumnData = function ( oSettings, columnIndex ) 
{
    return getColumnFromArray(this.fnGetFilteredData(), columnIndex);
}


//fnFilterEx: allows chaining of fnFilter
/*$.fn.dataTableExt.oApi.fnFilterEx  = function ( oSettings, filter, index )
{
    this.fnFilter(filter, index)	
    return this;
}*/
//Helper methods

var getDistinctColumnFromArray = function(arrayData, columnIndex)
{
    return jLinq.from(getColumnFromArray(arrayData, columnIndex)).distinct();
}
var getColumnFromArray = function(arrayData, columnIndex)
{
    return jlinq.from(arrayData).select(function(rec) { return rec[columnIndex ] ;} ); 
}


//TM Specific 
//handles the selection of a row  and the load of the respective GuidanceItem

/*	var checkForUserActivities = function(guidanceId)
    {							
        currentUserHasActivityInGuidanceItem(guidanceId,
                                             "READ",
                                             function(data) { 																
                                                                if (data.d)
                                                                {
                                                                    "selectedGuidanceItem".$().css('color','darkGray');
                                                                    "selectedGuidanceItem".$().html(' [READ] ' + "selectedGuidanceItem".$().html());
                                                                }
                                                            
        currentUserHasActivityInGuidanceItem(guidanceId,
                                             "COMPLETED",
                                             function(data) { 
                                                                if (data.d)
                                                                {
                                                                    "selectedGuidanceItem".$().css('color','darkred');
                                                                    "selectedGuidanceItem".$().html(' [COMPLETED] ' + "selectedGuidanceItem".$().html());
                                                                }
                                                        
        currentUserHasActivityInGuidanceItem(guidanceId,
                                             "HELP",
                                             function(data) { 																
                                                                if (data.d)																
                                                                {
                                                                    "selectedGuidanceItem".$().css('color','red');
                                                                    "selectedGuidanceItem".$().html(' [HELP] ' + "selectedGuidanceItem".$().html());
                                                                }
                                                            } );
                                                        } );	} );	
    }
*/	
    var viewGuidanceItemInNewWindow = function(guidanceId)
        {
            if (TM.Gui.CurrentUser.isViewer())
            {				
                window.open('/article/' + guidanceId,'_blank');
            }
        };
    
    // this was not working in chrome
    /*
    var checkForPopupBlocker = function(poppedWindow)
        {			
            var hasPopupBlocker = function()
                {
                var result = false;

                try 
                {
                    if (typeof poppedWindow == 'undefined') {
                        // Safari with popup blocker... leaves the popup window handle undefined
                        result = true;
                    }
                    else if (poppedWindow && poppedWindow.closed) {
                        // This happens if the user opens and closes the client window...
                        // Confusing because the handle is still available, but it's in a "closed" state.
                        // We're not saying that the window is not being blocked, we're just saying
                        // that the window has been closed before the test could be run.
                        result = false;
                    }
                    else if (poppedWindow && poppedWindow.TM.Gui.GuidanceItemEditor.popupTest) {
                        // This is the actual test. The client window should be fine.
                        result = false;
                    }
                    else {
                        // Else we'll assume the window is not OK
                        result = true;
                    }
                }
                catch (err) 
                {
                    return false;
                }

                return result;
            }
            
            if (hasPopupBlocker())
                TM.Gui.Dialog.showUserMessage("It looks like you have a Popup Blocker for this website which is blocking the Guidance Items Editor");			
        }
    */	
    var editGuidanceItemInNewWindow = function(guidanceId)
        {			
            
            if (TM.Gui.CurrentUser.isEditor())
            {				
                window.open('/editor/' + guidanceId,'_blank');
            }
            
            /*var url = '/editor/' + guidanceId;			
            openUrl(url, "GuidanceItem Editor",850 , 600);
            */
            /*
            poppedWindow = window.open('/html_pages/GuidanceItemEditor/GuidanceItemEditor.html?#id:' + guidanceId				
                                          ,'_blank'
                                          ,'location=1,status=1,scrollbars=1,  width=1020,height=700');			
                                          
            //setTimeout(function() { checkForPopupBlocker(poppedWindow)} , 3000);
        */	
        };	

    var fixGuidanceItemLinks = function()
    {
        $("#selectedGuidanceItem a[href]").each(function()
            {
                var href = $(this).attr('href');
                
                $(this).attr('target', '_blank')

/*				var id =  href.replace("/article/","");				   
                var giData= $.data[id];

                
                if (isDefined(giData) === false)
                    $(this).removeAttr('href').css("text-decoration" , "underline");
                    */
            })
    };
                
    var mapSelectedGuidanceItems = function(event)
    {
        
    }	
        
    var jsTree_Configure_SetRowSelected_forGuidanceItems = function() 
    {		
        
        if (isUndefined(TM.Gui.DataTable.currentDataTable))	
            return;
        
        var oTable = TM.Gui.DataTable.currentDataTable;		
        var showEditOptions = TM.Gui.DataTable.showEditOptions();
        var currentDataTable = TM.Gui.DataTable.currentDataTable;
        var giRedraw = false;		
        
        selectedGuidanceIds  = [];		
        
        $(document).ready(function() 
        {					
            currentDataTable.fnSetColumnVis(0, showEditOptions);
            if (showEditOptions)
            {		
                $("#guidanceItemsTable tbody").mousedown(function(event) 
                    {
                        lastIdAdded = "";
                        var inputElement = $(event.target.parentNode).find("input");										
                        if (event.target.nodeName == "INPUT")
                        {
                            //console.log(selectedGuidanceIds);
                            if (typeof(inputElement.attr("checked")) != "undefined")				
                            {								
                                inputElement.attr("checked", 'yes');						
                                $(event.target.parentNode).addClass('row_selected');												
                                var aPos = oTable.fnGetPosition( event.target );	
                                var selectedGuidanceId = oTable.fnGetData( aPos[0] )[6];
                                lastIdAdded = selectedGuidanceId;
                            }	
                            else
							{
                                lastIdAdded ="";
								return;
							}
                                $(oTable.fnSettings().aoData).each(function ()
                                {
                                    var rowDataId = oTable.fnGetData(this.nTr)[6];
                                    if ( typeof($(this.nTr).find("input").attr("checked")) != "undefined") 
                                    {  									
                                        //$(this.nTr).addClass('row_selected');
                                        if(selectedGuidanceIds.indexOf(rowDataId) == -1)
                                            selectedGuidanceIds.push(rowDataId);
                                    }
                                });								
                            
                        }
                        
                        if (selectedGuidanceIds.length > 0)
                            "guidanceTableEditorHelperText".$().html('<br/><b>Drag {0} to drop into a view</b>'
                                                                        .format(selectedGuidanceIds.length == 1 ? "item"
                                                                                                               :  selectedGuidanceIds.length + " items" ));
                    });
                    
                $("#guidanceItemsTable tbody").click(function(event) 
                    {													
                        selectedGuidanceIds = [];
                        //_oTable = oTable;																									
                        var inputElement = $(event.target.parentNode).find("input");										
                        if (event.target.nodeName != "INPUT")
                        {
                            if (typeof(inputElement.attr("checked")) == "undefined")				
                            {						
//								inputElement.attr("checked", 'yes');						
                                $(event.target.parentNode).addClass('row_selected');												
                            }
                            else
                            {
                                var aPos = oTable.fnGetPosition( event.target );	
                                var selectedGuidanceId = oTable.fnGetData( aPos[0] )[6];
                                if(lastIdAdded=="")
                                    inputElement.removeAttr("checked");	
                                //console.log(selectedGuidanceId + "   " + lastIdAdded);
                                
                                //$(event.target.parentNode).removeClass('row_selected');
                            }
                        }	
                        
                        $(oTable.fnSettings().aoData).each(function ()
                            {
                                var rowDataId = oTable.fnGetData(this.nTr)[6];
                                if ( typeof($(this.nTr).find("input").attr("checked")) != "undefined") 
                                {  									
                                    $(this.nTr).addClass('row_selected');
                                    selectedGuidanceIds.push(rowDataId);
                                }
                                else
                                    if (selectedGuidanceIds.has(rowDataId) == false)
                                        $(this.nTr).removeClass('row_selected');
                            });
                            
                        if (selectedGuidanceIds.length > 0)
                            "guidanceTableEditorHelperText".$().html('<br/><b>Drag {0} to drop into a view</b>'
                                                                        .format(selectedGuidanceIds.length == 1 ? "selected item"
                                                                                                               :  selectedGuidanceIds.length + " items" ));																			
                        else
                            "guidanceTableEditorHelperText".$().html('');							
//							console.log(selectedGuidanceIds);
                    });
            }
                    
            
            /* Add a click handler to the rows - this could be used as a callback */
            $("#guidanceItemsTable tbody").mousedown(function(event) 
                {					
                    if ($(event.target).hasClass("dataTables_empty"))  // we can use dataTables_empty to detect if there are no rows in the current table
                    {                        
                        return;
                    }
                    //selectedGuidanceIds = [];
                    //_target = event.target;
                    
                    if (showEditOptions === false)				
                    {
                        //clear past selections
                        $(oTable.fnSettings().aoData).each(function ()
                            { $(this.nTr).removeClass('row_selected');  });														
                    }														
                    
    //			$("#guidanceItemsTable tbody").click(function(event) {
                    
                    //$(event.target.parentNode).toggleClass('row_selected');
                    var aPos = oTable.fnGetPosition( event.target );	
            
                    if (typeof(aPos) != "undefined" && aPos != null)
                    {											
                        TM.Gui.DataTableViewer.selectedRowTarget = event.target;
                        TM.Gui.DataTableViewer.selectedRowIndex = aPos[0];
                        TM.Gui.DataTableViewer.selectedRowData = oTable.fnGetData( aPos[0] );
                        var selectedGuidanceId = oTable.fnGetData( aPos[0] )[6];
                        var selectedTitle = oTable.fnGetData( aPos[0] )[1];
                        
                        TM.Gui.selectedGuidanceId		= selectedGuidanceId;
                        TM.Gui.selectedGuidanceTitle  	= selectedTitle;
                        TM.Events.onShowGuidanceItem();						
                        /*if 	(addGuidanceItemIdToSelectedList)
                            selectedGuidanceIds.push(selectedGuidanceId);
                        else
                            selectedGuidanceIds.pop(selectedGuidanceId);*/
                        if (showEditOptions== false)						
                        { 
                            $(event.target.parentNode).addClass('row_selected'); 
                        };
                    }
                });
            

            // set table dblClick (to open the article in a new window
            $("#guidanceItemsTable tbody").dblclick(function(event) 
                {	
                    if (TM.Gui.disablePopups === true)
                        return;
                    var aPos = oTable.fnGetPosition( event.target );
                    if (aPos != null)
                    {
                        var selectedGuidanceId = oTable.fnGetData( aPos[0] )[6];				
                        if (showEditOptions)
                            editGuidanceItemInNewWindow(selectedGuidanceId);
                        else
                            viewGuidanceItemInNewWindow(selectedGuidanceId);
                    }

                });
                
                /* Add a click handler for the delete row */
    /*			$('#delete').click( function() {
                    var anSelected = fnGetSelected( oTable );
                    oTable.fnDeleteRow( anSelected[0] );
                } );
        */									
            //TM.Gui.Dialog.alertUser('dataTable - jsTree_Configure_SetRowSelected_forGuidanceItems')
            //oTable = $('#guidanceItemsTable').dataTable( );
        } );	
            /* Init the table */			
            
        


        /* Get the rows which are currently selected */
        function fnGetSelected( oTableLocal )
        {
            var aReturn = new Array();
            var aTrs = oTableLocal.fnGetNodes();
            
            for ( var i=0 ; i<aTrs.length ; i++ )
            {
                if ( $(aTrs[i]).hasClass('row_selected') )
                {
                    aReturn.push( aTrs[i] );
                }
            }
            return aReturn;
        }
    }
    

//Gui helpers




//TM.Gui.selectedNodeData.viewId

var getDragHelperElement = function(event)
{		
    if(selectedGuidanceIds.length == 1)
    {
        var cloneTr = $(event.target).parent().clone()
        //cloneTr.find('td').eq(0).remove();
        var dragText = cloneTr.find('td').eq(1).html();;
        
        return $( "<div id='dragHelper'>{0}</div>".format(dragText));
        //return cloneTr.find('td').eq(1).html();
    }
    
    if(selectedGuidanceIds.length == 0)
        return $("<div style='background:black ; color:red'><h2>There are no Guidance Item selected (please select a couple before dragging)<h2></div>");
        

    //var dragText = "<h2>Drop in a View to add the selected {0} GuidanceItems </h2".format(selectedGuidanceIds.length);
    var dragText = "<img src='/Images/ViewIcon.png'/> {0} items".format(selectedGuidanceIds.length);
    return $( "<div id='dragHelper' style='font-size:14pt'>{0}</div>".format(dragText))[0];
    //return $( "<div id='dragHelper' style='background:black ; color:white'>{0}</div>".format(dragText))[0];
}	



TM.Gui.LibraryView = {};

TM.Gui.LibraryView.openFolders = [];
TM.Gui.LibraryView.mapOpenFolders = function()
    {		
        TM.Gui.LibraryView.openFolders = [];		
        $(".LibraryTree li").each(function() 
            { 
                if ($(this).hasClass("jstree-open")) 
                { 
                    TM.Gui.LibraryView.openFolders.push( $(this).attr("id") );
                }   
            } );		
    };
    
TM.Gui.LibraryView.restoreOpenFolders = function()	
{
    if (TM.Gui.LibraryView.openFolders.length == 0)	// don't restore when there are no mappings	
        return;
    $(".LibraryTree li").each(function() 
        {   
            var currentNodeId = $(this).attr("id");
            if (TM.Gui.LibraryView.openFolders.indexOf(currentNodeId)  > -1)     
                $(this).removeClass("jstree-closed").addClass("jstree-open");
            else
                $(this).removeClass("jstree-open").addClass("jstree-closed");     
        });
};
    