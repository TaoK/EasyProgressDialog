/*
 * This file is part of EasyProgressDialog. EasyProgressDialog is free software: you can redistribute 
 * it and/or modify it under the terms of the GNU Lesser Public License as published by the Free 
 * Software Foundation, either version 3 of the License, or (at your option) any later version. 
 * Foobar is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even 
 * the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser 
 * Public License for more details. You should have received a copy of the GNU Lesser Public License
 * along with EasyProgressDialog.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

namespace KlerksSoft.EasyProgressDialog
{
    partial class ProgressDialogForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressDialogForm));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.txt_CurrentAction = new System.Windows.Forms.TextBox();
            this.lbl_Progress = new System.Windows.Forms.Label();
            this.lbl_TimeEstimate = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Maximum = 10000;
            this.progressBar1.Name = "progressBar1";
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // txt_CurrentAction
            // 
            resources.ApplyResources(this.txt_CurrentAction, "txt_CurrentAction");
            this.txt_CurrentAction.BackColor = System.Drawing.SystemColors.Control;
            this.txt_CurrentAction.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_CurrentAction.Name = "txt_CurrentAction";
            // 
            // lbl_Progress
            // 
            resources.ApplyResources(this.lbl_Progress, "lbl_Progress");
            this.lbl_Progress.Name = "lbl_Progress";
            // 
            // lbl_TimeEstimate
            // 
            resources.ApplyResources(this.lbl_TimeEstimate, "lbl_TimeEstimate");
            this.lbl_TimeEstimate.Name = "lbl_TimeEstimate";
            // 
            // ProgressDialogForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbl_TimeEstimate);
            this.Controls.Add(this.lbl_Progress);
            this.Controls.Add(this.txt_CurrentAction);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.progressBar1);
            this.Name = "ProgressDialogForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.ProgressBar progressBar1;
        internal System.Windows.Forms.Button btn_Cancel;
        internal System.Windows.Forms.TextBox txt_CurrentAction;
        internal System.Windows.Forms.Label lbl_Progress;
        internal System.Windows.Forms.Label lbl_TimeEstimate;
    }
}