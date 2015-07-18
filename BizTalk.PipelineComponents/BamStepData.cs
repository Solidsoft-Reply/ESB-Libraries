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

namespace SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents
{
    using System;
    using System.Collections;
    using System.Xml;

    using Microsoft.BizTalk.Message.Interop;

    /// <summary>
    /// Represents data provided to a BAM step in a BizTalk orchestration
    /// </summary>
    [Serializable]
    public class BamStepData : Resolution.BamStepData
    {
        /// <summary>
        /// The BizTalk transport message.
        /// </summary>
        private IBaseMessage bizTalkMessage;

        /// <summary>
        /// Gets an XML message.
        /// </summary>
        public new XmlDocument XmlDocument { get; private set; }

        /// <summary>
        /// Gets a dictionary of message properties
        /// </summary>
        public new IDictionary Properties { get; private set; }

        /// <summary>
        /// Gets or sets a BizTalk transport message.
        /// </summary>
        public IBaseMessage BizTalkMessage
        {
            get
            {
                return this.bizTalkMessage;
            }

            set
            {
                this.bizTalkMessage = value;

                // Get the XML content (if any) of the body part
                var part = this.bizTalkMessage.BodyOrFirstPartOrDefault();

                if (part != null)
                {
                    var xmlDocument = part.AsXmlDocument();
                    this.XmlDocument = xmlDocument.DocumentElement == null ? null : xmlDocument;
                }

                // Get the dictionary of properties
                var properties = new Hashtable();

                // Get the properties dictionary.
                foreach (var property in this.bizTalkMessage.Properties())
                {
                    properties.Add(string.Format("{0}#{1}", property.NameSpace, property.Name), property.Value);
                }

                this.Properties = properties;
            }
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
