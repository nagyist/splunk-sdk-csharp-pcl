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

// TODO:
// [O] Contracts
// [ ] Documentation
// [ ] Consider limiting the number of messages we process in a response body

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Provides a class that represents a Splunk service response message.
    /// </summary>
    public sealed class Message : IComparable, IComparable<Message>, IEquatable<Message>
    {
        #region Constructors
        
        internal Message(MessageType type, string text)
        {
            Contract.Requires<ArgumentOutOfRangeException>(MessageType.Debug <= type && type <= MessageType.Fatal, "type");
            Contract.Requires<ArgumentNullException>(text != null, "text");
            this.Type = type;
            this.Text = text;
        }

        #endregion

        #region Properties

        public string Text
        { get; private set; }

        public MessageType Type
        { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Compares the current <see cref="Message"/> with another object
        /// and returns an integer that indicates whether the current <see 
        /// cref="Message"/> precedes, follows, or appears in the same 
        /// position in the sort order as the other object.
        /// </summary>
        /// <param name="other">
        /// The object to compare to the current <see cref="Message"/>.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that indicates whether this instance 
        /// precedes, follows, or appears in the same position in the sort 
        /// order as <see cref="other"/>.
        /// <list type="table">
        /// <listheader>
        ///   <term>
        ///     Value
        ///   </term>
        ///   <description>
        ///     Condition
        ///   </description>
        /// </listheader>
        /// <item>
        ///   <term>
        ///     Less than zero
        ///   </term>
        ///   <description>
        ///     This instance precedes <see cref="other"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term>
        ///     Zero
        ///   </term>
        ///   <description>
        ///     This instance is in the same position in the sort order as <see
        ///     cref="other"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term>
        ///     Greater than zero
        ///   </term>
        ///   <description>
        ///     This instance follows <see cref="other"/>, <see cref="other"/>
        ///     is not an <see cref="Message"/>, or <see cref="other"/> is
        ///     <c>null</c>.
        ///   </description>
        /// </item>
        /// </list>
        /// </returns>
        public int CompareTo(object other)
        {
            return this.CompareTo(other as Message);
        }

        /// <summary>
        /// Compares the current <see cref="Message"/> with another one and
        /// returns an integer that indicates whether the current <see cref=
        /// "Message"/> precedes, follows, or appears in the same position in
        /// the sort order as the other one.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="Message"/>.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that indicates whether this instance 
        /// precedes, follows, or appears in the same position in the sort 
        /// order as <see cref="other"/>.
        /// <list type="table">
        /// <listheader>
        ///   <term>
        ///     Value
        ///   </term>
        ///   <description>
        ///     Condition
        ///   </description>
        /// </listheader>
        /// <item>
        ///   <term>
        ///     Less than zero
        ///   </term>
        ///   <description>
        ///     This instance precedes <see cref="other"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term>
        ///     Zero
        ///   </term>
        ///   <description>
        ///     This instance is in the same position in the sort order as <see
        ///     cref="other"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term>
        ///     Greater than zero
        ///   </term>
        ///   <description>
        ///     This instance follows <see cref="other"/> or <see cref="other"/>
        ///     is <c>null</c>.
        ///   </description>
        /// </item>
        /// </returns>
        public int CompareTo(Message other)
        {
            if (other == null)
            {
                return 1;
            }

            if (object.ReferenceEquals(this, other))
            {
                return 0;
            }
            
            int difference = this.Type - other.Type;
            return difference != 0 ? difference : this.Text.CompareTo(other.Text);
        }

        /// <summary>
        /// Determines whether the current <see cref="Message"/> and another
        /// object are equal.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="Message"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <see cref="other"/> is a non-null <see cref=
        /// "Message"/> and is the same as the current <see cref="Message"/>;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            return this.Equals(other as Message);
        }

        /// <summary>
        /// Determines whether the current <see cref="Message"/> and another
        /// one are equal.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="Message"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <see cref="other"/> is non-null and is the same as 
        /// the current <see cref="Message"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Message other)
        {
            if (other == null)
            {
                return false;
            }
            return object.ReferenceEquals(this, other) || (this.Type == other.Type && this.Text == other.Text);
        }

        /// <summary>
        /// Computes the hash code for the current <see cref="Message"/>.
        /// </summary>
        /// <returns>
        /// The hash code for the current <see cref="Message"/>.
        /// </returns>
        public override int GetHashCode()
        {
            // TODO: Check this against the algorithm presented in Effective Java
            int hash = 17;

            hash = (hash * 23) + this.Type.GetHashCode();
            hash = (hash * 23) + this.Text.GetHashCode();
            
            return hash;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat(this.Type.ToString(), ": ", this.Text);
        }

        #endregion

        #region Privates/internals

        internal static async Task<IReadOnlyList<Message>> ReadMessagesAsync(XmlReader reader)
        { 
            if (reader.ReadState == ReadState.Initial)
            {
                await reader.ReadAsync();

                if (reader.NodeType == XmlNodeType.XmlDeclaration)
                {
                    await reader.ReadAsync();
                }
            }

            if (!(reader.NodeType == XmlNodeType.Element && reader.Name == "response"))
            {
                throw new InvalidDataException();  // TODO: Diagnostics
            }

            await reader.ReadAsync();

            if (!(reader.NodeType == XmlNodeType.Element && reader.Name == "messages"))
            {
                throw new InvalidDataException();  // TODO: Diagnostics
            }

            var messages = new List<Message>();
            await reader.ReadAsync();

            while (reader.NodeType == XmlNodeType.Element)
            {
                if (reader.Name != "msg")
                {
                    throw new InvalidDataException(); // TODO: Diagnostics
                }

                // TODO: Throw InvalidDataException if type attribute is missing

                MessageType type = EnumConverter<MessageType>.Instance.Convert(reader.GetAttribute("type"));
                string text = await reader.ReadElementContentAsStringAsync();
                messages.Add(new Message(type, text));
            }

            return messages;
        }

        #endregion
    }
}
