/**
 * JavaScript Call Tracer
 *
 * Copyright 2007 (C) John McKerrell - john@mckerrell.net
 * This contents of this source file are subject to
 * the GNU Public License (GPL) Version 2.0
 *
 */
var JSCT = 
{
	/**
	 * The private array of functions that JSCT has overridden.
	 * @type Array
	 * @private
	 */
    _functions : [],
	/**
	 * If set to true then JSCT will not call any logging functions.
	 * @type boolean
	 * @private
	 */
    _silent : false,

    /**
     * Sets up tracing for an array of named objects or functions. These must
     * be available on the window object.
     * @param {Array} objects An array of object or function names, not references to the objects or functions themselves.
	 * @param {String} (optional) The prefix you would like to use, defaults to 'window', you might prefer 'self'.
	 * @param {String} (optional) Unused argument that would allow alternative loggers to firebug to be used.
     */
    setupTrace : function( objects, prefix, logger ) {
        if( ! prefix )
            prefix = 'window';
        for( var i = 0, l = objects.length; i < l; ++i ) {
            if( window[objects[i]] ) {
                this._setupObject( objects[i], window[objects[i]], window, prefix, logger );
            }
        }
    },

    /**
     * Shuts the tracer up, in case you have a noisy interval.
     */
    silence : function() {
        this._silent = true;
    },

    /**
     * Replaces all of the functions in an object and any child objects with
     * wrapped functions that can be traced. If passed a reference to a function
     * just wraps that function.
     * @private
     */
    _setupObject : function( name, obj, parent, prefix, logger ) {
        if( ! obj )
            return;
        if( obj.JSCTtouched )
            return;
		if (name === 'JSCTtouched' || name[0] ==='_')
		{			
			return;
		}		
			
        obj.JSCTtouched = true;
		
		if (typeof(parent) === "undefined")
			prefix = "";
		else
			if (typeof(prefix) != undefined)
			{
				// Only use foo['bar'] if we have to, otherwise use foo.bar
				if( name.match( /[$a-zA-Z][$a-zA-Z0-9]*/ ) ) 
				{
					if (prefix === "")
						prefix = name;
					else
					prefix = prefix+ '.'+name;
				} else {
					prefix = prefix+'[\''+name.replace( /\\/, "\\\\" ).replace( /'/g, "\'" )+'\']';
				}
			}
		
		if (typeof(trace) ==='function')
			trace("[hooking {0} : {1}]".format(typeof(obj),  prefix));
		else
			console.log("setting up: " + prefix);			
			
        switch( typeof( obj ) ) {
        case 'function':
			//console.log("in function: " + name);
            var funcNum = this._functions.length;
            this._functions[funcNum] = obj;
            parent[name] = this._makeFunction( name, funcNum, parent, prefix, logger );
            for( var key in obj ) {
                parent[name][key] = obj[key];				
				this._setupObject( key, obj[key], obj, prefix, logger );				
            }
            break;
        case 'object':			
			//console.log("in object:" + name);
            for( var key in obj ) 
			{	
				if (typeof( obj[key] ) != 'function' && obj[key].trace != true)
				{
					//console.error("Skyping:" + key);
				}
				else
					this._setupObject( key, obj[key], obj, prefix, logger );				
			}
        }
    },
    
	/**
	 * Creates an anonymous function closure to replace an existing function.
	 * When called, this function will perform the logging (unless JSCT
	 * has been silenced) and then call the original function.
	 * @private
	 */
	 
	 depth : [], 
	 
    _makeFunction : function( name, num, parent, funcName, logger ) 
		{
			return function() 
				{
					JSCT.depth.push('. ');	
					var eval_call = 'JSCT._functions['+num+'].call( this';
					var log_call = "console.group( funcName+'('";
					var start_time = new Date();
					
					for( var i = 0, l = arguments.length; i < l; ++i ) 
					{
						eval_call += ', arguments['+i+']';
						if(TM.Debug.callTrace_ShowParamsInConsoleLog)
						{
							if( i > 0 )
								log_call += ', ","';
							log_call += ', arguments['+i+']';
						}
					}			
					eval_call += ' )';
					log_call += ', ")" )';						
					if( ! JSCT._silent ) 
					{
						
						if (typeof(trace) ==='function')
						{
							var traceText = JSCT.depth.join('') + funcName;
							trace(traceText);
						}
						if(TM.Debug.callTrace_LogToConsole)
							eval( log_call );
					}
					var result = eval( eval_call );
					// !!! Function may return here.
					if( ! JSCT._silent ) 
					{
						console.groupEnd();
						JSCT.depth.pop();
						if(TM.Debug.callTrace_ShowExecutionTime)
						{
							if(TM.Debug.callTrace_ShowParamsInConsoleLog)
							{
								if( typeof( result ) == 'undefined' ) 
								{
									console.log( funcName+'(...) took '+((new Date()) - start_time)+'ms and returned nothing' );
								} else {
									console.log( funcName+'(...) took '+((new Date()) - start_time)+'ms and returned: ', result );
								}
							}
							else
								console.log( funcName+'(...) took '+((new Date()) - start_time)+'ms');
						}
					}
					return result;
				}
		}
}
