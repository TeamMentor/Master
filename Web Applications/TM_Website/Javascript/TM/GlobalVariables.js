var traceTMObjects	= 	true;
var isUndefined 	= 	function (object) 		  { return ( typeof (object) 		  === 'undefined'); }
var isDefined 		= 	function (object) 		  { return ( typeof (object) 		  !=  'undefined'); }
var notSet 			= 	function (object,varname) {	return ( typeof (window[varname]) === 'undefined'); }
var mapGlobal   	=   function(object,name)     {	if (notSet(object,name))  object[name] = { trace : traceTMObjects };        }
	
mapGlobal(window, "TM");

mapGlobal(TM, "Gui");
mapGlobal(TM, "QUnit"); 
mapGlobal(TM, "Const");
mapGlobal(TM, "Debug");			// this fails in IE (see below)
mapGlobal(TM, "Events");
mapGlobal(TM, "WebServices");
mapGlobal(TM, "ControlPanel");

//mapGlobal(TM, "WS");			// temp


mapGlobal(TM.WebServices,"Data");
mapGlobal(TM.WebServices,"Config");
mapGlobal(TM.WebServices,"Helper");
mapGlobal(TM.WebServices,"WS_Utils");
mapGlobal(TM.WebServices,"WS_Data");
mapGlobal(TM.WebServices,"WS_Users");
mapGlobal(TM.WebServices,"WS_Libraries");

mapGlobal(TM.Events,"Gui");

mapGlobal(TM.Gui, "GuidanceItemViewer");
mapGlobal(TM.Gui, "GuidanceItemEditor");
mapGlobal(TM.Gui, "AppliedFiltersList");
mapGlobal(TM.Gui, "DataTableViewer");
mapGlobal(TM.Gui, "AppliedFilters");
mapGlobal(TM.Gui, "TopRigthLinks");
mapGlobal(TM.Gui, "CurrentUser");
mapGlobal(TM.Gui, "TextSearch");
mapGlobal(TM.Gui, "DataTable");
mapGlobal(TM.Gui, "Dialog");
mapGlobal(TM.Gui, "Main");

//Extra IE mappings
if (typeof(TM.Debug) == "undefined")
	TM.Debug = {};

	
//Global vars & Constants

TM.Const.emptyGuid = "00000000-0000-0000-0000-000000000000";
TM.Const.EmptyFunction = function() {};
TM.WebServices.Data.lastDataTableData = { aoColumns : [] , aaData: [] }	
TM.WebServices.Data.filteredDataTable = { aoColumns : [] , aaData: [] }	

;
/*TM.ImageCache = function()
	{		
		imgCache = null;
		var loadedImages = [];
		
		return 	{
					setupCache		: function()
						{
							imgCache = $("<div>").attr('id', 'img-cache')
												 .append('<h2>Image Cache</h2>')
												 .hide();
							$('body').append(imgCache);											    														
						},
					loadImage		: function(path)
						{
							var image = $('<img />').attr('src',path);
							imgCache.append(image);
							loadedImages.push(image);							
						},
					get_ImgCache	: function() { return imgCache; },
					get_LoadedImages: function() { return loadedImages; }
				};
	}();	
*/	