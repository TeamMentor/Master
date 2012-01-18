//Generic Arrays
 

function getDistictColumnValue(arrayWithData, columnIndex)
{
	return jlinq.from(jlinq.from(arrayWithData)
						   .select(function(rec) { return rec[columnIndex] }))
				.distinct();				
}

TM.Gui.AppliedFilters.applyDataTableFilter = function(targetArray, queryFunction, newFilter)
{
	if (typeof(newFilter) != "undefined")
		currentFilter = newFilter;	
	if (isDefined(targetArray))
		return jlinq.from(targetArray).where(queryFunction).select();
}


TM.Gui.AppliedFilters.applyDataTableFilter_using_PivotPanelFilters = function (targetArray, queryFunction, newFilter)
{
	return TM.Gui.AppliedFilters.applyDataTableFilter(targetArray, queryFunction, newFilter);	
}

function removeArrayFromArray(sourceArray, toRemoveArray)
{
	return jlinq.from(sourceArray)
				.where(function(rec) 
					{
						asd = toRemoveArray;
						if (typeof(rec) != "undefined" && rec !="" && rec != null)
							return ! toRemoveArray.has(rec);
						return true;
					}) 
				.select()
}

function removeFromArray(sourceArray, itemToRemove)
{
	return jlinq.from(sourceArray)
				.where(function(rec) 
					{						
						if (typeof(rec) != "undefined" && rec !="" && rec != null)
							return rec != itemToRemove;
						return true;
					}) 
				.select()
}



//linq query to filter database
queryTo_filterDataTable = function (rec)
{		
	if (currentFilter =="")
		return true;
   var expectedMatches = 0;
   var matches = 0;
	for(var i = 2 ;  i < 6 ; i++)
	{			
		 var  filter = currentFilter[i];
		 if (typeof(filter) != "undefined" && filter !="" && filter != null)
		 {			
			expectedMatches ++
			$.each(filter.split('|'), function(index , subFilter)
			{
				if (rec[i] == subFilter)
					matches ++;
			});			
		 }
	}  
	return expectedMatches  == matches ;
}

//extensionMethod

Array.prototype.has=
	function(v,i)
	{
		for (var j=0;j<this.length;j++)
		{
			if (this[j]==v)	
				return true;
		}
		return false;
	}	