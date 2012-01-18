TM.Gui.ShowProgressBar =  
	{		
		baseDivId 		 : "#__tm_progressBarBaseDiv",
		progressBarDivId : "#__tm_progressBarDiv",		
		messageDivId	 : "#__tm_progressBarMessageDiv",		
		isOpen			 : false,
		nextAmount		 : 12,
		showStatusMessage: true,
		progressBar		 : undefined,
		currentMessage   : undefined,
		
		//_original_raiseProcessBarNextValue : undefined,
		//_original_raiseProcessBarNextValue : undefined,
		
		open : function(message)
				{			
					if ($(TM.Gui.progressBarDiv).length ==0)		
					{
						$("body").append($("<div>").attr("id", this.baseDivId.substr(1)));					
					
						this.progressBar = TM.Gui.addProgressBar(this.baseDivId)
						this.progressBar.width(300)
									    .center()
									    .zIndex(2000);																	
					}							
					this.message(message);														
					this.isOpen	= true;
					var that = this;
					
					
					//Global events to hook
					//this._original_raiseProcessBarNextValue	 	= TM.Events.raiseProcessBarNextValue;
					//this._original_raiseWebServiceReceivedData 	= TM.Events.raiseWebServiceReceivedData;
					
					//TM.Events.raiseWebServiceReceivedData.add(TM.Events.raiseProcessBarNextValue);

					TM.Events.raiseProcessBarNextValue	 .add(function(statusMessage) { that.progressBarNextValue(statusMessage) });;
					return this;
				},
		
		progressBarNextValue: function(statusMessage)
				{											
					if (this.showStatusMessage)
						this.message(statusMessage);
					this.inc(this.nextAmount);						
				},				
			
		close : function()	
				{					
					if (isDefined(this.progressBar))
						this.progressBar.setValue(100);																				
					$(this.baseDivId).remove();			
					this.isOpen	= false;
					this.progressBar = undefined;
					
					//restore hooked Global events
					/*if(isDefined(this._original_raiseProcessBarNextValue))
					{
						TM.Events.raiseProcessBarNextValue = this._original_raiseProcessBarNextValue;
						TM.Events.raiseWebServiceReceivedData = this._original_raiseWebServiceReceivedData;
					}*/
					TM.Events.raiseProcessBarNextValue.remove();
					//TM.Events.raiseWebServiceReceivedData.remove();
					return this;
				} ,
		message : function(message) 
				{
					if (isDefined(message))
					{		
						if ($(this.messageDivId).length ==0)
						{
							this.progressBar.append(
								$("<span>").attr("id", this.messageDivId.substr(1))
										  .css( { 
													position: "absolute" , 
													top: 0, 
													width: "100%" ,  
													"text-align": "center" , 
													"line-height": "1.9em" 
												  }));						
						}								
						$(this.messageDivId).html(message);						
					}					
					this.currentMessage = message;
				},		
		inc 	: function(value) 	 { this.progressBar.inc(value)				; return this; 	},
		incAfter: function(value) 	 { this.progressBar.inc_after_timeout(value)	; return this; 	},
		next	: function()		 { this.incAfter(this.nextAmount,20)		    ; return this;  },
		dec 	: function(value) 	 { this.progressBar.dec(value)				; return this; 	},
		reset 	: function() 		 { return this.close().open(); 	   			  	},
		value 	: function(newValue) { 
										 if (isDefined(newValue))
											this.progressBar.setValue(newValue);           	
										 return this.progressBar.getValue()           	
									 }		
	}


// helper method to create generic ProgressBars

TM.Gui.addProgressBar = function(hostElement, childElement)
	{		
		
		if (typeof(childElement) == "string")
		{
			$(hostElement).html($("<div>").attr("id",childElement.substr(1)));
			hostElement = childElement;	
		}		
			
		var options = 
			{
				steps			: 20,
				stepDuration	: 20,
				max				: 100,
				showText		: true,
				textFormat		: 'percentage',
				width			: 120,
				height			: 12,
				callback		: null				
			};
		var progressBar = $(hostElement).progressbar(options);	
		
		progressBar.getValue = function()
			{
				return $(hostElement).progressbar( "option", "value" );
			};
			
		progressBar.setValue = function(value)
			{				
				$(hostElement).progressbar( "option", "value" , value);
				return this;
			};
			
		progressBar.inc = function(value) 
			{  				
				if (isUndefined(value)) 
					value = 1;
				this.setValue(progressBar.getValue() + value);
				return this;
			}
			
		progressBar.inc_after_timeout= function(value, timeoutValue)
			{
				var that = this;
				setTimeout(function() { that.inc(value) }, timeoutValue);
			}		
		progressBar.dec = function(value) 
			{  				
				if (isUndefined(value)) 
					value = 1;
				this.inc(-value);
				return this;
			}
		
		progressBar.reset = function()
			{
				this.setValue(0);
				return this;
			};		
		
		return progressBar
	};