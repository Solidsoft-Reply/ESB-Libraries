// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespacePropertyInputDialog.cs" company="Solidsoft Reply Ltd.">
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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    using SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents.Properties;

    /// <summary>
    /// The input dialog box for namespaces and properties.
    /// </summary>
    internal sealed class NamespacePropertyInputDialog : Form
    {
        /// <summary>
        /// A data set for the data bound to the grid.
        /// </summary>
        private readonly DataSet propertyDataSet = new DataSet("PropertyDb");

        /// <summary>
        /// A data table for the properties.
        /// </summary>
        private readonly DataTable properties = new DataTable("Properties");

        /// <summary>
        /// The namespace column in the data table.
        /// </summary>
        private readonly DataColumn namespaceColumn = new DataColumn("Namespace", typeof(string));

        /// <summary>
        /// The property column in the data table.
        /// </summary>
        private readonly DataColumn propertyColumn = new DataColumn("Property Name", typeof(string));

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly IContainer components = null;

        /// <summary>
        /// The property grid.
        /// </summary>
        private DataGridView propertyGrid;

        /// <summary>
        /// The cancel button.
        /// </summary>
        private Button cancelButton;

        /// <summary>
        /// The OK button.
        /// </summary>
        private Button okayButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespacePropertyInputDialog"/> class.
        /// </summary>
        /// <param name="caption">The dialog windows caption.</param>
        /// <param name="properties">A list of properties.  Each property is a '|' delimited string containing a namespace and a value.</param>
        public NamespacePropertyInputDialog(string caption, IList<string> properties)
        {
            this.InitializeComponent();

            this.Text = caption;

            // Bind grid to data set table
            this.properties.Columns.Add(this.namespaceColumn);
            this.properties.Columns.Add(this.propertyColumn);
            this.propertyDataSet.Tables.Add(this.properties);
            this.propertyGrid.DataSource = this.propertyDataSet;
            this.propertyGrid.DataMember = this.properties.TableName;

            // Assign the list of properties provided by BizTalk Server
            this.Properties = properties;
        }

        /// <summary>
        /// Gets or sets the list of properties.
        /// </summary>
        public IList<string> Properties
        {
            get
            {
                return (from DataRow dataRow in this.properties.Rows
                        select string.Format("{0}|{1}", dataRow[0], dataRow[1])).ToList();
            }

            set
            {
                foreach (var parts in
                    from propertyValue in value
                    where !string.IsNullOrWhiteSpace(propertyValue)
                    select propertyValue.Split('|'))
                {
                    this.properties.Rows.Add(parts[0], parts[1]);
                }
            }
        }
        
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.propertyGrid = new DataGridView();
            this.cancelButton = new Button();
            this.okayButton = new Button();
            ((ISupportInitialize)this.propertyGrid).BeginInit();
            this.SuspendLayout();

            // ------
            // propertyGrid
            // ------
            this.propertyGrid.AllowUserToOrderColumns = true;
            this.propertyGrid.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left)
                                       | AnchorStyles.Right;
            this.propertyGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.propertyGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.propertyGrid.Location = new Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.RowTemplate.Height = 33;
            this.propertyGrid.Size = new Size(674, 157);
            this.propertyGrid.TabIndex = 0;

            // ------
            // cancelButton
            // ------
            this.cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.cancelButton.DialogResult = DialogResult.Cancel;
            this.cancelButton.Location = new Point(513, 175);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new Size(140, 42);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = Resources.CancleButtonText;
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += this.CancelButtonClick;

            // ------
            // okButton
            // ------
            this.okayButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.okayButton.DialogResult = DialogResult.OK;
            this.okayButton.Location = new Point(356, 175);
            this.okayButton.Name = "okayButton";
            this.okayButton.Size = new Size(140, 42);
            this.okayButton.TabIndex = 2;
            this.okayButton.Text = Resources.OkButtonText;
            this.okayButton.UseVisualStyleBackColor = true;
            this.okayButton.Click += this.OkayButtonClick;

            // ------
            // Form1
            // ------
            this.AutoScaleDimensions = new SizeF(12F, 25F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(674, 229);
            this.Controls.Add(this.okayButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.propertyGrid);
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            // ReSharper disable once NotResolvedInText
            this.Name = "Form1";
            ((ISupportInitialize)this.propertyGrid).EndInit();
            this.ResumeLayout(false);
        }

        /// <summary>
        /// Event handler.  Handles OK button clicks.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        // ReSharper disable once InconsistentNaming
        private void OkayButtonClick(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Event handler.  Handles Cancel button clicks.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        // ReSharper disable once InconsistentNaming
        private void CancelButtonClick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}