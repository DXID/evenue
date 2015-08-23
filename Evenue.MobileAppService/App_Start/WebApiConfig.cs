using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Config;
using Evenue.MobileAppService.DataObjects;
using Evenue.MobileAppService.Models;

namespace Evenue.MobileAppService
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            HttpConfiguration config = new HttpConfiguration();

            new MobileAppConfiguration()
                .UseDefaultConfiguration()
                .ApplyTo(config);

            Database.SetInitializer(new EvenueBackEndAPIInitializer());
        }
    }

    public class EvenueBackEndAPIInitializer : ClearDatabaseSchemaIfModelChanges<EvenueBackEndAPIContext>
    {
        protected override void Seed(EvenueBackEndAPIContext context)
        {
            List<Event> events = new List<Event>
            {
                new Event { Id = "1", Category="aa", Desc="vv", fee=99, Location="dd", Title="aa", EndDate="dddd", StartDate="ff" },
                new Event { Id = "1", Category="aa", Desc="vv", fee=99, Location="dd", Title="aa", EndDate="dddd", StartDate="ff" },
            };

            List<User> users = new List<User>
            {
                new User { Id = "1", Events = events }
            };

            base.Seed(context);
        }
    }
}

