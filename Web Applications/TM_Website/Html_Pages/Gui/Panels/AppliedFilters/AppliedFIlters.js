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
		
		TM.Events.onBuildFiltersGui()
	}




function buildPivotPanel(pivotPanelData, hostElement)
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
		var selectedFilter = arrayWithSelectedItems[index];		
		
		//items Checked	
		if (typeof(selectedFilter) == "undefined" ||  selectedFilter== null)
			selectedFilter = "";				
		if 	(selectedFilter=="")		
			var itemsChecked = [];		
		else				
			var itemsChecked = selectedFilter.split('|');
		
		var itemsAvail = []; 	
		//var itemsNotAvail = getDistictColumnValue(arrayWithAllData,index); 
		var itemsNotAvailRaw = getDistictColumnValue(arrayWithAllData,index); 
		
		//_itemsChecked = itemsChecked;
		$.each(getDistictColumnValue(arrayWithFilteredData,index).sort(), function(index,value)
			{
				//var checked = itemsChecked.indexOf(value) > -1;
				var checked = itemsChecked.has(value);//.indexOf(value) > -1;
				itemsAvail.push({ text : value , checked : checked  });
				itemsNotAvailRaw = removeFromArray(itemsNotAvailRaw, value);
			});
		
		var itemsNotAvail = [];
		itemsNotAvailRaw.sort();
		$.each(itemsNotAvailRaw, function(index,value)
			{				
				var checked = itemsChecked.has(value);				
				itemsNotAvail.push({ text : value , checked : checked  });				
			});
		
		//currentFilter
		var filterDataObject = 
			{
				title:title,
				column: index,				
				itemsAvail: itemsAvail, 
				itemsNotAvail: itemsNotAvail
			}
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

TM.Gui.AppliedFilters.showFilters = function(arrayWithSelectedItems, arrayWithAllData, arrayWithFilteredData, hostElement)
	{				
		//console.log("-------------showFilters	-------- for: " + arrayWithAllData.length);
		$("#pivotPanels input").parent().css('opacity', '0.5')
		
		var startTime = new Date();		
		TM.abortTableDataLoad = true;
		if (updatingFilters)
		{
			setTimeout(function() 
				{
					console.log("after wait: updatingFilters");
					TM.Gui.AppliedFilters.showFilters(arrayWithSelectedItems, arrayWithAllData, arrayWithFilteredData, hostElement)
				}, 200);;
			console.log("updatingFilters");
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
		
			//apply the text filter = 
			var aaData = TM.Gui.AppliedFilters.applyDataTableFilter_using_PivotPanelFilters(TM.WebServices.Data.lastDataTableData.aaData, queryTo_filterDataTable , currentFilters );		
		
			//TM.showFilter_cssFixes();		
		
		setTimeout(function(){
		
			TM.WebServices.Data.filteredDataTable = {};
			TM.WebServices.Data.filteredDataTable.aoColumns = TM.WebServices.Data.lastDataTableData.aoColumns;		
			TM.WebServices.Data.filteredDataTable.aaData = aaData
			
			
			//raise event			
			updatingFilters = false;						
			TM.Debug.TimeSpan_Gui_AppliedFilters_ShowFilters = startTime.toNow();
			TM.Events.onAppliedFieldsEnd();					
			
			//TM.Events.onDisplayDataTable();
			//TM.WebServices.Data.filteredDataTable);
			
		},timeOutInterval)
		},timeOutInterval)
		},timeOutInterval)
		},timeOutInterval)
		},timeOutInterval)
		},timeOutInterval)
		},timeOutInterval);
		
	}

TM.Gui.AppliedFilters.buildFiltersGui = function()	
{
	TM.Events.onInvalidateSearchText();	
	
	var aaData = TM.WebServices.Data.lastDataTableData.aaData;	
	var filterResult  = TM.Gui.AppliedFilters.applyDataTableFilter_using_PivotPanelFilters(aaData, queryTo_filterDataTable , currentFilters );				
	TM.Gui.AppliedFilters.showFilters(currentFilters, aaData, filterResult);				
}	

TM.Gui.AppliedFilters.buildFromSelectedNodeId = function()
	{
		currentPivotPanelFilters 			= new Array(); 		// reset applied filters
		currentFilters 						=[];		
		
		var selectedNodeId = TM.Gui.selectedNodeId;
		TM.WebServices.Data.getGuidanceItemsInGuid_For_DataTable(selectedNodeId);	
		TM.Gui.AppliedFilters.buildFiltersGui();
	}
		