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
    public class ApplicationCollectionEndpoint : Endpoint
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
        /// <param name="args">
        /// </param>
        internal ApplicationCollectionEndpoint(Service service)
            : base(service, ClassResourceName)
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously creates a new application from a template.
        /// </summary>
        /// <param name="name">
        /// Name of the application to be created.
        /// </param>
        /// <param name="template">
        /// Name of the template from which to create the application
        /// </param>
        /// <param name="attributes">
        /// Optional attributes for the application to be created.
        /// </param>
        /// <returns>
        /// Information about the application created.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/SzKzNX">POST 
        /// apps/local</a> endpoint to create the current <see cref=
        /// "Application"/>.
        /// </remarks>
        public async Task<Application> CreateAsync(string name, string template,
            ApplicationAttributes attributes = null)
        {
            var args = new CreationArgs()
            {
                ExplicitApplicationName = name,
                Filename = false,
                Name = name,
                Template = template
            }
            .Concat(attributes);

            using (var response = await this.Context.PostAsync(this.Namespace, this.Name, args))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created);
                var entity = new Application();
                var feed = new AtomFeed();

                await feed.ReadXmlAsync(response.XmlReader);
                entity.Initialize(this.Context, feed);

                return entity;
            }
        }

        /// <summary>
        /// Asynchronously retrieves the complete <see cref="ApplicationCollection"/>
        /// resource at the current endpoint <see cref="Address"/>.
        /// </summary>
        /// <returns>
        /// The collection of <see cref="Application"/> resources retrieved.
        /// </returns>
        public async Task<ApplicationCollection> GetAllAsync()
        {
            var args = new Argument[] { new Argument("count", "0") };

            using (Response response = await this.Context.GetAsync(this.Namespace, this.Name, args))
            {
                await response.EnsureStatusCodeAsync(System.Net.HttpStatusCode.OK);

                var feed = new AtomFeed();
                await feed.ReadXmlAsync(response.XmlReader);

                var entity = new ApplicationCollection();
                entity.Initialize(this.Context, feed);

                return entity;
            }
        }

        /// <summary>
        /// Asynchronously retrieves a slice of the <see cref="ApplicationCollection"/>
        /// resource at the current endpoint <see cref="Address"/>.
        /// </summary>
        /// <param name="args">
        /// Specifies the slice of the <see cref="ApplicationCollection"/>
        /// resource to be retrieved.
        /// </param>
        /// <returns>
        /// The collection of <see cref="Application"/> resources retrieved.
        /// </returns>
        public async Task<ApplicationCollection> GetSliceAsync(int offset = 0,
            int count = 30, ApplicationCollectionArgs args = null)
        {
            var slice = new Argument[] 
            { 
                new Argument("offset", offset),
                new Argument("count", count)
            };

            using (Response response = await this.Context.GetAsync(this.Namespace, this.Name, slice, args))
            {
                await response.EnsureStatusCodeAsync(System.Net.HttpStatusCode.OK);
                
                var feed = new AtomFeed();
                await feed.ReadXmlAsync(response.XmlReader);

                var entity = new ApplicationCollection();
                entity.Initialize(this.Context, feed);

                return entity;
            }
        }

        /// <summary>
        /// Gets an <see cref="ApplicationEndpoint"/> by name.
        /// </summary>
        /// <param name="name">
        /// The name of an <see cref="Application"/> resource.
        /// </param>
        /// <returns>
        /// An endpoint for manipulating the <see cref="Application"/> resource
        /// identified by <paramref name="name"/>.
        /// </returns>
        public ApplicationEndpoint GetEndpoint(string name)
        {
            return new ApplicationEndpoint(this.Context, this.Namespace, name);
        }

        /// <summary>
        /// Asynchronously installs an application from a Splunk application
        /// archive file.
        /// </summary>
        /// <param name="path">
        /// Specifies the location of a Splunk application archive file.
        /// </param>
        /// <param name="name">
        /// Optionally overrides the name of the application.
        /// </param>
        /// <param name="update">
        /// <c>true</c> if Splunk should allow the installation to update an
        /// existing application. The default value is <c>false</c>.
        /// </param>
        /// <returns>
        /// The <see cref="Application"/> installed.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/SzKzNX">POST 
        /// apps/local</a> endpoint to install the application from the archive
        /// file on <paramref name="path"/>.
        /// </remarks>
        public async Task<Application> InstallAsync(string path, string name = null, bool update = false)
        {
            var resourceName = ApplicationCollection.ClassResourceName;

            var args = new CreationArgs()
            {
                ExplicitApplicationName = name,
                Filename = true,
                Name = path,
                Update = update
            };

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName, args))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created);

                var entity = new Application();
                var feed = new AtomFeed();

                await feed.ReadXmlAsync(response.XmlReader);
                entity.Initialize(this.Context, feed);

                return entity;
            }
        }

        /// <summary>
        /// Asynchronously forces the Splunk server to reload data for the
        /// <see cref="ApplicationCollection"/> at the current endpoint
        /// <see cref="Address"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing this operation.
        /// </returns>
        public async Task ReloadAsync()
        {
            var reload = new ResourceName(this.Name, "_reload");

            using (Response response = await this.Context.GetAsync(this.Namespace, reload))
            {
                await response.EnsureStatusCodeAsync(System.Net.HttpStatusCode.OK);
            }
        }

        #endregion

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("apps", "local");

        class CreationArgs : Args<CreationArgs>
        {
            [DataMember(Name = "explicit_appname", EmitDefaultValue = false)]
            public string ExplicitApplicationName
            { get; set; }

            [DataMember(Name = "filename", IsRequired = true)]
            public bool? Filename
            { get; set; }

            [DataMember(Name = "name", IsRequired = true)]
            public string Name
            { get; set; }

            [DataMember(Name = "template", EmitDefaultValue = false)]
            public string Template
            { get; set; }

            [DataMember(Name = "update", EmitDefaultValue = false)]
            public bool? Update
            { get; set; }
        }

        #endregion
    }
}
