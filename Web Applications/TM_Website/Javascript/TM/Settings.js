//TM Settings
TM.tmVersion            = "TM 3.3";
TM.ArticleTitle         = "TeamMentor 3.3";

TM.tmWebServices        = '/Aspx_Pages/TM_WebServices.asmx/';
TM.NotAuthorizedPage    = '/Html_Pages/Gui/Panels/AD_Non_Authorized_User.html';

TM.Tracking_Google_Analytics_ID                 = null;    // configure this in  TM_Custom_Settings.js
TM.Gui.showLibraryStructureToAnonymous          = true;
TM.Gui.LoadLibraryData                          = true
TM.Gui.editMode                                 = false;

TM.Debug.addTimeStampToLoadedPages              = true;     // set true true during dev
TM.Debug.reuseLibraryMappingsObject             = true;
TM.Debug.load_HomePage                          = true;

TM.Debug.showExperimentalFeatures               = false;
TM.Debug.allow_start_checkUserLoop              = true;    // use to prevent the auto check of user settings 


TM.Debug.show_DebugDiv                          = false;  // show small box with links to open trace view
TM.Debug.show_TraceView                         = false;  // show call trace popup window
TM.Debug.callTrace_LoadEnvironment              = false;  // open traceview popup and setup call trace hooks
TM.Debug.callTrace_LoadEnvironment_Before_Gui   = false;
TM.Debug.callTrace_ShowExecutionTime            = false;
TM.Debug.callTrace_LogToConsole                 = false;
TM.Debug.callTrace_ShowParamsInConsoleLog       = false;

TM.Debug.logEventsRaised                        = false;    // set to view the events fired in the console
TM.Debug.logEventsRaised_CallTrace              = false;    // will also show the CallTrace of the logged calls
TM.Debug.logLoadedPages                         = false;

//only put real passwords here in QA environments
TM.QUnit.defaultUser_Admin                      = "qa_admin";
TM.QUnit.defaultUser_Editor                     = "qa_editor";
TM.QUnit.defaultUser_Reader                     = "qa_reader";
TM.QUnit.defaultPassord_Admin                   = "!!tmbeta";
TM.QUnit.defaultPassord_Editor                  = "!!tmbeta";
TM.QUnit.defaultPassord_Reader                  = "!!TMbeta";

//Individual trace options
TM.WebServices.Helper.trace                     = false;
TM.Gui.AppliedFilters.trace                     = false;
TM.WebServices.trace                            = false;