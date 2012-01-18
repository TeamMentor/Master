/*this is now all on TM.Gui.Dialog , so it should be removable from the main GUI*/

function loadPage(target, page, callback)
{				
	if (TM.Debug.logLoadedPages)
		console.log("DebugLog: opening page " + page + " in div " + target);
		
	if (isDefined(target.$()))
	{
		if (TM.Debug.addTimeStampToLoadedPages)
			page = page + '?time='+ new Date().getTime();
		target.$().load(page, callback);				
	}
	else
		console.log("Error: in loadPage, could not find target: " + target);
}

function loadDialog(page, target, callback)
{				
	if (typeof(target == "undefined"))
		target = '#JS_Dialog';
	target.$().load(page + '?time='+ new Date().getTime(), callback);
}


var showMessage_NotEnoughPriviledges = function()
{
	showUserMessage("You don't have enough priviledges to make this action");
}

var showUserMessage = function(message)
{
	TM.Gui.Dialog.alertUser(message);
/*		var div = "<div>{0}</div>".format(message).$();
	return div.dialog(
		{ 
			title:'user message...' , 
			modal:true, 
			height:150 
		} )*/
}