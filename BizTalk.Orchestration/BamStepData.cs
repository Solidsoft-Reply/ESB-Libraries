// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BamStepData.cs" company="Solidsoft Reply Ltd.">
//   Copyright (c) 2015 Solidsoft Reply Limited. All rights reserved.
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SolidsoftReply.Esb.Libraries.BizTalk.Orchestration
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Xml;

    using Microsoft.BizTalk.XLANGs.BTXEngine;
    using Microsoft.XLANGs.BaseTypes;

    /// <summary>
    /// Represents data provided to a BAM step in a BizTalk orchestration
    /// </summary>
    [Serializable]
    public class BamStepData : Resolution.BamStepData
    {
        /// <summary>
        /// The BizTalk orchestration message.
        /// </summary>
        private XLANGMessage bizTalkMessage;

        /// <summary>
        /// Gets an XML document.
        /// </summary>
        public new XmlDocument XmlDocument
        {
            get
            {
                return base.XmlDocument;
            }

            private set
            {
                base.XmlDocument = value;
            }
        }

        /// <summary>
        /// Gets a dictionary of properties.
        /// </summary>
        public new IDictionary Properties
        {
            get
            {
                return base.Properties;
            }
        }

        /// <summary>
        /// Gets or sets a BizTalk orchestration message.
        /// </summary>
        public XLANGMessage BizTalkMessage
        {
            get
            {
                return this.bizTalkMessage;
            }

            set
            {
                this.bizTalkMessage = value;

                // Invoke private method to unwrap message.
                var unwrappedBizTalkMessage =
                (BTXMessage)
                    this.bizTalkMessage.GetType()
                        .GetMethod("Unwrap", BindingFlags.Instance | BindingFlags.NonPublic)
                        .Invoke(this.bizTalkMessage, null);

                // Get the XML content (if any) of the body part
                var part = unwrappedBizTalkMessage.BodyPart ?? unwrappedBizTalkMessage[0];

                if (part != null)
                {
                    try
                    {
                        this.XmlDocument = (XmlDocument)part.RetrieveAs(typeof(XmlDocument));
                    }
                    catch
                    {
                        this.XmlDocument = null;
                    }
                }

                // Get the dictionary of properties
                var properties = new Hashtable();

                // Get content properties dictionary. Content properties are linked to message content via XSD annotations
                foreach (DictionaryEntry contentProperty in unwrappedBizTalkMessage.GetContentProperties() ?? new Hashtable())
                {
                    var queryNameContent = (XmlQName)contentProperty.Key;
                    properties.Add(string.Format("{0}#{1}", queryNameContent.Namespace, queryNameContent.Name), contentProperty.Value);
                }

                // Add context properties to dictionary. Context properties are not linked to any message content
                foreach (DictionaryEntry contextProperty in unwrappedBizTalkMessage.GetContextProperties() ?? new Hashtable())
                {
                    var queryNameContext = (XmlQName)contextProperty.Key;
                    properties.Add(string.Format("{0}#{1}", queryNameContext.Namespace, queryNameContext.Name), contextProperty.Value);
                }

                foreach (var messagePropertyKey in properties.Keys)
                {
                    if (this.Properties.Contains(messagePropertyKey))
                    {
                        this.Properties[messagePropertyKey] = properties[messagePropertyKey];
                    }
                    else
                    {
                        this.Properties.Add(messagePropertyKey, properties[messagePropertyKey]);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a count of the list of values.
        /// </summary>
        public int ValueListCount
        {
            get
            {
                return this.ValueList == null ? 0 : this.ValueList.Count;
            }
        }

        /// <summary>
        /// Writes a value to value list.  The value is added to the end of the list.
        /// </summary>
        /// <param name="value">The value to be written.</param>
        public void ValueListWrite(object value)
        {
            if (this.ValueList == null)
            {
                this.ValueList = new ArrayList();
            }

            this.ValueList.Add(value);
        }

        /// <summary>
        /// Writes a value to a given position in the value list.
        /// </summary>
        /// <param name="value">The value to be written.</param>
        /// <param name="position">The value position.</param>
        public void ValueListWrite(object value, int position)
        {
            if (position < 0)
            {
                // Throw exception here
            }
            else if (position >= this.ValueList.Count)
            {
                for (var index = this.ValueList.Count; index < position; index++)
                {
                    this.ValueList.Add(null);
                }

                this.ValueList.Add(value);
            }

            if (this.ValueList == null)
            {
                this.ValueList = new ArrayList();
            }

            this.ValueList.Add(value);
        }

        /// <summary>
        /// Reads a value from the value list
        /// </summary>
        /// <param name="position">The value position.</param>
        /// <returns>The value read.</returns>
        public object ValueListRead(int position)
        {
            return this.ValueList == null ? null : this.ValueList[position];
        }

        /// <summary>
        /// Disposes the BAM Step data.
        /// </summary>
        /// <param name="disposing">Flag indicates whether Dispose() was called.</param>
        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
            {
                return;
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            var disposable = this.bizTalkMessage as IDisposable;

            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
