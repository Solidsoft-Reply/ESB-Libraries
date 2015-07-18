// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolutionDataPropertyEditor.cs" company="Solidsoft Reply Ltd.">
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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Security.Permissions;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;

    using SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents.Properties;

    /// <summary>
    /// Example UITypeEditor that uses the IWindowsFormsEditorService  
    /// to display a Form.
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class ResolutionDataPropertyEditor : UITypeEditor
    {
        /// <summary>
        /// The editor service.
        /// </summary>
        private IWindowsFormsEditorService editorService;

        /// <summary>
        /// Returns the editor style.
        /// </summary>
        /// <param name="context">The type descriptor context</param>
        /// <returns>A UI type editor edit style.</returns>
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = false)]
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                // Indicates that this editor can display a Form-based interface. 
                return UITypeEditorEditStyle.Modal;
            }

            return base.GetEditStyle(context);
        }

        /// <summary>
        /// Edit the list of resolution data properties.
        /// </summary>
        /// <param name="context">Type descriptor context.</param>
        /// <param name="provider">Service provider</param>
        /// <param name="value">The list of resolution data properties</param>
        /// <returns>A list of resolution data properties</returns>
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = false)]
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            // Attempts to obtain an IWindowsFormsEditorService.
            if (provider != null)
            {
                this.editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            }

            if (this.editorService == null)
            {
                return null;
            }

            // Displays a StringInputDialog Form to get a user-adjustable  
            // string value. 
            using (var form = new NamespacePropertyInputDialog(Resources.ResolutionDataPropertyEditorCaption, (IList<string>)value))
            {
                if (this.editorService.ShowDialog(form) == DialogResult.OK)
                {
                    return new ResolutionDataPropertyList(form.Properties);
                }
            }

            // If OK was not pressed, return the original value 
            return value;
        }
    }
}