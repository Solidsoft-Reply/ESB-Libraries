// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolutionData.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents
{
    /// <summary>
    /// Controls the fact data that is asserted to the ESB service mediation resolver.
    /// </summary>
    public enum ResolutionData
    {
        /// <summary>
        /// Asserts resolution values, only.
        /// </summary>
        ValuesOnly,

        /// <summary>
        /// Asserts resolution values and listed promoted properties on the BizTalk message.
        /// </summary>
        ValuesWithListedPromotedProperties,

        /// <summary>
        /// Asserts resolution values and all listed properties on the BizTalk message.
        /// </summary>
        ValuesWithAllListedProperties,

        /// <summary>
        /// Asserts resolution values and all promoted properties on the BizTalk message.
        /// </summary>
        ValuesWithAllPromotedProperties,

        /// <summary>
        /// Asserts resolution values and any properties on the BizTalk message.
        /// </summary>
        ValuesWithAllProperties
    }
}
