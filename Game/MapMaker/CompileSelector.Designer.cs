namespace MapMaker
{
    partial class CompileSelector
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
            this.mapCheckList = new System.Windows.Forms.CheckedListBox();
            this.confirmationButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mapCheckList
            // 
            this.mapCheckList.FormattingEnabled = true;
            this.mapCheckList.Location = new System.Drawing.Point(12, 12);
            this.mapCheckList.Name = "mapCheckList";
            this.mapCheckList.Size = new System.Drawing.Size(295, 259);
            this.mapCheckList.TabIndex = 0;
            this.mapCheckList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.itemCheck);
            // 
            // confirmationButton
            // 
            this.confirmationButton.Location = new System.Drawing.Point(232, 286);
            this.confirmationButton.Name = "confirmationButton";
            this.confirmationButton.Size = new System.Drawing.Size(75, 23);
            this.confirmationButton.TabIndex = 1;
            this.confirmationButton.Text = "Confirm";
            this.confirmationButton.UseVisualStyleBackColor = true;
            this.confirmationButton.Click += new System.EventHandler(this.confirmationButton_Click);
            // 
            // CompileSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 321);
            this.Controls.Add(this.confirmationButton);
            this.Controls.Add(this.mapCheckList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "CompileSelector";
            this.Text = "Compile Selection";
            this.Load += new System.EventHandler(this.CompileSelector_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox mapCheckList;
        private System.Windows.Forms.Button confirmationButton;
    }
}