namespace ILC_ControlPanel
{
    partial class ControlPanelScreen
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

            /*if (connection != null)
            {
                connection.Close();
            }*/

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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Status", "icon_status.png");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Phone", "Telephone.png");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Database", "Database.gif");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("SMTP", "icon_smtp.ico");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Local Files", "icon_localFiles.png");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Logging", "icon_logging.png");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlPanelScreen));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new ILC_ControlPanel.SmoothProgressBar();
            this.lblLastCheck = new System.Windows.Forms.Label();
            this.lblCurrentState = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnQuickCheck = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblStartupMode = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.pbStatus = new System.Windows.Forms.PictureBox();
            this.lblTimeStarted = new System.Windows.Forms.Label();
            this.lblCurrentStatus = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.cleanButton = new System.Windows.Forms.Button();
            this.callButton = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.callStatusListBox = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.phoneNumberTextBox = new System.Windows.Forms.TextBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.devicesListBox = new System.Windows.Forms.CheckedListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tbConnectionString = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.tbUserId = new System.Windows.Forms.TextBox();
            this.tbDataSource = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tbEmailServer = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.tbEmailPassword = new System.Windows.Forms.TextBox();
            this.tbEmailLogin = new System.Windows.Forms.TextBox();
            this.btnTestEmail = new System.Windows.Forms.Button();
            this.label20 = new System.Windows.Forms.Label();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.deletePeriodComboBox = new System.Windows.Forms.ComboBox();
            this.deleteFiles_Button = new System.Windows.Forms.Button();
            this.logCheckBox = new System.Windows.Forms.CheckBox();
            this.audioCheckBox = new System.Windows.Forms.CheckBox();
            this.btnSelectAppPath = new System.Windows.Forms.Button();
            this.tbAppDataPath = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lblFreeGbytes = new System.Windows.Forms.Label();
            this.lblUsedGbytes = new System.Windows.Forms.Label();
            this.lblFreeBytes = new System.Windows.Forms.Label();
            this.lblUsedBytes = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.tbLoggingPath = new System.Windows.Forms.TextBox();
            this.dgwLogging = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label19 = new System.Windows.Forms.Label();
            this.btnOpenLog = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.lblServiceVersion = new System.Windows.Forms.Label();
            this.toolTipConfigFile = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipLogFile = new System.Windows.Forms.ToolTip(this.components);
            this.bgwSpaceCounter = new System.ComponentModel.BackgroundWorker();
            this.bgwCall = new System.ComponentModel.BackgroundWorker();
            this.listView1 = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.lblConfigHeader = new System.Windows.Forms.Label();
            this.tbConfigFile = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbStatus)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgwLogging)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Right;
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Location = new System.Drawing.Point(100, 12);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(826, 309);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.TabStop = false;
            this.tabControl1.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Selecting);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(767, 301);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Status";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.progressBar1);
            this.groupBox2.Controls.Add(this.lblLastCheck);
            this.groupBox2.Controls.Add(this.lblCurrentState);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.btnQuickCheck);
            this.groupBox2.Location = new System.Drawing.Point(0, 177);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(506, 124);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Activity";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(170, 34);
            this.progressBar1.Maximum = 0;
            this.progressBar1.Minimum = 0;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.ProgressBarColor = System.Drawing.Color.CornflowerBlue;
            this.progressBar1.ProgressText = "";
            this.progressBar1.Size = new System.Drawing.Size(134, 15);
            this.progressBar1.TabIndex = 6;
            this.progressBar1.Value = 0;
            // 
            // lblLastCheck
            // 
            this.lblLastCheck.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLastCheck.Location = new System.Drawing.Point(101, 75);
            this.lblLastCheck.Name = "lblLastCheck";
            this.lblLastCheck.Size = new System.Drawing.Size(203, 15);
            this.lblLastCheck.TabIndex = 5;
            this.lblLastCheck.Text = "06/22/2009 11:45AM";
            // 
            // lblCurrentState
            // 
            this.lblCurrentState.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCurrentState.Location = new System.Drawing.Point(101, 34);
            this.lblCurrentState.Name = "lblCurrentState";
            this.lblCurrentState.Size = new System.Drawing.Size(63, 15);
            this.lblCurrentState.TabIndex = 4;
            this.lblCurrentState.Text = "working";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(34, 75);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Last check:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Current state:";
            // 
            // btnQuickCheck
            // 
            this.btnQuickCheck.Location = new System.Drawing.Point(377, 42);
            this.btnQuickCheck.Name = "btnQuickCheck";
            this.btnQuickCheck.Size = new System.Drawing.Size(102, 41);
            this.btnQuickCheck.TabIndex = 1;
            this.btnQuickCheck.Text = "Quick Check";
            this.btnQuickCheck.UseVisualStyleBackColor = true;
            this.btnQuickCheck.Click += new System.EventHandler(this.btnQuickCheck_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblStartupMode);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.pbStatus);
            this.groupBox1.Controls.Add(this.lblTimeStarted);
            this.groupBox1.Controls.Add(this.lblCurrentStatus);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnStartStop);
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(506, 161);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Service";
            // 
            // lblStartupMode
            // 
            this.lblStartupMode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblStartupMode.Location = new System.Drawing.Point(101, 122);
            this.lblStartupMode.Name = "lblStartupMode";
            this.lblStartupMode.Size = new System.Drawing.Size(203, 15);
            this.lblStartupMode.TabIndex = 7;
            this.lblStartupMode.Text = "label7";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(29, 123);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Startup type:";
            // 
            // pbStatus
            // 
            this.pbStatus.Image = global::ILC_ControlPanel.Properties.Resources.StartedStatus;
            this.pbStatus.Location = new System.Drawing.Point(288, 29);
            this.pbStatus.Name = "pbStatus";
            this.pbStatus.Size = new System.Drawing.Size(16, 16);
            this.pbStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbStatus.TabIndex = 5;
            this.pbStatus.TabStop = false;
            // 
            // lblTimeStarted
            // 
            this.lblTimeStarted.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTimeStarted.Location = new System.Drawing.Point(101, 76);
            this.lblTimeStarted.Name = "lblTimeStarted";
            this.lblTimeStarted.Size = new System.Drawing.Size(203, 15);
            this.lblTimeStarted.TabIndex = 4;
            this.lblTimeStarted.Text = "06/22/2009 11:45AM";
            // 
            // lblCurrentStatus
            // 
            this.lblCurrentStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCurrentStatus.Location = new System.Drawing.Point(101, 30);
            this.lblCurrentStatus.Name = "lblCurrentStatus";
            this.lblCurrentStatus.Size = new System.Drawing.Size(181, 15);
            this.lblCurrentStatus.TabIndex = 3;
            this.lblCurrentStatus.Text = "started";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Time started:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Current status:";
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(377, 63);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(102, 41);
            this.btnStartStop.TabIndex = 0;
            this.btnStartStop.Text = "Stop";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox10);
            this.tabPage1.Controls.Add(this.groupBox9);
            this.tabPage1.Location = new System.Drawing.Point(4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(767, 301);
            this.tabPage1.TabIndex = 6;
            this.tabPage1.Text = "Phone";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.Leave += new System.EventHandler(this.tabPage1_Leave);
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.cleanButton);
            this.groupBox10.Controls.Add(this.callButton);
            this.groupBox10.Controls.Add(this.label10);
            this.groupBox10.Controls.Add(this.callStatusListBox);
            this.groupBox10.Controls.Add(this.label4);
            this.groupBox10.Controls.Add(this.phoneNumberTextBox);
            this.groupBox10.Location = new System.Drawing.Point(0, 143);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(506, 158);
            this.groupBox10.TabIndex = 1;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Simple Test Call";
            // 
            // cleanButton
            // 
            this.cleanButton.Location = new System.Drawing.Point(174, 72);
            this.cleanButton.Name = "cleanButton";
            this.cleanButton.Size = new System.Drawing.Size(76, 23);
            this.cleanButton.TabIndex = 2;
            this.cleanButton.Text = "Clean";
            this.cleanButton.UseVisualStyleBackColor = true;
            this.cleanButton.Click += new System.EventHandler(this.cleanButton_Click);
            // 
            // callButton
            // 
            this.callButton.Location = new System.Drawing.Point(174, 43);
            this.callButton.Name = "callButton";
            this.callButton.Size = new System.Drawing.Size(76, 23);
            this.callButton.TabIndex = 1;
            this.callButton.Text = "Call";
            this.callButton.UseVisualStyleBackColor = true;
            this.callButton.Click += new System.EventHandler(this.callButton_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(269, 18);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(40, 13);
            this.label10.TabIndex = 3;
            this.label10.Text = "Status:";
            // 
            // callStatusListBox
            // 
            this.callStatusListBox.FormattingEnabled = true;
            this.callStatusListBox.Location = new System.Drawing.Point(272, 35);
            this.callStatusListBox.Name = "callStatusListBox";
            this.callStatusListBox.Size = new System.Drawing.Size(218, 108);
            this.callStatusListBox.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(134, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Destination phone number:";
            // 
            // phoneNumberTextBox
            // 
            this.phoneNumberTextBox.Location = new System.Drawing.Point(20, 57);
            this.phoneNumberTextBox.Name = "phoneNumberTextBox";
            this.phoneNumberTextBox.Size = new System.Drawing.Size(131, 20);
            this.phoneNumberTextBox.TabIndex = 0;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.devicesListBox);
            this.groupBox9.Location = new System.Drawing.Point(0, 0);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(506, 129);
            this.groupBox9.TabIndex = 0;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Select Telephone Adapters";
            // 
            // devicesListBox
            // 
            this.devicesListBox.CheckOnClick = true;
            this.devicesListBox.FormattingEnabled = true;
            this.devicesListBox.Location = new System.Drawing.Point(15, 24);
            this.devicesListBox.Name = "devicesListBox";
            this.devicesListBox.Size = new System.Drawing.Size(475, 94);
            this.devicesListBox.TabIndex = 0;
            this.devicesListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.devicesListBox_ItemCheck);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.tbConnectionString);
            this.tabPage3.Controls.Add(this.groupBox3);
            this.tabPage3.Controls.Add(this.label11);
            this.tabPage3.Location = new System.Drawing.Point(4, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(767, 301);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Database";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tbConnectionString
            // 
            this.tbConnectionString.Location = new System.Drawing.Point(98, 2);
            this.tbConnectionString.Name = "tbConnectionString";
            this.tbConnectionString.ReadOnly = true;
            this.tbConnectionString.Size = new System.Drawing.Size(408, 20);
            this.tbConnectionString.TabIndex = 7;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnTestConnection);
            this.groupBox3.Controls.Add(this.tbPassword);
            this.groupBox3.Controls.Add(this.tbUserId);
            this.groupBox3.Controls.Add(this.tbDataSource);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Location = new System.Drawing.Point(0, 35);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(506, 266);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Set Connection String";
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Location = new System.Drawing.Point(337, 158);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(102, 41);
            this.btnTestConnection.TabIndex = 3;
            this.btnTestConnection.Text = "Test...";
            this.btnTestConnection.UseVisualStyleBackColor = true;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(113, 179);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(132, 20);
            this.tbPassword.TabIndex = 2;
            // 
            // tbUserId
            // 
            this.tbUserId.Location = new System.Drawing.Point(113, 123);
            this.tbUserId.Name = "tbUserId";
            this.tbUserId.Size = new System.Drawing.Size(132, 20);
            this.tbUserId.TabIndex = 1;
            // 
            // tbDataSource
            // 
            this.tbDataSource.Location = new System.Drawing.Point(113, 67);
            this.tbDataSource.Name = "tbDataSource";
            this.tbDataSource.Size = new System.Drawing.Size(326, 20);
            this.tbDataSource.TabIndex = 0;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(51, 182);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(56, 13);
            this.label15.TabIndex = 2;
            this.label15.Text = "Password:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(61, 126);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(46, 13);
            this.label14.TabIndex = 1;
            this.label14.Text = "User ID:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(39, 70);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(68, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Data source:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(0, 5);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(92, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Connection string:";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.tbEmailServer);
            this.tabPage4.Controls.Add(this.groupBox4);
            this.tabPage4.Controls.Add(this.label20);
            this.tabPage4.Location = new System.Drawing.Point(4, 4);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(767, 301);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tbEmailServer
            // 
            this.tbEmailServer.Location = new System.Drawing.Point(75, 2);
            this.tbEmailServer.Name = "tbEmailServer";
            this.tbEmailServer.Size = new System.Drawing.Size(431, 20);
            this.tbEmailServer.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.groupBox6);
            this.groupBox4.Controls.Add(this.btnTestEmail);
            this.groupBox4.Location = new System.Drawing.Point(0, 35);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(506, 266);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Connectivity Test";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.checkBox3);
            this.groupBox6.Controls.Add(this.label17);
            this.groupBox6.Controls.Add(this.label16);
            this.groupBox6.Controls.Add(this.tbEmailPassword);
            this.groupBox6.Controls.Add(this.tbEmailLogin);
            this.groupBox6.Location = new System.Drawing.Point(20, 57);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(298, 153);
            this.groupBox6.TabIndex = 0;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "                                              ";
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(13, -1);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(138, 17);
            this.checkBox3.TabIndex = 8;
            this.checkBox3.Text = "Authentication required:";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.Click += new System.EventHandler(this.checkBox3_Click);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(42, 45);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(61, 13);
            this.label17.TabIndex = 1;
            this.label17.Text = "User name:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(47, 96);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(56, 13);
            this.label16.TabIndex = 2;
            this.label16.Text = "Password:";
            // 
            // tbEmailPassword
            // 
            this.tbEmailPassword.Location = new System.Drawing.Point(104, 93);
            this.tbEmailPassword.Name = "tbEmailPassword";
            this.tbEmailPassword.Size = new System.Drawing.Size(132, 20);
            this.tbEmailPassword.TabIndex = 1;
            // 
            // tbEmailLogin
            // 
            this.tbEmailLogin.Location = new System.Drawing.Point(104, 42);
            this.tbEmailLogin.Name = "tbEmailLogin";
            this.tbEmailLogin.Size = new System.Drawing.Size(132, 20);
            this.tbEmailLogin.TabIndex = 0;
            // 
            // btnTestEmail
            // 
            this.btnTestEmail.Location = new System.Drawing.Point(361, 113);
            this.btnTestEmail.Name = "btnTestEmail";
            this.btnTestEmail.Size = new System.Drawing.Size(102, 41);
            this.btnTestEmail.TabIndex = 1;
            this.btnTestEmail.Text = "Test...";
            this.btnTestEmail.UseVisualStyleBackColor = true;
            this.btnTestEmail.Click += new System.EventHandler(this.btnTestEmail_Click);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(0, 5);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(69, 13);
            this.label20.TabIndex = 7;
            this.label20.Text = "Email Server:";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.groupBox7);
            this.tabPage5.Controls.Add(this.btnSelectAppPath);
            this.tabPage5.Controls.Add(this.tbAppDataPath);
            this.tabPage5.Controls.Add(this.groupBox5);
            this.tabPage5.Controls.Add(this.label21);
            this.tabPage5.Location = new System.Drawing.Point(4, 4);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(767, 301);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Local Files";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label8);
            this.groupBox7.Controls.Add(this.deletePeriodComboBox);
            this.groupBox7.Controls.Add(this.deleteFiles_Button);
            this.groupBox7.Controls.Add(this.logCheckBox);
            this.groupBox7.Controls.Add(this.audioCheckBox);
            this.groupBox7.Location = new System.Drawing.Point(0, 170);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(506, 131);
            this.groupBox7.TabIndex = 2;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Clean Temp Files";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(223, 42);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 13);
            this.label8.TabIndex = 25;
            this.label8.Text = "For the period:";
            // 
            // deletePeriodComboBox
            // 
            this.deletePeriodComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.deletePeriodComboBox.FormattingEnabled = true;
            this.deletePeriodComboBox.Items.AddRange(new object[] {
            "Everything",
            "Last day",
            "Last Week"});
            this.deletePeriodComboBox.Location = new System.Drawing.Point(304, 39);
            this.deletePeriodComboBox.Name = "deletePeriodComboBox";
            this.deletePeriodComboBox.Size = new System.Drawing.Size(186, 21);
            this.deletePeriodComboBox.TabIndex = 2;
            // 
            // deleteFiles_Button
            // 
            this.deleteFiles_Button.Location = new System.Drawing.Point(415, 73);
            this.deleteFiles_Button.Name = "deleteFiles_Button";
            this.deleteFiles_Button.Size = new System.Drawing.Size(75, 23);
            this.deleteFiles_Button.TabIndex = 3;
            this.deleteFiles_Button.Text = "Delete...";
            this.deleteFiles_Button.UseVisualStyleBackColor = true;
            this.deleteFiles_Button.Click += new System.EventHandler(this.OnDelete_Click);
            // 
            // logCheckBox
            // 
            this.logCheckBox.AutoSize = true;
            this.logCheckBox.Checked = true;
            this.logCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.logCheckBox.Location = new System.Drawing.Point(15, 73);
            this.logCheckBox.Name = "logCheckBox";
            this.logCheckBox.Size = new System.Drawing.Size(85, 17);
            this.logCheckBox.TabIndex = 1;
            this.logCheckBox.Text = "Session logs";
            this.logCheckBox.UseVisualStyleBackColor = true;
            this.logCheckBox.CheckedChanged += new System.EventHandler(this.deleteFileCheckChanged);
            // 
            // audioCheckBox
            // 
            this.audioCheckBox.AutoSize = true;
            this.audioCheckBox.Checked = true;
            this.audioCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.audioCheckBox.Location = new System.Drawing.Point(15, 41);
            this.audioCheckBox.Name = "audioCheckBox";
            this.audioCheckBox.Size = new System.Drawing.Size(100, 17);
            this.audioCheckBox.TabIndex = 0;
            this.audioCheckBox.Text = "Audio recording";
            this.audioCheckBox.UseVisualStyleBackColor = true;
            this.audioCheckBox.CheckedChanged += new System.EventHandler(this.deleteFileCheckChanged);
            // 
            // btnSelectAppPath
            // 
            this.btnSelectAppPath.Location = new System.Drawing.Point(481, 0);
            this.btnSelectAppPath.Name = "btnSelectAppPath";
            this.btnSelectAppPath.Size = new System.Drawing.Size(25, 23);
            this.btnSelectAppPath.TabIndex = 1;
            this.btnSelectAppPath.Text = "...";
            this.btnSelectAppPath.UseVisualStyleBackColor = true;
            this.btnSelectAppPath.Click += new System.EventHandler(this.btnSelectAppPath_Click);
            // 
            // tbAppDataPath
            // 
            this.tbAppDataPath.Location = new System.Drawing.Point(116, 2);
            this.tbAppDataPath.Name = "tbAppDataPath";
            this.tbAppDataPath.Size = new System.Drawing.Size(359, 20);
            this.tbAppDataPath.TabIndex = 0;
            // 
            // groupBox5
            // 
            this.groupBox5.AutoSize = true;
            this.groupBox5.Controls.Add(this.lblFreeGbytes);
            this.groupBox5.Controls.Add(this.lblUsedGbytes);
            this.groupBox5.Controls.Add(this.lblFreeBytes);
            this.groupBox5.Controls.Add(this.lblUsedBytes);
            this.groupBox5.Controls.Add(this.label24);
            this.groupBox5.Controls.Add(this.label23);
            this.groupBox5.Controls.Add(this.textBox10);
            this.groupBox5.Controls.Add(this.textBox11);
            this.groupBox5.Location = new System.Drawing.Point(0, 35);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(506, 121);
            this.groupBox5.TabIndex = 12;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Disk Space";
            // 
            // lblFreeGbytes
            // 
            this.lblFreeGbytes.Location = new System.Drawing.Point(416, 69);
            this.lblFreeGbytes.Name = "lblFreeGbytes";
            this.lblFreeGbytes.Size = new System.Drawing.Size(74, 13);
            this.lblFreeGbytes.TabIndex = 28;
            this.lblFreeGbytes.Text = "label28";
            this.lblFreeGbytes.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblUsedGbytes
            // 
            this.lblUsedGbytes.Location = new System.Drawing.Point(416, 39);
            this.lblUsedGbytes.Name = "lblUsedGbytes";
            this.lblUsedGbytes.Size = new System.Drawing.Size(74, 13);
            this.lblUsedGbytes.TabIndex = 27;
            this.lblUsedGbytes.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblFreeBytes
            // 
            this.lblFreeBytes.Location = new System.Drawing.Point(169, 69);
            this.lblFreeBytes.Name = "lblFreeBytes";
            this.lblFreeBytes.Size = new System.Drawing.Size(154, 13);
            this.lblFreeBytes.TabIndex = 26;
            this.lblFreeBytes.Text = "label26";
            this.lblFreeBytes.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblUsedBytes
            // 
            this.lblUsedBytes.Location = new System.Drawing.Point(169, 39);
            this.lblUsedBytes.Name = "lblUsedBytes";
            this.lblUsedBytes.Size = new System.Drawing.Size(154, 13);
            this.lblUsedBytes.TabIndex = 25;
            this.lblUsedBytes.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(34, 69);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(63, 13);
            this.label24.TabIndex = 24;
            this.label24.Text = "Free space:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(34, 39);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(103, 13);
            this.label23.TabIndex = 23;
            this.label23.Text = "Used by application:";
            // 
            // textBox10
            // 
            this.textBox10.BackColor = System.Drawing.Color.Blue;
            this.textBox10.Enabled = false;
            this.textBox10.Font = new System.Drawing.Font("Microsoft Sans Serif", 5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox10.Location = new System.Drawing.Point(15, 37);
            this.textBox10.MaximumSize = new System.Drawing.Size(16, 16);
            this.textBox10.Name = "textBox10";
            this.textBox10.ReadOnly = true;
            this.textBox10.Size = new System.Drawing.Size(15, 15);
            this.textBox10.TabIndex = 21;
            // 
            // textBox11
            // 
            this.textBox11.BackColor = System.Drawing.Color.Fuchsia;
            this.textBox11.Enabled = false;
            this.textBox11.Font = new System.Drawing.Font("Microsoft Sans Serif", 5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox11.Location = new System.Drawing.Point(15, 69);
            this.textBox11.MaximumSize = new System.Drawing.Size(16, 16);
            this.textBox11.Name = "textBox11";
            this.textBox11.ReadOnly = true;
            this.textBox11.Size = new System.Drawing.Size(15, 15);
            this.textBox11.TabIndex = 22;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(0, 5);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(110, 13);
            this.label21.TabIndex = 11;
            this.label21.Text = "Application data path:";
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.groupBox8);
            this.tabPage6.Location = new System.Drawing.Point(4, 4);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(767, 301);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "Logging";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.tbLoggingPath);
            this.groupBox8.Controls.Add(this.dgwLogging);
            this.groupBox8.Controls.Add(this.label19);
            this.groupBox8.Controls.Add(this.btnOpenLog);
            this.groupBox8.Location = new System.Drawing.Point(0, 5);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(506, 296);
            this.groupBox8.TabIndex = 11;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Log4net settings:";
            // 
            // tbLoggingPath
            // 
            this.tbLoggingPath.Location = new System.Drawing.Point(50, 261);
            this.tbLoggingPath.Name = "tbLoggingPath";
            this.tbLoggingPath.ReadOnly = true;
            this.tbLoggingPath.Size = new System.Drawing.Size(369, 20);
            this.tbLoggingPath.TabIndex = 6;
            // 
            // dgwLogging
            // 
            this.dgwLogging.AllowUserToAddRows = false;
            this.dgwLogging.AllowUserToDeleteRows = false;
            this.dgwLogging.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgwLogging.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgwLogging.ColumnHeadersVisible = false;
            this.dgwLogging.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dgwLogging.Location = new System.Drawing.Point(6, 19);
            this.dgwLogging.Name = "dgwLogging";
            this.dgwLogging.RowHeadersVisible = false;
            this.dgwLogging.Size = new System.Drawing.Size(494, 215);
            this.dgwLogging.TabIndex = 2;
            this.dgwLogging.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgwLogging_CellValueChanged);
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "Name";
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "Val";
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(6, 264);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(44, 13);
            this.label19.TabIndex = 3;
            this.label19.Text = "Log file:";
            // 
            // btnOpenLog
            // 
            this.btnOpenLog.Location = new System.Drawing.Point(424, 259);
            this.btnOpenLog.Name = "btnOpenLog";
            this.btnOpenLog.Size = new System.Drawing.Size(75, 23);
            this.btnOpenLog.TabIndex = 5;
            this.btnOpenLog.Text = "Open...";
            this.btnOpenLog.UseVisualStyleBackColor = true;
            this.btnOpenLog.Click += new System.EventHandler(this.btnOpenLog_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(372, 381);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 5;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(454, 381);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnApply
            // 
            this.btnApply.Enabled = false;
            this.btnApply.Location = new System.Drawing.Point(536, 381);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 7;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // lblServiceVersion
            // 
            this.lblServiceVersion.AutoSize = true;
            this.lblServiceVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblServiceVersion.Location = new System.Drawing.Point(101, 388);
            this.lblServiceVersion.Name = "lblServiceVersion";
            this.lblServiceVersion.Size = new System.Drawing.Size(77, 13);
            this.lblServiceVersion.TabIndex = 8;
            this.lblServiceVersion.Text = "service version";
            // 
            // bgwSpaceCounter
            // 
            this.bgwSpaceCounter.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSpaceCounter_DoWork);
            this.bgwSpaceCounter.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSpaceCounter_RunWorkerCompleted);
            // 
            // bgwCall
            // 
            this.bgwCall.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwCall_DoWork);
            this.bgwCall.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwCall_RunWorkerCompleted);
            // 
            // listView1
            // 
            this.listView1.HideSelection = false;
            this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6});
            this.listView1.LargeImageList = this.imageList1;
            this.listView1.Location = new System.Drawing.Point(13, 16);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.ShowGroups = false;
            this.listView1.Size = new System.Drawing.Size(72, 388);
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.TabIndex = 9;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView1_ItemSelectionChanged);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "icon_status.png");
            this.imageList1.Images.SetKeyName(1, "Telephone.png");
            this.imageList1.Images.SetKeyName(2, "Database.gif");
            this.imageList1.Images.SetKeyName(3, "icon_smtp.ico");
            this.imageList1.Images.SetKeyName(4, "icon_localFiles.png");
            this.imageList1.Images.SetKeyName(5, "icon_logging.png");
            // 
            // lblConfigHeader
            // 
            this.lblConfigHeader.AutoSize = true;
            this.lblConfigHeader.Location = new System.Drawing.Point(101, 339);
            this.lblConfigHeader.Name = "lblConfigHeader";
            this.lblConfigHeader.Size = new System.Drawing.Size(88, 13);
            this.lblConfigHeader.TabIndex = 10;
            this.lblConfigHeader.Text = "Configuration file:";
            // 
            // tbConfigFile
            // 
            this.tbConfigFile.Location = new System.Drawing.Point(195, 336);
            this.tbConfigFile.Name = "tbConfigFile";
            this.tbConfigFile.ReadOnly = true;
            this.tbConfigFile.Size = new System.Drawing.Size(415, 20);
            this.tbConfigFile.TabIndex = 12;
            // 
            // ControlPanelScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 420);
            this.Controls.Add(this.tbConfigFile);
            this.Controls.Add(this.lblConfigHeader);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.lblServiceVersion);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ControlPanelScreen";
            this.Text = "ILC Control Panel";
            this.Load += new System.EventHandler(this.ControlPanelScreen_Load);
            this.VisibleChanged += new System.EventHandler(this.ControlPanelScreen_VisibleChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControlPanelScreen_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbStatus)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabPage6.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgwLogging)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnQuickCheck;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Label lblCurrentStatus;
        private System.Windows.Forms.Label lblLastCheck;
        private System.Windows.Forms.Label lblCurrentState;
        private System.Windows.Forms.Label lblTimeStarted;
        private SmoothProgressBar progressBar1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label lblServiceVersion;
        private System.Windows.Forms.PictureBox pbStatus;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.TextBox tbUserId;
        private System.Windows.Forms.TextBox tbDataSource;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.TextBox tbEmailServer;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnTestEmail;
        private System.Windows.Forms.TextBox tbEmailPassword;
        private System.Windows.Forms.TextBox tbEmailLogin;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button btnSelectAppPath;
        private System.Windows.Forms.TextBox tbAppDataPath;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.DataGridView dgwLogging;
        private System.Windows.Forms.Button btnOpenLog;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label lblStartupMode;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ToolTip toolTipConfigFile;
        private System.Windows.Forms.ToolTip toolTipLogFile;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox deletePeriodComboBox;
        private System.Windows.Forms.Button deleteFiles_Button;
        private System.Windows.Forms.CheckBox logCheckBox;
        private System.Windows.Forms.CheckBox audioCheckBox;
        private System.Windows.Forms.Label lblFreeGbytes;
        private System.Windows.Forms.Label lblUsedGbytes;
        private System.Windows.Forms.Label lblFreeBytes;
        private System.Windows.Forms.Label lblUsedBytes;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.TextBox textBox11;
        private System.ComponentModel.BackgroundWorker bgwSpaceCounter;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.CheckedListBox devicesListBox;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox phoneNumberTextBox;
        private System.Windows.Forms.Button cleanButton;
        private System.Windows.Forms.Button callButton;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ListBox callStatusListBox;
        private System.ComponentModel.BackgroundWorker bgwCall;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label lblConfigHeader;
        private System.Windows.Forms.TextBox tbConfigFile;
        private System.Windows.Forms.TextBox tbConnectionString;
        private System.Windows.Forms.TextBox tbLoggingPath;
    }
}
