﻿/*
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
//// [X] Properties & Methods

namespace Splunk.Client
{
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an object representation of a Splunk configuration stanza.
    /// </summary>
    public class ConfigurationStanza : EntityCollection<ConfigurationStanza, ConfigurationSetting>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationStanza"/>
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="fileName">
        /// Name of a configuration file.
        /// </param>
        /// <param name="stanzaName">
        /// Name of a stanza within <paramref name="fileName"/> containing the 
        /// configuration stanza to be represented by the current instance.
        /// </param>
        internal ConfigurationStanza(Context context, Namespace ns, string fileName, string stanzaName)
            : base(context, ns, new ResourceName(ConfigurationCollection.ClassResourceName, fileName, stanzaName))
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "ConfigurationStanza"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. Use one of these
        /// methods to obtain a <see cref="ConfigurationStanza"/> instance:
        /// <list type="table">
        /// <listheader>
        ///   <term>Method</term>
        ///   <description>Description</description>
        /// </listheader>
        /// <item>
        ///   <term><see cref="Configuration.CreateStanzaAsync"/></term>
        ///   <description>
        ///   Asynchronously creates a new <see cref="ConfigurationStanza"/>
        ///   in the current <see cref="Configuration"/> file.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Configuration.GetStanzaAsync"/></term>
        ///   <description>
        ///   Asynchronously retrieves an existing <see cref="ConfigurationStanza"/>
        ///   in the current <see cref="Configuration"/> file.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Configuration.UpdateStanzaAsync"/></term>
        ///   <description>
        ///   Asynchronously updates an existing <see cref="ConfigurationStanza"/>
        ///   in the current <see cref="Configuration"/> file.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Service.CreateConfigurationStanzaAsync"/></term>
        ///   <description>
        ///   Asynchronously creates a <see cref="ConfigurationStanza"/> 
        ///   identified by configuration file and stanza name.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Service.GetConfigurationStanzaAsync"/></term>
        ///   <description>
        ///   Asynchronously retrieves a <see cref="ConfigurationStanza"/> 
        ///   identified by configuration file and stanza name.
        ///   </description>
        /// </item>
        /// </list>
        /// </remarks>
        public ConfigurationStanza()
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously creates the configuration stanza represented by the
        /// current instance.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/jae44k">POST 
        /// properties/{file_name></a> endpoint to create the configuration
        /// stanza identified by the current instance.
        /// </remarks>
        public async Task CreateAsync()
        {
            var args = new Argument[] { new Argument("__stanza", this.ResourceName.Title) };
            var resourceName = this.ResourceName.GetParent();

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName, args))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created);
            }
        }

        /// <summary>
        /// Asynchronously removes the configuration stanza represented by the
        /// current instance.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/dpbuhQ">DELETE
        /// configs/conf-{file}/{name}</a> endpoint to remove the configuration
        /// stanza identified by the current instance.
        /// </remarks>
        public async Task RemoveAsync()
        {
            var resourceName = new ResourceName("configs", "conf-" + this.ResourceName.Collection,
                this.ResourceName.Title);

            using (var response = await this.Context.DeleteAsync(this.Namespace, resourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Asynchronously adds or updates settings in the configuration stanza
        /// represented by the current instance.
        /// </summary>
        /// <param name="settings">
        /// A variable-length list of objects representing the settings to be
        /// added or updated.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/w742jw">POST 
        /// properties/{file_name}/{stanza_name}</a> endpoint to add or update
        /// <paramref name="settings"/> in the current <see cref="ConfigurationStanza"/>.
        /// </remarks>
        public async Task UpdateAsync(params Argument[] settings)
        {
            Contract.Requires(settings != null);

            if (settings.Length <= 0)
            {
                return;
            }
            
            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, settings))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Asynchronously retrieves a configuration setting from the current
        /// instance.
        /// </summary>
        /// <param name="keyName">
        /// The name of a configuration setting.
        /// </param>
        /// <returns>
        /// An object representing the configuration setting identified by
        /// <paramref name="keyName"/>.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/cqT50u">GET 
        /// properties/{file_name}/{stanza_name}/{key_Name}</a> endpoint to 
        /// construct the <see cref="ConfigurationSetting"/> identified by 
        /// <paramref name="keyName"/>.
        /// </remarks>
        public async Task<ConfigurationSetting> GetSettingAsync(string keyName)
        {
            var resource = new ConfigurationSetting(this.Context, this.Namespace,
                fileName: this.ResourceName.Collection,
                stanzaName: this.ResourceName.Title,
                keyName: keyName);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously updates the value of a setting in the current <see 
        /// cref="ConfigurationStanza"/>.
        /// </summary>
        /// <param name="keyName">
        /// The name of a configuration setting in the current <see cref=
        /// "ConfigurationStanza"/>.
        /// </param>
        /// <param name="value">
        /// A new value for the setting identified by <paramref name="keyName"/>.
        /// </param>
        /// <returns>
        /// An object representing the configuration setting that was updated.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/sSzcMy">POST 
        /// properties/{file_name}/{stanza_name}/{key_Name}</a> endpoint to 
        /// update the <see cref="ConfigurationSetting"/> identified by <paramref 
        /// name="keyName"/>.
        /// </remarks>
        public async Task<ConfigurationSetting> UpdateSettingAsync(string keyName, string value)
        {
            var resource = new ConfigurationSetting(this.Context, this.Namespace, 
                fileName: this.ResourceName.Collection, 
                stanzaName: this.ResourceName.Title, 
                keyName: keyName); 
            await resource.UpdateAsync(value);
            return resource;
        }

        #endregion
    }
}
