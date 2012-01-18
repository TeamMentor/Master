TM.Gui.Dialog = 
	{	
		// variables
		version : 1 , 
		defaultTargetDiv: '#TM_Gui_Dialog',
		
		loadPage: 			function (page, target, callback)
								{										
									target = this.ensureTargetExists(target);										
									var pageToLoad = page + '?time='+ new Date().getTime();									
									if($(target).length ==1)
										$(target).load(pageToLoad, callback);			
									else
										callback(); //something went wrong since the div was not created									
								},

		loadDialog: 		function (page, target, callback)
								{											
									target = this.ensureTargetExists(target);
									this.loadPage(page,target, function() 
													{
														$(target).dialog();
														callback();
													})
									
								},
		
		ensureTargetExists: function(target)
								{									
									if ($(target).length ==0)
									{										
										if (typeof(target) == "undefined")
											target = this.defaultTargetDiv;										
										var htmlDiv = "<div id='{0}'></div>".format(target.substr(1));										
										$("body").append(htmlDiv);
									}
									return target
								},
		
		showUserMessage:	function(message, callback, height, width)
								{								
									if (isUndefined(height))
										height = 150;
									if (isUndefined(width))
										width = 400;	
									var div = $("<div>{0}</div>".format(message));
									var userMessageDialog =  div.dialog(
										{ 
												title:'user message' 
											,	modal:true											
											,	height	: height 
											,	width	: width
											,	buttons	: 	{ 												
																"Ok": function() 
																		{ 
																			userMessageDialog.remove();
																			if (isDefined(callback))
																				callback()
																		}
															}
										} )
									return userMessageDialog;
								},
								
		askYesOrNoQuestion:	function(title, question, onYes, onNo, width, height )
								{
									if (isUndefined(height))
										height = 130;
									if (isUndefined(width))
										width = 400;										
									var div = $("<div>").text(question);
									var yesNoDialog = div.dialog(
										{ 
											title	:title , 
											modal	:true, 
											buttons	: 	{ 
															"No" : function() { yesNoDialog.remove() ; onNo() },
															"Yes": function() { yesNoDialog.remove() ; onYes() }
														},
											height	: height ,
											width	: width
											
										} )										
									return yesNoDialog;
								},
								
		deleteConfirmation:	function(description, onYes, width, height)
								{
									return TM.Gui.Dialog.askYesOrNoQuestion(
															"Delete confirmation", 
															"Are you sure you want to delete " + description, 
															onYes, 
															function() {  }
															,360 + description.length*5, height);
								},
								
		actionConfirmation:	function(description, onYes, width, height)
								{
									return TM.Gui.Dialog.askYesOrNoQuestion(
															"Action confirmation", 
															"Are you sure you want to " + description, 
															onYes, 
															function() {  },
															width, height);
								},						
								
		showMessage_NotEnoughPriviledges: function()
								{
									showUserMessage("You don't have enough priviledges to make this action");
								},
								
		isThereAnDialogOpen	: function()
								{
									return $(".ui-widget-overlay").length ==1;
								},
								
		alertUser			: function(message, title, timeout)
								{
									if(isUndefined(timeout))
										timeout = 3000;
									$.Growl.show(message, 
										{
											"icon": false,
											"title": title,
											"cls": "",
											"speed": 1000,
											"timeout": timeout
										});
												
								}
								
	}
	
//TM specific dialogs
TM.Gui.Dialog.loginPage = function()
	{
		var loginPage = "/Html_Pages/Gui/Dialogs/Login.Html";
		var targetDiv = "#loginPage";
				
		TM.Gui.Dialog.loadPage(loginPage, targetDiv);
		/*, function()
			{
				if (isDefined(onLoadCallback))
					TM.Events.onUserChange.add(onCloseCallback);
				if (isDefined(onLoadCallback))
					onLoadCallback();				
			});*/
	};
