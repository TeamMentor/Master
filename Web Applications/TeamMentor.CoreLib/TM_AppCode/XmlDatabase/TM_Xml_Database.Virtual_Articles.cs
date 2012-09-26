using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
//using System.Text;
using Microsoft.Security.Application;
using System.Security.Permissions;
using SecurityInnovation.TeamMentor.WebClient.WebServices;
//using Moq;
using O2.Kernel;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using urn.microsoft.guidanceexplorer;
using urn.microsoft.guidanceexplorer.guidanceItem;
using System.Xml.Serialization;
using O2.DotNetWrappers.Network;


namespace SecurityInnovation.TeamMentor.WebClient.WebServices
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
			return TM_Xml_Database.Path_XmlDatabase.pathCombine("Virtual_Articles.xml");
		}
		public static VirtualArticleAction add_Mapping_VirtualId(this TM_Xml_Database tmXmlDatabase, Guid id, Guid virtualId)
		{
			var virtualAction = new VirtualArticleAction()
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
			var virtualAction = new VirtualArticleAction()
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
			var virtualAction = new VirtualArticleAction()
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
			var virtualAction = new VirtualArticleAction()
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
			TM_Xml_Database.VirtualArticles = new Dictionary<Guid, VirtualArticleAction>();

			var virtualArticles = tmXmlDatabase.loadVirtualArticles();
			foreach (var virtualArticle in virtualArticles)
				TM_Xml_Database.VirtualArticles.add(virtualArticle.Id, virtualArticle);
			/*
			tmXmlDatabase.add_Mapping_VirtualId("c782a38d-dabc-4c67-a6cc-81a7fe305785".guid(), "31217d5a-1ad3-44a0-87c0-e09b9e004caa".guid());
			//.add_Mapping_VirtualId		("4f5039ae-54d4-4511-aad2-27920aa5a2c3".guid(), "31217d5a-1ad3-44a0-87c0-e09b9e004caa".guid())
			tmXmlDatabase.add_Mapping_ExternalArticle("4f5039ae-54d4-4511-aad2-27920aa5a2c3".guid(), "teammentor32.apphb.com", "4f5039ae-54d4-4511-aad2-27920aa5a2c3".guid());
			tmXmlDatabase.add_Mapping_Redirect("4f5039ae-54d4-4511-aad2-27920aa5a211".guid(), "http://www.google.com".uri());
			tmXmlDatabase.add_Mapping_Redirect("4f5039ae-54d4-4511-aad2-27920aa5a222".guid(), "http://teammentor32.apphb.com/article/4f5039ae-54d4-4511-aad2-27920aa5a2c3".uri())
				*/			;
			return tmXmlDatabase;
		}
		public static Dictionary<Guid, VirtualArticleAction> getVirtualArticles(this TM_Xml_Database tmXmlDatabase)
		{
			if (TM_Xml_Database.VirtualArticles.isNull())
				tmXmlDatabase.mapVirtualArticles();
			return TM_Xml_Database.VirtualArticles;

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
								return virtualArticle.createArticleFromExternalServiceData();
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

		public static TeamMentor_Article createArticleFromExternalServiceData(this VirtualArticleAction virtualArticle)
		{
			try
			{
				//Web.Https.ignoreServerSslErrors();
				switch (virtualArticle.Service)
				{ 
					case "wikipedia":
						{
						
							var externalArticle = new TeamMentor_Article();
							externalArticle.Metadata.Title = "From Wikipedia.org: " + virtualArticle.Service_Data;
							var wikipediaUrl = "https://en.wikipedia.org/wiki/{0}?action=render".format(virtualArticle.Service_Data);
							
							System.Net.WebClient webClient = new System.Net.WebClient();
							webClient.Headers.Add("User-Agent", "TeamMentor");
							//webClient.Headers.Add("Accept-Encoding", "gzip");							
							var wikipediaData = webClient.DownloadString(wikipediaUrl);
							externalArticle.Content.Data.Value = wikipediaData;
							return externalArticle;						
						}
					case "owasp":
						{

							var externalArticle = new TeamMentor_Article();
							externalArticle.Metadata.Title = "From owasp.org: " + virtualArticle.Service_Data;
							var owaspUrl = "https://www.owasp.org/index.php/{0}?action=render".format(virtualArticle.Service_Data);
							
							System.Net.WebClient webClient = new System.Net.WebClient();
							webClient.Headers.Add("User-Agent", "TeamMentor");
							//webClient.Headers.Add("Accept-Encoding", "gzip");							
							var owaspData = webClient.DownloadString(owaspUrl);
							externalArticle.Content.Data.Value = owaspData;
							return externalArticle;
						}
					case "msdn":
						{

							var externalArticle = new TeamMentor_Article();
							externalArticle.Metadata.Title = "From MSDN: " + virtualArticle.Service_Data;
							var msdnUrl = "http://msdn.microsoft.com/en-us/library/{0}.aspx".format(virtualArticle.Service_Data);
							/*
							System.Net.WebClient webClient = new System.Net.WebClient();
							webClient.Headers.Add("User-Agent", "TeamMentor");
							//webClient.Headers.Add("Accept-Encoding", "gzip");							
							var msdnData = webClient.DownloadString(msdnUrl);
							externalArticle.Content.Data.Value=  msdnData;*/
							externalArticle.Content.Data.Value = @"<IFrame src='{0}'/ id='msdnContent' style='width:99%;' frameborder='0'></IFrame>
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

		public static string getGuidRedirect(this TM_Xml_Database tmXmlDatabase, Guid id)
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
			return null;
		}


	}

}