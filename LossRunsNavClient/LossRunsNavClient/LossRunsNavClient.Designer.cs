namespace LossRunsNavClient
{
    partial class LossRunsNavClient
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LossRunsNavClient));
            this.Output1 = new System.Windows.Forms.WebBrowser();
            this.btnStart = new System.Windows.Forms.Button();
            this.Output2 = new System.Windows.Forms.WebBrowser();
            this.Output3 = new System.Windows.Forms.WebBrowser();
            this.Output4 = new System.Windows.Forms.WebBrowser();
            this.Output5 = new System.Windows.Forms.WebBrowser();
            this.Output6 = new System.Windows.Forms.WebBrowser();
            this.Output7 = new System.Windows.Forms.WebBrowser();
            this.Output8 = new System.Windows.Forms.WebBrowser();
            this.tbHostUrl = new System.Windows.Forms.TextBox();
            this.lblHostUrl = new System.Windows.Forms.Label();
            this.tbMaxWindows = new System.Windows.Forms.TextBox();
            this.lblMaxWindows = new System.Windows.Forms.Label();
            this.lblGlobalTimeout = new System.Windows.Forms.Label();
            this.tbGlobalTimeout = new System.Windows.Forms.TextBox();
            this.lblMaxCpu = new System.Windows.Forms.Label();
            this.tbMaxCpu = new System.Windows.Forms.TextBox();
            this.lblMaxMem = new System.Windows.Forms.Label();
            this.tbMaxMem = new System.Windows.Forms.TextBox();
            this.btnConfig = new System.Windows.Forms.Button();
            this.lblBatchRemain = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAdvConfig = new System.Windows.Forms.Button();
            this.gbProcessingLevel = new System.Windows.Forms.GroupBox();
            this.lblProcessingLevel = new System.Windows.Forms.Label();
            this.rbHigh = new System.Windows.Forms.RadioButton();
            this.rbLow = new System.Windows.Forms.RadioButton();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.lblGlobalStats = new System.Windows.Forms.Label();
            this.lblGlobalBatRem = new System.Windows.Forms.Label();
            this.lblGlobalPer = new System.Windows.Forms.Label();
            this.gbProcessingLevel.SuspendLayout();
            this.SuspendLayout();
            // 
            // Output1
            // 
            this.Output1.Location = new System.Drawing.Point(298, 12);
            this.Output1.MinimumSize = new System.Drawing.Size(20, 20);
            this.Output1.Name = "Output1";
            this.Output1.Size = new System.Drawing.Size(171, 138);
            this.Output1.TabIndex = 0;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(219, 246);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(45, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // Output2
            // 
            this.Output2.Location = new System.Drawing.Point(475, 12);
            this.Output2.MinimumSize = new System.Drawing.Size(20, 20);
            this.Output2.Name = "Output2";
            this.Output2.Size = new System.Drawing.Size(171, 138);
            this.Output2.TabIndex = 2;
            this.Output2.Visible = false;
            // 
            // Output3
            // 
            this.Output3.Location = new System.Drawing.Point(298, 158);
            this.Output3.MinimumSize = new System.Drawing.Size(20, 20);
            this.Output3.Name = "Output3";
            this.Output3.Size = new System.Drawing.Size(171, 138);
            this.Output3.TabIndex = 3;
            this.Output3.Visible = false;
            // 
            // Output4
            // 
            this.Output4.Location = new System.Drawing.Point(475, 158);
            this.Output4.MinimumSize = new System.Drawing.Size(20, 20);
            this.Output4.Name = "Output4";
            this.Output4.Size = new System.Drawing.Size(171, 138);
            this.Output4.TabIndex = 4;
            this.Output4.Visible = false;
            // 
            // Output5
            // 
            this.Output5.Location = new System.Drawing.Point(298, 302);
            this.Output5.MinimumSize = new System.Drawing.Size(20, 20);
            this.Output5.Name = "Output5";
            this.Output5.Size = new System.Drawing.Size(171, 138);
            this.Output5.TabIndex = 5;
            this.Output5.Visible = false;
            // 
            // Output6
            // 
            this.Output6.Location = new System.Drawing.Point(475, 302);
            this.Output6.MinimumSize = new System.Drawing.Size(20, 20);
            this.Output6.Name = "Output6";
            this.Output6.Size = new System.Drawing.Size(171, 138);
            this.Output6.TabIndex = 6;
            this.Output6.Visible = false;
            // 
            // Output7
            // 
            this.Output7.Location = new System.Drawing.Point(298, 446);
            this.Output7.MinimumSize = new System.Drawing.Size(20, 20);
            this.Output7.Name = "Output7";
            this.Output7.Size = new System.Drawing.Size(171, 138);
            this.Output7.TabIndex = 7;
            this.Output7.Visible = false;
            // 
            // Output8
            // 
            this.Output8.Location = new System.Drawing.Point(475, 449);
            this.Output8.MinimumSize = new System.Drawing.Size(20, 20);
            this.Output8.Name = "Output8";
            this.Output8.Size = new System.Drawing.Size(171, 138);
            this.Output8.TabIndex = 8;
            this.Output8.Visible = false;
            // 
            // tbHostUrl
            // 
            this.tbHostUrl.Location = new System.Drawing.Point(31, 50);
            this.tbHostUrl.Name = "tbHostUrl";
            this.tbHostUrl.Size = new System.Drawing.Size(234, 20);
            this.tbHostUrl.TabIndex = 9;
            this.tbHostUrl.Text = "https://dev.btisinc.com/LossRuns/";
            this.tbHostUrl.Visible = false;
            this.tbHostUrl.TextChanged += new System.EventHandler(this.tbHostUrl_TextChanged);
            // 
            // lblHostUrl
            // 
            this.lblHostUrl.AutoSize = true;
            this.lblHostUrl.Location = new System.Drawing.Point(31, 31);
            this.lblHostUrl.Name = "lblHostUrl";
            this.lblHostUrl.Size = new System.Drawing.Size(48, 13);
            this.lblHostUrl.TabIndex = 10;
            this.lblHostUrl.Text = "Host Url:";
            this.lblHostUrl.Visible = false;
            // 
            // tbMaxWindows
            // 
            this.tbMaxWindows.Location = new System.Drawing.Point(34, 145);
            this.tbMaxWindows.Name = "tbMaxWindows";
            this.tbMaxWindows.Size = new System.Drawing.Size(45, 20);
            this.tbMaxWindows.TabIndex = 11;
            this.tbMaxWindows.Text = "8";
            this.tbMaxWindows.Visible = false;
            this.tbMaxWindows.TextChanged += new System.EventHandler(this.tbMaxWindows_TextChanged);
            // 
            // lblMaxWindows
            // 
            this.lblMaxWindows.AutoSize = true;
            this.lblMaxWindows.Location = new System.Drawing.Point(31, 129);
            this.lblMaxWindows.Name = "lblMaxWindows";
            this.lblMaxWindows.Size = new System.Drawing.Size(77, 13);
            this.lblMaxWindows.TabIndex = 12;
            this.lblMaxWindows.Text = "Max Windows:";
            this.lblMaxWindows.Visible = false;
            // 
            // lblGlobalTimeout
            // 
            this.lblGlobalTimeout.AutoSize = true;
            this.lblGlobalTimeout.Location = new System.Drawing.Point(114, 129);
            this.lblGlobalTimeout.Name = "lblGlobalTimeout";
            this.lblGlobalTimeout.Size = new System.Drawing.Size(103, 13);
            this.lblGlobalTimeout.TabIndex = 14;
            this.lblGlobalTimeout.Text = "Global Timeout (ms):";
            this.lblGlobalTimeout.Visible = false;
            // 
            // tbGlobalTimeout
            // 
            this.tbGlobalTimeout.Location = new System.Drawing.Point(117, 145);
            this.tbGlobalTimeout.Name = "tbGlobalTimeout";
            this.tbGlobalTimeout.Size = new System.Drawing.Size(100, 20);
            this.tbGlobalTimeout.TabIndex = 13;
            this.tbGlobalTimeout.Text = "5000";
            this.tbGlobalTimeout.Visible = false;
            this.tbGlobalTimeout.TextChanged += new System.EventHandler(this.tbGlobalTimeout_TextChanged);
            // 
            // lblMaxCpu
            // 
            this.lblMaxCpu.AutoSize = true;
            this.lblMaxCpu.Location = new System.Drawing.Point(31, 185);
            this.lblMaxCpu.Name = "lblMaxCpu";
            this.lblMaxCpu.Size = new System.Drawing.Size(66, 13);
            this.lblMaxCpu.TabIndex = 16;
            this.lblMaxCpu.Text = "Max CPU %:";
            this.lblMaxCpu.Visible = false;
            // 
            // tbMaxCpu
            // 
            this.tbMaxCpu.Location = new System.Drawing.Point(34, 201);
            this.tbMaxCpu.Name = "tbMaxCpu";
            this.tbMaxCpu.Size = new System.Drawing.Size(45, 20);
            this.tbMaxCpu.TabIndex = 15;
            this.tbMaxCpu.Text = "30.0";
            this.tbMaxCpu.Visible = false;
            this.tbMaxCpu.TextChanged += new System.EventHandler(this.tbMaxCpu_TextChanged);
            // 
            // lblMaxMem
            // 
            this.lblMaxMem.AutoSize = true;
            this.lblMaxMem.Location = new System.Drawing.Point(114, 185);
            this.lblMaxMem.Name = "lblMaxMem";
            this.lblMaxMem.Size = new System.Drawing.Size(67, 13);
            this.lblMaxMem.TabIndex = 18;
            this.lblMaxMem.Text = "Max Mem %:";
            this.lblMaxMem.Visible = false;
            // 
            // tbMaxMem
            // 
            this.tbMaxMem.Location = new System.Drawing.Point(117, 201);
            this.tbMaxMem.Name = "tbMaxMem";
            this.tbMaxMem.Size = new System.Drawing.Size(45, 20);
            this.tbMaxMem.TabIndex = 17;
            this.tbMaxMem.Text = "25.0";
            this.tbMaxMem.Visible = false;
            this.tbMaxMem.TextChanged += new System.EventHandler(this.tbMaxMem_TextChanged);
            // 
            // btnConfig
            // 
            this.btnConfig.Image = ((System.Drawing.Image)(resources.GetObject("btnConfig.Image")));
            this.btnConfig.Location = new System.Drawing.Point(232, 12);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(32, 32);
            this.btnConfig.TabIndex = 20;
            this.btnConfig.UseVisualStyleBackColor = true;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // lblBatchRemain
            // 
            this.lblBatchRemain.AutoSize = true;
            this.lblBatchRemain.Location = new System.Drawing.Point(30, 256);
            this.lblBatchRemain.Name = "lblBatchRemain";
            this.lblBatchRemain.Size = new System.Drawing.Size(100, 13);
            this.lblBatchRemain.TabIndex = 21;
            this.lblBatchRemain.Text = "Batch Remaining: 0";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(219, 197);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(45, 23);
            this.btnReset.TabIndex = 23;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Visible = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(161, 246);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(52, 23);
            this.btnCancel.TabIndex = 24;
            this.btnCancel.Text = "Pause";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAdvConfig
            // 
            this.btnAdvConfig.Location = new System.Drawing.Point(193, 91);
            this.btnAdvConfig.Name = "btnAdvConfig";
            this.btnAdvConfig.Size = new System.Drawing.Size(71, 23);
            this.btnAdvConfig.TabIndex = 25;
            this.btnAdvConfig.Text = "Advanced";
            this.btnAdvConfig.UseVisualStyleBackColor = true;
            this.btnAdvConfig.Visible = false;
            this.btnAdvConfig.Click += new System.EventHandler(this.btnAdvConfig_Click);
            // 
            // gbProcessingLevel
            // 
            this.gbProcessingLevel.Controls.Add(this.lblProcessingLevel);
            this.gbProcessingLevel.Controls.Add(this.rbHigh);
            this.gbProcessingLevel.Controls.Add(this.rbLow);
            this.gbProcessingLevel.Location = new System.Drawing.Point(33, 77);
            this.gbProcessingLevel.Name = "gbProcessingLevel";
            this.gbProcessingLevel.Size = new System.Drawing.Size(147, 37);
            this.gbProcessingLevel.TabIndex = 26;
            this.gbProcessingLevel.TabStop = false;
            this.gbProcessingLevel.Visible = false;
            // 
            // lblProcessingLevel
            // 
            this.lblProcessingLevel.AutoSize = true;
            this.lblProcessingLevel.Location = new System.Drawing.Point(-1, 2);
            this.lblProcessingLevel.Name = "lblProcessingLevel";
            this.lblProcessingLevel.Size = new System.Drawing.Size(91, 13);
            this.lblProcessingLevel.TabIndex = 31;
            this.lblProcessingLevel.Text = "Processing Level:";
            // 
            // rbHigh
            // 
            this.rbHigh.AutoSize = true;
            this.rbHigh.Location = new System.Drawing.Point(53, 18);
            this.rbHigh.Name = "rbHigh";
            this.rbHigh.Size = new System.Drawing.Size(47, 17);
            this.rbHigh.TabIndex = 30;
            this.rbHigh.TabStop = true;
            this.rbHigh.Text = "High";
            this.rbHigh.UseVisualStyleBackColor = true;
            this.rbHigh.CheckedChanged += new System.EventHandler(this.rbHigh_CheckedChanged);
            // 
            // rbLow
            // 
            this.rbLow.AutoSize = true;
            this.rbLow.Location = new System.Drawing.Point(2, 18);
            this.rbLow.Name = "rbLow";
            this.rbLow.Size = new System.Drawing.Size(45, 17);
            this.rbLow.TabIndex = 29;
            this.rbLow.TabStop = true;
            this.rbLow.Text = "Low";
            this.rbLow.UseVisualStyleBackColor = true;
            this.rbLow.CheckedChanged += new System.EventHandler(this.rbLow_CheckedChanged);
            // 
            // tbLog
            // 
            this.tbLog.Location = new System.Drawing.Point(35, 302);
            this.tbLog.MaxLength = 1500000;
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(229, 129);
            this.tbLog.TabIndex = 27;
            // 
            // lblGlobalStats
            // 
            this.lblGlobalStats.AutoSize = true;
            this.lblGlobalStats.Location = new System.Drawing.Point(35, 536);
            this.lblGlobalStats.Name = "lblGlobalStats";
            this.lblGlobalStats.Size = new System.Drawing.Size(68, 13);
            this.lblGlobalStats.TabIndex = 28;
            this.lblGlobalStats.Text = "Server Stats:";
            // 
            // lblGlobalBatRem
            // 
            this.lblGlobalBatRem.AutoSize = true;
            this.lblGlobalBatRem.Location = new System.Drawing.Point(35, 560);
            this.lblGlobalBatRem.Name = "lblGlobalBatRem";
            this.lblGlobalBatRem.Size = new System.Drawing.Size(88, 13);
            this.lblGlobalBatRem.TabIndex = 29;
            this.lblGlobalBatRem.Text = "Batches Remain:";
            // 
            // lblGlobalPer
            // 
            this.lblGlobalPer.AutoSize = true;
            this.lblGlobalPer.Location = new System.Drawing.Point(35, 583);
            this.lblGlobalPer.Name = "lblGlobalPer";
            this.lblGlobalPer.Size = new System.Drawing.Size(94, 13);
            this.lblGlobalPer.TabIndex = 30;
            this.lblGlobalPer.Text = "Percent Complete:";
            // 
            // LossRunsNavClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 618);
            this.Controls.Add(this.lblGlobalPer);
            this.Controls.Add(this.lblGlobalBatRem);
            this.Controls.Add(this.lblGlobalStats);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.gbProcessingLevel);
            this.Controls.Add(this.btnAdvConfig);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.lblBatchRemain);
            this.Controls.Add(this.btnConfig);
            this.Controls.Add(this.lblMaxMem);
            this.Controls.Add(this.tbMaxMem);
            this.Controls.Add(this.lblMaxCpu);
            this.Controls.Add(this.tbMaxCpu);
            this.Controls.Add(this.lblGlobalTimeout);
            this.Controls.Add(this.tbGlobalTimeout);
            this.Controls.Add(this.lblMaxWindows);
            this.Controls.Add(this.tbMaxWindows);
            this.Controls.Add(this.lblHostUrl);
            this.Controls.Add(this.tbHostUrl);
            this.Controls.Add(this.Output8);
            this.Controls.Add(this.Output7);
            this.Controls.Add(this.Output6);
            this.Controls.Add(this.Output5);
            this.Controls.Add(this.Output4);
            this.Controls.Add(this.Output3);
            this.Controls.Add(this.Output2);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.Output1);
            this.Name = "LossRunsNavClient";
            this.Text = "LossRunsNavClient";
            this.gbProcessingLevel.ResumeLayout(false);
            this.gbProcessingLevel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser Output1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.WebBrowser Output2;
        private System.Windows.Forms.WebBrowser Output3;
        private System.Windows.Forms.WebBrowser Output4;
        private System.Windows.Forms.WebBrowser Output5;
        private System.Windows.Forms.WebBrowser Output6;
        private System.Windows.Forms.WebBrowser Output7;
        private System.Windows.Forms.WebBrowser Output8;
        private System.Windows.Forms.TextBox tbHostUrl;
        private System.Windows.Forms.Label lblHostUrl;
        private System.Windows.Forms.TextBox tbMaxWindows;
        private System.Windows.Forms.Label lblMaxWindows;
        private System.Windows.Forms.Label lblGlobalTimeout;
        private System.Windows.Forms.TextBox tbGlobalTimeout;
        private System.Windows.Forms.Label lblMaxCpu;
        private System.Windows.Forms.TextBox tbMaxCpu;
        private System.Windows.Forms.Label lblMaxMem;
        private System.Windows.Forms.TextBox tbMaxMem;
        private System.Windows.Forms.Button btnConfig;
        private System.Windows.Forms.Label lblBatchRemain;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAdvConfig;
        private System.Windows.Forms.GroupBox gbProcessingLevel;
        private System.Windows.Forms.Label lblProcessingLevel;
        private System.Windows.Forms.RadioButton rbHigh;
        private System.Windows.Forms.RadioButton rbLow;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Label lblGlobalStats;
        private System.Windows.Forms.Label lblGlobalBatRem;
        private System.Windows.Forms.Label lblGlobalPer;
    }
}

