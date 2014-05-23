using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using System.Xml.Serialization;


namespace TeamMentor.CoreLib
{	
	[Serializable]
	public class VirtualArticleAction
	{
		[XmlAttribute] public Guid		Id				{ get; set; }
		[XmlAttribute] public String	Action			{ get; set; }		
		[XmlAttribute] public Guid		Target_Id		{ get; set; }						
		[XmlAttribute] public String	Redirect_Uri	{ get; set; }		// this should be an Uri but Uri's are not serializable
		[XmlAttribute] public string	TM_Server		{ get; set; }						
		[XmlAttribute] public string	Service			{ get; set; }
		[XmlAttribute] public string	Service_Data		{ get; set; }
	}

	public static class TM_Xml_Database_VirtualArticle_ExtensionMethods
	{
		public static string getVirtualArticlesFile(this TM_Xml_Database tmXmlDatabase)
		{
			//return TM_Xml_Database.Path_XmlDatabase.pathCombine("Virtual_Articles.xml");
			return TM_Xml_Database.Current.Path_XmlLibraries.pathCombine("Virtual_Articles.xml");
		}
		public static VirtualArticleAction add_Mapping_VirtualId(this TM_Xml_Database tmXmlDatabase, Guid id, Guid virtualId)
		{
			var virtualAction = new VirtualArticleAction
										{
											Id = id,
											Action = "Virtual_Id",
											Target_Id = virtualId
										};
			tmXmlDatabase.getVirtualArticles().add(id, virtualAction);
			tmXmlDatabase.saveVirtualArticles();
			return virtualAction;
		}
		public static VirtualArticleAction add_Mapping_Redirect(this TM_Xml_Database tmXmlDatabase, Guid id, Uri redirectUri)
		{
			var virtualAction = new VirtualArticleAction
										{
											Id = id,
											Action = "Redirect",
											Redirect_Uri = redirectUri.str()
										};
			tmXmlDatabase.getVirtualArticles().add(id, virtualAction);
			tmXmlDatabase.saveVirtualArticles();
			return virtualAction;
		}
		public static VirtualArticleAction add_Mapping_ExternalArticle(this TM_Xml_Database tmXmlDatabase, Guid id, string tmServer, Guid externalId)
		{
			var virtualAction = new VirtualArticleAction
									{
										Id = id,
										Action = "ExternalArticle",
										TM_Server = tmServer,
										Target_Id = externalId,
									};
			tmXmlDatabase.getVirtualArticles().add(id, virtualAction);
			tmXmlDatabase.saveVirtualArticles();
			return virtualAction;
		}
		public static VirtualArticleAction add_Mapping_ExternalService(this TM_Xml_Database tmXmlDatabase, Guid id, string service, string data)
		{
			var virtualAction = new VirtualArticleAction
									{
										Id = id,
										Action = "ExternalService",
										Service = service,
										Service_Data = data,
									};
			tmXmlDatabase.getVirtualArticles().add(id, virtualAction);
			tmXmlDatabase.saveVirtualArticles();
			return virtualAction;
		}
		public static bool remove_Mapping_VirtualId(this TM_Xml_Database tmXmlDatabase, Guid id)
		{
			var virtualArticles = tmXmlDatabase.getVirtualArticles();
			if (virtualArticles.hasKey(id))
			{
				virtualArticles.Remove(id);
				tmXmlDatabase.saveVirtualArticles();
				return true;
			}
			return false;
		}
		public static List<VirtualArticleAction> loadVirtualArticles(this TM_Xml_Database tmXmlDatabase)
		{
			var virtualArticlesFile = tmXmlDatabase.getVirtualArticlesFile();
			if (virtualArticlesFile.fileExists())
				return virtualArticlesFile.load<List<VirtualArticleAction>>();
			return new List<VirtualArticleAction>();
		}
		public static TM_Xml_Database saveVirtualArticles(this TM_Xml_Database tmXmlDatabase)
		{
			var virtualArticlesFile = tmXmlDatabase.getVirtualArticlesFile();

			var virtualArticles = tmXmlDatabase.getVirtualArticles().Values.toList();
			virtualArticles.saveAs(virtualArticlesFile);
			return tmXmlDatabase;
		}
		public static TM_Xml_Database mapVirtualArticles(this TM_Xml_Database tmXmlDatabase)
		{
            var virtualArticles = tmXmlDatabase.VirtualArticles;
            virtualArticles.Clear();

			var virtualArticles_ToMap = tmXmlDatabase.loadVirtualArticles();
			foreach (var virtualArticle in virtualArticles_ToMap)
				virtualArticles.add(virtualArticle.Id, virtualArticle);
			return tmXmlDatabase;
		}
		public static Dictionary<Guid, VirtualArticleAction> getVirtualArticles(this TM_Xml_Database tmXmlDatabase)
		{
			if (TM_Xml_Database.Current.VirtualArticles.isNull())
				tmXmlDatabase.mapVirtualArticles();
			return TM_Xml_Database.Current.VirtualArticles;

		}
		public static VirtualArticleAction virtualArticle(this TM_Xml_Database tmXmlDatabase, Guid id)
		{
			var virtualArticles = tmXmlDatabase.getVirtualArticles();
			if (virtualArticles.hasKey(id))
				return virtualArticles[id];
			return null;
		}

		public static Guid getVirtualGuid_if_MappingExists(this TM_Xml_Database tmXmlDatabase, Guid id)
		{
			var virtualArticle = tmXmlDatabase.virtualArticle(id);
			if (virtualArticle.notNull() && virtualArticle.Action == "Virtual_Id")
				return virtualArticle.Target_Id;
			return id;
		}
		public static TeamMentor_Article getExternalTeamMentorArticle_if_MappingExists(this TM_Xml_Database tmXmlDatabase, Guid id)
		{
			try
			{
				var virtualArticle = tmXmlDatabase.virtualArticle(id);
				if (virtualArticle.notNull())
				{
					switch (virtualArticle.Action)
					{
						case "ExternalArticle":
							{

								var articleUri = "{0}/raw/{1}".format(virtualArticle.TM_Server, virtualArticle.Target_Id).uri();
								var articleXml = articleUri.get_Html();
								var externalArticle = articleXml.deserialize<TeamMentor_Article>(false);
								return externalArticle;
							}
						case "ExternalService":
							{
								return virtualArticle.createArticle_from_ExternalServiceData();
							}
					}
					
				}
			}
			catch (Exception ex)
			{
				ex.log("in getExternalTeamMentorArticle_if_MappingExists");
			}
			return null;
		}

		public static TeamMentor_Article createArticle_from_ExternalServiceData(this VirtualArticleAction virtualArticle)
		{
			if (virtualArticle.Service.valid() && virtualArticle.Service_Data.valid())
				return virtualArticle.Service.createArticle_from_ExternalServiceData(virtualArticle.Service_Data);
			return null;
		}
		public static TeamMentor_Article createArticle_from_ExternalServiceData(this string service, string serviceData)
		{
			try
			{
				Func<string, string, TeamMentor_Article> createArticleFromUrl = 
					(title, url) =>	{
										var externalArticle = new TeamMentor_Article {Metadata = {Title = title}};
					               	    var webClient = new System.Net.WebClient();
										webClient.Headers.Add("User-Agent", "TeamMentor");
										var htmlContent = webClient.DownloadString(url);
										var sanitizedHtml = Microsoft.Security.Application.Sanitizer.GetSafeHtmlFragment(htmlContent);										
										externalArticle.Content.Data.Value = sanitizedHtml;
										return externalArticle;
									};

				//Web.Https.ignoreServerSslErrors();
				switch (service)
				{ 
					case "wikipedia":
						{							
							var url = "https://en.wikipedia.org/wiki/{0}?action=render".format(serviceData);
							return createArticleFromUrl("From Wikipedia.org: " + serviceData, url);						
						}
					case "owasp":
						{
							
							var title  = "From owasp.org: " + serviceData;
							var url = "https://www.owasp.org/index.php/{0}?action=render".format(serviceData);
							return createArticleFromUrl(title, url);													
						}
					case "msdn":
						{

							var externalArticle = new TeamMentor_Article {Metadata = {Title = "From MSDN: " + service}};
						    var msdnUrl = "http://msdn.microsoft.com/en-us/library/{0}.aspx".format(serviceData);
						
							externalArticle.Content.Data.Value = @"<IFrame src='{0}'/ id='msdnContent' style='width:99%;height:500px' frameborder='0'></IFrame>
																	<script>$('#msdnContent').height(document.height-180)</script>".format(msdnUrl);

							return externalArticle;
						}
				}
			}
			catch (Exception ex)
			{
				ex.log("in createArticleFromExternalServiceData");
			}
			return null;
		}

		public static string get_GuidRedirect(this TM_Xml_Database tmXmlDatabase, Guid id)
		{
			var virtualArticle = tmXmlDatabase.virtualArticle(id);
			if (virtualArticle.notNull())
			{
				if (virtualArticle.Action == "Redirect")
				{
					var redirect = virtualArticle.Redirect_Uri.str();
					"Found Guid Rediction: {0} -> {1}".info(id, redirect);		
					return redirect;
				}

			}	
			var virtualArticles_Config = tmXmlDatabase.tmConfig().VirtualArticles;
		    if (virtualArticles_Config.AutoRedirectIfGuidNotFound)
		    {
		        var redirect = virtualArticles_Config.AutoRedirectTarget.pathCombine(id.str());
		        "[AutoRedirectIfGuidNotFound is set] set redirection to {0}".info(redirect);
		        return redirect;
		    }
		    return null;
		}
	}

}