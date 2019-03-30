namespace MapMaker
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.mapPanel = new System.Windows.Forms.Panel();
            this.UITabs = new System.Windows.Forms.TabControl();
            this.toolsTab = new System.Windows.Forms.TabPage();
            this.platformOptionPanel = new System.Windows.Forms.Panel();
            this.platformSizeNumeric = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.decObjectButton = new System.Windows.Forms.RadioButton();
            this.platformButton = new System.Windows.Forms.RadioButton();
            this.saveTab = new System.Windows.Forms.TabPage();
            this.mapNameTextBox = new System.Windows.Forms.TextBox();
            this.compileMapFileButton = new System.Windows.Forms.Button();
            this.saveMapButton = new System.Windows.Forms.Button();
            this.loadTab = new System.Windows.Forms.TabPage();
            this.loadMapFileButton = new System.Windows.Forms.Button();
            this.drawTimer = new System.Windows.Forms.Timer(this.components);
            this.clearButton = new System.Windows.Forms.Button();
            this.UITabs.SuspendLayout();
            this.toolsTab.SuspendLayout();
            this.platformOptionPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.platformSizeNumeric)).BeginInit();
            this.saveTab.SuspendLayout();
            this.loadTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // mapPanel
            // 
            this.mapPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mapPanel.Location = new System.Drawing.Point(12, 121);
            this.mapPanel.Name = "mapPanel";
            this.mapPanel.Size = new System.Drawing.Size(400, 400);
            this.mapPanel.TabIndex = 0;
            this.mapPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.mapPanelPaint);
            this.mapPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mapPanelMouseClick);
            // 
            // UITabs
            // 
            this.UITabs.Controls.Add(this.toolsTab);
            this.UITabs.Controls.Add(this.saveTab);
            this.UITabs.Controls.Add(this.loadTab);
            this.UITabs.Location = new System.Drawing.Point(12, 12);
            this.UITabs.Name = "UITabs";
            this.UITabs.SelectedIndex = 0;
            this.UITabs.Size = new System.Drawing.Size(400, 103);
            this.UITabs.TabIndex = 1;
            // 
            // toolsTab
            // 
            this.toolsTab.Controls.Add(this.platformOptionPanel);
            this.toolsTab.Controls.Add(this.decObjectButton);
            this.toolsTab.Controls.Add(this.platformButton);
            this.toolsTab.Location = new System.Drawing.Point(4, 22);
            this.toolsTab.Name = "toolsTab";
            this.toolsTab.Padding = new System.Windows.Forms.Padding(3);
            this.toolsTab.Size = new System.Drawing.Size(392, 77);
            this.toolsTab.TabIndex = 0;
            this.toolsTab.Text = "Tools";
            this.toolsTab.UseVisualStyleBackColor = true;
            // 
            // platformOptionPanel
            // 
            this.platformOptionPanel.Controls.Add(this.platformSizeNumeric);
            this.platformOptionPanel.Controls.Add(this.label1);
            this.platformOptionPanel.Location = new System.Drawing.Point(123, 6);
            this.platformOptionPanel.Name = "platformOptionPanel";
            this.platformOptionPanel.Size = new System.Drawing.Size(263, 65);
            this.platformOptionPanel.TabIndex = 2;
            // 
            // platformSizeNumeric
            // 
            this.platformSizeNumeric.Location = new System.Drawing.Point(77, 4);
            this.platformSizeNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.platformSizeNumeric.Name = "platformSizeNumeric";
            this.platformSizeNumeric.Size = new System.Drawing.Size(120, 20);
            this.platformSizeNumeric.TabIndex = 1;
            this.platformSizeNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Platform Size";
            // 
            // decObjectButton
            // 
            this.decObjectButton.AutoSize = true;
            this.decObjectButton.Location = new System.Drawing.Point(6, 29);
            this.decObjectButton.Name = "decObjectButton";
            this.decObjectButton.Size = new System.Drawing.Size(111, 17);
            this.decObjectButton.TabIndex = 1;
            this.decObjectButton.Text = "Decorative Object";
            this.decObjectButton.UseVisualStyleBackColor = true;
            // 
            // platformButton
            // 
            this.platformButton.AutoSize = true;
            this.platformButton.Checked = true;
            this.platformButton.Location = new System.Drawing.Point(6, 6);
            this.platformButton.Name = "platformButton";
            this.platformButton.Size = new System.Drawing.Size(63, 17);
            this.platformButton.TabIndex = 0;
            this.platformButton.TabStop = true;
            this.platformButton.Text = "Platform";
            this.platformButton.UseVisualStyleBackColor = true;
            this.platformButton.CheckedChanged += new System.EventHandler(this.platformButton_CheckedChanged);
            // 
            // saveTab
            // 
            this.saveTab.Controls.Add(this.clearButton);
            this.saveTab.Controls.Add(this.mapNameTextBox);
            this.saveTab.Controls.Add(this.compileMapFileButton);
            this.saveTab.Controls.Add(this.saveMapButton);
            this.saveTab.Location = new System.Drawing.Point(4, 22);
            this.saveTab.Name = "saveTab";
            this.saveTab.Padding = new System.Windows.Forms.Padding(3);
            this.saveTab.Size = new System.Drawing.Size(392, 77);
            this.saveTab.TabIndex = 1;
            this.saveTab.Text = "Save";
            this.saveTab.UseVisualStyleBackColor = true;
            // 
            // mapNameTextBox
            // 
            this.mapNameTextBox.Location = new System.Drawing.Point(6, 8);
            this.mapNameTextBox.Name = "mapNameTextBox";
            this.mapNameTextBox.Size = new System.Drawing.Size(274, 20);
            this.mapNameTextBox.TabIndex = 2;
            this.mapNameTextBox.Text = "mapName";
            // 
            // compileMapFileButton
            // 
            this.compileMapFileButton.Location = new System.Drawing.Point(286, 48);
            this.compileMapFileButton.Name = "compileMapFileButton";
            this.compileMapFileButton.Size = new System.Drawing.Size(100, 23);
            this.compileMapFileButton.TabIndex = 1;
            this.compileMapFileButton.Text = "Compile Map File";
            this.compileMapFileButton.UseVisualStyleBackColor = true;
            this.compileMapFileButton.Click += new System.EventHandler(this.compileMapFileButton_Click);
            // 
            // saveMapButton
            // 
            this.saveMapButton.Location = new System.Drawing.Point(286, 6);
            this.saveMapButton.Name = "saveMapButton";
            this.saveMapButton.Size = new System.Drawing.Size(100, 23);
            this.saveMapButton.TabIndex = 0;
            this.saveMapButton.Text = "Save Map";
            this.saveMapButton.UseVisualStyleBackColor = true;
            this.saveMapButton.Click += new System.EventHandler(this.saveMapButton_Click);
            // 
            // loadTab
            // 
            this.loadTab.Controls.Add(this.loadMapFileButton);
            this.loadTab.Location = new System.Drawing.Point(4, 22);
            this.loadTab.Name = "loadTab";
            this.loadTab.Size = new System.Drawing.Size(392, 77);
            this.loadTab.TabIndex = 2;
            this.loadTab.Text = "Load";
            this.loadTab.UseVisualStyleBackColor = true;
            // 
            // loadMapFileButton
            // 
            this.loadMapFileButton.Location = new System.Drawing.Point(3, 3);
            this.loadMapFileButton.Name = "loadMapFileButton";
            this.loadMapFileButton.Size = new System.Drawing.Size(88, 23);
            this.loadMapFileButton.TabIndex = 0;
            this.loadMapFileButton.Text = "Load Map File";
            this.loadMapFileButton.UseVisualStyleBackColor = true;
            this.loadMapFileButton.Click += new System.EventHandler(this.loadMapFileButton_Click);
            // 
            // drawTimer
            // 
            this.drawTimer.Enabled = true;
            this.drawTimer.Interval = 10;
            this.drawTimer.Tick += new System.EventHandler(this.drawTimer_Tick);
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(6, 48);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 3;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 533);
            this.Controls.Add(this.UITabs);
            this.Controls.Add(this.mapPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Map Maker";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.UITabs.ResumeLayout(false);
            this.toolsTab.ResumeLayout(false);
            this.toolsTab.PerformLayout();
            this.platformOptionPanel.ResumeLayout(false);
            this.platformOptionPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.platformSizeNumeric)).EndInit();
            this.saveTab.ResumeLayout(false);
            this.saveTab.PerformLayout();
            this.loadTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mapPanel;
        private System.Windows.Forms.TabControl UITabs;
        private System.Windows.Forms.TabPage toolsTab;
        private System.Windows.Forms.RadioButton decObjectButton;
        private System.Windows.Forms.RadioButton platformButton;
        private System.Windows.Forms.TabPage saveTab;
        private System.Windows.Forms.Panel platformOptionPanel;
        private System.Windows.Forms.NumericUpDown platformSizeNumeric;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer drawTimer;
        private System.Windows.Forms.Button compileMapFileButton;
        private System.Windows.Forms.Button saveMapButton;
        private System.Windows.Forms.TextBox mapNameTextBox;
        private System.Windows.Forms.TabPage loadTab;
        private System.Windows.Forms.Button loadMapFileButton;
        private System.Windows.Forms.Button clearButton;
    }
}

