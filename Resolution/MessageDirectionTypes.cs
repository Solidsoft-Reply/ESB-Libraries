// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageDirectionTypes.cs" company="Solidsoft Reply Ltd.">
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
    using System.Xml.Serialization;

    /// <summary>
    /// Enumeration for Message Direction values
    /// </summary>
    public enum MessageDirectionTypes
    {
        /// <summary>
        /// The message direction is not specified.
        /// </summary>
        [XmlIgnore]
        NotSpecified,

        /// <summary>
        /// The message direction is in.
        /// </summary>
        [XmlIgnore]
        MsgIn,

        /// <summary>
        /// The message direction is out.
        /// </summary>
        [XmlIgnore]
        MsgOut,

        /// <summary>
        /// The message direction is both in and out.
        /// </summary>
        [XmlIgnore]
        Both
    }
}
