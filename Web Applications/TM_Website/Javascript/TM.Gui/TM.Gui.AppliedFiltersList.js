currentPivotPanelFilters = new Array(); 

TM.Gui.AppliedFiltersList.removeFilters = function()
	{		
		currentFilters =[];
		currentPivotPanelFilters = new Array(); 
		TM.Gui.AppliedFiltersList.populateAppliedFiltersTable() ;	
		TM.Events.onFiltersRemoved()
	}

TM.Gui.AppliedFiltersList.removeCriteraFromCriteriaCollection = function(text,title, column, state)
	{

		setPivotPanelFilter(text, title, column, false);
	}
	
TM.Gui.AppliedFiltersList.clear_FiltersGui = function()
	{
		$("#AppliedFilterItems").html('');
	}
	
TM.Gui.AppliedFiltersList.add_Filter = function(text, title, column, showDeleteButton)
	{
		var filterDiv = 	  $("<div class='AppliedFilterItem'>");
		var deleteImg = 	  $("<img src='/Images/DeleteFilterIcon.png' alt='Delete View = Database Security' class='DeleteButton' />");
		var filterItemImage = $("<img id='ctl00_ContentPlaceHolder1_AppliedFilters1_AppliedFiltersListView_ctrl0_Image1' src='/Images/SingleLibrary.png' style='border-width:0px;' />"); 
		//filterItemText  = $("<span id='ctl00_ContentPlaceHolder1_AppliedFilters1_AppliedFiltersListView_ctrl0_Label1'>");
		
		filterItemText = $("<span>").append(" {0} = {1}".format(title, text));		
		deleteImg.click(function()
			{				
				TM.Gui.AppliedFiltersList.removeCriteraFromCriteriaCollection(text, title, column, false);
			})

		filterDiv.append(filterItemImage);
		filterDiv.append(filterItemText);
		if(showDeleteButton)
			filterDiv.append(deleteImg)		
		
		$("#AppliedFilterItems").append(filterDiv);
	}

TM.Gui.AppliedFiltersList.populateAppliedFiltersTable = function ()
{		
	if (isUndefined(TM.Gui.selectedNodeData))
		return;
	TM.Gui.AppliedFiltersList.clear_FiltersGui();
	
//	TM.Gui.AppliedFiltersList.add_Filter(TM.Gui.selectedNodeData.name, TM.Gui.selectedNodeData.type, -1 , false);
	
	$.each(currentPivotPanelFilters, function()
		{			
			TM.Gui.AppliedFiltersList.add_Filter(this.text, this.title, this.column, true);
		})
		
	if (TM.Gui.DataTable.currentTextFilter != "") //escape
		TM.Gui.AppliedFiltersList.add_Filter(htmlEscape(TM.Gui.DataTable.currentTextFilter), 'Search', -1 , true);
		
	TM.Gui.AppliedFiltersList.fixCSS_appliedFilters();
}

TM.Gui.AppliedFiltersList.fixCSS_appliedFilters = function()
{	
	try
	{
		if ( $.browser.msie )
		{
			//"AppliedFilterHeader".$().height(32);		
			"AppliedFilterHeader".$().height(26);
			"ctl00_ContentPlaceHolder1_AppliedFilters1_RemoveFiltersImage".$()
				.absolute()
				.right(0)
				.top(5);
		}
		$("#AppliedFiltersPanel").width("97%")	
		/*}	
		else
		{
			"AppliedFiltersPanel".$().width('95%');
		}*/
	}
	catch(message) {}	
}



function htmlEscape(str) 
{
    return String(str)
            .replace(/&/g, '&amp;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;');
}