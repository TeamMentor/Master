// String Extension methods

jQuery(function() 			// add the new functions after page load
{
	String.prototype.add_Time 		= function() { return "{0}_{1}".format(this, new Date().getTime());  };
	String.prototype.add_TimeToUrl 	= function() { return "{0}?time={1}".format(this, new Date().getTime());  };
	String.prototype.add_Random 	= function() { return "{0}_{1}".format(this, Math.random().toString().replace(".",""));  };	
	
	String.prototype.wait = function(callback) {  setTimeout(function() { callback(); }, this );};	
});

// int Extension methods

//Number.prototype.wait = function() { alert('there'); };  // doesn't seem to work directly (only if a number is assigned to a variable)