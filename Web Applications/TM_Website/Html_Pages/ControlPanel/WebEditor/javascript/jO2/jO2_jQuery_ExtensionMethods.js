jQuery(function() 			// add the new functions after page load
{

	// String based Extension methods
	String.prototype.$id = 	  function() { return $('#' + this); }
	String.prototype.$class = function() { return $(''  + this); } 
	String.prototype.$css =   function() { return $('.' + this); } 	
	String.prototype.format = function() { return jQuery.validator.format( this,[].slice.call(arguments)) };

	
	jQuery.fn.id     = function(left)	{ return this.attr('id'); };
	jQuery.fn.value  = function(left)	{ return this.attr('value'); };
	
	jQuery.fn.top    = function(top) 	{ this.css('top',top)	; return this; };
	jQuery.fn.left   = function(left)	{ this.css('left',left)	; return this; };
  	jQuery.fn.bottom = function(top) 	{ this.css('bottom',top)	; return this; };
	jQuery.fn.right  = function(left)	{ this.css('right',left)	; return this; };

	jQuery.fn.absolute = function() 	{ return this.css('position','absolute'); };
	jQuery.fn.static   = function()		{ return this.css('position','static'); };
	jQuery.fn.relative = function()		{ return this.css('position','relative'); };
	
	
	jQuery.fn.add_Link = function(title, url)
									{
										if (typeof(url)=="undefined")
												url = "#";
										//var linkId = "link".add_Random();
										
										var linkId = "link_{0}".format(title.replace(/ /g, '_'));
										console.error('linkId: ' + linkId);
										this.append("<a id='{0}' href='{1}'>{2}</a>".format(linkId, url, title));
										return linkId.$id();								
									};
									
	jQuery.fn.add_TextBox = function(text)
									{
										var textBoxId = "textBox".add_Random();
										this.append("<input id='{0}' type='text'/>".format(textBoxId));
										var textBox = textBoxId.$id();
										textBox.attr('value',text);
										return textBox;
									};
	
	jQuery.fn.add_Div = function(id, text)
									{
										if(typeof(id) == "undefined")										
											id = "div".add_Random();										
										if (typeof(this.children("#" + id).val()) == "undefined")
											this.append("<div id='{0}'></div>".format(id));
										var div = this.children("#" + id);
										if(typeof(text)!="undefined")
											div.html(text);
										return div;
									};									

	jQuery.fn.insert_Link = function(title, url)
									{
										var linkId = "link".add_Random();
										this.before("<a id='{0}' href='{1}'>{2}</a> aa ".format(linkId, url, title));
										return linkId.$id();	
									};
									
	jQuery.fn.load_InDiv = function(link, callback)
									{
										var div = $("<div></div>");
										var linkWithTime = link.add_TimeToUrl();										
										div.load(linkWithTime, callback);
										this.append(div);
										return div;
									}
									
	String.prototype.$  = function(value) 
									{
										if (typeof(this.$id().val()) == "string")	
											return this.$id();
										if (typeof(this.$class().val()) == "string")	
											return this.$class();
										if (typeof(this.$css().val()) == "string")	
											return this.$css();
									};
									
	String.prototype.$html  = function(value) 
									{
										if (typeof(this.$id().val()) == "string")	
											return this.$id().html(value);
										if (typeof(this.$class().val()) == "string")	
											return this.$class().html(value);
										if (typeof(this.$css().val()) == "string")	
											return this.$css().html(value);
									};
						

	
	
	//experiments
	/*String.prototype.$findId =   function() { return $.find('#' + this); } 
	String.prototype.$findClass =   function() { return $.find('#' + this); } 
	String.prototype.$findCss =   function() { return $.find('#' + this); } */
	
	//String.prototype.$eval =   function() { return $.find('#' + this); } */
	
	
});

function reloadEx()
{
	jQuery.getScript('/javascript/jO2/jO2_String_ExtensionMethods.js');
	jQuery.getScript('/javascript/jO2/jO2_jQuery_ExtensionMethods.js');	
	jQuery.getScript('/javascript/jO2/jO2_MiscGuiControls_ExtensionMethods.js');
	
}