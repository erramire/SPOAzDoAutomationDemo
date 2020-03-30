//This code is not production Ready, please change it to fulfill your company code standards and policies 

using Microsoft.Ajax.Utilities;


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiEntregaSPODemo.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ServiEntregaSPODemo.Controllers
{
    public class StatusWorkItemsController : ApiController
    {
        

        [HttpPost]
        public async System.Threading.Tasks.Task<string> ValidateWorkItemStateAsync() {
            string result = "";
            string bodycontent = await Request.Content.ReadAsStringAsync();
            
            JObject wit = JObject.Parse(bodycontent);

            var resource = wit["resource"];
            var workItemId = resource["workItemId"];
            var fieldStatus = resource["fields"].Where(t => t.ToString().Contains("System.State")).SingleOrDefault();
            
            var oldValue = fieldStatus.Values("oldValue").Select(t => (string)t).SingleOrDefault();
            var newValue = fieldStatus.Values("newValue").Select(t => (string)t).SingleOrDefault();
            if (!IsValidStated(oldValue,newValue))
            {
                WorkItemManager aux = new WorkItemManager();
                aux.ReassignState(workItemId.ToString(), oldValue);
            }
            return result;
        }

        private bool IsValidStated(string oldValue, string newValue)
        {
            bool result;
            switch (newValue)
            {
                case "Radicada":
                    result=ValidateRadicadaState(oldValue);
                    break;
                default:
                    result = true;
                    break;
            }
            return result;
        }

        private bool ValidateRadicadaState(string oldValue)
        {
            bool result;
            switch (oldValue)
            {
                case "En Documentación":
                    result = false;
                    break;
                default:
                    result=true;
                    break;
            }
            return result;
        }

        
    }
}
