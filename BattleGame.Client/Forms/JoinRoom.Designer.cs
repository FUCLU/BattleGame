namespace BattleGame.Client.Forms
{
    partial class JoinRoom
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JoinRoom));
            panel1 = new Panel();
            textBox1 = new TextBox();
            button2 = new Button();
            label5 = new Label();
            label4 = new Label();
            btnRefresh = new Button();
            btnBack = new Button();
            btnCreateRoom = new Button();
            label3 = new Label();
            txtPass = new TextBox();
            label2 = new Label();
            txtRoomName = new TextBox();
            label1 = new Label();
            flowLayoutPanelRooms = new FlowLayoutPanel();
            panelRoomTemplate = new Panel();
            picLock = new PictureBox();
            btnJoin = new Button();
            lblSlot = new Label();
            lblRoomCode = new Label();
            lblRoomName = new Label();
            button1 = new Button();
            panel1.SuspendLayout();
            panelRoomTemplate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLock).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.Transparent;
            panel1.BackgroundImage = (Image)resources.GetObject("panel1.BackgroundImage");
            panel1.BackgroundImageLayout = ImageLayout.Stretch;
            panel1.Controls.Add(textBox1);
            panel1.Controls.Add(button2);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(btnRefresh);
            panel1.Controls.Add(btnBack);
            panel1.Controls.Add(btnCreateRoom);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(txtPass);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(txtRoomName);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(46, 80);
            panel1.Name = "panel1";
            panel1.Size = new Size(359, 454);
            panel1.TabIndex = 0;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(147, 279);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(196, 27);
            textBox1.TabIndex = 9;
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button2.Location = new Point(19, 224);
            button2.Name = "button2";
            button2.Size = new Size(324, 35);
            button2.TabIndex = 8;
            button2.Text = "Choose map";
            button2.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.ForeColor = Color.Gold;
            label5.Location = new Point(19, 280);
            label5.Name = "label5";
            label5.Size = new Size(93, 23);
            label5.TabIndex = 7;
            label5.Text = "Time Limit:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.Gold;
            label4.Location = new Point(16, 197);
            label4.Name = "label4";
            label4.Size = new Size(121, 23);
            label4.TabIndex = 6;
            label4.Text = "Map selection:";
            // 
            // btnRefresh
            // 
            btnRefresh.BackgroundImage = (Image)resources.GetObject("btnRefresh.BackgroundImage");
            btnRefresh.BackgroundImageLayout = ImageLayout.Stretch;
            btnRefresh.FlatStyle = FlatStyle.Popup;
            btnRefresh.Location = new Point(181, 327);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(162, 49);
            btnRefresh.TabIndex = 2;
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnBack
            // 
            btnBack.BackColor = Color.Transparent;
            btnBack.BackgroundImage = (Image)resources.GetObject("btnBack.BackgroundImage");
            btnBack.BackgroundImageLayout = ImageLayout.Stretch;
            btnBack.FlatStyle = FlatStyle.Popup;
            btnBack.Location = new Point(95, 386);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(162, 54);
            btnBack.TabIndex = 2;
            btnBack.UseVisualStyleBackColor = false;
            btnBack.Click += btnBack_Click;
            // 
            // btnCreateRoom
            // 
            btnCreateRoom.BackgroundImage = (Image)resources.GetObject("btnCreateRoom.BackgroundImage");
            btnCreateRoom.BackgroundImageLayout = ImageLayout.Stretch;
            btnCreateRoom.FlatStyle = FlatStyle.Popup;
            btnCreateRoom.Location = new Point(16, 327);
            btnCreateRoom.Name = "btnCreateRoom";
            btnCreateRoom.Size = new Size(159, 48);
            btnCreateRoom.TabIndex = 5;
            btnCreateRoom.UseVisualStyleBackColor = true;
            btnCreateRoom.Click += btnCreateRoom_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.Gold;
            label3.Location = new Point(16, 127);
            label3.Name = "label3";
            label3.Size = new Size(86, 23);
            label3.TabIndex = 4;
            label3.Text = "Password:";
            // 
            // txtPass
            // 
            txtPass.Location = new Point(19, 157);
            txtPass.Name = "txtPass";
            txtPass.Size = new Size(324, 27);
            txtPass.TabIndex = 3;
            txtPass.UseSystemPasswordChar = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.Gold;
            label2.Location = new Point(16, 56);
            label2.Name = "label2";
            label2.Size = new Size(111, 23);
            label2.TabIndex = 2;
            label2.Text = "Room Name:";
            // 
            // txtRoomName
            // 
            txtRoomName.Location = new Point(19, 86);
            txtRoomName.Name = "txtRoomName";
            txtRoomName.Size = new Size(324, 27);
            txtRoomName.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Bookman Old Style", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.PaleGreen;
            label1.Location = new Point(66, 11);
            label1.Name = "label1";
            label1.Size = new Size(240, 32);
            label1.TabIndex = 0;
            label1.Text = "CREATE ROOM";
            // 
            // flowLayoutPanelRooms
            // 
            flowLayoutPanelRooms.AutoScroll = true;
            flowLayoutPanelRooms.BackColor = Color.Transparent;
            flowLayoutPanelRooms.BackgroundImage = (Image)resources.GetObject("flowLayoutPanelRooms.BackgroundImage");
            flowLayoutPanelRooms.BackgroundImageLayout = ImageLayout.Stretch;
            flowLayoutPanelRooms.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanelRooms.Location = new Point(418, 82);
            flowLayoutPanelRooms.Margin = new Padding(10);
            flowLayoutPanelRooms.Name = "flowLayoutPanelRooms";
            flowLayoutPanelRooms.Padding = new Padding(10, 10, 0, 10);
            flowLayoutPanelRooms.Size = new Size(667, 454);
            flowLayoutPanelRooms.TabIndex = 1;
            flowLayoutPanelRooms.WrapContents = false;
            // 
            // panelRoomTemplate
            // 
            panelRoomTemplate.BackColor = Color.SteelBlue;
            panelRoomTemplate.Controls.Add(picLock);
            panelRoomTemplate.Controls.Add(btnJoin);
            panelRoomTemplate.Controls.Add(lblSlot);
            panelRoomTemplate.Controls.Add(lblRoomCode);
            panelRoomTemplate.Controls.Add(lblRoomName);
            panelRoomTemplate.Location = new Point(226, 540);
            panelRoomTemplate.Name = "panelRoomTemplate";
            panelRoomTemplate.Size = new Size(619, 64);
            panelRoomTemplate.TabIndex = 0;
            panelRoomTemplate.Visible = false;
            // 
            // picLock
            // 
            picLock.Image = (Image)resources.GetObject("picLock.Image");
            picLock.Location = new Point(412, 9);
            picLock.Name = "picLock";
            picLock.Size = new Size(43, 40);
            picLock.SizeMode = PictureBoxSizeMode.StretchImage;
            picLock.TabIndex = 4;
            picLock.TabStop = false;
            // 
            // btnJoin
            // 
            btnJoin.BackColor = Color.DarkKhaki;
            btnJoin.Location = new Point(483, 9);
            btnJoin.Name = "btnJoin";
            btnJoin.Size = new Size(116, 46);
            btnJoin.TabIndex = 3;
            btnJoin.Text = "JOIN";
            btnJoin.UseVisualStyleBackColor = false;
            btnJoin.Click += btnJoin_Click_1;
            // 
            // lblSlot
            // 
            lblSlot.AutoSize = true;
            lblSlot.Font = new Font("Yu Gothic UI", 10.2F, FontStyle.Bold);
            lblSlot.ForeColor = Color.PeachPuff;
            lblSlot.Location = new Point(287, 20);
            lblSlot.Name = "lblSlot";
            lblSlot.Size = new Size(35, 23);
            lblSlot.TabIndex = 2;
            lblSlot.Text = "0/2";
            // 
            // lblRoomCode
            // 
            lblRoomCode.AutoSize = true;
            lblRoomCode.ForeColor = Color.Yellow;
            lblRoomCode.Location = new Point(31, 35);
            lblRoomCode.Name = "lblRoomCode";
            lblRoomCode.Size = new Size(87, 20);
            lblRoomCode.TabIndex = 1;
            lblRoomCode.Text = "Code: ------";
            // 
            // lblRoomName
            // 
            lblRoomName.AutoSize = true;
            lblRoomName.Font = new Font("Yu Gothic UI", 10.2F, FontStyle.Bold);
            lblRoomName.ForeColor = Color.PeachPuff;
            lblRoomName.Location = new Point(32, 9);
            lblRoomName.Name = "lblRoomName";
            lblRoomName.Size = new Size(104, 23);
            lblRoomName.TabIndex = 0;
            lblRoomName.Text = "Room name";
            // 
            // button1
            // 
            button1.BackgroundImage = (Image)resources.GetObject("button1.BackgroundImage");
            button1.BackgroundImageLayout = ImageLayout.Stretch;
            button1.FlatStyle = FlatStyle.Popup;
            button1.Location = new Point(877, 540);
            button1.Name = "button1";
            button1.Size = new Size(194, 52);
            button1.TabIndex = 2;
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // JoinRoom
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1128, 616);
            Controls.Add(panelRoomTemplate);
            Controls.Add(button1);
            Controls.Add(flowLayoutPanelRooms);
            Controls.Add(panel1);
            DoubleBuffered = true;
            Name = "JoinRoom";
            Text = "JoinRoom";
            Load += JoinRoom_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panelRoomTemplate.ResumeLayout(false);
            panelRoomTemplate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picLock).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private FlowLayoutPanel flowLayoutPanelRooms;
        private Button btnBack;
        private Label label2;
        private TextBox txtRoomName;
        private Label label1;
        private Label label3;
        private TextBox txtPass;
        private Button btnCreateRoom;
        private Panel panelRoomTemplate;
        private Button btnJoin;
        private Label lblSlot;
        private Label lblRoomCode;
        private Label lblRoomName;
        private PictureBox picLock;
        private Button btnRefresh;
        private Button button1;
        private Label label5;
        private Label label4;
        private Button button2;
        private TextBox textBox1;
    }
}