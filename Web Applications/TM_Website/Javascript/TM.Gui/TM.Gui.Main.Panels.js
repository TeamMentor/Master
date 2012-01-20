TM.Gui.Main.Panels = 
	{
			trace						: true
        ,   div_North                   : 'gui_North'
        ,   div_Center                  : 'gui_Center'
        ,   div_Center_Center           : 'gui_CenterCenter'
        ,   div_Center_North            : 'gui_CenterNorth'
        ,   div_Center_North_Top        : 'gui_CenterNorth_top' 
        ,   div_Center_North_Bottom     : 'gui_CenterNorth_bottom'
        ,   div_West_Bottom             : 'gui_West_bottom'
        ,   div_West_Top                : 'gui_West_top'
        ,   div_CenterCenter            : 'gui_CenterCenter'
        ,   div_East                    : 'gui_East'
        ,   div_TopMenuLinks            : 'TopMenuLinks'

		,	panelsDir 					: '/Html_Pages/Gui/Panels/'
			
		,	openDefaultPanes 			: function()
				{	
					loadPage(this.div_North                 , this.panelsDir + 'Top_Banner.html');								
					loadPage(this.div_Center_North_Top      , this.panelsDir + 'Top_SearchPanel.html');
					loadPage(this.div_Center_North_Bottom   , this.panelsDir + 'AppliedFilters/PivotPanels.html');		
					loadPage(this.div_West_Bottom           , this.panelsDir + 'Left_LibraryTree.html');	
					loadPage(this.div_West_Top              , this.panelsDir + 'AppliedFilters/TopLeft_AppliedFilters.html');							
					loadPage(this.div_East                  , this.panelsDir + 'Right_GuidanceItem.html');
                    loadPage(this.div_CenterCenter          , this.panelsDir + 'Middle_GuidanceItems.html');				

///

                    loadPage('TopMenuLinks',			this.panelsDir + 'TopRight_Links.html');
					
				}
				
		,	setHomePageViewFromUrlHash 	: function()
			{		
				TM.Gui.Main.Panels.applyHomePageView(window.location.hash.slice(1).split("&"));
			}	
	};

TM.Gui.Main.Panels.createLayout = function()
	{			
		var layoutDefaults = 				
			{												
					onclose_start		: function() { TM.Gui.disablePopups = true; }
				, 	onclose_end			: function() { TM.Gui.disablePopups= false }
			};
			
		myLayout = $('#MainTMGui').layout(
			{	
					defaults	:   layoutDefaults 
				,	north		: 	{
											resizable		: false
										,	closable		: false		
										,	spacing_open	: 0	
										,	size			: 78		 								
									}
				,	center		:	{		
											onresize		: TM.Gui.Main.Panels.onGuiResize
		//								,	minWidth		: 100			
									}
				,	east		:	{
											size			: 350							
										,	minSize			: 250
										,	maxSize			: screen.availWidth - 400
									}
				,	west		:	{
											size			: 260
										,	minSize			: 230
										,	maxSize			: screen.availWidth - 400
									}	
			});			
	
		westLayout = $('div.ui-layout-center').layout(
			{	
				defaults: layoutDefaults,
				north:  {
								paneSelector	: ".center-north" 		
							,	size			: 225
							,	minSize			: 150
							,	maxSize			: screen.availHeight - 400
								
						},
				center: {
								paneSelector	: ".center-center"		
							,	resizable		: true													
							,	onresize		: TM.Gui.Main.Panels.onGuiResize
						}											
			});		
							
				
		TM.Gui.Main.Panels.cssFixesForHomePage();
		
	}

TM.Gui.Main.Panels.openAllPages = function()
{
	myLayout.open("west"); 
	westLayout.open("north"); 
}
	
TM.Gui.Main.Panels.onlyShowTableAndGuidanceItem = function()
{
	myLayout.close("west"); 
	westLayout.close("north"); 
	myLayout.sizePane("east","MainTMGui".$().width() /2);
}
	
TM.Gui.Main.Panels.applyHomePageView = function(commands)
	{						
		jQuery.each(commands, function()
				{
					var splitCommand = this.split(":");
					if(splitCommand.length ==2)
					{
						var command = splitCommand[0];
						var value = splitCommand[1];
						switch(command)
						{
							case "showTree":
								if (value =="false")
									myLayout.close("west");
								else
									myLayout.open("west");
								break;
							case "showFilters": 
								if (value =="false")																			
									westLayout.close("north"); 
								else
									westLayout.open("north");
								break;	
							case "centerGuidanceItems":
								if (value =="false")
									myLayout.sizePane("east",350);
								else
									myLayout.sizePane("east","MainTMGui".$().width() /2);	
								break;
							case "loadLibrary":
								TM.InitialLibrary = value;																		
							default:
								//console.log("Unrecognized command ******: " + command);
								break;									
						}
					}						
				});
	}

	
		
TM.Gui.Main.Panels.buildGui = function()
{			    

	TM.Events.raiseProcessBarNextValue("Creating User Interface");					
	TM.Gui.Main.Panels.createLayout();		
		
	if ($.browser.msie && $.browser.version == '7.0')		//wreird IE 7 bug where we need to call this twice for the layouts to be visible
		TM.Gui.Main.Panels.createLayout();		
				
	TM.Events.raiseProcessBarNextValue("Opening Default View");
		
	TM.Gui.Main.Panels.openDefaultPanes();	
		
	TM.Events.raiseProcessBarNextValue("Opening Default View");
	TM.Gui.Main.Panels.setHomePageViewFromUrlHash();		
		
		
	$('#JS_Dialog').html('');
		
		
	if (TM.Debug.gui_LoadLibraryData)
	{		
		TM.Events.onFolderStructureLoaded.add(function()
			{			
				TM.Events.raiseProcessBarNextValue("Loading Library Tree")		
					
				$("#" + TM.Gui.Main.Panels.div_Center).show();
				TM.HomePageLoaded = "TM: HomePage Loaded";					
										
				TM.Events.onMainGuiLoaded();		
				TM.Gui.ShowProgressBar.close();
			});		
				
		TM.Events.raiseProcessBarNextValue("Downloading Data");	
		TM.Events.onFolderStructureLoaded();
	}
	else		
		TM.Gui.ShowProgressBar.close();	

    $(window).bind('hashchange', TM.Gui.Main.Panels.setHomePageViewFromUrlHash);

    TM.Gui.Main.Panels.enableChromeCPUSpikeBugFix();
}

	

				
TM.Gui.Main.Panels.cssFixesForHomePage = function()
	{
        var that = TM.Gui.Main.Panels;
		if ($.browser.msie)
			that.div_North.$().height(78)	
            			
		that.div_CenterCenter.$().css('overflow','hidden')

		that.onGuiResize();
	}
		
//due to the way the layouts sets theses clases, we have to reset these values	
TM.Gui.Main.Panels.onGuiResize = function()
    {		
	    if (TM.Gui.Dialog.isThereAnDialogOpen())
	    {			
		    return ;
	    }
			
	    // enforce center min-width

	    var diff =  $("#" + TM.Gui.Main.Panels.div_Center).width() - 300
	    if (diff < 0)
	    {
		    myLayout.sizePane("east", $("#" + TM.Gui.Main.Panels.div_East).width() + diff)		
		    return ;
	    }	
			
	    "ui-layout-resizer-north".$().width('100%')	
    
	    TM.Gui.Main.Panels.div_Center_North.$().width("100%");	
	    TM.Gui.Main.Panels.div_Center_Center.$().width('100%');	
	
		
	    if (isDefined(TM.showFilter_cssFixes))
		    TM.showFilter_cssFixes();
		
	    //$(".showFiltersPanel").width(750)
	    TM.Gui.Main.Panels.div_Center_North.$().css({ 'overflow-y':'hidden'});
		

	    $(".SearchTextBox").width("center-north".$().width()-150)			
		
	    TM.Gui.DataTable.reDraw();
    }



TM.Gui.Main.Panels.enableChromeCPUSpikeBugFix = function()
    {
        if($.browser.safari)
        {
            TM.Events.onLoginDialogOpen .add( function () { $("#guidanceItemsTable").fadeOut(); }) ;
            TM.Events.onLoginDialogClose.add( function () { $("#guidanceItemsTable").fadeIn(); }) ;


            TM.Events.onSignupDialogOpen .add( function () { $("#guidanceItemsTable").fadeOut(); }) ;
            TM.Events.onSignupDialogClose.add( function () { $("#guidanceItemsTable").fadeIn(); }) ;

        }
    }
;
