namespace Proposal
{
    using Splunk.Client;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    class Program
    {
        static int Main(string[] args)
        {
            return 0;
        }

        public async Task AsIs()
        {
            //// This major issue surfaces over and over again
            //// - Confusion due to uncertainty about side effects
            //// Question: When must I call GetAsync to update the state of an
            //// entity or entity collection.

            using (var service = new Service(Scheme.Https, "localhost", 8089))
            {
                var collection = await service.GetApplicationsAsync(new ApplicationCollectionArgs()
                {
                    Offset = 0,
                    Count = 0
                });

                await collection.ReloadAsync(); // Instructs Splunk to reload all application state.
                await collection.GetAsync();    // Did reload update the collection? The docs say no so I call GetAsync

                //// Iterate through the collection using pagination

                int offset = 0;

                do
                {
                    collection = await service.GetApplicationsAsync(new ApplicationCollectionArgs()
                    {
                        Offset = offset
                    });

                    foreach (var application in collection)
                    {
                        if (application.Name == "twitter2")
                        {
                            await application.DisableAsync();                  // Does this method update state?
                            await application.GetAsync();                      // The docs say no so I GetAsync.

                            if (application.Disabled)                          // Thank goodness I checked the docs
                            {
                                Console.WriteLine("Verified: It worked");
                            }
                        }
                    }

                    offset += collection.Pagination.ItemsPerPage;
                }
                while (offset < collection.Pagination.TotalResults);
            }
        }

        public async Task MayBe()
        {
            using (var service = new Service(Scheme.Https,  "localhost", 8089))
            {
                //// Force Splunk to reload all data for the Applications entity collection

                var original = await service.ApplicationsEndpoint.GetAllAsync();
                await service.ApplicationsEndpoint.ReloadAsync();

                //// See if the list of application resources changed as a result of the reload

                var updated = await service.ApplicationsEndpoint.GetAllAsync();

                if (!updated.SequenceEqual(original))
                {
                    Console.WriteLine("List of application resources changed.");
                }

                //// Use pagination to iterate through the Applications entity collection (default: 30 at a time)

                var applicationsEndpoint = service.ApplicationsEndpoint;
                ApplicationCollection collection;
                ApplicationEndpoint endpoint;
                int offset = 0;
                int i = 0;

                do
                {
                    collection = await applicationsEndpoint.GetSliceAsync(offset);

                    foreach (var application in collection)
                    {
                        Console.WriteLine("{0}. {1}", ++i, application.Id);

                        if (application.Name == "twitter2")
                        {

                            //// As is: Confusion about what a REST API call because of side effects

                            await application.DisableAsync();                  // Does this method update state?
                            await application.GetAsync();                      // The docs say no so I GetAsync.

                            if (application.Disabled)                          // Thank goodness I checked the docs
                            {
                                Console.WriteLine("It worked");
                            }

                            //// Might be: One extra line of code, but greater clarity because of these two rules
                            //// 1. Methods produce no side effects
                            //// 2. Intellisense tells me what the REST API returns: an entity, an entity collection, 
                            ////    status, nothing, or something else.

                            endpoint = applicationsEndpoint.GetEndpoint("twitter2");

                            await endpoint.DisableAsync();                     // Rule: Methods produce no side effects
                            var twitter2 = await endpoint.GetAsync();          // Rule: Intellisense tells me what's returned

                            if (twitter2.Disabled)                             // Thank goodness: No need to check the docs!
                            {
                                Console.WriteLine("Verified: It worked!");
                            }
                        }
                    }

                    offset += collection.Pagination.ItemsPerPage;
                }
                while (offset < collection.Pagination.TotalResults);

                //// The loop was not really necessary

                endpoint = applicationsEndpoint.GetEndpoint("twitter2");
                Console.WriteLine("Renabling the twitter2 application at ",    // All endpoints have addresses
                    endpoint.Address);

                try
                {
                    await endpoint.EnableAsync();                              // Rule: Methods produce no side effects
                    var twitter2 = await endpoint.GetAsync();                  // Rule: Intellisense tells me what's returned

                    if (!twitter2.Disabled)
                    {
                        Console.WriteLine("Verified: It worked!");
                    }
                }
                catch (ResourceNotFoundException)
                {
                    Console.WriteLine("Say what?");
                }
            }
        }
    }
}
