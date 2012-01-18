//TM.WS = {};

var lastDataReceived = {};
function invokeWebService(url, params, handleData, handleError)
{		
    $.ajax({
        type: "POST",
        url: url,
        data: params,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {   
			lastDataReceived = msg;	
			if (typeof(handleData)!="undefined")
				handleData(msg)
			else
				handleError(msg);	
        },
        failure: function (msg) {
			handleError(msg);
            //handleError("failure:" + JSON.stringify(msg));
        },
        error: function (msg) {
			handleError(msg);
            //handleError("error: " + JSON.stringify(msg));
        }
    });
}


function defaultErrorHandler(msg)
{
	//showMessage_NotEnoughPriviledges
	try
	{		
		_msg = msg;
		if (typeof(msg)=="object" && typeof(msg.d)!="undefined")
		{
			showUserMessage(JSON.stringify(msg.d));
		}
		else
		{
			
			var errorMessage = JSON.parse(msg.responseText).Message;
			if ( $.browser.msie )			//no idea why the concat below doesn't work in IE (so for now let's show the error message without the extra explanation
				showUserMessage(errorMessage);
			else
				showUserMessage(errorMessage + "<br/><br/>Most likely because the current user doesn't have enough priviledges to execute this operation");			
		}
	}
	catch(error)
	{		
		console.error("error in defaultErrorHandler: " + error.message);
	}	
}

//users
function getUsers(callback)
{
	var url = TM.tmWebServices + 'GetUsers';
	var params = "{}";
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function deleteUser(userId, callback)
{
	var url = TM.tmWebServices + 'DeleteUser';
	var params = "{userId: '"+ userId + "'}";
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function createUser(username , passwordHash,  email, firstname, lastname, note, callback)
{
	var url = TM.tmWebServices + 'CreateUser';
	var params = "{ newUser: { 	username: '"+  username+ "', passwordHash: '"+  passwordHash + "',email:  '"+ email+ "', firstname:  '"+ firstname+ "',lastname:  '"+ lastname+ "',note: '"+  note+ "' } }";
	invokeWebService( url, params, callback, defaultErrorHandler);	
}

function batchUserCreation(batchUserData, callback)
{
	var url = TM.tmWebServices + 'BatchUserCreation';
   	var params =  JSON.stringify( { batchUserData : batchUserData } );
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function updateUser(userId, userName, firstname, lastname, title, company, email, groupId, callback)
{
	var url = TM.tmWebServices + 'UpdateUser';
   	var params =  JSON.stringify( { userId : userId  ,userName: userName , firstname : firstname, lastname : lastname, title : title, company : company, email : email, groupId : groupId } );	
	//alert("updating user with: {0}".format(JSON.stringify(params)));
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function updateUserPassword(userId, userName, newPassword, callback)
{
	var url = TM.tmWebServices + 'SetUserPasswordHash';	
	var passwordHash = SHA256(userName + newPassword);
	var params =  JSON.stringify( { userId : userId  ,passwordHash : passwordHash  } );		
	invokeWebService( url, params, callback, defaultErrorHandler);
}
//config
function getTime(callback)
{
	var url = TM.tmWebServices + 'GetTime';
	var params = "{}";
	invokeWebService( url, params, callback, defaultErrorHandler);
}

//Folders & Tables data
/*function jsTreeWithFolders(callback, handleError)
{
	alert('jsTreeWithFolders - in WS');
	var url = TM.tmWebServices + 'JsTreeWithFolders';
	var params = "{}";
	invokeWebService( url, params, callback, handleError);
}*/

function jsTreeWithFoldersAndGuidanceItems(callback, handleError)
{
	var url = TM.tmWebServices + 'JsTreeWithFoldersAndGuidanceItems';
	var params = "{}";
	invokeWebService( url, params, callback, handleError);
}

function jsTableWithGuidanceItems(viewIds, callback)
{
	var url = TM.tmWebServices + 'JsDataTableWithGuidanceItemsInViews';
	var params = "{viewIds : "+ viewIds + "}";
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function jsTableWithAllGuidanceItems(callback)
{
	var url = TM.tmWebServices + 'JsDataTableWithAllGuidanceItemsInViews';
	var params = "{}";
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function jsTableWithGuidanceItemsIn_Library(libraryId, callback)
{
	var url = TM.tmWebServices + 'JsDataTableWithGuidanceItemsIn_Library';
	var params = "{libraryId : '"+ libraryId + "'}";
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function jsTableWithGuidanceItemsIn_Folder(folderId, callback)
{
	var url = TM.tmWebServices + 'JsDataTableWithGuidanceItemsIn_Folder';
	var params =  JSON.stringify( { folderId : folderId } );	
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function jsTableWithGuidanceItemsIn_View(viewId, callback)
{
	var url = TM.tmWebServices + 'JsDataTableWithGuidanceItemsIn_View';
	var params = "{viewId : '"+ viewId + "'}";
	invokeWebService( url, params, callback, defaultErrorHandler);
}


//Guidance Item & Views
function getGuidanceItemsInViews(viewIds, callback)
{	
	var url = TM.tmWebServices + 'GetGuidanceItemsInViews';
	var params = "{viewIds : "+ viewIds + "}";
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function getGuidanceItemHtml(id, callback)
{
	var url = TM.tmWebServices + 'GetGuidanceItemHtml';
	var params = "{ GuidanceItemId : '" + id + "'}";
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function getGuidanceItemData(id, callback)
{
	var url = TM.tmWebServices + 'GetGuidanceItemById';
	var params = "{ guidanceItemId : '" + id + "'}";
	invokeWebService( url, params, callback, defaultErrorHandler);
}
 

function updateGuidanceItemHtml(id, htmlContent, callback)
{
	var url = TM.tmWebServices + 'UpdateGuidanceItemHtml';
   	var params =  JSON.stringify( { guidanceItemId : id  , htmlContent : htmlContent } );
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function updateGuidanceItemData(guidanceItemId, topic, technology, category, rule_Type, priority, status, author, 
								libraryId,  guidanceType , creatorId ,creatorCaption, title, 
								images, lastUpdate ,  htmlContent, callback)
{	
   	var guidanceItem = { guidanceItem: { 
							guidanceItemId : guidanceItemId  ,
							topic: topic,
							technology : technology, 
							category: category, 
							rule_Type: rule_Type, 
							priority : priority, 
							status : status, 
							author :  author, 
							libraryId : libraryId , 
							guidanceType : guidanceType , 
							creatorId : creatorId, 
							creatorCaption: creatorCaption, 
							title : title, 
							images : images, 
							lastUpdate : lastUpdate,
							htmlContent : htmlContent
						}};
	updateGuidanceItem(guidanceItem, callback);
}
function updateGuidanceItem(guidanceItem, callback)
{
	var url = TM.tmWebServices + 'UpdateGuidanceItem';
	var params =  JSON.stringify(guidanceItem);
//	alert("saving data" + params);						
	invokeWebService( url, params, callback, defaultErrorHandler);
}

//session management
function login(username, password, callback)
{	
	var url = TM.tmWebServices + 'Login';
	var passwordHash = SHA256(username + password)	
	var params =  JSON.stringify( { username : username  , passwordHash : passwordHash } );	
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function loginToWindows(username, password, callback)
{
	var url = TM.tmWebServices + 'LoginToWindows';
	var params =  JSON.stringify( { username : username  , password : password } );		
	invokeWebService( url, params, callback, defaultErrorHandler);
}


//session management
/*function logout(callback)
{
	var url = TM.tmWebServices + 'Logout';
	var params = "{}";
	invokeWebService( url, params, callback, defaultErrorHandler);
}*/

//Logged-in user and RBAC

/*function getCurrentUser(callback)
{
	var url = TM.tmWebServices + 'Current_User';
	var params = "{}";
	invokeWebService( url, params, callback, defaultErrorHandler);
}*/

function userIsAuthenticated(callback)
{
	var url = TM.tmWebServices + 'RBAC_CurrentIdentity_IsAuthenticated';
	var params = "{}";
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function currentUserHasRole(role, callback)
{
	var url = TM.tmWebServices + 'RBAC_HasRole';
	var params = "{ role : '" + role + "'}";
	invokeWebService( url, params, callback, defaultErrorHandler);
}

/*function currentUserRoles(callback)
{
	var url = TM.tmWebServices + 'RBAC_CurrentPrincipal_Roles';
	var params = "{}";
	invokeWebService( url, params, callback, defaultErrorHandler);
}*/

//activity tracking
function getPagesHistory_by_PageId(guidanceItemId, callback)
{
	var url = TM.tmWebServices + 'getPagesHistory_by_PageId';
	var params = "{ guidanceItemId : '" + guidanceItemId + "'}";
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function userHasActivityInGuidanceItem(guidanceItemId, userName, userActivity, callback)
{
	var url = TM.tmWebServices + 'userHasActivityInGuidanceItem';	
	var params =  JSON.stringify( { guidanceItemId : guidanceItemId  , userName : userName , userActivity : userActivity } );	
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function currentUserHasActivityInGuidanceItem(guidanceItemId, userActivity, callback)
{
	var url = TM.tmWebServices + 'currentUserHasActivityInGuidanceItem';
	var params =  JSON.stringify( { guidanceItemId : guidanceItemId, userActivity : userActivity } );	
	invokeWebService( url, params, callback, defaultErrorHandler);
}


//guidanceItems Search
function guidanceItems_Search(searchText, callback)
{
	var url = TM.tmWebServices + 'XmlDatabase_GuidanceItem_SearchHtml';
	var params = "{ searchText : '" + searchText + "'}";
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function guidanceItems_SearchTitle(searchText, callback)
{
	var url = TM.tmWebServices + 'XmlDatabase_GuidanceItem_Search_Title';
	var params = "{ searchText : '" + searchText + "'}";
	invokeWebService( url, params, callback, defaultErrorHandler);
}


//guidanceItems Logging
function guidanceItem_LogPageUserActivity(guidanceItemId, userActivity, callback)
{ 
	var url = TM.tmWebServices + 'logPageUserActivity';
	var params =  JSON.stringify( { guidanceItemId : guidanceItemId  , userActivity : userActivity } );	
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function guidanceItem_LogPageUserComment(guidanceItemId, userComment, callback)
{ 
	var url = TM.tmWebServices + 'logPageUserComment';
	var params =  JSON.stringify( { guidanceItemId : guidanceItemId  , userComment : userComment } );	
	invokeWebService( url, params, callback, defaultErrorHandler);
}

//Cache management
function reloadData(callback)
{
	var url = TM.tmWebServices + 'XmlDatabase_ReloadData';
	var params = '{ }';
	invokeWebService( url, params, callback, defaultErrorHandler);
}

//Libraries management
function createLibrary(libraryName, callback)
{
	var url = TM.tmWebServices + 'CreateLibrary';
	var params = '{ library : { id: "00000000-0000-0000-0000-000000000000", caption : "'  + libraryName + '" } }';
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function deleteLibrary(libraryId, callback)
{
	var url = TM.tmWebServices + 'DeleteLibrary';
	var params = '{ libraryId : "'+ libraryId + '" }';	
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function renameLibrary(libraryId, newName, callback)
{
	var url = TM.tmWebServices + 'UpdateLibrary';
	var params = '{ library : { id : "'+ libraryId + '" , caption  : "'+ newName + '"  } }';	
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function renameFolder(libraryId, folderId, newFolderName, callback)
{
	var url = TM.tmWebServices + 'RenameFolder';
	var params = '{ libraryId : "'+ libraryId + '" , folderId : "'+ folderId + '" , newFolderName  : "'+ newFolderName + '"  }';	
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function createFolder(libraryId, parentFolderId, newFolderName, callback)
{
	if (typeof(parentFolderId) == "undefined")
		parentFolderId = "00000000-0000-0000-0000-000000000000";
	var url = TM.tmWebServices + 'CreateFolder';
	var params =  JSON.stringify( { libraryId : libraryId   , parentFolderId: parentFolderId , newFolderName : newFolderName } );		
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function deleteFolder(libraryId, folderId, callback)
{ 
	var url = TM.tmWebServices + 'DeleteFolder';
	var params =  JSON.stringify( { libraryId : libraryId  , folderId : folderId } );	
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function createView(libraryId, folderId, viewName, callback)
{
	if (typeof(folderId) == "undefined")
		folderId = "00000000-0000-0000-0000-000000000000";
	var url = TM.tmWebServices + 'CreateView';
	var params = '{ folderId : "' +  folderId +'" ,' + 
				   ' view : { id: "00000000-0000-0000-0000-000000000000", parentFolder : "' + "" + 
				         '", caption : "' + viewName + 
						 '", creatorCaption : "guidanceLibrary" , criteria : "", library: "' + libraryId + 
						 '" } }';
	//alert('creatingView with: ' + params);
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function renameView(libraryId, viewId, parentFolder, viewName, callback)
{
	var url = TM.tmWebServices + 'UpdateView';
	var params = '{ view : { id: "' + viewId + '", parentFolder : "' + parentFolder + 
				         '", caption : "' + viewName + 
						 '", creatorCaption : "guidanceLibrary" , criteria : "", library: "' + libraryId + 
						 '" } }';
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function deleteView(libraryId , viewId, callback)
{ 
	var url = TM.tmWebServices + 'RemoveViewFromFolder';
	var params =  JSON.stringify( { libraryId  : libraryId  , viewId : viewId  } );		
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function moveViewToFolder(viewId , folderId, callback)
	{ 
		if(typeof(folderId) == "undefined")
			folderId = "00000000-0000-0000-0000-000000000000";
		var url = TM.tmWebServices + 'MoveViewToFolder';
		var params =  JSON.stringify( { viewId  : viewId  , folderId : folderId  } );			
		invokeWebService( url, params, callback, defaultErrorHandler);
	}

function createGuidanceItem(title,  htmlContent,libraryId,  callback)
{
	createGuidanceItem_AllValues('', '', '', '', '', '','' , libraryId , 'fbb1db92-c314-4fb0-a0db-1ff82bc2d68f', 'fbb1db92-c314-4fb0-a0db-1ff82bc2d68f', 'guidanceLibrary', title, htmlContent, callback);
	//createGuidanceItem_AllValues('', '', '', '', '', '','' ,'00000000-0000-0000-0000-000000000000', 'fbb1db92-c314-4fb0-a0db-1ff82bc2d68f', 'fbb1db92-c314-4fb0-a0db-1ff82bc2d68f', 'guidanceLibrary', title, htmlContent, callback);
} 

function createGuidanceItem_AllValues(topic, technology, category, rule_Type, priority, status, author, libraryId, guidanceType, creatorId, creatorCaption, title, htmlContent, callback)
{
	var url = TM.tmWebServices + 'CreateGuidanceItem';
	var params = '{ guidanceItem: { "topic": "' + topic + 
	                             '", "technology": " ' + technology +
								 '", "category": " ' + category +
								 '", "rule_Type": "' + rule_Type +
								 '", "priority": "' + priority + 
								 '", "status": "' + status +
								 '", "author": "' + author +
								 '" , "libraryId": "' + libraryId +
								 '", "guidanceType": " ' + guidanceType +
								 '", "creatorId": "' + creatorId +
								 '", "creatorCaption": "' + creatorCaption +
								 '", "title": "' + title +
								 '", "images": "GI images", "lastUpdate":  "\\\/Date(1309232562210)\\\/"' + 
								 ' , "htmlContent": "' + htmlContent +'"} } ';
	//alert('creating GuidanceItem  with: ' + params);
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function addGuidanceItemToView(viewId, guidanceItemId, callback)
{
	var url = TM.tmWebServices + 'AddGuidanceItemsToView';
	var params = '{ viewId : "' + viewId + '" , guidanceItemIds: ["' + guidanceItemId + '"]  }';
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function addGuidanceItemToViews(viewId, guidanceItemIds, callback)
{
	var url = TM.tmWebServices + 'AddGuidanceItemsToView';
	var params =  JSON.stringify( { viewId  : viewId   , guidanceItemIds : guidanceItemIds } );		
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function removeGuidanceItemsFromView(viewId, guidanceItemIds, callback)
{
	var url = TM.tmWebServices + 'RemoveGuidanceItemsFromView';
	var params =  JSON.stringify( { viewId  : viewId   , guidanceItemIds : guidanceItemIds } );		
	invokeWebService( url, params, callback, defaultErrorHandler);
}



function renameGuidanceItemTitle(guidanceItemId, title, callback)
{
	var url = TM.tmWebServices + 'RenameGuidanceItemTitle';
	var params = '{ guidanceItemId : "'+ guidanceItemId + '" ,  title  : "'+ title + '"  }';	
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function deleteGuidanceItem(guidanceItemId, callback)
{
	var url = TM.tmWebServices + 'DeleteGuidanceItem';
	var params = '{ guidanceItemId : "'+ guidanceItemId + '" }';	
	invokeWebService( url, params, callback, defaultErrorHandler);
}

//{ guidanceItem: { "topic": "Topic..", "technology": "Technology....", "category": "Category...", "rule_Type": "RuleType...", "priority": "Priority...", "status": "Status..", "author": "Author..." , "libraryId": "10000000-0000-0000-0000-000000000000", "guidanceType": "f531f722-47d1-43a3-b0cf-62cf5dcf8c35", "creatorId": "fbb1db92-c314-4fb0-a0db-1ff82bc2d68f", "creatorCaption": "guidanceLibrary", "title": "GI title", "images": "GI images", "lastUpdate": "\/Date(1309232562210)\/" , "htmlContent": "GI HTML content"} } 

//Gui objects

function getStringIndexes(callback)
{
	var url = TM.tmWebServices + 'GetStringIndexes';
	var params = '{}';	
	invokeWebService( url, params, callback, defaultErrorHandler);
}

function getGuiObjects(callback)
{
	var url = TM.tmWebServices + 'GetGUIObjects_Small';
	var params = '{}';	
	invokeWebService( url, params, callback, defaultErrorHandler);
}




	


