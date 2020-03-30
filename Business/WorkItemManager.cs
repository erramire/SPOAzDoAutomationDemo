//This code is not production Ready, please change it to fulfill your company code standards and policies 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;

namespace ServiEntregaSPODemo.Business
{
    public class WorkItemManager
    {
        public void ReassignState(string workItemId, string oldValue)
        {
            var uri = new Uri("https://dev.azure.com");
            var personalAccessToken = "xxxxxx"; //Personal access token generated in AzDO
            var projectName = "xxxxxx"; //Project name in AzDo

            var credentials = new VssBasicCredential("", personalAccessToken);

            var connection = new Microsoft.VisualStudio.Services.WebApi.VssConnection(uri, credentials);

            var workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();

            var wiqlQuery = new Wiql() { Query = "Select * from WorkItems Where [ID] = " + workItemId };

            var workItemQueryResultForWiqlBasedQuery = workItemTrackingHttpClient.QueryByWiqlAsync(wiqlQuery).Result;

            var workItemsForQueryResultForWiqlBasedQuery = workItemTrackingHttpClient
                .GetWorkItemsAsync(
                    workItemQueryResultForWiqlBasedQuery.WorkItems.Select(workItemReference => workItemReference.Id),
                    expand: WorkItemExpand.All).Result;
            foreach (var item in workItemsForQueryResultForWiqlBasedQuery)
            {
                item.Fields["State"] = oldValue;
                var auxResult = workItemTrackingHttpClient.UpdateWorkItemAsync(GetUpdateWITOpp(oldValue), (int)item.Id).Result;

            }
        }

        private JsonPatchDocument GetUpdateWITOpp(string oldValue)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                                new JsonPatchOperation()
                                {
                                    Operation = Operation.Replace, //Tried Replace as well as Add
                                    Path = "/fields/System.State",
                                    Value = oldValue
                                }
                            );

            return patchDocument;

        }
    }
}