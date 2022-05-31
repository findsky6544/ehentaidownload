
using System.Windows.Forms;

namespace EHentaiDwonload
{
    partial class Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.pauseAddUrlButton = new System.Windows.Forms.Button();
            this.remainTimeLabel = new System.Windows.Forms.Label();
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.allMangasProgressBarLabel = new System.Windows.Forms.Label();
            this.stopAddUrlButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mangaListView = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.deleteMangaButton = new System.Windows.Forms.Button();
            this.exportMangaButton = new System.Windows.Forms.Button();
            this.ImportMangaButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.addUrlButton = new System.Windows.Forms.Button();
            this.downloadButton = new System.Windows.Forms.Button();
            this.stopDownloadButton = new System.Windows.Forms.Button();
            this.pauseDownloadButton = new System.Windows.Forms.Button();
            this.oneMangaProgressBarLabel = new System.Windows.Forms.Label();
            this.allMangasProgressBar = new System.Windows.Forms.ProgressBar();
            this.oneMangaProgressBar = new System.Windows.Forms.ProgressBar();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel10 = new System.Windows.Forms.Panel();
            this.minWaitTimeTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.downloadPathTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.applyConfigButton = new System.Windows.Forms.Button();
            this.ServerIpTextBox = new System.Windows.Forms.TextBox();
            this.userIdTextBox = new System.Windows.Forms.TextBox();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel10.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 652);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.pauseAddUrlButton);
            this.tabPage1.Controls.Add(this.remainTimeLabel);
            this.tabPage1.Controls.Add(this.urlTextBox);
            this.tabPage1.Controls.Add(this.allMangasProgressBarLabel);
            this.tabPage1.Controls.Add(this.stopAddUrlButton);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.addUrlButton);
            this.tabPage1.Controls.Add(this.downloadButton);
            this.tabPage1.Controls.Add(this.stopDownloadButton);
            this.tabPage1.Controls.Add(this.pauseDownloadButton);
            this.tabPage1.Controls.Add(this.oneMangaProgressBarLabel);
            this.tabPage1.Controls.Add(this.allMangasProgressBar);
            this.tabPage1.Controls.Add(this.oneMangaProgressBar);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 626);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "下载";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // pauseAddUrlButton
            // 
            this.pauseAddUrlButton.Enabled = false;
            this.pauseAddUrlButton.Location = new System.Drawing.Point(633, 0);
            this.pauseAddUrlButton.Name = "pauseAddUrlButton";
            this.pauseAddUrlButton.Size = new System.Drawing.Size(75, 23);
            this.pauseAddUrlButton.TabIndex = 15;
            this.pauseAddUrlButton.Text = "暂停添加";
            this.pauseAddUrlButton.UseVisualStyleBackColor = true;
            this.pauseAddUrlButton.Click += new System.EventHandler(this.pauseAddUrlButton_Click);
            // 
            // remainTimeLabel
            // 
            this.remainTimeLabel.Location = new System.Drawing.Point(3, 608);
            this.remainTimeLabel.Name = "remainTimeLabel";
            this.remainTimeLabel.Size = new System.Drawing.Size(786, 15);
            this.remainTimeLabel.TabIndex = 0;
            this.remainTimeLabel.Text = "下载速度 -秒/页 已下载时间-天-时-分-秒 预计剩余-天-时-分-秒";
            this.remainTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // urlTextBox
            // 
            this.urlTextBox.Location = new System.Drawing.Point(3, 2);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(543, 21);
            this.urlTextBox.TabIndex = 1;
            // 
            // allMangasProgressBarLabel
            // 
            this.allMangasProgressBarLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.allMangasProgressBarLabel.AutoSize = true;
            this.allMangasProgressBarLabel.ForeColor = System.Drawing.Color.Black;
            this.allMangasProgressBarLabel.Location = new System.Drawing.Point(322, 559);
            this.allMangasProgressBarLabel.Name = "allMangasProgressBarLabel";
            this.allMangasProgressBarLabel.Size = new System.Drawing.Size(143, 12);
            this.allMangasProgressBarLabel.TabIndex = 20;
            this.allMangasProgressBarLabel.Text = "已完成-本 剩余-本 -/-页";
            // 
            // stopAddUrlButton
            // 
            this.stopAddUrlButton.Enabled = false;
            this.stopAddUrlButton.Location = new System.Drawing.Point(714, 0);
            this.stopAddUrlButton.Name = "stopAddUrlButton";
            this.stopAddUrlButton.Size = new System.Drawing.Size(75, 23);
            this.stopAddUrlButton.TabIndex = 3;
            this.stopAddUrlButton.Text = "停止添加";
            this.stopAddUrlButton.UseVisualStyleBackColor = true;
            this.stopAddUrlButton.Click += new System.EventHandler(this.stopAddUrlButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.mangaListView);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Location = new System.Drawing.Point(6, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(381, 496);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "待提取列表";
            // 
            // mangaListView
            // 
            this.mangaListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.mangaListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mangaListView.FullRowSelect = true;
            this.mangaListView.HideSelection = false;
            this.mangaListView.Location = new System.Drawing.Point(3, 17);
            this.mangaListView.MultiSelect = false;
            this.mangaListView.Name = "mangaListView";
            this.mangaListView.ShowItemToolTips = true;
            this.mangaListView.Size = new System.Drawing.Size(375, 444);
            this.mangaListView.TabIndex = 5;
            this.mangaListView.UseCompatibleStateImageBehavior = false;
            this.mangaListView.View = System.Windows.Forms.View.Details;
            this.mangaListView.SelectedIndexChanged += new System.EventHandler(this.mangaListView_SelectedIndexChanged);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "网址";
            this.columnHeader2.Width = 25;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "名称";
            this.columnHeader1.Width = 400;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "总页数";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "已完成";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "剩余";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.deleteMangaButton);
            this.panel1.Controls.Add(this.exportMangaButton);
            this.panel1.Controls.Add(this.ImportMangaButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 461);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(375, 32);
            this.panel1.TabIndex = 8;
            // 
            // deleteMangaButton
            // 
            this.deleteMangaButton.Enabled = false;
            this.deleteMangaButton.Location = new System.Drawing.Point(58, 4);
            this.deleteMangaButton.Name = "deleteMangaButton";
            this.deleteMangaButton.Size = new System.Drawing.Size(75, 23);
            this.deleteMangaButton.TabIndex = 4;
            this.deleteMangaButton.Text = "删除";
            this.deleteMangaButton.UseVisualStyleBackColor = true;
            this.deleteMangaButton.Click += new System.EventHandler(this.deleteMangaButton_Click);
            // 
            // exportMangaButton
            // 
            this.exportMangaButton.Enabled = false;
            this.exportMangaButton.Location = new System.Drawing.Point(222, 4);
            this.exportMangaButton.Name = "exportMangaButton";
            this.exportMangaButton.Size = new System.Drawing.Size(75, 23);
            this.exportMangaButton.TabIndex = 7;
            this.exportMangaButton.Text = "导出";
            this.exportMangaButton.UseVisualStyleBackColor = true;
            this.exportMangaButton.Click += new System.EventHandler(this.exportMangaButton_Click);
            // 
            // ImportMangaButton
            // 
            this.ImportMangaButton.Location = new System.Drawing.Point(140, 4);
            this.ImportMangaButton.Name = "ImportMangaButton";
            this.ImportMangaButton.Size = new System.Drawing.Size(75, 23);
            this.ImportMangaButton.TabIndex = 6;
            this.ImportMangaButton.Text = "导入";
            this.ImportMangaButton.UseVisualStyleBackColor = true;
            this.ImportMangaButton.Click += new System.EventHandler(this.ImportMangaButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.outputTextBox);
            this.groupBox2.Location = new System.Drawing.Point(391, 29);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(398, 496);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "输出";
            // 
            // outputTextBox
            // 
            this.outputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputTextBox.Location = new System.Drawing.Point(3, 17);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.outputTextBox.Size = new System.Drawing.Size(392, 476);
            this.outputTextBox.TabIndex = 0;
            // 
            // addUrlButton
            // 
            this.addUrlButton.Location = new System.Drawing.Point(552, 0);
            this.addUrlButton.Name = "addUrlButton";
            this.addUrlButton.Size = new System.Drawing.Size(75, 23);
            this.addUrlButton.TabIndex = 2;
            this.addUrlButton.Text = "添加";
            this.addUrlButton.UseVisualStyleBackColor = true;
            this.addUrlButton.Click += new System.EventHandler(this.addUrlButton_Click);
            // 
            // downloadButton
            // 
            this.downloadButton.Enabled = false;
            this.downloadButton.Location = new System.Drawing.Point(258, 582);
            this.downloadButton.Name = "downloadButton";
            this.downloadButton.Size = new System.Drawing.Size(75, 23);
            this.downloadButton.TabIndex = 9;
            this.downloadButton.Text = "下载";
            this.downloadButton.UseVisualStyleBackColor = true;
            this.downloadButton.Click += new System.EventHandler(this.downloadButton_Click);
            // 
            // stopDownloadButton
            // 
            this.stopDownloadButton.Enabled = false;
            this.stopDownloadButton.Location = new System.Drawing.Point(420, 582);
            this.stopDownloadButton.Name = "stopDownloadButton";
            this.stopDownloadButton.Size = new System.Drawing.Size(75, 23);
            this.stopDownloadButton.TabIndex = 11;
            this.stopDownloadButton.Text = "停止下载";
            this.stopDownloadButton.UseVisualStyleBackColor = true;
            this.stopDownloadButton.Click += new System.EventHandler(this.stopDownloadButton_Click);
            // 
            // pauseDownloadButton
            // 
            this.pauseDownloadButton.Enabled = false;
            this.pauseDownloadButton.Location = new System.Drawing.Point(339, 582);
            this.pauseDownloadButton.Name = "pauseDownloadButton";
            this.pauseDownloadButton.Size = new System.Drawing.Size(75, 23);
            this.pauseDownloadButton.TabIndex = 10;
            this.pauseDownloadButton.Text = "暂停下载";
            this.pauseDownloadButton.UseVisualStyleBackColor = true;
            this.pauseDownloadButton.Click += new System.EventHandler(this.pauseDownloadButton_Click);
            // 
            // oneMangaProgressBarLabel
            // 
            this.oneMangaProgressBarLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.oneMangaProgressBarLabel.AutoSize = true;
            this.oneMangaProgressBarLabel.ForeColor = System.Drawing.Color.Black;
            this.oneMangaProgressBarLabel.Location = new System.Drawing.Point(334, 530);
            this.oneMangaProgressBarLabel.Name = "oneMangaProgressBarLabel";
            this.oneMangaProgressBarLabel.Size = new System.Drawing.Size(119, 12);
            this.oneMangaProgressBarLabel.TabIndex = 19;
            this.oneMangaProgressBarLabel.Text = "正在提取第-页,共-页";
            // 
            // allMangasProgressBar
            // 
            this.allMangasProgressBar.Location = new System.Drawing.Point(3, 553);
            this.allMangasProgressBar.Name = "allMangasProgressBar";
            this.allMangasProgressBar.Size = new System.Drawing.Size(786, 23);
            this.allMangasProgressBar.TabIndex = 18;
            // 
            // oneMangaProgressBar
            // 
            this.oneMangaProgressBar.Location = new System.Drawing.Point(3, 524);
            this.oneMangaProgressBar.Name = "oneMangaProgressBar";
            this.oneMangaProgressBar.Size = new System.Drawing.Size(786, 23);
            this.oneMangaProgressBar.TabIndex = 17;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panel10);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(792, 626);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "设置";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel10
            // 
            this.panel10.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel10.Controls.Add(this.minWaitTimeTextBox);
            this.panel10.Controls.Add(this.label5);
            this.panel10.Controls.Add(this.label2);
            this.panel10.Controls.Add(this.downloadPathTextBox);
            this.panel10.Controls.Add(this.label3);
            this.panel10.Controls.Add(this.label1);
            this.panel10.Controls.Add(this.label4);
            this.panel10.Controls.Add(this.applyConfigButton);
            this.panel10.Controls.Add(this.ServerIpTextBox);
            this.panel10.Controls.Add(this.userIdTextBox);
            this.panel10.Controls.Add(this.portTextBox);
            this.panel10.Location = new System.Drawing.Point(286, 111);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(217, 189);
            this.panel10.TabIndex = 9;
            // 
            // minWaitTimeTextBox
            // 
            this.minWaitTimeTextBox.Location = new System.Drawing.Point(98, 133);
            this.minWaitTimeTextBox.Name = "minWaitTimeTextBox";
            this.minWaitTimeTextBox.Size = new System.Drawing.Size(100, 21);
            this.minWaitTimeTextBox.TabIndex = 10;
            this.minWaitTimeTextBox.Text = "7";
            this.minWaitTimeTextBox.GotFocus += new System.EventHandler(this.minWaitTimeTextBox_GotFocus);
            this.minWaitTimeTextBox.LostFocus += new System.EventHandler(this.minWaitTimeTextBox_LostFocus);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(27, 136);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "最短间隔：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "服务器地址：";
            // 
            // downloadPathTextBox
            // 
            this.downloadPathTextBox.Location = new System.Drawing.Point(98, 106);
            this.downloadPathTextBox.Name = "downloadPathTextBox";
            this.downloadPathTextBox.Size = new System.Drawing.Size(100, 21);
            this.downloadPathTextBox.TabIndex = 8;
            this.downloadPathTextBox.TextChanged += new System.EventHandler(this.downloadPathTextBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(51, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "端口：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "下载路径：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(39, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "用户ID：";
            // 
            // applyConfigButton
            // 
            this.applyConfigButton.Location = new System.Drawing.Point(69, 163);
            this.applyConfigButton.Name = "applyConfigButton";
            this.applyConfigButton.Size = new System.Drawing.Size(75, 23);
            this.applyConfigButton.TabIndex = 6;
            this.applyConfigButton.Text = "应用";
            this.applyConfigButton.UseVisualStyleBackColor = true;
            this.applyConfigButton.Click += new System.EventHandler(this.applyConfigButton_Click);
            // 
            // ServerIpTextBox
            // 
            this.ServerIpTextBox.Location = new System.Drawing.Point(98, 25);
            this.ServerIpTextBox.Name = "ServerIpTextBox";
            this.ServerIpTextBox.Size = new System.Drawing.Size(100, 21);
            this.ServerIpTextBox.TabIndex = 3;
            this.ServerIpTextBox.TextChanged += new System.EventHandler(this.ServerIpTextBox_TextChanged);
            // 
            // userIdTextBox
            // 
            this.userIdTextBox.Location = new System.Drawing.Point(98, 79);
            this.userIdTextBox.Name = "userIdTextBox";
            this.userIdTextBox.Size = new System.Drawing.Size(100, 21);
            this.userIdTextBox.TabIndex = 5;
            this.userIdTextBox.TextChanged += new System.EventHandler(this.userIdTextBox_TextChanged);
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(98, 52);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(100, 21);
            this.portTextBox.TabIndex = 4;
            this.portTextBox.TextChanged += new System.EventHandler(this.portTextBox_TextChanged);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 652);
            this.Controls.Add(this.tabControl1);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EHentaiDownload";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.SizeChanged += new System.EventHandler(this.Main_SizeChanged);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.Button stopDownloadButton;
        private System.Windows.Forms.Button pauseDownloadButton;
        private System.Windows.Forms.Button downloadButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView mangaListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Button exportMangaButton;
        private System.Windows.Forms.Button deleteMangaButton;
        private System.Windows.Forms.Button ImportMangaButton;
        private System.Windows.Forms.Button stopAddUrlButton;
        private System.Windows.Forms.Button addUrlButton;
        private System.Windows.Forms.TextBox urlTextBox;
        private System.Windows.Forms.Label remainTimeLabel;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox userIdTextBox;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.TextBox ServerIpTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button applyConfigButton;
        private System.Windows.Forms.Button pauseAddUrlButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private ProgressBar allMangasProgressBar;
        private ProgressBar oneMangaProgressBar;
        private System.Windows.Forms.Label oneMangaProgressBarLabel;
        private Label allMangasProgressBarLabel;
        private TextBox downloadPathTextBox;
        private Label label1;
        private Panel panel10;
        private TextBox minWaitTimeTextBox;
        private Label label5;
        private Panel panel1;
    }
}

