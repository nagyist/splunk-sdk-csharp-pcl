/*
 * Copyright 2014 Splunk, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"): you may
 * not use this file except in compliance with the License. You may obtain
 * a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 */

//// TODO:
//// [O] Contracts
//// [O] Documentation

namespace Splunk.Client
{
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an object representation of the Splunk Applications endpoint.
    /// </summary>
    public class ApplicationEndpoint : Endpoint
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationEndpoint"/>
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="name">
        /// Name of the <see cref="ApplicationEntity"/> resource.
        /// </param>
        public ApplicationEndpoint(Service service, string name)
            : base(service, new ResourceName(ApplicationCollectionEndpoint.ClassResourceName, name))
        { }

        internal ApplicationEndpoint(Context context, Namespace ns, string name)
            : base(context, ns, new ResourceName(ApplicationCollectionEndpoint.ClassResourceName, name))
        {
        }
        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously disables the <see cref="Application"/> resource at
        /// <see cref="Address"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing this operation.
        /// </returns>
        /// <remarks>
        /// This method uses the POST apps/local/{name}/disable endpoint to 
        /// disable the current <see cref="Application"/>.
        /// </remarks>
        public async Task DisableAsync()
        {
            var name = new ResourceName(this.Name, "disable");

            using (var response = await this.Context.PostAsync(this.Namespace, name))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Asynchronously enables the <see cref="Application"/> resource at 
        /// <see cref="Address"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing this operation.
        /// </returns>
        /// <remarks>
        /// This method uses the POST apps/local/{name}/enable endpoint to 
        /// enable the current <see cref="Application"/>.
        /// </remarks>
        public async Task EnableAsync()
        {
            var name = new ResourceName(this.Name, "enable");

            using (var response = await this.Context.PostAsync(this.Namespace, name))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Asynchronously retrieves the <see cref="Application"/> resource at 
        /// <see cref="Address"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="Application"/> resource at <see cref="Entity.Address"/>.
        /// </returns>
        /// This method uses the <a href="http://goo.gl/fIQOrK">GET 
        /// apps/local/{name}</a> endpoint to retrieve the <see cref=
        /// "Application"/> resource at <see cref="Addresss"/>.
        /// </remarks>
        public async Task<Application> GetAsync()
        {
            using (Response response = await this.Context.GetAsync(this.Namespace, this.Name))
            {
                await response.EnsureStatusCodeAsync(System.Net.HttpStatusCode.OK);

                var entity = new Application();
                var feed = new AtomFeed();
                
                await feed.ReadXmlAsync(response.XmlReader);
                entity.Initialize(this.Context, feed);

                return entity;
            }
        }

        /// <summary>
        /// Asynchronously retrieves setup information for the <see cref=
        /// "Application"/> resource at <see cref="Address"/>.
        /// </summary>
        /// <returns>
        /// An object containing setup information for the <see cref="Application"/>
        /// resource at <see cref="Address"/>.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/mUT9gU">GET 
        /// apps/local/{name}/setup</a> endpoint to construct the <see cref=
        /// "ApplicationSetupInfo"/> instance it returns.
        /// </remarks>
        public async Task<ApplicationSetupInfo> GetSetupInfoAsync()
        {
            var resource = new ApplicationSetupInfo(this.Context, this.Namespace, this.Name.Title);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously retrieves update information for the <see cref=
        /// "Application"/> resource at <see cref="Address"/>.
        /// </summary>
        /// <returns>
        /// An object containing update information for the <see cref="Application"/>
        /// resource at <see cref="Address"/>.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/mrbtRj">GET 
        /// apps/local/{name}/update</a> endpoint to construct the <see cref=
        /// "ApplicationUpdateInfo"/> instance it returns.
        /// </remarks>
        public async Task<ApplicationUpdateInfo> GetUpdateInfoAsync()
        {
            var resource = new ApplicationUpdateInfo(this.Context, this.Namespace, this.Name.Title);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously archives the <see cref="Application"/> resource at
        /// <see cref="Address"/>.
        /// </summary>
        /// <returns>
        /// An object containing information about the newly created archive.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/DJkT7S">GET 
        /// apps/local/{name}/package</a> endpoint to create an archive of the 
        /// current <see cref="Application"/>.
        /// </remarks>
        public async Task<ApplicationArchiveInfo> PackageAsync()
        {
            var resource = new ApplicationArchiveInfo(this.Context, this.Namespace, this.Name.Title);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously removes the <see cref="Application"/> resource at
        /// <see cref="Address"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing this operation.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/fIQOrK">DELETE 
        /// apps/local/{name}</a> endpoint to remove the current <see cref=
        /// "Application"/>.
        /// </remarks>
        public async Task RemoveAsync()
        {
            using (var response = await this.Context.DeleteAsync(this.Namespace, this.Name))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Asynchronously updates the attributes of the <see cref="Application"/>
        /// at <see cref="Address"/>.
        /// </summary>
        /// <param name="attributes">
        /// New attributes for the <see cref="Application"/> at <see cref=
        /// "Address"/>.
        /// </param>
        /// <param name="checkForUpdates">
        /// A value of <c>true</c>, if Splunk should check Splunkbase for 
        /// updates to the current <see cref="Application"/> instance.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing this operation.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/dKraaR">POST 
        /// apps/local/{name}</a> endpoint to update the attributes of the 
        /// current <see cref="Application"/> and optionally check for
        /// updates on Splunkbase.
        /// </remarks>
        public async Task UpdateAsync(ApplicationAttributes attributes, bool checkForUpdates = false)
        {
            using (var response = await this.Context.PostAsync(this.Namespace, this.Name, attributes))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        #endregion
    }
}
