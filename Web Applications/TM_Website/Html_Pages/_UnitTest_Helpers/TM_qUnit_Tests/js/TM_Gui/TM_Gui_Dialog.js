module("TM.Gui.Dialog");

//humm jquery load doesn't support spaces (the one below doesn't work
//jQuery("#Canvas").load("/Html_Pages/Gui/Dialogs/Dialog with not a lot of content.html")
//but this does (with url encode)
var testPage = "/Html_Pages/Gui/Dialogs/Dialog%20with%20not%20a%20lot%20of%20content.html";
var expectedContent = "This is an html dialog with not a lot of content :)";
var testPause = 0;//100;

test("Check TM.Gui.Dialog object", function() 
	{	
		ok(TM.Gui							, "TM.GUI was set");
		ok(TM.Gui.Dialog					, "TM.Gui.Dialog was set");	
		ok(TM.Gui.Dialog.version			, "TM.Gui.Dialog.version was set");	
		ok(TM.Gui.Dialog.defaultTargetDiv	, "TM.Gui.Dialog.defaultTargetDiv was set");			
		equals(TM.Gui.Dialog.version, 1		, "TM.Gui.Dialog.version was the expected one");				
		
		ok(TM.Gui.Dialog.loadPage 			, "loadPage method was there");
		ok(TM.Gui.Dialog.loadDialog			, "loadDialog method was there");
		ok(TM.Gui.Dialog.showUserMessage	, "showUserMessage method was there");
		ok(TM.Gui.Dialog.showMessage_NotEnoughPriviledges	, "showMessage_NotEnoughPriviledges method was there");
		
		ok(TM.Gui.Dialog.loginPage 			, "loginPage method was there");
	});

asyncTest("loadPage: testPage into TM.Gui.Dialog.defaultTargetDiv ", function()
	{					
		$(TM.Gui.Dialog.defaultTargetDiv).remove()
		ok($(TM.Gui.Dialog.defaultTargetDiv).length ==0, "TM.Gui.Dialog.defaultTargetDiv was not there");		
		TM.Gui.Dialog.loadPage(testPage, undefined, function()
			{				
				ok($(TM.Gui.Dialog.defaultTargetDiv).length ==1, "TM.Gui.Dialog.defaultTargetDiv was there");
				var htmlContent = $(TM.Gui.Dialog.defaultTargetDiv).html();
				equal(htmlContent,expectedContent, "expected content was there"); 
				$(TM.Gui.Dialog.defaultTargetDiv).remove()
				ok($(TM.Gui.Dialog.defaultTargetDiv).length == 0, "TM.Gui.Dialog.defaultTargetDiv is not there again");
				start();				
			});  		
	});
	
asyncTest("loadPage: testPage into #Canvas", function()	
	{		
		var testPage = "/Html_Pages/Gui/Dialogs/Dialog%20with%20not%20a%20lot%20of%20content.html";
		var targetDiv = "#Canvas";
		$(targetDiv).html('');
		equal($(targetDiv).html(), "", "targetDiv was empty");
		TM.Gui.Dialog.loadPage(testPage, targetDiv, function()
			{
				notEqual($(targetDiv).html(), "", "targetDiv was not empty");
				equal($(targetDiv).html(), expectedContent,"expected content was there"); 
				start();
			});				
	});
	
asyncTest("loadDialog: testPage", function()	
	{		
		var testPage = "/Html_Pages/Gui/Dialogs/Dialog%20with%20not%20a%20lot%20of%20content.html";
		var dialogDiv = TM.Gui.Dialog.defaultTargetDiv;
		ok($(dialogDiv).length ==0, "TM.Gui.Dialog.defaultTargetDiv was not there");
		//var targetDiv = "#Canvas";
		TM.Gui.Dialog.loadDialog(testPage, undefined, function()
			{				
				equal($(dialogDiv).html(), expectedContent,"expected content was there"); 				
				//add a small delay so that we get a visual clue that all is working ok
				setTimeout(function () 
					{					
						$(dialogDiv).remove();
						ok($(dialogDiv).length ==0, "TM.Gui.Dialog.defaultTargetDiv was not there");
						start();								
					}, testPause);											
				
			});							
	});	

asyncTest("loadPage: login Page - directly", function()		
	{
		var loginPage = "/Html_Pages/Gui/Dialogs/Login.Html";
		var targetDiv = "#loginPage";
		ok($(targetDiv).length ==0, "TM.Gui.Dialog.defaultTargetDiv was not there");
		TM.Gui.Dialog.loadPage(loginPage, targetDiv, function()
			{
				ok($(targetDiv).length == 1, "TM.Gui.Dialog.defaultTargetDiv was there");
				ok($(".ui-dialog").length == 1, "Dialog was there");
				
				setTimeout(function () 
					{					
						$(".ui-dialog").remove();
						ok($(".ui-dialog").length == 0, "Dialog was not there");
						start();
					}, testPause);											
			});
	});
	
asyncTest("loadPage: login Page ", function()			
	{
		TM.Gui.CurrentUser.loadUserData = function(userdata)
			{
			};
			
		TM.WebServices.WS_Users.login = function(username,password, callback)
			{
				var testGuid = TM.Const.emptyGuid.replace("0000","1234")								
				callback(testGuid);
				//callback(TM.Const.emptyGuid);
			}
			
		var onLoadCallback = function()
			{
				$('#UsernameBox').val('username');
				$('#PasswordBox').val('password');
				$("button").click();				
				
			};			
		var onCloseCallback = function()
			{
				start();
			};				
		TM.Gui.Dialog.loginPage(onLoadCallback, onCloseCallback);		
	});