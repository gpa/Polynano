namespace Polynano.Startup.Views
{
    partial class MainView
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
            this.leftPanel = new System.Windows.Forms.Panel();
            this.simplifyGroupBox = new System.Windows.Forms.GroupBox();
            this.simplifyButton = new System.Windows.Forms.Button();
            this.complexityTrackbar = new System.Windows.Forms.TrackBar();
            this.persistenceGroupBox = new System.Windows.Forms.GroupBox();
            this.loadButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.displayGroupBox = new System.Windows.Forms.GroupBox();
            this.ShowVerticesCheckbox = new System.Windows.Forms.CheckBox();
            this.ShowEdgesCheckbox = new System.Windows.Forms.CheckBox();
            this.ShowFacesCheckbox = new System.Windows.Forms.CheckBox();
            this.simplificationTimer = new System.Windows.Forms.Timer(this.components);
            this.modelStatsTable = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.statsSizeLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.StatsVertexCountLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.StatsFaceCountLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.statsComplexityLabel = new System.Windows.Forms.Label();
            this.leftPanel.SuspendLayout();
            this.simplifyGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.complexityTrackbar)).BeginInit();
            this.persistenceGroupBox.SuspendLayout();
            this.displayGroupBox.SuspendLayout();
            this.modelStatsTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // leftPanel
            // 
            this.leftPanel.Controls.Add(this.modelStatsTable);
            this.leftPanel.Controls.Add(this.simplifyGroupBox);
            this.leftPanel.Controls.Add(this.persistenceGroupBox);
            this.leftPanel.Controls.Add(this.displayGroupBox);
            this.leftPanel.Location = new System.Drawing.Point(0, 0);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(137, 447);
            this.leftPanel.TabIndex = 1;
            // 
            // simplifyGroupBox
            // 
            this.simplifyGroupBox.Controls.Add(this.simplifyButton);
            this.simplifyGroupBox.Controls.Add(this.complexityTrackbar);
            this.simplifyGroupBox.Location = new System.Drawing.Point(4, 211);
            this.simplifyGroupBox.Name = "simplifyGroupBox";
            this.simplifyGroupBox.Size = new System.Drawing.Size(127, 100);
            this.simplifyGroupBox.TabIndex = 4;
            this.simplifyGroupBox.TabStop = false;
            this.simplifyGroupBox.Text = "Simplify";
            // 
            // simplifyButton
            // 
            this.simplifyButton.Enabled = false;
            this.simplifyButton.Location = new System.Drawing.Point(0, 60);
            this.simplifyButton.Name = "simplifyButton";
            this.simplifyButton.Size = new System.Drawing.Size(121, 34);
            this.simplifyButton.TabIndex = 1;
            this.simplifyButton.Text = "Simplify";
            this.simplifyButton.UseVisualStyleBackColor = true;
            this.simplifyButton.Click += new System.EventHandler(this.simplifyButton_Click);
            // 
            // complexityTrackbar
            // 
            this.complexityTrackbar.Enabled = false;
            this.complexityTrackbar.Location = new System.Drawing.Point(0, 20);
            this.complexityTrackbar.Maximum = 100;
            this.complexityTrackbar.Name = "complexityTrackbar";
            this.complexityTrackbar.Size = new System.Drawing.Size(124, 45);
            this.complexityTrackbar.TabIndex = 0;
            this.complexityTrackbar.TabStop = false;
            this.complexityTrackbar.Value = 100;
            // 
            // persistenceGroupBox
            // 
            this.persistenceGroupBox.Controls.Add(this.loadButton);
            this.persistenceGroupBox.Controls.Add(this.saveButton);
            this.persistenceGroupBox.Location = new System.Drawing.Point(3, 3);
            this.persistenceGroupBox.Name = "persistenceGroupBox";
            this.persistenceGroupBox.Size = new System.Drawing.Size(128, 100);
            this.persistenceGroupBox.TabIndex = 3;
            this.persistenceGroupBox.TabStop = false;
            this.persistenceGroupBox.Text = "Persistence";
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(6, 19);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(116, 36);
            this.loadButton.TabIndex = 0;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.LoadButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Enabled = false;
            this.saveButton.Location = new System.Drawing.Point(6, 58);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(116, 36);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // displayGroupBox
            // 
            this.displayGroupBox.Controls.Add(this.ShowVerticesCheckbox);
            this.displayGroupBox.Controls.Add(this.ShowEdgesCheckbox);
            this.displayGroupBox.Controls.Add(this.ShowFacesCheckbox);
            this.displayGroupBox.Location = new System.Drawing.Point(3, 109);
            this.displayGroupBox.Name = "displayGroupBox";
            this.displayGroupBox.Size = new System.Drawing.Size(128, 95);
            this.displayGroupBox.TabIndex = 2;
            this.displayGroupBox.TabStop = false;
            this.displayGroupBox.Text = "Display";
            // 
            // ShowVerticesCheckbox
            // 
            this.ShowVerticesCheckbox.AutoSize = true;
            this.ShowVerticesCheckbox.Enabled = false;
            this.ShowVerticesCheckbox.Location = new System.Drawing.Point(7, 68);
            this.ShowVerticesCheckbox.Name = "ShowVerticesCheckbox";
            this.ShowVerticesCheckbox.Size = new System.Drawing.Size(64, 17);
            this.ShowVerticesCheckbox.TabIndex = 2;
            this.ShowVerticesCheckbox.Text = "Vertices";
            this.ShowVerticesCheckbox.UseVisualStyleBackColor = true;
            this.ShowVerticesCheckbox.CheckedChanged += new System.EventHandler(this.ShowVerticesCheckbox_CheckedChanged);
            // 
            // ShowEdgesCheckbox
            // 
            this.ShowEdgesCheckbox.AutoSize = true;
            this.ShowEdgesCheckbox.Enabled = false;
            this.ShowEdgesCheckbox.Location = new System.Drawing.Point(7, 44);
            this.ShowEdgesCheckbox.Name = "ShowEdgesCheckbox";
            this.ShowEdgesCheckbox.Size = new System.Drawing.Size(56, 17);
            this.ShowEdgesCheckbox.TabIndex = 1;
            this.ShowEdgesCheckbox.Text = "Edges";
            this.ShowEdgesCheckbox.UseVisualStyleBackColor = true;
            this.ShowEdgesCheckbox.CheckedChanged += new System.EventHandler(this.ShowEdgesCheckbox_CheckedChanged);
            // 
            // ShowFacesCheckbox
            // 
            this.ShowFacesCheckbox.AutoSize = true;
            this.ShowFacesCheckbox.Checked = true;
            this.ShowFacesCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowFacesCheckbox.Enabled = false;
            this.ShowFacesCheckbox.Location = new System.Drawing.Point(7, 20);
            this.ShowFacesCheckbox.Name = "ShowFacesCheckbox";
            this.ShowFacesCheckbox.Size = new System.Drawing.Size(55, 17);
            this.ShowFacesCheckbox.TabIndex = 0;
            this.ShowFacesCheckbox.Text = "Faces";
            this.ShowFacesCheckbox.UseVisualStyleBackColor = true;
            this.ShowFacesCheckbox.CheckedChanged += new System.EventHandler(this.ShowFacesCheckbox_CheckedChanged);
            // 
            // simplificationTimer
            // 
            this.simplificationTimer.Tick += new System.EventHandler(this.OnSimplificationTimerTick);
            // 
            // modelStatsTable
            // 
            this.modelStatsTable.ColumnCount = 2;
            this.modelStatsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.modelStatsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 17F));
            this.modelStatsTable.Controls.Add(this.label6, 0, 3);
            this.modelStatsTable.Controls.Add(this.statsSizeLabel, 1, 3);
            this.modelStatsTable.Controls.Add(this.label1, 0, 2);
            this.modelStatsTable.Controls.Add(this.StatsVertexCountLabel, 1, 2);
            this.modelStatsTable.Controls.Add(this.label4, 0, 1);
            this.modelStatsTable.Controls.Add(this.StatsFaceCountLabel, 1, 1);
            this.modelStatsTable.Controls.Add(this.label2, 0, 0);
            this.modelStatsTable.Controls.Add(this.statsComplexityLabel, 1, 0);
            this.modelStatsTable.Location = new System.Drawing.Point(0, 317);
            this.modelStatsTable.Name = "modelStatsTable";
            this.modelStatsTable.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.modelStatsTable.RowCount = 4;
            this.modelStatsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.modelStatsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.modelStatsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.modelStatsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.modelStatsTable.Size = new System.Drawing.Size(131, 118);
            this.modelStatsTable.TabIndex = 3;
            this.modelStatsTable.Visible = false;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(42, 95);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Size:";
            // 
            // statsSizeLabel
            // 
            this.statsSizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.statsSizeLabel.AutoSize = true;
            this.statsSizeLabel.Location = new System.Drawing.Point(78, 95);
            this.statsSizeLabel.Name = "statsSizeLabel";
            this.statsSizeLabel.Size = new System.Drawing.Size(50, 13);
            this.statsSizeLabel.TabIndex = 14;
            this.statsSizeLabel.Text = "-";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Vertices";
            // 
            // StatsVertexCountLabel
            // 
            this.StatsVertexCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.StatsVertexCountLabel.AutoSize = true;
            this.StatsVertexCountLabel.Location = new System.Drawing.Point(78, 65);
            this.StatsVertexCountLabel.Name = "StatsVertexCountLabel";
            this.StatsVertexCountLabel.Size = new System.Drawing.Size(50, 13);
            this.StatsVertexCountLabel.TabIndex = 13;
            this.StatsVertexCountLabel.Text = "-";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Faces:";
            // 
            // StatsFaceCountLabel
            // 
            this.StatsFaceCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.StatsFaceCountLabel.AutoSize = true;
            this.StatsFaceCountLabel.Location = new System.Drawing.Point(78, 36);
            this.StatsFaceCountLabel.Name = "StatsFaceCountLabel";
            this.StatsFaceCountLabel.Size = new System.Drawing.Size(50, 13);
            this.StatsFaceCountLabel.TabIndex = 12;
            this.StatsFaceCountLabel.Text = "-";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Complexity:";
            // 
            // statsComplexityLabel
            // 
            this.statsComplexityLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.statsComplexityLabel.AutoSize = true;
            this.statsComplexityLabel.Location = new System.Drawing.Point(78, 8);
            this.statsComplexityLabel.Name = "statsComplexityLabel";
            this.statsComplexityLabel.Size = new System.Drawing.Size(50, 13);
            this.statsComplexityLabel.TabIndex = 16;
            this.statsComplexityLabel.Text = "-";
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1304, 789);
            this.Controls.Add(this.leftPanel);
            this.Name = "MainView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Polynano";
            this.leftPanel.ResumeLayout(false);
            this.simplifyGroupBox.ResumeLayout(false);
            this.simplifyGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.complexityTrackbar)).EndInit();
            this.persistenceGroupBox.ResumeLayout(false);
            this.displayGroupBox.ResumeLayout(false);
            this.displayGroupBox.PerformLayout();
            this.modelStatsTable.ResumeLayout(false);
            this.modelStatsTable.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.GroupBox displayGroupBox;
        private System.Windows.Forms.CheckBox ShowVerticesCheckbox;
        private System.Windows.Forms.CheckBox ShowEdgesCheckbox;
        private System.Windows.Forms.CheckBox ShowFacesCheckbox;
        private System.Windows.Forms.GroupBox persistenceGroupBox;
        private System.Windows.Forms.GroupBox simplifyGroupBox;
        private System.Windows.Forms.Button simplifyButton;
        private System.Windows.Forms.TrackBar complexityTrackbar;
        private System.Windows.Forms.Timer simplificationTimer;
        private System.Windows.Forms.TableLayoutPanel modelStatsTable;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label statsSizeLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label StatsVertexCountLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label StatsFaceCountLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label statsComplexityLabel;
    }
}