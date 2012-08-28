	currentPivotPanelFilters = new Array();
	currentFilters = []; 	
	
	function setPivotPanelFilter(text,title, column, state)
	{				
		var updatedFilter = new Array();		
		for (i=0; i < currentPivotPanelFilters.length; i++) 
		{
			var filter = currentPivotPanelFilters[i];
			
			if (filter.text == text && filter.column == column)
			{					
			}
			else if (filter.title == "Text Search")
			{				
				if (column == 0)				
					filter.text = text;													
				else				
					updatedFilter.push(filter);						
			}
			else
			{  				
				updatedFilter.push(filter);
			}
		}			
		
		currentPivotPanelFilters = updatedFilter;		
		
		if(state && text!= "")		
		{
			currentPivotPanelFilters.push( { 'text' : text , 'title' : title , 'column' : column });			
		}	
		
		currentFilters = [];
		for (i=0; i < currentPivotPanelFilters.length; i++) 
		{					
			var filter = currentPivotPanelFilters[i];
			if (typeof(currentFilters[filter.column]) == "undefined")
				currentFilters[filter.column] = filter.text;
			else
				currentFilters[filter.column] += "|" + filter.text;
		}		
						
		//setTimeout(TM.Gui.AppliedFiltersList.populateAppliedFiltersTable , 25);
        //if (TM.Gui.AppliedFilters.raise_onBuildFiltersGui)		

		 TM.Events.onBuildFiltersGui()
	}




function buildPivotPanel(pivotPanelData)
{		
	if (typeof("PivotPanelTemplate".$())!= "undefined")
	{
		var divName = "pivotPanel_" + pivotPanelData.title;
		jQuery('#pivotPanels').append("<div id='" + divName  + "'></div>");		
		jQuery('#' + divName).setTemplate("PivotPanelTemplate".$().html());		
		jQuery('#' + divName).processTemplate(pivotPanelData);  
	}	
}	

TM.Gui.AppliedFilters.getFilterDataObject = function (arrayWithSelectedItems, arrayWithAllData, arrayWithFilteredData,  title, index)
	{				
        var filterDataObject = 
			{
				title:title,
				column: index,				
				itemsAvail: [], 
				itemsNotAvail: []
			}	
        if (arrayWithAllData.length === 0) // there is nothing to do    
            return filterDataObject;
        
		var selectedFilter = arrayWithSelectedItems[index];		

		//items Checked	
		if (typeof(selectedFilter) == "undefined" ||  selectedFilter== null)
			selectedFilter = "";				
		if 	(selectedFilter=="")		
			var itemsChecked = [];		
		else				
			var itemsChecked = selectedFilter.split('|');
		_itemsChecked = itemsChecked;
		itemsAvail = []; 			
		itemsNotAvailRaw = getDistictColumnValue(arrayWithAllData,index); 
        var gotAtLeastOneMatch = false;
		$.each(getDistictColumnValue(arrayWithFilteredData,index).sort(), function(index,value)
			{				
				var checked = itemsChecked.has(value);
				itemsAvail.push({ text : value , checked : checked  });
				itemsNotAvailRaw = removeFromArray(itemsNotAvailRaw, value);
                if (checked)
                    gotAtLeastOneMatch = true;
			});
        // handle the case where there is no match for a value provided in the filter
		if (gotAtLeastOneMatch === false && itemsChecked.length > 0)
        {
            itemsAvail = [];
            itemsNotAvailRaw = getDistictColumnValue(arrayWithAllData,index);
            TM.WebServices.Data.lastDataTableData.aaData = []
            console.log("gotAtLeastOneMatch was false!!!!! with "  + itemsChecked);
            console.log("itemsAvail: " + itemsAvail.length);
            console.log("itemsNotAvailRaw: " + itemsNotAvailRaw);
        }
		itemsNotAvail = [];
		itemsNotAvailRaw.sort();
		$.each(itemsNotAvailRaw, function(index,value)
			{				
				var checked = itemsChecked.has(value);				
				itemsNotAvail.push({ text : value , checked : checked  });				
			});
		
		//currentFilter
		filterDataObject.itemsAvail = itemsAvail;
        filterDataObject.itemsNotAvail = itemsNotAvail;

		return filterDataObject;	
	}
	
function getTempFilterResult(index)
	{
		var aaData = TM.WebServices.Data.lastDataTableData.aaData;
		var tempFilter = currentFilters[index];
		currentFilters[index] = "";
		var filterResult  = TM.Gui.AppliedFilters.applyDataTableFilter_using_PivotPanelFilters(aaData, queryTo_filterDataTable , currentFilters );
		currentFilters[index] = tempFilter;
		return filterResult;
	}
	
var updatingFilters = false
TM.Gui.AppliedFilters.refresh = function () {};

TM.Gui.AppliedFilters.showFilters = function(arrayWithSelectedItems, arrayWithAllData, arrayWithFilteredData, raise_onAppliedFieldsEnd)
	{				
        TM.Gui.AppliedFilters.refresh = function() { TM.Gui.AppliedFilters.showFilters(arrayWithSelectedItems, arrayWithAllData, arrayWithFilteredData, false); };

		//console.log("-------------showFilters	-------- for: " + arrayWithAllData.length);
		$("#pivotPanels input").parent().css('opacity', '0.5')
		
		var startTime = new Date();		
		TM.abortTableDataLoad = true;
		if (updatingFilters)
		{
			setTimeout(function() 
				{
					//console.log("after wait: updatingFilters");
					TM.Gui.AppliedFilters.showFilters(arrayWithSelectedItems, arrayWithAllData, arrayWithFilteredData,raise_onAppliedFieldsEnd)
				}, 200);;
			//console.log("updatingFilters");
			return;
		}		
	
		updatingFilters = true;
		filterData = []				
		var timeOutInterval = 5;
		setTimeout(function(){
			filterData.push(TM.Gui.AppliedFilters.getFilterDataObject(arrayWithSelectedItems, arrayWithAllData, getTempFilterResult(2), "Technology", 2));				

		setTimeout(function(){
			filterData.push(TM.Gui.AppliedFilters.getFilterDataObject(arrayWithSelectedItems, arrayWithAllData, getTempFilterResult(3), "Phase", 3));
			
		setTimeout(function(){
			filterData.push(TM.Gui.AppliedFilters.getFilterDataObject(arrayWithSelectedItems, arrayWithAllData, getTempFilterResult(4), "Type", 4));
			
		setTimeout(function(){
			filterData.push(TM.Gui.AppliedFilters.getFilterDataObject(arrayWithSelectedItems, arrayWithAllData, getTempFilterResult(5), "Category", 5));
		
		setTimeout(function(){
		
			$.each(filterData, function(index, filterDataEntry)
				{ buildPivotPanel(filterDataEntry) } );			
		
		setTimeout(function(){
		
            //var aaData = TM.Gui.AppliedFilters.applyDataTableFilter_using_PivotPanelFilters(TM.WebServices.Data.filteredDataTable.aaData, queryTo_filterDataTable , currentFilters );		
			//apply the text filter = 
			var aaData = TM.Gui.AppliedFilters.applyDataTableFilter_using_PivotPanelFilters(TM.WebServices.Data.lastDataTableData.aaData, queryTo_filterDataTable , currentFilters );		
		
			//TM.showFilter_cssFixes();		
		
		setTimeout(function(){
		
			TM.WebServices.Data.filteredDataTable = {};
			TM.WebServices.Data.filteredDataTable.aoColumns = TM.WebServices.Data.lastDataTableData.aoColumns;		
			TM.WebServices.Data.filteredDataTable.aaData = aaData;
		
			
			//raise event			
			updatingFilters = false;						
			TM.Debug.TimeSpan_Gui_AppliedFilters_ShowFilters = startTime.toNow();
            if (raise_onAppliedFieldsEnd)
			    TM.Events.onAppliedFieldsEnd();

            //run text search (which will also trigger a table
    //            TM.Gui.TextSearch.getValueAndApplyGlobalFilter();					
			
			
		},timeOutInterval)
		},timeOutInterval)
		},timeOutInterval)
		},timeOutInterval)
		},timeOutInterval)
		},timeOutInterval)
		},timeOutInterval);
		
	}

TM.Gui.AppliedFilters.buildFiltersGui = function () 
    {                
	    //TM.Events.onInvalidateSearchText();
        TM.Events.onBuildFiltersGui.enabled = false;    
	    //TM.Gui.AppliedFilters.raise_onBuildFiltersGui = false;
    
	    TM.Gui.AppliedFilters.MapFiltersFromUrl(); // new one
        TM.Events.onBuildFiltersGui.enabled = true; 
//	    TM.Gui.AppliedFilters.raise_onBuildFiltersGui = true;

//	    var aaData = TM.WebServices.Data.lastDataTableData.aaData;
        var aaData = TM.WebServices.Data.filteredDataTable.aaData;

	    filterResult = TM.Gui.AppliedFilters.applyDataTableFilter_using_PivotPanelFilters(aaData, queryTo_filterDataTable, currentFilters);
        
	    TM.Gui.AppliedFilters.showFilters(currentFilters, aaData, filterResult, true);
	    
        //TM.Gui.AppliedFilters.raise_onBuildFiltersGui = true;

        //TM.Gui.TextSearch.getValueAndApplyGlobalFilter();

        
        //TM.Events.onDisplayDataTable()
	}

TM.Gui.AppliedFilters.buildFromSelectedNodeId = function () {
    
    //this will reset the filters on node click
    currentPivotPanelFilters = new Array(); 		// reset applied filters
    currentFilters = [];

    var selectedNodeId = TM.Gui.selectedNodeId;
    TM.WebServices.Data.getGuidanceItemsInGuid_For_DataTable(selectedNodeId);

    //TM.Gui.AppliedFilters.buildFiltersGui();
    TM.Events.onTextSearch();
    
    //TM.Gui.AppliedFilters.MapFiltersFromUrl();
}

$(window).bind('hashchange', function () 
    {
        currentPivotPanelFilters = new Array();
        TM.Events.onTextSearch()
        //TM.Gui.AppliedFilters.buildFiltersGui();
        //TM.Gui.AppliedFilters.MapFiltersFromUrl();
    });

TM.Gui.AppliedFilters.MapFiltersFromUrl = function () 
    {    
        var commands = window.location.hash.slice(1).split("&");    
        jQuery.each(commands, function () {
            var splitCommand = this.split(":");
            if (splitCommand.length == 2) {
                var command = splitCommand[0];
                var value = splitCommand[1];
                switch (command) {
                    case "technology":                    
                        setPivotPanelFilter(value, "Technology", 2, true);
                        break;
                    case "phase":                    
                        setPivotPanelFilter(value, "Phase", 3, true);
                        break;
                    case "type":
                        setPivotPanelFilter(value, "Type", 4, true);
                        break;
                    case "category":
                        setPivotPanelFilter(value, "Category", 5, true);
                        break;
                }
            }
        });
    }
		