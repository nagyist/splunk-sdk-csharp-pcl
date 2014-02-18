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

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ResourceName : IReadOnlyList<string>
    {
        #region Constructors

        public ResourceName(IEnumerable<string> parts)
        {
            this.parts = parts.ToArray();
        }

        public ResourceName(params string[] parts)
            : this((IEnumerable<string>)parts)
        { }

        #endregion

        #region Fields

        public static readonly ResourceName AppsLocal = new ResourceName("apps", "local");
        public static readonly ResourceName Capabilities = new ResourceName("authorization", "capabilities");
        public static readonly ResourceName Configs = new ResourceName("configs");
        public static readonly ResourceName Confs = new ResourceName("properties");
        public static readonly ResourceName Export = new ResourceName("search", "jobs", "export");
        public static readonly ResourceName Indexes = new ResourceName("data", "indexes");
        public static readonly ResourceName Info = new ResourceName("server", "info");
        public static readonly ResourceName Inputs = new ResourceName("data", "inputs");
        public static readonly ResourceName Jobs = new ResourceName("search", "jobs");
        public static readonly ResourceName Login = new ResourceName("auth", "login");
        public static readonly ResourceName Logger = new ResourceName("server", "logger");
        public static readonly ResourceName Messages = new ResourceName("messages");
        public static readonly ResourceName ModularInputKinds = new ResourceName("data", "modular-inputs");
        public static readonly ResourceName Roles = new ResourceName("authorization", "roles");
        public static readonly ResourceName SavedSearches = new ResourceName("saved", "searches");
        public static readonly ResourceName Settings = new ResourceName("server", "settings");
        public static readonly ResourceName Stanza = new ResourceName("configs", "conf-%s", "%s");
        public static readonly ResourceName Users = new ResourceName("authentication", "users");
        
        #endregion

        #region Properties

        public string this[int index]
        {
	        get { return this.parts[index]; }
        }

        public int Count
        {
            get { return this.parts.Count; }
        }

        #endregion

        #region Methods

        public IEnumerator<string> GetEnumerator()
        {
            return this.parts.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.parts.GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join("/", from segment in this select Uri.EscapeUriString(segment));
        }

        #endregion

        #region Privates

        readonly IReadOnlyList<string> parts;

        #endregion
    }
}