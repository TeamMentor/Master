	/*var askUserQuestion_AnswerCallback = function(answer)
	{
		alert("answer: " + answer);
	}*/
	//askUserQuestion('Security Question', 'what is the edit password', function(answer) { alert("password: = " + answer);} ;
	
	var askUserSecretQuestion = function(title, question, askUserQuestion_AnswerCallback)
	{
		"body".$()
			  .add_Div("userDialogs","aa")
			  .hide()
			  .load_InDiv('/Javascript/jQuery.UniForm/userQuestion.html', 
					function() 
						{ 
							askSecretQuestion(title, question, askUserQuestion_AnswerCallback); 
						});
	}
	
	var askUserQuestion = function(title, question, askUserQuestion_AnswerCallback)
	{		
		"body".$()
			  .add_Div("userDialogs","aa")
			  .hide()
			  .load_InDiv('/Javascript/jQuery.UniForm/userQuestion.html', 
					function() 
						{ 
							askQuestion(title, question, askUserQuestion_AnswerCallback); 
						});
	}
	

	var addTextField = function(divId, labelText, inputId) 
		{			
			addInputField(divId, labelText, inputId, 'text');
		}
		
	var addPasswordField = function(divId, labelText, inputId) 
		{			
			addInputField(divId, labelText, inputId, 'password');
		}	
		
	var addInputField = function(divId, labelText, inputId, type) 
		{			
			divId.$().append(
				//'<_div class="ctrlHolder">' + 
				'<label for="">{0}</label>'.format(labelText) + 
				'<input type="{0}" id="{1}" name="" value="" size="15" class="textInput"/> '.format(type, inputId) 
				//+ 
				//'</_div>'
				);    
		}		
		
	var addButton = function(divId, buttonText, onClickEventFunctionName) 	
		{
			divId.$().append(
				'<div class="buttonHolder">' + 
					'<button type="button" class="primaryAction" id="button_{0}" onclick="javascript:{1}()">{0}</button>'.format(buttonText, onClickEventFunctionName) +
				'</div>	');
				
		}
		
	var cssFixes_UniForm = function() 
		{
			"buttonHolder".$().css('background','#333333');
		}