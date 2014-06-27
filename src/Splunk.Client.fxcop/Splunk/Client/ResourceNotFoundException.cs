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

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.Serialization;

    /// <summary>
    /// The exception that is thrown when a request to retrieve a <see cref=
    /// "Resource&lt;TResource&gt;"/> results in <see cref="HttpStatusCode.NotFound"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
    [Serializable]
    public sealed class ResourceNotFoundException : RequestException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceNotFoundException"/>
        /// class.
        /// </summary>
        /// <param name="message">
        /// An object representing an HTTP response message including the status
        /// code and data.
        /// </param>
        /// <param name="details">
        /// A sequence of <see cref="Message"/> instances detailing the cause
        /// of the <see cref="ResourceNotFoundException"/>.
        /// </param>
        internal ResourceNotFoundException(HttpResponseMessage message, IEnumerable<Message> details)
            : base(message, details)
        {
            Contract.Requires<ArgumentException>(message.StatusCode == HttpStatusCode.NotFound);
        }

        ResourceNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}