TM.Gui.Main.Panels = 
	{
			trace						: true
		,	panelsDir 					: '/Html_Pages/Gui/Panels/'
			
		,	openDefaultPanes 			: function()
				{	
					loadPage('gui_North', 				this.panelsDir + 'Top_Banner.html');			
					loadPage('TopMenuLinks',			this.panelsDir + 'TopRight_Links.html');
					loadPage('gui_CenterNorth_top',	  	this.panelsDir + 'Top_SearchPanel.html');
					loadPage('gui_CenterNorth_bottom',	this.panelsDir + 'AppliedFilters/PivotPanels.html');		
					loadPage('gui_West_bottom', 		this.panelsDir + 'Left_LibraryTree.html');	
					loadPage('gui_West_top',			this.panelsDir + 'AppliedFilters/TopLeft_AppliedFilters.html');		
					loadPage('gui_CenterCenter',		this.panelsDir + 'Middle_GuidanceItems.html');				
					loadPage('gui_East',				this.panelsDir + 'Right_GuidanceItem.html');
					
				}
				
		,	setHomePageViewFromUrlHash 	: function()
			{		
				TM.Gui.Main.Panels.applyHomePageView(window.location.hash.slice(1).split("&"));
			}	
	};
