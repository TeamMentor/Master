
//TM.Gui.notAuthorizedMessage = "You must be logged in to view this page. Please <a href='javascript:loginPage()'>login</a> or <a href='javascript:signupPage()'>sign up</a>.";
TM.Gui.Main.targetDiv = '#guiWithPanels';

TM.Gui.Main.homePage = function(targetDiv)
	{
		TM.Gui.Main.showLoadingImageOnAjaxStartAndStop();
		if (isUndefined(targetDiv))
			targetDiv = TM.Gui.Main.targetDiv
		else
			TM.Gui.Main.targetDiv = targetDiv;		
		loadPage(targetDiv,'/Html_Pages/Gui/TM_3_with_Panels.html');	
	}	

/*TM.Gui.Main.loadLibraryTree = function()
	{	
		loadPage('gui_West_bottom', TM.Gui.Main.Panels.panelsDir + 'Left_LibraryTree.html');	
	}
*/

TM.Gui.Main.showLoadingImageOnAjaxStartAndStop = 	function()	
	{
		var loadingImg = $("<img>").attr("src", '/Javascript/jQuery.jsTree/themes/default/throbber.gif')
								   .zIndex(40).absolute().right(10).top(40)
								   .hide();
		loadingImg.appendTo("body")
				  .ajaxStart(function() { loadingImg.show()})
				  .ajaxStop(function() { loadingImg.hide()});;
	}

TM.Gui.changeMode = function(editMode)
	{
		if (TM.Gui.editMode != editMode)
		{			
			TM.Gui.editMode = editMode;		
			//TM.Gui.Main.loadLibraryTree();
			//loadPage('gui_West_bottom',TM.Gui.Main.Panels.panelsDir + 'Left_LibraryTree.html');	
			//TM.Gui.TopRigthLinks.refresh();
			TM.Events.onEditModeChange();			
		}
	}
	
TM.Gui.showEditMode = function() { 	TM.Gui.changeMode(true );  	}	
TM.Gui.showUserMode = function() {	TM.Gui.changeMode(false);	}
;