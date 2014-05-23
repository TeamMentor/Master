$.fn.dataTableExt.oApi.fnFilterClear  = function ( oSettings )
{	
	/* Remove global filter */
	oSettings.oPreviousSearch.sSearch = "";
	
	/* Remove the text of the global filter in the input boxes */
	if ( typeof oSettings.aanFeatures.f != 'undefined' )
	{
		var n = oSettings.aanFeatures.f;
		for ( var i=0, iLen=n.length ; i<iLen ; i++ )
		{
			$('input', n[i]).val( '' );
		}
	}
	
	/* Remove the search text for the column filters - NOTE - if you have input boxes for these
	 * filters, these will need to be reset
	 */
	for ( var i=0, iLen=oSettings.aoPreSearchCols.length ; i<iLen ; i++ )
	{
		oSettings.aoPreSearchCols[i].sSearch = "";
	}
	
	/* Redraw */
	oSettings.oApi._fnReDraw( oSettings );	
	return this;								//DC so that we can do method chaining
}
//var _fnFilter = $.fn.dataTableExt.oApi.fnFilter;
//jQuery.fn.dataTable().fnFilter.prototype
/*$.fn.dataTableExt.oApi.fnFilterEx  = function ( oSettings, filter, index )
{
	//alert('in filter:' + this);
	//alert('in filter:' + oSettings);
	//alert('in filter:' + filter);
	this.fnFilter(filter, index)
	//_fnFilter.apply(this,arguments);
	return this;
}*/