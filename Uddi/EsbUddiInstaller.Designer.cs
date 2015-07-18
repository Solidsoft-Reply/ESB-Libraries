// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EsbUddiInstaller.Designer.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.Uddi
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Designer code for EsbUddiInstaller.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    partial class EsbUddiInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// The event log installer.
        /// </summary>
        private System.Diagnostics.EventLogInstaller eventLogInstaller;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.eventLogInstaller = new System.Diagnostics.EventLogInstaller();
            // 
            // eventLogInstaller
            // 
            this.eventLogInstaller.CategoryCount = 0;
            this.eventLogInstaller.CategoryResourceFile = null;
            this.eventLogInstaller.Log = "Application";
            this.eventLogInstaller.MessageResourceFile = null;
            this.eventLogInstaller.ParameterResourceFile = null;
            this.eventLogInstaller.Source = "Solidsoft Reply ESB UDDI";
            // 
            // EsbUddiInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.eventLogInstaller});

        }

        #endregion
    }
}