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

namespace SolidsoftReply.Esb.Libraries.Resolution
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Xml;

    /// <summary>
    /// Represents data provided to a BAM step
    /// </summary>
    [ComVisible(true)]
    [Serializable]
    public class BamStepData : IDisposable
    {
        /// <summary>
        /// Dictionary of properties
        /// </summary>
        private readonly IDictionary properties = new Hashtable();

        /// <summary>
        /// XML content of an XML document.
        /// </summary>
        private string xmlContent;

        /// <summary>
        /// Flag indicates if the object is disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="BamStepData"/> class. 
        /// </summary>
        public BamStepData()
        {
            this.ValueList = new ArrayList();   
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="BamStepData"/> class. 
        /// </summary>
        ~BamStepData()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets or sets an XML document.
        /// </summary>
        public XmlDocument XmlDocument
        {
            get
            {
                var outXmlDocument = new XmlDocument();

                if (!string.IsNullOrWhiteSpace(this.xmlContent))
                {
                    outXmlDocument.LoadXml(this.xmlContent);
                }

                return outXmlDocument;
            }

            set
            {
                this.xmlContent = value == null ? string.Empty : value.OuterXml;
            }
        }

        /// <summary>
        /// Gets a dictionary of properties.
        /// </summary>
        public IDictionary Properties
        {
            get
            {
                return this.properties;
            }
        }

        /// <summary>
        /// Gets or sets a list of values for the positional arguments of a format string.
        /// </summary>
        public IList ValueList { get; set; }

        /// <summary>
        /// Disposes the BAM Step data.
        /// </summary>
        /// <param name="disposing">Flag indicates whether Dispose() was called.</param>
        public virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                var disposable = this.Properties as IDisposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            this.disposed = true;
        }

        /// <summary>
        /// Disposes the BAM Step data.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
