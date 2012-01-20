//Global events
TM.Events = 
	{
			trace				: true
		,	_target				: 'body'
		, 	_eventCount			: 0
		,	_registerEvents		: function(eventNames)
									{
										$.each(eventNames, function(index, value) { TM.Events._register(value) })
									}
		,	_register			: function(name, callback)
									{
										if(isUndefined(callback))
										{											
											$(TM.Events._target).bind(name, function() { } );		// add an empty function
											TM.Events._events(name).pop();							// add remove it so that we get the entry with no function
										}
										else
											$(TM.Events._target).bind(name, callback);
											
										TM.Events[name] 	= function() 
																{ 
																	TM.Events._raise(name) 
																} ;
										TM.Events[name].add = function(_callback)
																{
																	$(TM.Events._target).bind(name, _callback);
																}
										TM.Events[name].remove = function()
																{	
																	TM.Events._remove(name);
																}
										TM.Events[name].events = function()
																{	
																	return TM.Events._events(name);
																}						
										TM.Events[name].trace	= false;						
									}		
		, 	_remove				: function(name)
									{
										$(TM.Events._target).unbind(name);
									}
		, 	_raise				: function(name)
									{
										TM.Events._eventCount++;
										if(TM.Debug.logLoadedEvents)
											console.log("event #{0} : {1}".format(TM.Events._eventCount, name));
										setTimeout(function()
											{
												//$(TM.Events._target).trigger(name); // this throws error in IE for events across popup windows
												if (TM.Events[name].events() != null)
													$.each(TM.Events[name].events(),  function()         
														{          
															this.handler();
														});
											} , 
											20);											
									}
		, 	_allEvents			: function()
									{
										return $("body").data("events");
									}
		, 	_allEvents_List		: function(onlyShowEventsWithHandlers)
									{
										var eventsList = [];
										$.each(TM.Events._allEvents(), function(name, handler) 
												{ 
													if(onlyShowEventsWithHandlers != false || handler.length > 0)
														eventsList.push("{0} \t\t [{1} event(s)]".format(name, handler.length))
												}); 
										return eventsList.join('\n');
									}
		, 	_events 			: function(eventName)
									{
										var allEvents = TM.Events._allEvents();
										if (isUndefined(name))
											return allEvents;
										var match = null;	
										$.each(allEvents, function (name, handler) 
											{ 												
												if (name ===eventName)
												{
													match = handler;
													return false;
												}
											})												
										return match;
									}					
	}

TM.Events._add = TM.Events._register;	


TM.Events._eventsFor_Gui = 
		[
				'onRefreshGuiData'							// to signal that the GUI data should be refreshed
			,	'onAppliedFieldsEnd'						// when the AppliedFields table is built and shown
			,	'onResize'
			,	'onEditModeChange'
			,	'onDisplayDataTable'						// to be set by the panel that wants to show this 			
			,	'onDataTableDisplayed'						// after the data table is displayed
			, 	'onLibraryTreeSelected'						// where there is a new selected node 
			,	'onLibraryTreeLoaded'						// when the library tree is loaded
			,	'onBuildFiltersGui'							
			,	'onMainGuiDivAvailable'						// when the main gui div is available		
			,	'onMainGuiLoaded'							// when the main gui is loaded (not when the entire GUI is ready)
			,	'onHomePageLinksLoaded'						// when the top rigth links are loaded
			,	'onLoginDialogOpen'							// when the login dialog is openeded
			,	'onLoginDialogClose'						// when the login dialog is closed
            ,   'onSignupDialogOpen'
            ,   'onSignupDialogClose'
			,	'onUserDataLoaded'							// called after we refreshed the current user data
			,	'onUserChange'								// called when the current user changes (login or logout)			
			, 	'onShowGuidanceItem'						// when we want to show a guidanceItem
			,	'raiseProcessBarNextValue'
//			,	'raiseWebServiceReceivedData'
			,	'onMainGuiScriptsLoaded'
			,	'onGuiObjectsLoaded'
			,	'onFolderStructureLoaded'			
			,	'onFolderStructureMapped'
			,	'onFiltersRemoved'
			,	'onTextSearch'
			,	'onInvalidateSearchText'
			
		];

TM.Events._eventsFor_Libraries = 
		[
				
				'onNewLibrary'								// called when there was a new library added to the Database
			,	'onNewFolder'								// called when there was a new folder added to the Database
			,	'onNewView'									// called when there was a new view added to the Database
			,	'onRemovedLibrary'							// called when there was a library was removed from the Database				
			,	'onRemovedFolder'							// called when there was a folder was removed from the Database
			,	'onRemovedView'								// called when there was a view was removed from the Database
			,	'onRenamedLibrary'							// called when there was a library was renamed from the Database
			,	'onRenamedFolder'							// called when there was a folder was renamed from the Database
			,	'onRenamedView'								// called when there was a view was renamed from the Database
		]

TM.Events._eventsFor_ControlPanel = 
		[
				'onControlPanelScriptsLoaded'
			,	'onControlPanelGuiLoaded'
			,	'onControlPanelViewLoaded'
			,	'onCreateUsers'
			,	'onFileUploaded'
		]	
		
		
$(function()
	{
		TM.Events._registerEvents(TM.Events._eventsFor_Gui);	
		TM.Events._registerEvents(TM.Events._eventsFor_Libraries);
		TM.Events._registerEvents(TM.Events._eventsFor_ControlPanel);
		TM.Events._register('eventTest');		


		//two special cases for data loaded
		TM.Events.onFolderStructureLoaded.add(function()	
			{ 				
				TM.WebServices.Data.extractFolderStructure()				
			});
		TM.Events.onGuiObjectsLoaded.add(function() 	
			{ 
				TM.WebServices.Data.extractGuiObjects(); 						
			});
	});

	
/*TM.Events.raiseProcessBarNextValue		= function(message)	{ } // to be used to let progress bars know about a new even
TM.Events.raiseWebServiceReceivedData 	= function ()			{ }  // when there is data received from a webservice

TM.Events.onMainGuiScriptsLoaded	= function()			{}
TM.Events.onGuiObjectsLoaded 	  	= function(callback) 	{ $(function() { TM.WebServices.Data.extractGuiObjects(callback); 		}); };
TM.Events.onFolderStructureLoaded 	= function(callback)	{ $(function() { TM.WebServices.Data.extractFolderStructure(callback); 	}); };
*/
/*TM.Events.onDisplayDataTable 	  	= function() 			{} // to be set by the panel that wants to show this 
TM.Events.onDataTableDisplayed 	  	= function() 			{} // after the data table is displayed
TM.Events.onLibraryTreeLoaded		= function() 			{} // when the library tree is loaded
TM.Events.onBuildFiltersGui		    = function()			{} 
TM.Events.onMainGuiDivAvailable		= function()			{} // when the main gui div is available
TM.Events.onMainGuiLoaded			= function()			{} // when the main gui is loaded (not when the entire GUI is ready)
TM.Events.onHomePageLinksLoaded		= function()			{} // when the top rigth links are loaded
TM.Events.onLoginDialogOpen			= function()			{} // when the login dialog is visible
*/
	
/*TM.Events.onAppliedFieldsEnd  		= function() 			{} // when the AppliedFields table is built and shown
TM.Events.onNewUser 	  			= function() 			{} // called when there is a new user
TM.Events.onNewLibrary 	  			= function() 			{} // called when there was a new library added to the Database
TM.Events.onNewFolder				= function() 			{} // called when there was a new folder added to the Database
TM.Events.onNewView					= function() 			{} // called when there was a new view added to the Database
TM.Events.onRemovedLibrary 	  		= function() 			{} // called when there was a library was removed from the Database
TM.Events.onRemovedFolder			= function() 			{} // called when there was a folder was removed from the Database
TM.Events.onRemovedView				= function() 			{} // called when there was a view was removed from the Database
TM.Events.onRenamedLibrary			= function() 			{} // called when there was a library was renamed from the Database
TM.Events.onRenamedFolder			= function() 			{} // called when there was a folder was renamed from the Database
TM.Events.onRenamedView				= function() 			{} // called when there was a view was renamed from the Database
*/

//Control Panel events and Variables
//TM.Events.onControlPanelScriptsLoaded	= function()		{}
//TM.Events.onControlPanelGuiLoaded		= function()		{}
//TM.Events.onControlPanelViewLoaded  	= function()		{}
//TM.Events.onCreateUsers					= function()		{}
//TM.Events.onFileUploaded				= function()		{};

//TM.Gui.DataTable.reDraw				= function()			{}		// on resize events


//Other globally called functions
TM.Gui.Dialog.showUserMessage		= function(message) 	{ console.log("TM USER MESSAGE:" + message); }  // to be overriten by a GUI class
TM.Gui.showUserMode					= function()			{}					// used to render UserMode
TM.Gui.showEditMode					= function()			{}					// used to render EditMode
TM.Gui.Dialog.isThereAnDialogOpen   = function()			{ return false; }	

;