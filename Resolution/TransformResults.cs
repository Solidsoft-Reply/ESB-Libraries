// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformResults.cs" company="Solidsoft Reply Ltd.">
//   Copyright 2015 Solidsoft Reply Limited.
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
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Xsl;

    /// <summary>
    /// The results of a map-based transform.
    /// </summary>
    public struct TransformResults
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformResults"/> struct. 
        /// </summary>
        /// <param name="originalDocument">
        /// The original document to be mapped.
        /// </param>
        /// <param name="transformedDocument">
        /// The transformed document.
        /// </param>
        /// <param name="xslt">
        /// The XSLT document.
        /// </param>
        /// <param name="xsltArguments">
        /// The XSLT arguments.
        /// </param>
        /// <param name="sourceSchemas">
        /// The source schemas.
        /// </param>
        /// <param name="targetSchemas">
        /// The target schemas.
        /// </param>
        /// <param name="targetSchemaStrongNames">
        /// The strong names of the target schemas.
        /// </param>
        public TransformResults(
            XmlDocument originalDocument,
            XmlDocument transformedDocument,
            XmlDocument xslt,
            XsltArgumentList xsltArguments,
            string[] sourceSchemas,
            string[] targetSchemas,
            IEnumerable<string> targetSchemaStrongNames) : this()
        {
            this.OriginalDocument = originalDocument;
            this.TransformedDocument = transformedDocument;
            this.Xslt = xslt;
            this.XsltArguments = xsltArguments;
            this.SourceSchemas = sourceSchemas;
            this.TargetSchemas = targetSchemas;
            this.TargetSchemaStrongNames = targetSchemaStrongNames;
        }

        /// <summary>
        /// Gets the original document to be mapped.
        /// </summary>
        public XmlDocument OriginalDocument { get; private set; }

        /// <summary>
        /// Gets the transformed document.
        /// </summary>
        public XmlDocument TransformedDocument { get; private set; }

        /// <summary>
        /// Gets the XSLT document.
        /// </summary>
        public XmlDocument Xslt { get; private set; }

        /// <summary>
        /// Gets the XSLT arguments.
        /// </summary>
        public XsltArgumentList XsltArguments { get; private set; }

        /// <summary>
        /// Gets the source schemas.
        /// </summary>
        public string[] SourceSchemas { get; private set; }

        /// <summary>
        /// Gets the target schemas.
        /// </summary>
        public string[] TargetSchemas { get; private set; }

        /// <summary>
        /// Gets the strong names of the target schemas.
        /// </summary>
        public IEnumerable<string> TargetSchemaStrongNames { get; private set; }
    }
}
