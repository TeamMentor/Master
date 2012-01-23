//TM Settings
TM.tmVersion = "TM 3.0 (23 Jan)";	

TM.tmWebServices = '/Aspx_Pages/TM_WebServices.asmx/';

TM.Gui.windowsLoginEnabled 						= false;

TM.Debug.gui_LoadLibraryData					= true
TM.Debug.addTimeStampToLoadedPages 			  	= true;
TM.Debug.reuseLibraryMappingsObject 		  	= true;
TM.Debug.load_HomePage 						  	= true;

TM.Debug.showExperimentalFeatures				= false;

TM.Gui.editMode			   						= false;
TM.Debug.show_DebugDiv						  	= false;	// show small box with links to open trace view
TM.Debug.show_TraceView							= false; 	// show call trace popup window
TM.Debug.callTrace_LoadEnvironment 			  	= false;	// open traceview popup and setup call trace hooks
TM.Debug.callTrace_LoadEnvironment_Before_Gui 	= false;
TM.Debug.callTrace_ShowExecutionTime		  	= false;
TM.Debug.callTrace_LogToConsole				  	= false;
TM.Debug.callTrace_ShowParamsInConsoleLog	  	= false;

TM.Debug.logLoadedEvents					  	= false;
TM.Debug.logLoadedPages						  	= false;

//only put real passwords here in QA environments
TM.QUnit.defaultPassord_Admin                   = "!!tmbeta";
TM.QUnit.defaultPassord_Editor                  = "!!tmbeta";
TM.QUnit.defaultPassord_Reader                  = "!!tmbeta";

//Individual trace options
TM.WebServices.Helper.trace						= false;
TM.Gui.AppliedFilters.trace						= false;
TM.WebServices.trace							= false;