// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interchange.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.Resolution.ResolutionService
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents an interchange of directives.  Provides a convenient approach to accessing
    /// parameters as a dictionary.
    /// </summary>
    public partial class Interchange
    {
        /// <summary>
        /// Gets or sets the collection of general purpose parameters.
        /// </summary>
        [XmlIgnore]
        public Dictionaries.Parameters ParametersDictionary
        {
            get
            {
                if (this.parametersField == null)
                {
                    return null;
                }

                var parameters = new Dictionaries.Parameters();

                foreach (var parametersItem in this.parametersField)
                {
                    var valueSerializer = new NetDataContractSerializer();
                    using (var ms = new MemoryStream())
                    {
                        var sw = new StreamWriter(ms);
                        sw.Write(parametersItem.Value.OuterXml);
                        sw.Flush();
                        ms.Seek(0, SeekOrigin.Begin);

                        parameters.Add(parametersItem.Key.@string, valueSerializer.Deserialize(ms));
                    }
                }

                return parameters;
            }

            set
            {
                foreach (var parametersItem in value)
                {
                    var item = new Parameters();
                    var valueSerializer = new NetDataContractSerializer();

                    item.Key.@string = parametersItem.Key;

                    using (var ms = new MemoryStream())
                    {
                        valueSerializer.Serialize(ms, parametersItem.Value);
                        ms.Position = 0;
                        var xmlDoc = new XmlDocument();
                        xmlDoc.Load(ms);

                        var newItemsArray = new Parameters[this.parametersField.Length + 1];
                        this.parametersField.CopyTo(newItemsArray, 0);
                        newItemsArray[this.parametersField.Length] = new Parameters
                                                               {
                                                                   Key =
                                                                       new ParametersKey()
                                                                           {
                                                                               @string
                                                                                   =
                                                                                   parametersItem
                                                                                   .Key
                                                                           },
                                                                   Value =
                                                                       xmlDoc.DocumentElement
                                                               };

                        this.parametersField = newItemsArray;
                    }
                }
            }
        }
    }
}