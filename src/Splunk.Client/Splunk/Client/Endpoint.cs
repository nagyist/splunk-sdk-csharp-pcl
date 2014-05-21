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
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an object representation of a Splunk service endpoint.
    /// </summary>
    public class Endpoint
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Splunk <see cref="Endpoint"/> 
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="name">
        /// An object representing the name of an endpoint in the services 
        /// namespace.
        /// </param>
        public Endpoint(Service service, ResourceName name)
            : this(service.Context, service.Namespace, name)
        {
            Contract.Requires<ArgumentNullException>(service != null);
        }

        internal Endpoint(Context context, Namespace ns, ResourceName name)
        {
            Contract.Requires<ArgumentNullException>(context != null);
            Contract.Requires<ArgumentNullException>(ns != null);
            Contract.Requires<ArgumentNullException>(name != null);

            this.context = context;
            this.ns = ns;
            this.name = name;

            var builder = new StringBuilder(context.ToString());

            builder.Append("/");
            builder.Append(ns.ToUriString());
            builder.Append("/");
            builder.Append(name.ToUriString());

            this.address = new Uri(builder.ToString());
        }

        #endregion

        #region Properties

        public Uri Address
        {
            get { return this.address; }
        }

        protected Context Context
        {
            get { return this.context; }
        }

        protected Namespace Namespace
        {
            get { return this.ns; }
        }

        protected ResourceName Name
        {
            get { return this.name; }
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return this.Address.ToString();
        }

        #endregion

        #region Privates/internals

        readonly Uri address;
        readonly Context context;
        readonly Namespace ns;
        readonly ResourceName name;

        #endregion
    }
}
