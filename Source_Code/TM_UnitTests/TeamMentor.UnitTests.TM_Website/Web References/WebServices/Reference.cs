namespace TeamMentor.UnitTests.TM_Website.WebServices
{
	using System;
	using System.Web.Services;
	using System.Diagnostics;
	using System.Web.Services.Protocols;
	using System.Xml.Serialization;
	using System.ComponentModel;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Web.Services.WebServiceBindingAttribute(Name = "TM_WebServicesSoap", Namespace = "http://teammentor.net/")]
	[System.Xml.Serialization.XmlIncludeAttribute(typeof(Identifiable))]
	[System.Xml.Serialization.XmlIncludeAttribute(typeof(object[][]))]
	public partial class TM_WebServices : System.Web.Services.Protocols.SoapHttpClientProtocol
	{
		private bool useDefaultCredentialsSetExplicitly;
		public TM_WebServices()
		{
			this.Url = global::TeamMentor.UnitTests.TM_Website.Properties.Settings.Default.TeamMentor_UnitTests_TM_Website_WebServices_TM_WebServices;
			if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
				this.UseDefaultCredentials = true;
				this.useDefaultCredentialsSetExplicitly = false;
			} else {
				this.useDefaultCredentialsSetExplicitly = true;
			}
		}
		public new string Url {
			get { return base.Url; }
			set {
				if ((((this.IsLocalFileSystemWebService(base.Url) == true) && (this.useDefaultCredentialsSetExplicitly == false)) && (this.IsLocalFileSystemWebService(value) == false))) {
					base.UseDefaultCredentials = false;
				}
				base.Url = value;
			}
		}
		public new bool UseDefaultCredentials {
			get { return base.UseDefaultCredentials; }
			set {
				base.UseDefaultCredentials = value;
				this.useDefaultCredentialsSetExplicitly = true;
			}
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/VirtualArticle_CreateArticle_from_ExternalServiceData", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public TeamMentor_Article VirtualArticle_CreateArticle_from_ExternalServiceData(string service, string serviceData)
		{
			object[] results = this.Invoke("VirtualArticle_CreateArticle_from_ExternalServiceData", new object[] {
				service,
				serviceData
			});
			return ((TeamMentor_Article)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/getGuidForMapping", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Guid getGuidForMapping(string mapping)
		{
			object[] results = this.Invoke("getGuidForMapping", new object[] { mapping });
			return ((System.Guid)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/IsGuidMappedInThisServer", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool IsGuidMappedInThisServer(System.Guid guid)
		{
			object[] results = this.Invoke("IsGuidMappedInThisServer", new object[] { guid });
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetFolderStructure_Libraries", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public Library_V3[] GetFolderStructure_Libraries()
		{
			object[] results = this.Invoke("GetFolderStructure_Libraries", new object[0]);
			return ((Library_V3[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetFolderStructure_Library", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public Library_V3 GetFolderStructure_Library(System.Guid libraryId)
		{
			object[] results = this.Invoke("GetFolderStructure_Library", new object[] { libraryId });
			return ((Library_V3)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetCurrentSessionLibrary", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Guid GetCurrentSessionLibrary()
		{
			object[] results = this.Invoke("GetCurrentSessionLibrary", new object[0]);
			return ((System.Guid)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/ClearGUIObjects", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool ClearGUIObjects()
		{
			object[] results = this.Invoke("ClearGUIObjects", new object[0]);
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetGUIObjects", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public TM_GUI_Objects GetGUIObjects()
		{
			object[] results = this.Invoke("GetGUIObjects", new object[0]);
			return ((TM_GUI_Objects)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetStringIndexes", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string[] GetStringIndexes()
		{
			object[] results = this.Invoke("GetStringIndexes", new object[0]);
			return ((string[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetGuidanceItemsMappings", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string[] GetGuidanceItemsMappings()
		{
			object[] results = this.Invoke("GetGuidanceItemsMappings", new object[0]);
			return ((string[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/Upload_File_To_Library", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool Upload_File_To_Library(System.Guid libraryId, string filename, 		[System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
byte[] contents)
		{
			object[] results = this.Invoke("Upload_File_To_Library", new object[] {
				libraryId,
				filename,
				contents
			});
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetLibraries", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public TM_Library[] GetLibraries()
		{
			object[] results = this.Invoke("GetLibraries", new object[0]);
			return ((TM_Library[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetAllFolders", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public Folder_V3[] GetAllFolders()
		{
			object[] results = this.Invoke("GetAllFolders", new object[0]);
			return ((Folder_V3[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetViews", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public View_V3[] GetViews()
		{
			object[] results = this.Invoke("GetViews", new object[0]);
			return ((View_V3[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetFolders", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public Folder_V3[] GetFolders(System.Guid libraryId)
		{
			object[] results = this.Invoke("GetFolders", new object[] { libraryId });
			return ((Folder_V3[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetGuidanceItemsInFolder", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public TeamMentor_Article[] GetGuidanceItemsInFolder(System.Guid folderId)
		{
			object[] results = this.Invoke("GetGuidanceItemsInFolder", new object[] { folderId });
			return ((TeamMentor_Article[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetGuidanceItemsInView", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public TeamMentor_Article[] GetGuidanceItemsInView(System.Guid viewId)
		{
			object[] results = this.Invoke("GetGuidanceItemsInView", new object[] { viewId });
			return ((TeamMentor_Article[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetGuidanceItemsInViews", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public TeamMentor_Article[] GetGuidanceItemsInViews(System.Guid[] viewIds)
		{
			object[] results = this.Invoke("GetGuidanceItemsInViews", new object[] { viewIds });
			return ((TeamMentor_Article[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetGuidanceItemHtml", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string GetGuidanceItemHtml(System.Guid guidanceItemId)
		{
			object[] results = this.Invoke("GetGuidanceItemHtml", new object[] { guidanceItemId });
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetGuidanceItemsHtml", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string[] GetGuidanceItemsHtml(System.Guid[] guidanceItemsIds)
		{
			object[] results = this.Invoke("GetGuidanceItemsHtml", new object[] { guidanceItemsIds });
			return ((string[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetViewsInLibraryRoot", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public View_V3[] GetViewsInLibraryRoot(System.Guid libraryId)
		{
			object[] results = this.Invoke("GetViewsInLibraryRoot", new object[] { libraryId });
			return ((View_V3[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetViewById", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public View_V3 GetViewById(System.Guid viewId)
		{
			object[] results = this.Invoke("GetViewById", new object[] { viewId });
			return ((View_V3)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetAllLibraryIds", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string[] GetAllLibraryIds()
		{
			object[] results = this.Invoke("GetAllLibraryIds", new object[0]);
			return ((string[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetLibraryById", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public Library GetLibraryById(System.Guid libraryId)
		{
			object[] results = this.Invoke("GetLibraryById", new object[] { libraryId });
			return ((Library)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetLibraryByName", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public Library GetLibraryByName(string libraryName)
		{
			object[] results = this.Invoke("GetLibraryByName", new object[] { libraryName });
			return ((Library)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetGuidanceItemById", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public TeamMentor_Article GetGuidanceItemById(System.Guid guidanceItemId)
		{
			object[] results = this.Invoke("GetGuidanceItemById", new object[] { guidanceItemId });
			return ((TeamMentor_Article)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/CreateLibrary", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public Library_V3 CreateLibrary(Library library)
		{
			object[] results = this.Invoke("CreateLibrary", new object[] { library });
			return ((Library_V3)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/UpdateLibrary", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool UpdateLibrary(Library library)
		{
			object[] results = this.Invoke("UpdateLibrary", new object[] { library });
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/CreateView", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public View_V3 CreateView(System.Guid folderId, View view)
		{
			object[] results = this.Invoke("CreateView", new object[] {
				folderId,
				view
			});
			return ((View_V3)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/UpdateView", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool UpdateView(View view)
		{
			object[] results = this.Invoke("UpdateView", new object[] { view });
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/AddGuidanceItemsToView", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool AddGuidanceItemsToView(System.Guid viewId, System.Guid[] guidanceItemIds)
		{
			object[] results = this.Invoke("AddGuidanceItemsToView", new object[] {
				viewId,
				guidanceItemIds
			});
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/RemoveGuidanceItemsFromView", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool RemoveGuidanceItemsFromView(System.Guid viewId, System.Guid[] guidanceItemIds)
		{
			object[] results = this.Invoke("RemoveGuidanceItemsFromView", new object[] {
				viewId,
				guidanceItemIds
			});
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/RemoveViewFromFolder", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool RemoveViewFromFolder(System.Guid libraryId, System.Guid viewId)
		{
			object[] results = this.Invoke("RemoveViewFromFolder", new object[] {
				libraryId,
				viewId
			});
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/MoveViewToFolder", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool MoveViewToFolder(System.Guid viewId, System.Guid targetFolderId, System.Guid targetLibraryId)
		{
			object[] results = this.Invoke("MoveViewToFolder", new object[] {
				viewId,
				targetFolderId,
				targetLibraryId
			});
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/CreateGuidanceItem", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Guid CreateGuidanceItem(GuidanceItem_V3 guidanceItem)
		{
			object[] results = this.Invoke("CreateGuidanceItem", new object[] { guidanceItem });
			return ((System.Guid)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/CreateArticle", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Guid CreateArticle(TeamMentor_Article article)
		{
			object[] results = this.Invoke("CreateArticle", new object[] { article });
			return ((System.Guid)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/CreateArticle_Simple", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Guid CreateArticle_Simple(System.Guid libraryId, string title, string dataType, string htmlCode)
		{
			object[] results = this.Invoke("CreateArticle_Simple", new object[] {
				libraryId,
				title,
				dataType,
				htmlCode
			});
			return ((System.Guid)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/UpdateGuidanceItem", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool UpdateGuidanceItem(TeamMentor_Article guidanceItem)
		{
			object[] results = this.Invoke("UpdateGuidanceItem", new object[] { guidanceItem });
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/SetArticleHtml", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool SetArticleHtml(System.Guid articleId, string htmlContent)
		{
			object[] results = this.Invoke("SetArticleHtml", new object[] {
				articleId,
				htmlContent
			});
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/SetArticleContent", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool SetArticleContent(System.Guid articleId, string dataType, string content)
		{
			object[] results = this.Invoke("SetArticleContent", new object[] {
				articleId,
				dataType,
				content
			});
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/DeleteGuidanceItem", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool DeleteGuidanceItem(System.Guid guidanceItemId)
		{
			object[] results = this.Invoke("DeleteGuidanceItem", new object[] { guidanceItemId });
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/DeleteGuidanceItems", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool DeleteGuidanceItems(System.Guid[] guidanceItemIds)
		{
			object[] results = this.Invoke("DeleteGuidanceItems", new object[] { guidanceItemIds });
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/RenameFolder", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool RenameFolder(System.Guid libraryId, System.Guid folderId, string newFolderName)
		{
			object[] results = this.Invoke("RenameFolder", new object[] {
				libraryId,
				folderId,
				newFolderName
			});
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/CreateFolder", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public Folder_V3 CreateFolder(System.Guid libraryId, System.Guid parentFolderId, string newFolderName)
		{
			object[] results = this.Invoke("CreateFolder", new object[] {
				libraryId,
				parentFolderId,
				newFolderName
			});
			return ((Folder_V3)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/DeleteFolder", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool DeleteFolder(System.Guid libraryId, System.Guid folderId)
		{
			object[] results = this.Invoke("DeleteFolder", new object[] {
				libraryId,
				folderId
			});
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/DeleteLibrary", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool DeleteLibrary(System.Guid libraryId)
		{
			object[] results = this.Invoke("DeleteLibrary", new object[] { libraryId });
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/RenameLibrary", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool RenameLibrary(System.Guid libraryId, string newName)
		{
			object[] results = this.Invoke("RenameLibrary", new object[] {
				libraryId,
				newName
			});
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetAllGuidanceItems", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public TeamMentor_Article[] GetAllGuidanceItems()
		{
			object[] results = this.Invoke("GetAllGuidanceItems", new object[0]);
			return ((TeamMentor_Article[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetGuidanceItemsInLibrary", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public TeamMentor_Article[] GetGuidanceItemsInLibrary(System.Guid libraryId)
		{
			object[] results = this.Invoke("GetGuidanceItemsInLibrary", new object[] { libraryId });
			return ((TeamMentor_Article[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/JsTreeWithFolders", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public JsTree JsTreeWithFolders()
		{
			object[] results = this.Invoke("JsTreeWithFolders", new object[0]);
			return ((JsTree)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/JsTreeWithFoldersAndGuidanceItems", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public JsTree JsTreeWithFoldersAndGuidanceItems()
		{
			object[] results = this.Invoke("JsTreeWithFoldersAndGuidanceItems", new object[0]);
			return ((JsTree)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/JsDataTableWithAllGuidanceItemsInViews", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public JsDataTable JsDataTableWithAllGuidanceItemsInViews()
		{
			object[] results = this.Invoke("JsDataTableWithAllGuidanceItemsInViews", new object[0]);
			return ((JsDataTable)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/PasswordReset", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool PasswordReset(string userName, System.Guid token, string newPassword)
		{
			object[] results = this.Invoke("PasswordReset", new object[] {
				userName,
				token,
				newPassword
			});
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/CreateUser", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public int CreateUser(NewUser newUser)
		{
			object[] results = this.Invoke("CreateUser", new object[] { newUser });
			return ((int)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/CreateUser_Validate", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string[] CreateUser_Validate(NewUser newUser)
		{
			object[] results = this.Invoke("CreateUser_Validate", new object[] { newUser });
			return ((string[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/CreateUser_Random", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public TM_User CreateUser_Random()
		{
			object[] results = this.Invoke("CreateUser_Random", new object[0]);
			return ((TM_User)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/SendPasswordReminder", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool SendPasswordReminder(string email)
		{
			object[] results = this.Invoke("SendPasswordReminder", new object[] { email });
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetCurrentUserPasswordExpiryUrl", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string GetCurrentUserPasswordExpiryUrl()
		{
			object[] results = this.Invoke("GetCurrentUserPasswordExpiryUrl", new object[0]);
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/NewPasswordResetToken", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Guid NewPasswordResetToken(string email)
		{
			object[] results = this.Invoke("NewPasswordResetToken", new object[] { email });
			return ((System.Guid)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetUser_byID", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public TM_User GetUser_byID(int userId)
		{
			object[] results = this.Invoke("GetUser_byID", new object[] { userId });
			return ((TM_User)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetUsers_byID", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public TM_User[] GetUsers_byID(int[] userIds)
		{
			object[] results = this.Invoke("GetUsers_byID", new object[] { userIds });
			return ((TM_User[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetUser_byName", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public TM_User GetUser_byName(string name)
		{
			object[] results = this.Invoke("GetUser_byName", new object[] { name });
			return ((TM_User)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetUsers", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public TM_User[] GetUsers()
		{
			object[] results = this.Invoke("GetUsers", new object[0]);
			return ((TM_User[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/CreateUsers", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public TM_User[] CreateUsers(NewUser[] newUsers)
		{
			object[] results = this.Invoke("CreateUsers", new object[] { newUsers });
			return ((TM_User[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/BatchUserCreation", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public TM_User[] BatchUserCreation(string batchUserData)
		{
			object[] results = this.Invoke("BatchUserCreation", new object[] { batchUserData });
			return ((TM_User[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/DeleteUser", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool DeleteUser(int userId)
		{
			object[] results = this.Invoke("DeleteUser", new object[] { userId });
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/DeleteUsers", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool[] DeleteUsers(int[] userIds)
		{
			object[] results = this.Invoke("DeleteUsers", new object[] { userIds });
			return ((bool[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/UpdateUser", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool UpdateUser(int userId, string userName, string firstname, string lastname, string title, string company, string email, string country, string state, System.DateTime accountExpiration,
		bool passwordExpired, bool userEnabled, int groupId)
		{
			object[] results = this.Invoke("UpdateUser", new object[] {
				userId,
				userName,
				firstname,
				lastname,
				title,
				company,
				email,
				country,
				state,
				accountExpiration,
				passwordExpired,
				userEnabled,
				groupId
			});
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/SetUserPassword", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool SetUserPassword(int userId, string password)
		{
			object[] results = this.Invoke("SetUserPassword", new object[] {
				userId,
				password
			});
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetUserGroupId", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public int GetUserGroupId(int userId)
		{
			object[] results = this.Invoke("GetUserGroupId", new object[] { userId });
			return ((int)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetUserGroupName", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string GetUserGroupName(int userId)
		{
			object[] results = this.Invoke("GetUserGroupName", new object[] { userId });
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/SetUserGroupId", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool SetUserGroupId(int userId, int roleId)
		{
			object[] results = this.Invoke("SetUserGroupId", new object[] {
				userId,
				roleId
			});
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetUserRoles", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string[] GetUserRoles(int userId)
		{
			object[] results = this.Invoke("GetUserRoles", new object[] { userId });
			return ((string[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/SendEmail", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool SendEmail(EmailMessage_Post emailMessagePost)
		{
			object[] results = this.Invoke("SendEmail", new object[] { emailMessagePost });
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetUser_AuthTokens", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Guid[] GetUser_AuthTokens(int userId)
		{
			object[] results = this.Invoke("GetUser_AuthTokens", new object[] { userId });
			return ((System.Guid[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/CreateUser_AuthToken", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Guid CreateUser_AuthToken(int userId)
		{
			object[] results = this.Invoke("CreateUser_AuthToken", new object[] { userId });
			return ((System.Guid)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/SetUser_PostLoginView", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public TM_User SetUser_PostLoginView(string userName, string postLoginView)
		{
			object[] results = this.Invoke("SetUser_PostLoginView", new object[] {
				userName,
				postLoginView
			});
			return ((TM_User)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/SetUser_PostLoginScript", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public TM_User SetUser_PostLoginScript(string userName, string postLoginScript)
		{
			object[] results = this.Invoke("SetUser_PostLoginScript", new object[] {
				userName,
				postLoginScript
			});
			return ((TM_User)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/Login", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Guid Login(string username, string password)
		{
			object[] results = this.Invoke("Login", new object[] {
				username,
				password
			});
			return ((System.Guid)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/Login_Using_AuthToken", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Guid Login_Using_AuthToken(System.Guid authToken)
		{
			object[] results = this.Invoke("Login_Using_AuthToken", new object[] { authToken });
			return ((System.Guid)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/Logout", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Guid Logout()
		{
			object[] results = this.Invoke("Logout", new object[0]);
			return ((System.Guid)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/Current_SessionID", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Guid Current_SessionID()
		{
			object[] results = this.Invoke("Current_SessionID", new object[0]);
			return ((System.Guid)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/Current_User", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public TM_User Current_User()
		{
			object[] results = this.Invoke("Current_User", new object[0]);
			return ((TM_User)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetCurrentUserRoles", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string[] GetCurrentUserRoles()
		{
			object[] results = this.Invoke("GetCurrentUserRoles", new object[0]);
			return ((string[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/SetCurrentUserPassword", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool SetCurrentUserPassword(string currentPassword, string newPassword)
		{
			object[] results = this.Invoke("SetCurrentUserPassword", new object[] {
				currentPassword,
				newPassword
			});
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetTime", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string GetTime()
		{
			object[] results = this.Invoke("GetTime", new object[0]);
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/Ping", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string Ping(string message)
		{
			object[] results = this.Invoke("Ping", new object[] { message });
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/XmlDatabase_GetDatabasePath", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string XmlDatabase_GetDatabasePath()
		{
			object[] results = this.Invoke("XmlDatabase_GetDatabasePath", new object[0]);
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/XmlDatabase_GetLibraryPath", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string XmlDatabase_GetLibraryPath()
		{
			object[] results = this.Invoke("XmlDatabase_GetLibraryPath", new object[0]);
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/XmlDatabase_GetUserDataPath", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string XmlDatabase_GetUserDataPath()
		{
			object[] results = this.Invoke("XmlDatabase_GetUserDataPath", new object[0]);
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/XmlDatabase_ReloadData", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string XmlDatabase_ReloadData()
		{
			object[] results = this.Invoke("XmlDatabase_ReloadData", new object[0]);
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/XmlDatabase_IsUsingFileStorage", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool XmlDatabase_IsUsingFileStorage()
		{
			object[] results = this.Invoke("XmlDatabase_IsUsingFileStorage", new object[0]);
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/XmlDatabase_WithoutFileStorage", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool XmlDatabase_WithoutFileStorage()
		{
			object[] results = this.Invoke("XmlDatabase_WithoutFileStorage", new object[0]);
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/XmlDatabase_ImportLibrary_fromZipFile", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool XmlDatabase_ImportLibrary_fromZipFile(string pathToZipFile, string unzipPassword)
		{
			object[] results = this.Invoke("XmlDatabase_ImportLibrary_fromZipFile", new object[] {
				pathToZipFile,
				unzipPassword
			});
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/XmlDatabase_SetLibraryPath", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string XmlDatabase_SetLibraryPath(string libraryPath)
		{
			object[] results = this.Invoke("XmlDatabase_SetLibraryPath", new object[] { libraryPath });
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/XmlDatabase_SetUserDataPath", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool XmlDatabase_SetUserDataPath(string userDataPath)
		{
			object[] results = this.Invoke("XmlDatabase_SetUserDataPath", new object[] { userDataPath });
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/XmlDatabase_GuidanceItems_SearchTitleAndHtml", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Guid[] XmlDatabase_GuidanceItems_SearchTitleAndHtml(System.Guid[] guidanceItemsIds, string searchText)
		{
			object[] results = this.Invoke("XmlDatabase_GuidanceItems_SearchTitleAndHtml", new object[] {
				guidanceItemsIds,
				searchText
			});
			return ((System.Guid[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/XmlDatabase_GetGuidanceItemXml", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string XmlDatabase_GetGuidanceItemXml(System.Guid guidanceItemId)
		{
			object[] results = this.Invoke("XmlDatabase_GetGuidanceItemXml", new object[] { guidanceItemId });
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/XmlDatabase_GetGuidanceItemPath", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string XmlDatabase_GetGuidanceItemPath(System.Guid guidanceItemId)
		{
			object[] results = this.Invoke("XmlDatabase_GetGuidanceItemPath", new object[] { guidanceItemId });
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/RBAC_CurrentIdentity_Name", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string RBAC_CurrentIdentity_Name()
		{
			object[] results = this.Invoke("RBAC_CurrentIdentity_Name", new object[0]);
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/RBAC_CurrentIdentity_IsAuthenticated", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool RBAC_CurrentIdentity_IsAuthenticated()
		{
			object[] results = this.Invoke("RBAC_CurrentIdentity_IsAuthenticated", new object[0]);
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/RBAC_CurrentPrincipal_Roles", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string[] RBAC_CurrentPrincipal_Roles()
		{
			object[] results = this.Invoke("RBAC_CurrentPrincipal_Roles", new object[0]);
			return ((string[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/RBAC_HasRole", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool RBAC_HasRole(string role)
		{
			object[] results = this.Invoke("RBAC_HasRole", new object[] { role });
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/RBAC_IsAdmin", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool RBAC_IsAdmin()
		{
			object[] results = this.Invoke("RBAC_IsAdmin", new object[0]);
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/RBAC_Demand_Admin", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool RBAC_Demand_Admin()
		{
			object[] results = this.Invoke("RBAC_Demand_Admin", new object[0]);
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/RBAC_Demand_EditArticles", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool RBAC_Demand_EditArticles()
		{
			object[] results = this.Invoke("RBAC_Demand_EditArticles", new object[0]);
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/RBAC_Demand_ReadArticles", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool RBAC_Demand_ReadArticles()
		{
			object[] results = this.Invoke("RBAC_Demand_ReadArticles", new object[0]);
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/RBAC_Demand_ManageUsers", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool RBAC_Demand_ManageUsers()
		{
			object[] results = this.Invoke("RBAC_Demand_ManageUsers", new object[0]);
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GitHub_Pull_Origin", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string GitHub_Pull_Origin()
		{
			object[] results = this.Invoke("GitHub_Pull_Origin", new object[0]);
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GitHub_Push_Origin", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string GitHub_Push_Origin()
		{
			object[] results = this.Invoke("GitHub_Push_Origin", new object[0]);
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GitHub_Push_Commit", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string GitHub_Push_Commit()
		{
			object[] results = this.Invoke("GitHub_Push_Commit", new object[0]);
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/Git_Execute", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string Git_Execute(string gitCommand)
		{
			object[] results = this.Invoke("Git_Execute", new object[] { gitCommand });
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/CreateWebEditorSecret", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string CreateWebEditorSecret()
		{
			object[] results = this.Invoke("CreateWebEditorSecret", new object[0]);
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/TMConfigFileLocation", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string TMConfigFileLocation()
		{
			object[] results = this.Invoke("TMConfigFileLocation", new object[0]);
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/TMConfigFile", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public TMConfig TMConfigFile()
		{
			object[] results = this.Invoke("TMConfigFile", new object[0]);
			return ((TMConfig)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/SetTMConfigFile", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool SetTMConfigFile(TMConfig tmConfig)
		{
			object[] results = this.Invoke("SetTMConfigFile", new object[] { tmConfig });
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/Get_Firebase_ClientConfig", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public Firebase_ClientConfig Get_Firebase_ClientConfig()
		{
			object[] results = this.Invoke("Get_Firebase_ClientConfig", new object[0]);
			return ((Firebase_ClientConfig)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/Get_Libraries_Zip_Folder", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string Get_Libraries_Zip_Folder()
		{
			object[] results = this.Invoke("Get_Libraries_Zip_Folder", new object[0]);
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/Get_Libraries_Zip_Folder_Files", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string[] Get_Libraries_Zip_Folder_Files()
		{
			object[] results = this.Invoke("Get_Libraries_Zip_Folder_Files", new object[0]);
			return ((string[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/Set_Libraries_Zip_Folder", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string Set_Libraries_Zip_Folder(string folder)
		{
			object[] results = this.Invoke("Set_Libraries_Zip_Folder", new object[] { folder });
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetUploadToken", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Guid GetUploadToken()
		{
			object[] results = this.Invoke("GetUploadToken", new object[0]);
			return ((System.Guid)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/GetLogs", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string GetLogs()
		{
			object[] results = this.Invoke("GetLogs", new object[0]);
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/ResetLogs", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string ResetLogs()
		{
			object[] results = this.Invoke("ResetLogs", new object[0]);
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/REPL_ExecuteSnippet", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string REPL_ExecuteSnippet(string snippet)
		{
			object[] results = this.Invoke("REPL_ExecuteSnippet", new object[] { snippet });
			return ((string)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/VirtualArticle_GetCurrentMappings", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public VirtualArticleAction[] VirtualArticle_GetCurrentMappings()
		{
			object[] results = this.Invoke("VirtualArticle_GetCurrentMappings", new object[0]);
			return ((VirtualArticleAction[])(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/VirtualArticle_Add_Mapping_VirtualId", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public VirtualArticleAction VirtualArticle_Add_Mapping_VirtualId(System.Guid id, System.Guid virtualId)
		{
			object[] results = this.Invoke("VirtualArticle_Add_Mapping_VirtualId", new object[] {
				id,
				virtualId
			});
			return ((VirtualArticleAction)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/VirtualArticle_Add_Mapping_Redirect", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public VirtualArticleAction VirtualArticle_Add_Mapping_Redirect(System.Guid id, string redirectUri)
		{
			object[] results = this.Invoke("VirtualArticle_Add_Mapping_Redirect", new object[] {
				id,
				redirectUri
			});
			return ((VirtualArticleAction)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/VirtualArticle_Add_Mapping_ExternalArticle", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public VirtualArticleAction VirtualArticle_Add_Mapping_ExternalArticle(System.Guid id, string tmServer, System.Guid externalId)
		{
			object[] results = this.Invoke("VirtualArticle_Add_Mapping_ExternalArticle", new object[] {
				id,
				tmServer,
				externalId
			});
			return ((VirtualArticleAction)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/VirtualArticle_Add_Mapping_ExternalService", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public VirtualArticleAction VirtualArticle_Add_Mapping_ExternalService(System.Guid id, string service, string data)
		{
			object[] results = this.Invoke("VirtualArticle_Add_Mapping_ExternalService", new object[] {
				id,
				service,
				data
			});
			return ((VirtualArticleAction)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/VirtualArticle_Remove_Mapping", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool VirtualArticle_Remove_Mapping(System.Guid id)
		{
			object[] results = this.Invoke("VirtualArticle_Remove_Mapping", new object[] { id });
			return ((bool)(results[0]));
		}
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://teammentor.net/VirtualArticle_Get_GuidRedirect", RequestNamespace = "http://teammentor.net/", ResponseNamespace = "http://teammentor.net/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public string VirtualArticle_Get_GuidRedirect(System.Guid id)
		{
			object[] results = this.Invoke("VirtualArticle_Get_GuidRedirect", new object[] { id });
			return ((string)(results[0]));
		}
		private bool IsLocalFileSystemWebService(string url)
		{
			if (((url == null) || (url == string.Empty))) {
				return false;
			}
			System.Uri wsUri = new System.Uri(url);
			if (((wsUri.Port >= 1024) && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
				return true;
			}
			return false;
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class TeamMentor_Article
	{
		private TeamMentor_Article_Metadata metadataField;
		private TeamMentor_Article_Content contentField;
		private int metadata_HashField;
		private int content_HashField;
		public TeamMentor_Article_Metadata Metadata {
			get { return this.metadataField; }
			set { this.metadataField = value; }
		}
		public TeamMentor_Article_Content Content {
			get { return this.contentField; }
			set { this.contentField = value; }
		}
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public int Metadata_Hash {
			get { return this.metadata_HashField; }
			set { this.metadata_HashField = value; }
		}
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public int Content_Hash {
			get { return this.content_HashField; }
			set { this.content_HashField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class TeamMentor_Article_Metadata
	{
		private System.Guid idField;
		private string id_HistoryField;
		private System.Guid library_IdField;
		private string titleField;
		private string categoryField;
		private string phaseField;
		private string technologyField;
		private string typeField;
		private string directLinkField;
		private string tagField;
		private string security_DemandField;
		private string authorField;
		private string priorityField;
		private string statusField;
		private string sourceField;
		private string licenseField;
		public System.Guid Id {
			get { return this.idField; }
			set { this.idField = value; }
		}
		public string Id_History {
			get { return this.id_HistoryField; }
			set { this.id_HistoryField = value; }
		}
		public System.Guid Library_Id {
			get { return this.library_IdField; }
			set { this.library_IdField = value; }
		}
		public string Title {
			get { return this.titleField; }
			set { this.titleField = value; }
		}
		public string Category {
			get { return this.categoryField; }
			set { this.categoryField = value; }
		}
		public string Phase {
			get { return this.phaseField; }
			set { this.phaseField = value; }
		}
		public string Technology {
			get { return this.technologyField; }
			set { this.technologyField = value; }
		}
		public string Type {
			get { return this.typeField; }
			set { this.typeField = value; }
		}
		public string DirectLink {
			get { return this.directLinkField; }
			set { this.directLinkField = value; }
		}
		public string Tag {
			get { return this.tagField; }
			set { this.tagField = value; }
		}
		public string Security_Demand {
			get { return this.security_DemandField; }
			set { this.security_DemandField = value; }
		}
		public string Author {
			get { return this.authorField; }
			set { this.authorField = value; }
		}
		public string Priority {
			get { return this.priorityField; }
			set { this.priorityField = value; }
		}
		public string Status {
			get { return this.statusField; }
			set { this.statusField = value; }
		}
		public string Source {
			get { return this.sourceField; }
			set { this.sourceField = value; }
		}
		public string License {
			get { return this.licenseField; }
			set { this.licenseField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class VirtualArticleAction
	{
		private System.Guid idField;
		private string actionField;
		private System.Guid target_IdField;
		private string redirect_UriField;
		private string tM_ServerField;
		private string serviceField;
		private string service_DataField;
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public System.Guid Id {
			get { return this.idField; }
			set { this.idField = value; }
		}
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Action {
			get { return this.actionField; }
			set { this.actionField = value; }
		}
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public System.Guid Target_Id {
			get { return this.target_IdField; }
			set { this.target_IdField = value; }
		}
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Redirect_Uri {
			get { return this.redirect_UriField; }
			set { this.redirect_UriField = value; }
		}
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string TM_Server {
			get { return this.tM_ServerField; }
			set { this.tM_ServerField = value; }
		}
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Service {
			get { return this.serviceField; }
			set { this.serviceField = value; }
		}
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Service_Data {
			get { return this.service_DataField; }
			set { this.service_DataField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class Firebase_ClientConfig
	{
		private string firebase_SiteField;
		private string firebase_AuthTokenField;
		private string firebase_RootAreaField;
		private bool enabledField;
		public string Firebase_Site {
			get { return this.firebase_SiteField; }
			set { this.firebase_SiteField = value; }
		}
		public string Firebase_AuthToken {
			get { return this.firebase_AuthTokenField; }
			set { this.firebase_AuthTokenField = value; }
		}
		public string Firebase_RootArea {
			get { return this.firebase_RootAreaField; }
			set { this.firebase_RootAreaField = value; }
		}
		public bool Enabled {
			get { return this.enabledField; }
			set { this.enabledField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class OnInstallation_Config
	{
		private bool forceAdminPasswordResetField;
		private string defaultLibraryToInstall_NameField;
		private string defaultLibraryToInstall_LocationField;
		public bool ForceAdminPasswordReset {
			get { return this.forceAdminPasswordResetField; }
			set { this.forceAdminPasswordResetField = value; }
		}
		public string DefaultLibraryToInstall_Name {
			get { return this.defaultLibraryToInstall_NameField; }
			set { this.defaultLibraryToInstall_NameField = value; }
		}
		public string DefaultLibraryToInstall_Location {
			get { return this.defaultLibraryToInstall_LocationField; }
			set { this.defaultLibraryToInstall_LocationField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class Git_Config
	{
		private bool autoCommit_UserDataField;
		private bool autoCommit_LibraryDataField;
		public bool AutoCommit_UserData {
			get { return this.autoCommit_UserDataField; }
			set { this.autoCommit_UserDataField = value; }
		}
		public bool AutoCommit_LibraryData {
			get { return this.autoCommit_LibraryDataField; }
			set { this.autoCommit_LibraryDataField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class WindowsAuthentication_Config
	{
		private bool enabledField;
		private string readerGroupField;
		private string editorGroupField;
		private string adminGroupField;
		public bool Enabled {
			get { return this.enabledField; }
			set { this.enabledField = value; }
		}
		public string ReaderGroup {
			get { return this.readerGroupField; }
			set { this.readerGroupField = value; }
		}
		public string EditorGroup {
			get { return this.editorGroupField; }
			set { this.editorGroupField = value; }
		}
		public string AdminGroup {
			get { return this.adminGroupField; }
			set { this.adminGroupField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class TMSecurity_Config
	{
		private bool show_ContentToAnonymousUsersField;
		private bool sSL_RedirectHttpToHttpsField;
		private bool newAccounts_EnabledField;
		private bool evalAccounts_EnabledField;
		private int evalAccounts_DaysField;
		private bool rEST_AllowCrossDomainAccessField;
		private bool singleSignOn_EnabledField;
		private bool sanitize_HtmlContentField;
		private string default_AdminUserNameField;
		private string default_AdminPasswordField;
		private string default_AdminEmailField;
		public bool Show_ContentToAnonymousUsers {
			get { return this.show_ContentToAnonymousUsersField; }
			set { this.show_ContentToAnonymousUsersField = value; }
		}
		public bool SSL_RedirectHttpToHttps {
			get { return this.sSL_RedirectHttpToHttpsField; }
			set { this.sSL_RedirectHttpToHttpsField = value; }
		}
		public bool NewAccounts_Enabled {
			get { return this.newAccounts_EnabledField; }
			set { this.newAccounts_EnabledField = value; }
		}
		public bool EvalAccounts_Enabled {
			get { return this.evalAccounts_EnabledField; }
			set { this.evalAccounts_EnabledField = value; }
		}
		public int EvalAccounts_Days {
			get { return this.evalAccounts_DaysField; }
			set { this.evalAccounts_DaysField = value; }
		}
		public bool REST_AllowCrossDomainAccess {
			get { return this.rEST_AllowCrossDomainAccessField; }
			set { this.rEST_AllowCrossDomainAccessField = value; }
		}
		public bool SingleSignOn_Enabled {
			get { return this.singleSignOn_EnabledField; }
			set { this.singleSignOn_EnabledField = value; }
		}
		public bool Sanitize_HtmlContent {
			get { return this.sanitize_HtmlContentField; }
			set { this.sanitize_HtmlContentField = value; }
		}
		public string Default_AdminUserName {
			get { return this.default_AdminUserNameField; }
			set { this.default_AdminUserNameField = value; }
		}
		public string Default_AdminPassword {
			get { return this.default_AdminPasswordField; }
			set { this.default_AdminPasswordField = value; }
		}
		public string Default_AdminEmail {
			get { return this.default_AdminEmailField; }
			set { this.default_AdminEmailField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class TMSetup_Config
	{
		private bool useAppDataFolderField;
		private bool onlyLoadUserDataField;
		private string tMLibraryDataVirtualPathField;
		private string xmlLibrariesPathField;
		private string userDataPathField;
		private string librariesUploadedFilesField;
		private bool enableGZipForWebServicesField;
		private bool enable304RedirectsField;
		public bool UseAppDataFolder {
			get { return this.useAppDataFolderField; }
			set { this.useAppDataFolderField = value; }
		}
		public bool OnlyLoadUserData {
			get { return this.onlyLoadUserDataField; }
			set { this.onlyLoadUserDataField = value; }
		}
		public string TMLibraryDataVirtualPath {
			get { return this.tMLibraryDataVirtualPathField; }
			set { this.tMLibraryDataVirtualPathField = value; }
		}
		public string XmlLibrariesPath {
			get { return this.xmlLibrariesPathField; }
			set { this.xmlLibrariesPathField = value; }
		}
		public string UserDataPath {
			get { return this.userDataPathField; }
			set { this.userDataPathField = value; }
		}
		public string LibrariesUploadedFiles {
			get { return this.librariesUploadedFilesField; }
			set { this.librariesUploadedFilesField = value; }
		}
		public bool EnableGZipForWebServices {
			get { return this.enableGZipForWebServicesField; }
			set { this.enableGZipForWebServicesField = value; }
		}
		public bool Enable304Redirects {
			get { return this.enable304RedirectsField; }
			set { this.enable304RedirectsField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class TMConfig
	{
		private TMSetup_Config tMSetupField;
		private TMSecurity_Config tMSecurityField;
		private WindowsAuthentication_Config windowsAuthenticationField;
		private Git_Config gitField;
		private OnInstallation_Config onInstallationField;
		public TMSetup_Config TMSetup {
			get { return this.tMSetupField; }
			set { this.tMSetupField = value; }
		}
		public TMSecurity_Config TMSecurity {
			get { return this.tMSecurityField; }
			set { this.tMSecurityField = value; }
		}
		public WindowsAuthentication_Config WindowsAuthentication {
			get { return this.windowsAuthenticationField; }
			set { this.windowsAuthenticationField = value; }
		}
		public Git_Config Git {
			get { return this.gitField; }
			set { this.gitField = value; }
		}
		public OnInstallation_Config OnInstallation {
			get { return this.onInstallationField; }
			set { this.onInstallationField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class EmailMessage_Post
	{
		private string toField;
		private string subjectField;
		private string messageField;
		public string To {
			get { return this.toField; }
			set { this.toField = value; }
		}
		public string Subject {
			get { return this.subjectField; }
			set { this.subjectField = value; }
		}
		public string Message {
			get { return this.messageField; }
			set { this.messageField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class TM_User
	{
		private string companyField;
		private string countryField;
		private string firstNameField;
		private string lastNameField;
		private string stateField;
		private string titleField;
		private int userIdField;
		private string userNameField;
		private string emailField;
		private long createdDateField;
		private string cSRF_TokenField;
		private System.DateTime expirationDateField;
		private bool passwordExpiredField;
		private bool userEnabledField;
		private int groupIDField;
		private UserTag[] userTagsField;
		public string Company {
			get { return this.companyField; }
			set { this.companyField = value; }
		}
		public string Country {
			get { return this.countryField; }
			set { this.countryField = value; }
		}
		public string FirstName {
			get { return this.firstNameField; }
			set { this.firstNameField = value; }
		}
		public string LastName {
			get { return this.lastNameField; }
			set { this.lastNameField = value; }
		}
		public string State {
			get { return this.stateField; }
			set { this.stateField = value; }
		}
		public string Title {
			get { return this.titleField; }
			set { this.titleField = value; }
		}
		public int UserId {
			get { return this.userIdField; }
			set { this.userIdField = value; }
		}
		public string UserName {
			get { return this.userNameField; }
			set { this.userNameField = value; }
		}
		public string Email {
			get { return this.emailField; }
			set { this.emailField = value; }
		}
		public long CreatedDate {
			get { return this.createdDateField; }
			set { this.createdDateField = value; }
		}
		public string CSRF_Token {
			get { return this.cSRF_TokenField; }
			set { this.cSRF_TokenField = value; }
		}
		public System.DateTime ExpirationDate {
			get { return this.expirationDateField; }
			set { this.expirationDateField = value; }
		}
		public bool PasswordExpired {
			get { return this.passwordExpiredField; }
			set { this.passwordExpiredField = value; }
		}
		public bool UserEnabled {
			get { return this.userEnabledField; }
			set { this.userEnabledField = value; }
		}
		public int GroupID {
			get { return this.groupIDField; }
			set { this.groupIDField = value; }
		}
		public UserTag[] UserTags {
			get { return this.userTagsField; }
			set { this.userTagsField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class UserTag
	{
		private string keyField;
		private string valueField;
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Key {
			get { return this.keyField; }
			set { this.keyField = value; }
		}
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Value {
			get { return this.valueField; }
			set { this.valueField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class NewUser
	{
		private string companyField;
		private string countryField;
		private string firstnameField;
		private string lastnameField;
		private string noteField;
		private string stateField;
		private string titleField;
		private string passwordField;
		private string usernameField;
		private string emailField;
		private int groupIdField;
		private UserTag[] userTagsField;
		public string Company {
			get { return this.companyField; }
			set { this.companyField = value; }
		}
		public string Country {
			get { return this.countryField; }
			set { this.countryField = value; }
		}
		public string Firstname {
			get { return this.firstnameField; }
			set { this.firstnameField = value; }
		}
		public string Lastname {
			get { return this.lastnameField; }
			set { this.lastnameField = value; }
		}
		public string Note {
			get { return this.noteField; }
			set { this.noteField = value; }
		}
		public string State {
			get { return this.stateField; }
			set { this.stateField = value; }
		}
		public string Title {
			get { return this.titleField; }
			set { this.titleField = value; }
		}
		public string Password {
			get { return this.passwordField; }
			set { this.passwordField = value; }
		}
		public string Username {
			get { return this.usernameField; }
			set { this.usernameField = value; }
		}
		public string Email {
			get { return this.emailField; }
			set { this.emailField = value; }
		}
		public int GroupId {
			get { return this.groupIdField; }
			set { this.groupIdField = value; }
		}
		public UserTag[] UserTags {
			get { return this.userTagsField; }
			set { this.userTagsField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class JsDataColumn
	{
		private string sTitleField;
		private string sClassField;
		public string sTitle {
			get { return this.sTitleField; }
			set { this.sTitleField = value; }
		}
		public string sClass {
			get { return this.sClassField; }
			set { this.sClassField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class JsDataTable
	{
		private object[][] aaDataField;
		private JsDataColumn[] aoColumnsField;
		[System.Xml.Serialization.XmlArrayItemAttribute("ArrayOfAnyType")]
		[System.Xml.Serialization.XmlArrayItemAttribute(NestingLevel = 1)]
		public object[][] aaData {
			get { return this.aaDataField; }
			set { this.aaDataField = value; }
		}
		public JsDataColumn[] aoColumns {
			get { return this.aoColumnsField; }
			set { this.aoColumnsField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class Data
	{
		private string titleField;
		private string iconField;
		public string title {
			get { return this.titleField; }
			set { this.titleField = value; }
		}
		public string icon {
			get { return this.iconField; }
			set { this.iconField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class Attributes
	{
		private string idField;
		private string relField;
		private string mdataField;
		public string id {
			get { return this.idField; }
			set { this.idField = value; }
		}
		public string rel {
			get { return this.relField; }
			set { this.relField = value; }
		}
		public string mdata {
			get { return this.mdataField; }
			set { this.mdataField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class JsTreeNode
	{
		private Attributes attrField;
		private Data dataField;
		private string stateField;
		private JsTreeNode[] childrenField;
		public Attributes attr {
			get { return this.attrField; }
			set { this.attrField = value; }
		}
		public Data data {
			get { return this.dataField; }
			set { this.dataField = value; }
		}
		public string state {
			get { return this.stateField; }
			set { this.stateField = value; }
		}
		public JsTreeNode[] children {
			get { return this.childrenField; }
			set { this.childrenField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class JsTree
	{
		private JsTreeNode[] dataField;
		public JsTreeNode[] data {
			get { return this.dataField; }
			set { this.dataField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class GuidanceItem_V3
	{
		private System.Guid guidanceItemIdField;
		private System.Guid guidanceItemId_OriginalField;
		private System.Guid source_guidanceItemIdField;
		private System.Guid libraryIdField;
		private System.Guid guidanceTypeField;
		private System.Guid creatorIdField;
		private string creatorCaptionField;
		private string titleField;
		private string imagesField;
		private string topicField;
		private string technologyField;
		private string categoryField;
		private string phaseField;
		private string rule_TypeField;
		private string priorityField;
		private string statusField;
		private string authorField;
		private bool deleteField;
		private string htmlContentField;
		public System.Guid guidanceItemId {
			get { return this.guidanceItemIdField; }
			set { this.guidanceItemIdField = value; }
		}
		public System.Guid guidanceItemId_Original {
			get { return this.guidanceItemId_OriginalField; }
			set { this.guidanceItemId_OriginalField = value; }
		}
		public System.Guid source_guidanceItemId {
			get { return this.source_guidanceItemIdField; }
			set { this.source_guidanceItemIdField = value; }
		}
		public System.Guid libraryId {
			get { return this.libraryIdField; }
			set { this.libraryIdField = value; }
		}
		public System.Guid guidanceType {
			get { return this.guidanceTypeField; }
			set { this.guidanceTypeField = value; }
		}
		public System.Guid creatorId {
			get { return this.creatorIdField; }
			set { this.creatorIdField = value; }
		}
		public string creatorCaption {
			get { return this.creatorCaptionField; }
			set { this.creatorCaptionField = value; }
		}
		public string title {
			get { return this.titleField; }
			set { this.titleField = value; }
		}
		public string images {
			get { return this.imagesField; }
			set { this.imagesField = value; }
		}
		public string topic {
			get { return this.topicField; }
			set { this.topicField = value; }
		}
		public string technology {
			get { return this.technologyField; }
			set { this.technologyField = value; }
		}
		public string category {
			get { return this.categoryField; }
			set { this.categoryField = value; }
		}
		public string phase {
			get { return this.phaseField; }
			set { this.phaseField = value; }
		}
		public string rule_Type {
			get { return this.rule_TypeField; }
			set { this.rule_TypeField = value; }
		}
		public string priority {
			get { return this.priorityField; }
			set { this.priorityField = value; }
		}
		public string status {
			get { return this.statusField; }
			set { this.statusField = value; }
		}
		public string author {
			get { return this.authorField; }
			set { this.authorField = value; }
		}
		public bool delete {
			get { return this.deleteField; }
			set { this.deleteField = value; }
		}
		public string htmlContent {
			get { return this.htmlContentField; }
			set { this.htmlContentField = value; }
		}
	}
	[System.Xml.Serialization.XmlIncludeAttribute(typeof(View))]
	[System.Xml.Serialization.XmlIncludeAttribute(typeof(FolderStructure))]
	[System.Xml.Serialization.XmlIncludeAttribute(typeof(Library))]
	[System.Xml.Serialization.XmlIncludeAttribute(typeof(Folder))]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class Identifiable
	{
		private string idField;
		private bool deleteField;
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string id {
			get { return this.idField; }
			set { this.idField = value; }
		}
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public bool delete {
			get { return this.deleteField; }
			set { this.deleteField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class View : Identifiable
	{
		private string captionField;
		private string creatorField;
		private string parentFolderField;
		private string libraryField;
		private string creatorCaptionField;
		private string criteriaField;
		private System.DateTime lastUpdateField;
		private bool lastUpdateFieldSpecified;
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string caption {
			get { return this.captionField; }
			set { this.captionField = value; }
		}
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string creator {
			get { return this.creatorField; }
			set { this.creatorField = value; }
		}
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string parentFolder {
			get { return this.parentFolderField; }
			set { this.parentFolderField = value; }
		}
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string library {
			get { return this.libraryField; }
			set { this.libraryField = value; }
		}
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string creatorCaption {
			get { return this.creatorCaptionField; }
			set { this.creatorCaptionField = value; }
		}
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string criteria {
			get { return this.criteriaField; }
			set { this.criteriaField = value; }
		}
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public System.DateTime lastUpdate {
			get { return this.lastUpdateField; }
			set { this.lastUpdateField = value; }
		}
		[System.Xml.Serialization.XmlIgnoreAttribute()]
		public bool lastUpdateSpecified {
			get { return this.lastUpdateFieldSpecified; }
			set { this.lastUpdateFieldSpecified = value; }
		}
	}
	[System.Xml.Serialization.XmlIncludeAttribute(typeof(Library))]
	[System.Xml.Serialization.XmlIncludeAttribute(typeof(Folder))]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class FolderStructure : Identifiable
	{
		private View[] viewsField;
		private Folder[] foldersField;
		private string captionField;
		[System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
		public View[] Views {
			get { return this.viewsField; }
			set { this.viewsField = value; }
		}
		[System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
		public Folder[] Folders {
			get { return this.foldersField; }
			set { this.foldersField = value; }
		}
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string caption {
			get { return this.captionField; }
			set { this.captionField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class Folder : FolderStructure
	{
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class Library : FolderStructure
	{
		private bool readProtectionField;
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public bool readProtection {
			get { return this.readProtectionField; }
			set { this.readProtectionField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class TM_Library
	{
		private System.Guid idField;
		private string captionField;
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public System.Guid Id {
			get { return this.idField; }
			set { this.idField = value; }
		}
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Caption {
			get { return this.captionField; }
			set { this.captionField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class TM_GUI_Objects
	{
		private string[] guidanceItemsMappingsField;
		private string[] uniqueStringsField;
		public string[] GuidanceItemsMappings {
			get { return this.guidanceItemsMappingsField; }
			set { this.guidanceItemsMappingsField = value; }
		}
		public string[] UniqueStrings {
			get { return this.uniqueStringsField; }
			set { this.uniqueStringsField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class View_V3
	{
		private System.Guid libraryIdField;
		private System.Guid folderIdField;
		private System.Guid viewIdField;
		private string captionField;
		private string authorField;
		private string guidanceItems_IndexesField;
		private System.Guid[] guidanceItemsField;
		public System.Guid libraryId {
			get { return this.libraryIdField; }
			set { this.libraryIdField = value; }
		}
		public System.Guid folderId {
			get { return this.folderIdField; }
			set { this.folderIdField = value; }
		}
		public System.Guid viewId {
			get { return this.viewIdField; }
			set { this.viewIdField = value; }
		}
		public string caption {
			get { return this.captionField; }
			set { this.captionField = value; }
		}
		public string author {
			get { return this.authorField; }
			set { this.authorField = value; }
		}
		public string guidanceItems_Indexes {
			get { return this.guidanceItems_IndexesField; }
			set { this.guidanceItems_IndexesField = value; }
		}
		public System.Guid[] guidanceItems {
			get { return this.guidanceItemsField; }
			set { this.guidanceItemsField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class Folder_V3
	{
		private System.Guid libraryIdField;
		private System.Guid folderIdField;
		private string nameField;
		private View_V3[] viewsField;
		private Folder_V3[] subFoldersField;
		public System.Guid libraryId {
			get { return this.libraryIdField; }
			set { this.libraryIdField = value; }
		}
		public System.Guid folderId {
			get { return this.folderIdField; }
			set { this.folderIdField = value; }
		}
		public string name {
			get { return this.nameField; }
			set { this.nameField = value; }
		}
		public View_V3[] views {
			get { return this.viewsField; }
			set { this.viewsField = value; }
		}
		public Folder_V3[] subFolders {
			get { return this.subFoldersField; }
			set { this.subFoldersField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class Library_V3
	{
		private System.Guid libraryIdField;
		private string nameField;
		private Folder_V3[] subFoldersField;
		private View_V3[] viewsField;
		private System.Guid[] guidanceItemsField;
		public System.Guid libraryId {
			get { return this.libraryIdField; }
			set { this.libraryIdField = value; }
		}
		public string name {
			get { return this.nameField; }
			set { this.nameField = value; }
		}
		public Folder_V3[] subFolders {
			get { return this.subFoldersField; }
			set { this.subFoldersField = value; }
		}
		public View_V3[] views {
			get { return this.viewsField; }
			set { this.viewsField = value; }
		}
		public System.Guid[] guidanceItems {
			get { return this.guidanceItemsField; }
			set { this.guidanceItemsField = value; }
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://teammentor.net/")]
	public partial class TeamMentor_Article_Content
	{
		private string descriptionField;
		private string[] filesField;
		private System.Xml.XmlNode dataField;
		private bool sanitizedField;
		private string dataTypeField;
		public string Description {
			get { return this.descriptionField; }
			set { this.descriptionField = value; }
		}
		[System.Xml.Serialization.XmlElementAttribute("Files")]
		public string[] Files {
			get { return this.filesField; }
			set { this.filesField = value; }
		}
		public System.Xml.XmlNode Data {
			get { return this.dataField; }
			set { this.dataField = value; }
		}
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public bool Sanitized {
			get { return this.sanitizedField; }
			set { this.sanitizedField = value; }
		}
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string DataType {
			get { return this.dataTypeField; }
			set { this.dataTypeField = value; }
		}
	}
}
