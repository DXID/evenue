using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using Evenue.MobileAppService.DataObjects;
using Evenue.MobileAppService.Models;
using System;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Web.Configuration;

namespace Evenue.MobileAppService.Controllers
{
    [Authorize]
    public class EventController : TableController<Event>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            EvenueBackEndAPIContext context = new EvenueBackEndAPIContext();
            DomainManager = new EntityDomainManager<Event>(context, Request);
        }

        // GET tables/Event
        public IQueryable<Event> GetAllEvent()
        {
            return Query(); 
        }

        // GET tables/Event/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Event> GetEvent(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Event/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Event> PatchEvent(string id, Delta<Event> patch)
        {
            
            return UpdateAsync(id, patch);
        }

        // POST tables/Event
        public async Task<IHttpActionResult> PostEvent(Event item)
        {
            // Put the storage account name and key here. You can also take the value from the portal, or web.config file
            string storageAccountName = WebConfigurationManager.AppSettings["STORAGE_ACCOUNT_NAME"];
            string storageAccountKey = WebConfigurationManager.AppSettings["STORAGE_ACCOUNT_ACCESS_KEY"];

            // Set the URI for the Blob Storage service.
            Uri blobEndpoint = new Uri(string.Format("https://{0}.blob.core.windows.net", storageAccountName));

            // Create the BLOB service client.
            CloudBlobClient blobClient = new CloudBlobClient(blobEndpoint,
                new StorageCredentials(storageAccountName, storageAccountKey));

            if (item.ContainerName != null)
            {
                // Set the BLOB store container name on the item, which must be lowercase.
                item.ContainerName = item.ContainerName.ToLower();

                // Create a container, if it doesn't already exist.
                CloudBlobContainer container = blobClient.GetContainerReference(item.ContainerName);
                await container.CreateIfNotExistsAsync();

                // Create a shared access permission policy. 
                BlobContainerPermissions containerPermissions = new BlobContainerPermissions();

                // Enable anonymous read access to BLOBs.
                containerPermissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                container.SetPermissions(containerPermissions);

                // Define a policy that gives write access to the container for 5 minutes.                                   
                SharedAccessBlobPolicy sasPolicy = new SharedAccessBlobPolicy()
                {
                    SharedAccessStartTime = DateTime.UtcNow,
                    SharedAccessExpiryTime = DateTime.MaxValue,
                    Permissions = SharedAccessBlobPermissions.Write
                };

                // Get the SAS as a string.
                item.SasQueryString = container.GetSharedAccessSignature(sasPolicy);

                // Set the URL used to store the image.
                item.ImageUri = string.Format("{0}{1}/{2}", blobEndpoint.ToString(),
                    item.ContainerName, item.ResourceName);
            }

            Event current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Event/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteEvent(string id)
        {
             return DeleteAsync(id);
        }

    }
}