WebEditor = 
	{
		securityToken : '',
		
		callService : function (data, callback) 
						{							
							if (typeof(callback) == "undefined")
								callback = this.OnComplete;
								
							jQuery.ajax({
								type: 'POST',
								url: "./WebEditorService.ashx",
								/*contentType: "application/json; charset=utf-8",*/
								contentType: "application/x-www-form-urlencoded",
								dataType: "json",
								data: data ,
								responseType: "json",
								success: callback,
								error: this.OnFail
							
							});							
						},

		OnComplete : function(result) 
						{
							alert(JSON.stringify(result));							
						} , 

		OnFail : function(result) 
						{
							alert('Request Failed:' + result);
						} , 

		getPage : function (page, callback)
						{
							var data = { 
											securityToken : this.securityToken, 
											page : page 
										};
							this.callService(data, callback);
						} , 
		getFiles : function (callback)
						{						
							var data = { 
											securityToken : this.securityToken, 
											command : 'getFiles' 
										};
							this.callService(data, callback);
						},
		saveFile : function(file, contents, callback)
						{
							var data = { 
											securityToken : this.securityToken, 
											command : 'saveFile' , 
											file: file , 
											contents: contents										
										};
							this.callService(data, callback);
						} , 
		getFile : function(file, callback)
						{
							var data = { 
											securityToken : this.securityToken, 
											command : 'getFile' , 
											file: file 
										};
							this.callService(data, callback);
						} , 
		deleteFile : function(file, callback)
						{
							var data = { 
											securityToken : this.securityToken, 
											command : 'deleteFile' , 
											file: file 
										};
							this.callService(data, callback);
						}
						
	};

