TM.Debug.TraceView = 
	{
			load		: function(setupHooks)
							{
								console.log("loading TraceView");
								$.ajaxSetup({ async:false});	
								$.getScript('/javascript/jsTrace/trace-src.js')
								$.ajaxSetup({ async:true});									
								if (setupHooks)								
									TM.Debug.CallTrace.trace_TM_Object();								
							}
	}



TM.Debug.CallTrace 	= 
	{
			load		: function()
							{				
								console.log("loading CallTrace");
								$.ajaxSetup({ async:false});		
								$.getScript('/javascript/jsTrace/jscalltracer.js')								
								$.ajaxSetup({ async:true});								
								return this;
							}
		,   reloadSelf	: function()
							{								
								$.ajaxSetup({ async:false});		
								$.getScript('/javascript/TM/debug.js')
								$.ajaxSetup({ async:true});
								return this;								
							}
		,	traceObject	: function(name, object)
							{
								console.log("Setting up hooks for {0}".format(name));
								if (typeof(JSCT) == "undefined")
									this.load();
								JSCT._setupObject(name, object);
								return this;
							}
							
		, 	trace_TM_Object	: function()
							{
								this.traceObject("TM", TM);
							}
							
		, 	trace_Events	: function()
							{
								this.traceObject("Events", TM.Events);
							}
							
		, 	trace_CurrentUser	: function()
							{
								this.traceObject("CurrentUser", TM.Gui.CurrentUser);
							}
	}

TM.Debug.DebugDiv = 
	{
			show			: function()
							{
								debugDiv = $("<div>").absolute().top(0).left(400).css({ border: "1px solid",  padding: '2px' }).append("<h2>debug div</h2>").zIndex(1000);
								debugDiv.appendTo("body");
								debugDiv.add_Link("Open Trace View").click(function() { TM.Debug.TraceView.load(false)} );								
								debugDiv.append("<br>");
								debugDiv.add_Link("Open Trace View (and hooks)").click(function() { TM.Debug.TraceView.load(true) });
								debugDiv.append("<br>");
								debugDiv.add_Link("Setup Trace Hooks").click(function() { TM.Debug.CallTrace.trace_TM_Object() });
								

								debugDiv.draggable();
								//debugDiv.zIndex(1000);
								
							}
			
	}

TM.Debug.FireBugLite =
	{
			htmlPage 		: '/Javascript/Firebug/beta/Firebug.html'
		,	targetDiv 		: 'FireBugDiv'
		,	open			: function()
								{
									$("<div>").attr('id',this.targetDiv).appendTo("body").load(this.htmlPage)	
								}
	}

$(function()
	{
		if (TM.Debug.callTrace_LoadEnvironment)
		{	
		/*	if ($.browser.msie) 
			{
				//setTimeout(TM.Debug.DebugDiv.show, 200);		
			}
			else*/
			{		
				if (TM.Debug.callTrace_LoadEnvironment_Before_Gui)
				{				
					TM.Events.onMainGuiDivAvailable.add(function() 
						{ 
							TM.Debug.TraceView.load(true); 
							TM.Events.onMainGuiDivAvailable.remove()
						} );
				}				
				else
					TM.Events.onMainGuiLoaded.add(function() 
						{ 
							TM.Debug.TraceView.load(true);
							TM.Events.onMainGuiLoaded.remove();
						} );
			}
		}

		if (TM.Debug.show_TraceView)
			TM.Debug.TraceView.load(false); 
		if (TM.Debug.show_DebugDiv)
			setTimeout(TM.Debug.DebugDiv.show, 200);
})