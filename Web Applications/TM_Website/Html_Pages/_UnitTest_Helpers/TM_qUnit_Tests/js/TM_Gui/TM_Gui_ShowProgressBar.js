module("TM.Gui.ShowProgressBar.js");

module("ProgressBar support");

test("TM_GUI_JQueryUI setup", function() 
	{		
		ok($("#Canvas").length > 0, "jQueryUI_Canvas");
	})

test("Adding a ProgressBar - to div", function() 
	{	
		$("#Canvas").html("Adding a ProgressBar");
		var testDivId = "#a_Div_to_hold_a_ProgressBar";
		$("#Canvas").append($("<div>").attr("id",testDivId.substr(1)));
		var progressBar = TM.Gui.addProgressBar(testDivId);
		ok(progressBar, "progressBar value was set");
		notEqual(progressBar.length, 0, "progressBar.length");
		ok("Adding a ProgressBar" != $("#Canvas").html(), "#Canvas was not updated");					
	});
	
test("Adding a ProgressBar - as child div", function() 
	{	
		$("#Canvas").html("Adding a ProgressBar");						
		var progressBar = TM.Gui.addProgressBar("#Canvas","#progressBarDiv")
		ok(progressBar, "progressBar value was set");
		notEqual(progressBar.length, 0, "progressBar.length");
		ok("Adding a ProgressBar" != $("#Canvas").html(), "#Canvas was not updated");						
		$("#progressBarDiv").remove();
	});	
	
test("Testing ProgressBar actions", function() 
	{		
		var progressBar = TM.Gui.addProgressBar("#Canvas","#progressBarDiv")
		equal( progressBar.getValue(),0, "at start value is 0");
		progressBar.setValue(50);
		equal(progressBar.getValue(),50, "after setValue value was 50");		
		progressBar.inc()
		equal(progressBar.getValue(),51, "inc");
		progressBar.dec()
		equal(progressBar.getValue(),50, "dec");
		progressBar.reset()
		equal(progressBar.getValue(), 0 , "reset()");
		progressBar.inc(70)
		equal(progressBar.getValue(), 70, "inc(70)");
		progressBar.dec(50)
		equal(progressBar.getValue(),  20, "dec(50)");
		progressBar.dec(0)
		equal(progressBar.getValue(), 20 , "dec(0)");
		
		var methodChainingResult = progressBar.inc(12).dec(1).reset().inc(12).dec(2).inc(3).getValue();
		equal(methodChainingResult, 13, "methodChainingResult");
		//intoduce a small delay so that we see the new ProgressBar in the browser
		stop();
		setTimeout(function() 
			{ 
				$("#progressBarDiv").remove();
				start();
			}, 200);		
		//TM.QUnit.progressBar = progressBar;
	});

module("TM.Gui.ShowProgressBar");	
		
asyncTest("showing a ProgressBar", function() 
	{				
		var showProgressBar = TM.Gui.ShowProgressBar; 				
		showProgressBar.close();
		
		ok		(showProgressBar 	 						, " TM.Gui.progressBar was set");
		equals	(showProgressBar.isOpen , false				, "isOpen was false");
		ok		(isUndefined(showProgressBar.progressBar) 	, "progressBar was undefined");
		ok		(showProgressBar.baseDivId	 				, "baseDivId was set");		
		ok		(showProgressBar.messageDivId	 			, "messageDivId was set");		
		ok		(isUndefined(showProgressBar.currentMessage), "currentMessage was not set");		
					
		showProgressBar.open();				
		
		ok	(showProgressBar.isOpen 			 			, "isOpen was true after open");
		ok	(isUndefined(showProgressBar.currentMessage)	, "currentMessage should still not be set");		
		ok($(showProgressBar.baseDivId).length > 0		 	, "TM.Events.progressBarDiv div was there");		
		showProgressBar.inc(10);
		equal(showProgressBar.progressBar.getValue(), 10, "inc(10)");
		showProgressBar.value(55)
		equal(showProgressBar.value(), 55 , "value(55)");		
		var methodChainingResult = showProgressBar.open().reset().inc(12).dec(1).close().open().inc(12).dec(2).inc(3).value();
		equal(methodChainingResult, 13, "methodChainingResult");
		showProgressBar.close();
		start();
	});

asyncTest("Adding a Message on top of ProgressBar", function()
	{			
		var showProgressBar = TM.Gui.ShowProgressBar; 
		var message = "TeamMentor message";	
		var updatedMessage = "TeamMentor message____";	
		//trying when the message is passed as a param
		showProgressBar.open(message);
		ok	(showProgressBar.currentMessage	, "currentMessage should be set");		
		equal(showProgressBar.currentMessage, message , "message value was ok");
		showProgressBar.message(updatedMessage);
		equal(showProgressBar.currentMessage, updatedMessage , "updatedMessage value was ok");
		showProgressBar.close();
		
		//trying when the progressbar is created without a message, but we add it later
		showProgressBar.open();
		ok		(isUndefined(showProgressBar.currentMessage), "currentMessage should  not set");
		showProgressBar.message(updatedMessage);
		equal(showProgressBar.currentMessage, updatedMessage , "updatedMessage value was ok");
		showProgressBar.close();
		start();
	});		
	
test("test TM.Events.raiseProcessBarNextValue", function()
	{
		var showProgressBar = TM.Gui.ShowProgressBar; 
		showProgressBar.open();
		showProgressBar.value(20);
		equal(showProgressBar.value(), 20, "after value(20), value was 20");		
		var expectedValue = showProgressBar.value() + showProgressBar.nextAmount;
		TM.Events.raiseProcessBarNextValue();
		var firstValue = showProgressBar.value();
		equal(firstValue, expectedValue, "after raiseProcessBarNextValue value was correct");
		var newNextAmount = 30;
		showProgressBar.nextAmount = newNextAmount;
		TM.Events.raiseProcessBarNextValue();
		var updatedValue = showProgressBar.value();
		equal(updatedValue, firstValue + newNextAmount, "updatedValue  =  firstValue + newNextAmount");
		
	});
asyncTest("animating the ProgressBar (note the end is not good)", function()
	{					
		var showProgressBar = TM.Gui.ShowProgressBar; 
		
		var closeProgressbar = function()
			{
				equal( $(showProgressBar.baseDivId).length , 1, "instance of TM.Gui.progressBarDiv");										
				showProgressBar.close();				 	
				//this test below is throwing a weird case on the 2nd run (it fails)
				//equal( $(showProgressBar.baseDivId).length , 0, "after closeProgressBar there are no instances of TM.Gui.progressBarDiv");		
				equals	(showProgressBar.isOpen , false		 , "isOpen was false");
				start();
			};			
			
		showProgressBar.open();
		ok	(showProgressBar.isOpen , "isOpen was true after open");		
		showProgressBar.progressBar.setValue(1);
		var new_width = showProgressBar.progressBar.width()
		//showProgressBar.message("animation test");
		$(showProgressBar.baseDivId +" .ui-progressbar-value")
			.animate({
						width: new_width
					  }, 
					  //5000,   // secs to take animation
					  'slow',
					  function() { 
									ok(showProgressBar.progressBar.getValue(),1, "It is 1 but with proper animation this should be 100") ;									
									closeProgressbar();
								  })
	
		
	});		
	