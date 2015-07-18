// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationLevel.cs" company="Solidsoft Reply Ltd.">
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

    /// <summary>
    /// Represents the level of a validation entry.
    /// </summary>
    [Serializable]
    public enum ValidationLevel
    {
        /// <summary>
        /// A validation error.
        /// </summary>
        Error,

        /// <summary>
        /// Valid, but with warnings
        /// </summary>
        Warning,

        /// <summary>
        /// Valid - information provided.
        /// </summary>
        Information
    }
}
