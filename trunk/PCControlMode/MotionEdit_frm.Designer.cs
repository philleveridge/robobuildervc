namespace RobobuilderLib
{
    partial class MotionEdit_frm
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
            this.closeBtn = new System.Windows.Forms.Button();
            this.saveFile = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.debugFlag = new System.Windows.Forms.CheckBox();
            this.playAll = new System.Windows.Forms.Button();
            this.record = new System.Windows.Forms.Button();
            this.loadFile = new System.Windows.Forms.Button();
            this.fnstring = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.updateRow = new System.Windows.Forms.Button();
            this.playRow = new System.Windows.Forms.Button();
            this.setBasic = new System.Windows.Forms.Button();
            this.queryValues = new System.Windows.Forms.Button();
            this.delRow = new System.Windows.Forms.Button();
            this.autopose = new System.Windows.Forms.Button();
            this.xV = new System.Windows.Forms.Label();
            this.yV = new System.Windows.Forms.Label();
            this.zV = new System.Windows.Forms.Label();
            this.all_pass_chk = new System.Windows.Forms.CheckBox();
            this.base_chk = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.torq_list = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.close = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.servoStatus1 = new RobobuilderLib.ServoStatus();
            this.servoPoseDataBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.timeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Steps = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.s0DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.s1DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.s2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.s3DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.s4DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.s5DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.S6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.S7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.S8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.S9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.S10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.S11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.S12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.S13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.S14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.S15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.S16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.S17 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.S18 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.S19 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.X = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Y = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Z = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.servoPoseDataBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // closeBtn
            // 
            this.closeBtn.Location = new System.Drawing.Point(497, 221);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(50, 21);
            this.closeBtn.TabIndex = 0;
            this.closeBtn.Text = "Close";
            this.closeBtn.UseVisualStyleBackColor = true;
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            // 
            // saveFile
            // 
            this.saveFile.Location = new System.Drawing.Point(307, 6);
            this.saveFile.Name = "saveFile";
            this.saveFile.Size = new System.Drawing.Size(48, 21);
            this.saveFile.TabIndex = 67;
            this.saveFile.Text = "Save";
            this.saveFile.UseVisualStyleBackColor = true;
            this.saveFile.Click += new System.EventHandler(this.saveFile_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(270, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 13);
            this.label2.TabIndex = 66;
            this.label2.Text = "1";
            // 
            // debugFlag
            // 
            this.debugFlag.AutoSize = true;
            this.debugFlag.Location = new System.Drawing.Point(249, 11);
            this.debugFlag.Name = "debugFlag";
            this.debugFlag.Size = new System.Drawing.Size(15, 14);
            this.debugFlag.TabIndex = 65;
            this.debugFlag.UseVisualStyleBackColor = true;
            // 
            // playAll
            // 
            this.playAll.Location = new System.Drawing.Point(423, 220);
            this.playAll.Name = "playAll";
            this.playAll.Size = new System.Drawing.Size(69, 21);
            this.playAll.TabIndex = 64;
            this.playAll.Text = "Play All";
            this.playAll.UseVisualStyleBackColor = true;
            this.playAll.Click += new System.EventHandler(this.playAll_Click);
            // 
            // record
            // 
            this.record.BackColor = System.Drawing.Color.Red;
            this.record.Location = new System.Drawing.Point(62, 4);
            this.record.Name = "record";
            this.record.Size = new System.Drawing.Size(28, 26);
            this.record.TabIndex = 63;
            this.record.Text = "R";
            this.record.UseVisualStyleBackColor = false;
            this.record.Click += new System.EventHandler(this.record_Click);
            // 
            // loadFile
            // 
            this.loadFile.Location = new System.Drawing.Point(361, 6);
            this.loadFile.Name = "loadFile";
            this.loadFile.Size = new System.Drawing.Size(46, 21);
            this.loadFile.TabIndex = 62;
            this.loadFile.Text = "Load";
            this.loadFile.UseVisualStyleBackColor = true;
            this.loadFile.Click += new System.EventHandler(this.loadFile_Click);
            // 
            // fnstring
            // 
            this.fnstring.BackColor = System.Drawing.SystemColors.ControlLight;
            this.fnstring.Location = new System.Drawing.Point(413, 5);
            this.fnstring.Name = "fnstring";
            this.fnstring.Size = new System.Drawing.Size(110, 18);
            this.fnstring.TabIndex = 69;
            this.fnstring.Text = "..";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.timeDataGridViewTextBoxColumn,
            this.Steps,
            this.s0DataGridViewTextBoxColumn,
            this.s1DataGridViewTextBoxColumn,
            this.s2DataGridViewTextBoxColumn,
            this.s3DataGridViewTextBoxColumn,
            this.s4DataGridViewTextBoxColumn,
            this.s5DataGridViewTextBoxColumn,
            this.S6,
            this.S7,
            this.S8,
            this.S9,
            this.S10,
            this.S11,
            this.S12,
            this.S13,
            this.S14,
            this.S15,
            this.S16,
            this.S17,
            this.S18,
            this.S19,
            this.X,
            this.Y,
            this.Z});
            this.dataGridView1.DataBindings.Add(new System.Windows.Forms.Binding("Tag", this.servoPoseDataBindingSource, "Time", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.dataGridView1.DataSource = this.servoPoseDataBindingSource;
            this.dataGridView1.Location = new System.Drawing.Point(14, 247);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(684, 199);
            this.dataGridView1.TabIndex = 71;
            // 
            // updateRow
            // 
            this.updateRow.Location = new System.Drawing.Point(96, 4);
            this.updateRow.Name = "updateRow";
            this.updateRow.Size = new System.Drawing.Size(22, 25);
            this.updateRow.TabIndex = 72;
            this.updateRow.Text = "U";
            this.updateRow.UseVisualStyleBackColor = true;
            this.updateRow.Click += new System.EventHandler(this.updateRow_Click);
            // 
            // playRow
            // 
            this.playRow.Location = new System.Drawing.Point(124, 4);
            this.playRow.Name = "playRow";
            this.playRow.Size = new System.Drawing.Size(22, 25);
            this.playRow.TabIndex = 73;
            this.playRow.Text = "P";
            this.playRow.UseVisualStyleBackColor = true;
            this.playRow.Click += new System.EventHandler(this.playRow_Click);
            // 
            // setBasic
            // 
            this.setBasic.Location = new System.Drawing.Point(12, 4);
            this.setBasic.Name = "setBasic";
            this.setBasic.Size = new System.Drawing.Size(44, 25);
            this.setBasic.TabIndex = 74;
            this.setBasic.Text = "Basic";
            this.setBasic.UseVisualStyleBackColor = true;
            this.setBasic.Click += new System.EventHandler(this.setBasic_Click);
            // 
            // queryValues
            // 
            this.queryValues.Location = new System.Drawing.Point(197, 5);
            this.queryValues.Name = "queryValues";
            this.queryValues.Size = new System.Drawing.Size(22, 25);
            this.queryValues.TabIndex = 75;
            this.queryValues.Text = "?";
            this.queryValues.UseVisualStyleBackColor = true;
            this.queryValues.Click += new System.EventHandler(this.queryValues_Click);
            // 
            // delRow
            // 
            this.delRow.Location = new System.Drawing.Point(152, 4);
            this.delRow.Name = "delRow";
            this.delRow.Size = new System.Drawing.Size(22, 25);
            this.delRow.TabIndex = 76;
            this.delRow.Text = "X";
            this.delRow.UseVisualStyleBackColor = true;
            this.delRow.Click += new System.EventHandler(this.delRow_Click);
            // 
            // autopose
            // 
            this.autopose.Location = new System.Drawing.Point(423, 30);
            this.autopose.Name = "autopose";
            this.autopose.Size = new System.Drawing.Size(45, 30);
            this.autopose.TabIndex = 77;
            this.autopose.Text = "Auto";
            this.autopose.UseVisualStyleBackColor = true;
            this.autopose.Click += new System.EventHandler(this.autopose_Click);
            // 
            // xV
            // 
            this.xV.AutoSize = true;
            this.xV.Location = new System.Drawing.Point(387, 56);
            this.xV.Name = "xV";
            this.xV.Size = new System.Drawing.Size(20, 13);
            this.xV.TabIndex = 78;
            this.xV.Text = "X=";
            // 
            // yV
            // 
            this.yV.AutoSize = true;
            this.yV.Location = new System.Drawing.Point(387, 79);
            this.yV.Name = "yV";
            this.yV.Size = new System.Drawing.Size(20, 13);
            this.yV.TabIndex = 79;
            this.yV.Text = "Y=";
            // 
            // zV
            // 
            this.zV.AutoSize = true;
            this.zV.Location = new System.Drawing.Point(387, 104);
            this.zV.Name = "zV";
            this.zV.Size = new System.Drawing.Size(20, 13);
            this.zV.TabIndex = 80;
            this.zV.Text = "Z=";
            // 
            // all_pass_chk
            // 
            this.all_pass_chk.AutoSize = true;
            this.all_pass_chk.Location = new System.Drawing.Point(422, 190);
            this.all_pass_chk.Name = "all_pass_chk";
            this.all_pass_chk.Size = new System.Drawing.Size(76, 17);
            this.all_pass_chk.TabIndex = 81;
            this.all_pass_chk.Text = "All passive";
            this.all_pass_chk.UseVisualStyleBackColor = true;
            this.all_pass_chk.CheckedChanged += new System.EventHandler(this.all_pass_chk_CheckedChanged);
            // 
            // base_chk
            // 
            this.base_chk.AutoSize = true;
            this.base_chk.Location = new System.Drawing.Point(423, 165);
            this.base_chk.Name = "base_chk";
            this.base_chk.Size = new System.Drawing.Size(71, 17);
            this.base_chk.TabIndex = 82;
            this.base_chk.Text = "Use base";
            this.base_chk.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(550, 204);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 38);
            this.label1.TabIndex = 84;
            this.label1.Text = "Info=";
            // 
            // torq_list
            // 
            this.torq_list.FormattingEnabled = true;
            this.torq_list.Items.AddRange(new object[] {
            "0 (max Torq)",
            "1",
            "2 (default Torq)",
            "3",
            "4 (Min Torq)"});
            this.torq_list.Location = new System.Drawing.Point(423, 118);
            this.torq_list.Name = "torq_list";
            this.torq_list.Size = new System.Drawing.Size(92, 30);
            this.torq_list.TabIndex = 85;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.close);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Location = new System.Drawing.Point(21, 476);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(212, 64);
            this.panel1.TabIndex = 86;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(151, 32);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(44, 20);
            this.button2.TabIndex = 4;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // close
            // 
            this.close.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.close.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.close.Location = new System.Drawing.Point(175, 1);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(22, 14);
            this.close.TabIndex = 3;
            this.close.Text = "X";
            this.close.UseVisualStyleBackColor = true;
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Range (ID)";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(81, 32);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(59, 20);
            this.textBox2.TabIndex = 1;
            this.textBox2.Text = "254";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(16, 32);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(59, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "0";
            // 
            // servoStatus1
            // 
            this.servoStatus1.Location = new System.Drawing.Point(560, 6);
            this.servoStatus1.Name = "servoStatus1";
            this.servoStatus1.Size = new System.Drawing.Size(138, 209);
            this.servoStatus1.TabIndex = 83;
            // 
            // servoPoseDataBindingSource
            // 
            this.servoPoseDataBindingSource.DataSource = typeof(RobobuilderLib.ServoPoseData);
            // 
            // timeDataGridViewTextBoxColumn
            // 
            this.timeDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.timeDataGridViewTextBoxColumn.DataPropertyName = "Time";
            this.timeDataGridViewTextBoxColumn.HeaderText = "Time";
            this.timeDataGridViewTextBoxColumn.Name = "timeDataGridViewTextBoxColumn";
            this.timeDataGridViewTextBoxColumn.Width = 55;
            // 
            // Steps
            // 
            this.Steps.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Steps.DataPropertyName = "Steps";
            this.Steps.HeaderText = "Steps";
            this.Steps.Name = "Steps";
            this.Steps.Width = 59;
            // 
            // s0DataGridViewTextBoxColumn
            // 
            this.s0DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.s0DataGridViewTextBoxColumn.DataPropertyName = "S0";
            this.s0DataGridViewTextBoxColumn.HeaderText = "S0";
            this.s0DataGridViewTextBoxColumn.Name = "s0DataGridViewTextBoxColumn";
            this.s0DataGridViewTextBoxColumn.Width = 45;
            // 
            // s1DataGridViewTextBoxColumn
            // 
            this.s1DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.s1DataGridViewTextBoxColumn.DataPropertyName = "S1";
            this.s1DataGridViewTextBoxColumn.HeaderText = "S1";
            this.s1DataGridViewTextBoxColumn.Name = "s1DataGridViewTextBoxColumn";
            this.s1DataGridViewTextBoxColumn.Width = 45;
            // 
            // s2DataGridViewTextBoxColumn
            // 
            this.s2DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.s2DataGridViewTextBoxColumn.DataPropertyName = "S2";
            this.s2DataGridViewTextBoxColumn.HeaderText = "S2";
            this.s2DataGridViewTextBoxColumn.Name = "s2DataGridViewTextBoxColumn";
            this.s2DataGridViewTextBoxColumn.Width = 45;
            // 
            // s3DataGridViewTextBoxColumn
            // 
            this.s3DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.s3DataGridViewTextBoxColumn.DataPropertyName = "S3";
            this.s3DataGridViewTextBoxColumn.HeaderText = "S3";
            this.s3DataGridViewTextBoxColumn.Name = "s3DataGridViewTextBoxColumn";
            this.s3DataGridViewTextBoxColumn.Width = 45;
            // 
            // s4DataGridViewTextBoxColumn
            // 
            this.s4DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.s4DataGridViewTextBoxColumn.DataPropertyName = "S4";
            this.s4DataGridViewTextBoxColumn.HeaderText = "S4";
            this.s4DataGridViewTextBoxColumn.Name = "s4DataGridViewTextBoxColumn";
            this.s4DataGridViewTextBoxColumn.Width = 45;
            // 
            // s5DataGridViewTextBoxColumn
            // 
            this.s5DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.s5DataGridViewTextBoxColumn.DataPropertyName = "S5";
            this.s5DataGridViewTextBoxColumn.HeaderText = "S5";
            this.s5DataGridViewTextBoxColumn.Name = "s5DataGridViewTextBoxColumn";
            this.s5DataGridViewTextBoxColumn.Width = 45;
            // 
            // S6
            // 
            this.S6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.S6.DataPropertyName = "S6";
            this.S6.HeaderText = "S6";
            this.S6.Name = "S6";
            this.S6.Width = 45;
            // 
            // S7
            // 
            this.S7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.S7.DataPropertyName = "S7";
            this.S7.HeaderText = "S7";
            this.S7.Name = "S7";
            this.S7.Width = 45;
            // 
            // S8
            // 
            this.S8.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.S8.DataPropertyName = "S8";
            this.S8.HeaderText = "S8";
            this.S8.Name = "S8";
            this.S8.Width = 45;
            // 
            // S9
            // 
            this.S9.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.S9.DataPropertyName = "S9";
            this.S9.HeaderText = "S9";
            this.S9.Name = "S9";
            this.S9.Width = 45;
            // 
            // S10
            // 
            this.S10.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.S10.DataPropertyName = "S10";
            this.S10.HeaderText = "S10";
            this.S10.Name = "S10";
            this.S10.Width = 51;
            // 
            // S11
            // 
            this.S11.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.S11.DataPropertyName = "S11";
            this.S11.HeaderText = "S11";
            this.S11.Name = "S11";
            this.S11.Width = 51;
            // 
            // S12
            // 
            this.S12.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.S12.DataPropertyName = "S12";
            this.S12.HeaderText = "S12";
            this.S12.Name = "S12";
            this.S12.Width = 51;
            // 
            // S13
            // 
            this.S13.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.S13.DataPropertyName = "S13";
            this.S13.HeaderText = "S13";
            this.S13.Name = "S13";
            this.S13.Width = 51;
            // 
            // S14
            // 
            this.S14.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.S14.DataPropertyName = "S14";
            this.S14.HeaderText = "S14";
            this.S14.Name = "S14";
            this.S14.Width = 51;
            // 
            // S15
            // 
            this.S15.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.S15.DataPropertyName = "S15";
            this.S15.HeaderText = "S15";
            this.S15.Name = "S15";
            this.S15.Width = 51;
            // 
            // S16
            // 
            this.S16.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.S16.DataPropertyName = "S16";
            this.S16.HeaderText = "S16";
            this.S16.Name = "S16";
            this.S16.Width = 51;
            // 
            // S17
            // 
            this.S17.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.S17.DataPropertyName = "S17";
            this.S17.HeaderText = "S17";
            this.S17.Name = "S17";
            this.S17.Width = 51;
            // 
            // S18
            // 
            this.S18.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.S18.DataPropertyName = "S18";
            this.S18.HeaderText = "S18";
            this.S18.Name = "S18";
            this.S18.Width = 51;
            // 
            // S19
            // 
            this.S19.DataPropertyName = "S19";
            this.S19.HeaderText = "S19";
            this.S19.Name = "S19";
            // 
            // X
            // 
            this.X.DataPropertyName = "X";
            this.X.HeaderText = "X";
            this.X.Name = "X";
            this.X.ReadOnly = true;
            // 
            // Y
            // 
            this.Y.DataPropertyName = "Y";
            this.Y.HeaderText = "Y";
            this.Y.Name = "Y";
            // 
            // Z
            // 
            this.Z.DataPropertyName = "Z";
            this.Z.HeaderText = "Z";
            this.Z.Name = "Z";
            this.Z.ReadOnly = true;
            // 
            // MotionEdit_frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(708, 457);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.torq_list);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.servoStatus1);
            this.Controls.Add(this.base_chk);
            this.Controls.Add(this.all_pass_chk);
            this.Controls.Add(this.zV);
            this.Controls.Add(this.yV);
            this.Controls.Add(this.xV);
            this.Controls.Add(this.autopose);
            this.Controls.Add(this.delRow);
            this.Controls.Add(this.queryValues);
            this.Controls.Add(this.setBasic);
            this.Controls.Add(this.playRow);
            this.Controls.Add(this.updateRow);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.fnstring);
            this.Controls.Add(this.saveFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.debugFlag);
            this.Controls.Add(this.playAll);
            this.Controls.Add(this.record);
            this.Controls.Add(this.loadFile);
            this.Controls.Add(this.closeBtn);
            this.Name = "MotionEdit_frm";
            this.Text = "Motion editor";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.servoPoseDataBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button closeBtn;
        private System.Windows.Forms.Button saveFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox debugFlag;
        private System.Windows.Forms.Button playAll;
        private System.Windows.Forms.Button record;
        private System.Windows.Forms.Button loadFile;
        private System.Windows.Forms.Label fnstring;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource servoPoseDataBindingSource;
        private System.Windows.Forms.Button updateRow;
        private System.Windows.Forms.Button playRow;
        private System.Windows.Forms.Button setBasic;
        private System.Windows.Forms.Button queryValues;
        private System.Windows.Forms.Button delRow;
        private System.Windows.Forms.Button autopose;
        private System.Windows.Forms.Label xV;
        private System.Windows.Forms.Label yV;
        private System.Windows.Forms.Label zV;
        private System.Windows.Forms.CheckBox all_pass_chk;
        private System.Windows.Forms.CheckBox base_chk;
        private ServoStatus servoStatus1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox torq_list;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button close;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn timeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Steps;
        private System.Windows.Forms.DataGridViewTextBoxColumn s0DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn s1DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn s2DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn s3DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn s4DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn s5DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn S6;
        private System.Windows.Forms.DataGridViewTextBoxColumn S7;
        private System.Windows.Forms.DataGridViewTextBoxColumn S8;
        private System.Windows.Forms.DataGridViewTextBoxColumn S9;
        private System.Windows.Forms.DataGridViewTextBoxColumn S10;
        private System.Windows.Forms.DataGridViewTextBoxColumn S11;
        private System.Windows.Forms.DataGridViewTextBoxColumn S12;
        private System.Windows.Forms.DataGridViewTextBoxColumn S13;
        private System.Windows.Forms.DataGridViewTextBoxColumn S14;
        private System.Windows.Forms.DataGridViewTextBoxColumn S15;
        private System.Windows.Forms.DataGridViewTextBoxColumn S16;
        private System.Windows.Forms.DataGridViewTextBoxColumn S17;
        private System.Windows.Forms.DataGridViewTextBoxColumn S18;
        private System.Windows.Forms.DataGridViewTextBoxColumn S19;
        private System.Windows.Forms.DataGridViewTextBoxColumn X;
        private System.Windows.Forms.DataGridViewTextBoxColumn Y;
        private System.Windows.Forms.DataGridViewTextBoxColumn Z;
    }
}