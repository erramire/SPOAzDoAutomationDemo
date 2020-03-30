//This code is not production Ready, please change it to fulfill your company code standards and policies 
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using OfficeDevPnP.Core;
using System.Security.Cryptography;

namespace ServiEntregaSPODemo.Controllers
{
    public class SPOSitesController : ApiController
    {
        [HttpGet]
        public string Get(string title, string description)
        {
            String result = "";
            
            string siteUrl = "https://xxxxx.sharepoint.com/sites/xxxxx"; // Sharepoint URL where you have administrative permisions 
            
            using (var cc = new OfficeDevPnP.Core.AuthenticationManager().GetAppOnlyAuthenticatedContext(siteUrl, "xxxxxxx", "xxxxxxx")) //put the client id and secret generate in sharepoint app registration site
            {
                cc.Load(cc.Web, p => p.Title);
                cc.ExecuteQuery();
                Console.WriteLine(cc.Web.Title);
                Web web = cc.Web;

                if (ProvisionSubSite(title,description,cc))
                {
                    result = "Hola el Site group fue creado satisfactoriamente: " + title + " Descripción: " + description;
                }
                else
                {
                    result = "Algo salio mal";
                }

            };
            return result;
        }

        private bool ProvisionSubSite(string subSiteInternalName, string description , ClientContext ctx)
        {
            bool result = false;
            try
            {                
                Web subSiteWeb = ctx.Site.RootWeb;
                if (subSiteWeb.WebExists(subSiteInternalName))
                {
                    Console.WriteLine("El Sitio ya existe");
                    subSiteWeb = ctx.Site.RootWeb.GetWeb(subSiteInternalName);
                }
                else
                {
                    WebCreationInformation webCreationInformation = new WebCreationInformation()
                    {                        
                        WebTemplate = "STS#0",
                        Title = subSiteInternalName,
                        Description= description,
                        Url = subSiteInternalName,
                        Language = 1033,
                        UseSamePermissionsAsParentSite = true
                    };
                    subSiteWeb = ctx.Web.Webs.Add(webCreationInformation);
                }
                ctx.Load(subSiteWeb);
                ctx.ExecuteQuery();
                string pageUrl = subSiteWeb.ServerRelativeUrl + "/SitePages/Home.aspx";                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Algo salio mal. " + ex.Message);
            }

            result = true;
            return result;
        }
    }
}
