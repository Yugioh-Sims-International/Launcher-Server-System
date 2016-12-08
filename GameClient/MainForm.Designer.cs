﻿/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/7
 * 时间: 20:31
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
namespace GameClient
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.btn_single = new System.Windows.Forms.Button();
            this.btn_match = new System.Windows.Forms.Button();
            this.tbn_tag = new System.Windows.Forms.Button();
            this.rb_allmsg = new System.Windows.Forms.RichTextBox();
            this.btn_send = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_other = new System.Windows.Forms.Button();
            this.btn_clean = new System.Windows.Forms.Button();
            this.chb_closemsg = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chb_nonane = new System.Windows.Forms.CheckBox();
            this.btn_join = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuitem_chat = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitem_join = new System.Windows.Forms.ToolStripMenuItem();
            this.rb_msg = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_editdeck = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_replay = new System.Windows.Forms.Button();
            this.btn_logout = new System.Windows.Forms.Button();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_single
            // 
            this.btn_single.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_single.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_single.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_single.Location = new System.Drawing.Point(6, 22);
            this.btn_single.Name = "btn_single";
            this.btn_single.Size = new System.Drawing.Size(90, 56);
            this.btn_single.TabIndex = 0;
            this.btn_single.Text = "Quick Single";
            this.btn_single.UseVisualStyleBackColor = true;
            this.btn_single.Click += new System.EventHandler(this.Btn_singleClick);
            // 
            // btn_match
            // 
            this.btn_match.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_match.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_match.Location = new System.Drawing.Point(104, 21);
            this.btn_match.Name = "btn_match";
            this.btn_match.Size = new System.Drawing.Size(90, 56);
            this.btn_match.TabIndex = 0;
            this.btn_match.Text = "Quick Match";
            this.btn_match.UseVisualStyleBackColor = true;
            this.btn_match.Click += new System.EventHandler(this.Btn_matchClick);
            // 
            // tbn_tag
            // 
            this.tbn_tag.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.tbn_tag.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbn_tag.Location = new System.Drawing.Point(200, 21);
            this.tbn_tag.Name = "tbn_tag";
            this.tbn_tag.Size = new System.Drawing.Size(90, 56);
            this.tbn_tag.TabIndex = 0;
            this.tbn_tag.Text = "Quick Tag";
            this.tbn_tag.UseVisualStyleBackColor = true;
            this.tbn_tag.Click += new System.EventHandler(this.Tbn_tagClick);
            // 
            // rb_allmsg
            // 
            this.rb_allmsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rb_allmsg.BackColor = System.Drawing.SystemColors.Window;
            this.rb_allmsg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rb_allmsg.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rb_allmsg.Location = new System.Drawing.Point(8, 581);
            this.rb_allmsg.Name = "rb_allmsg";
            this.rb_allmsg.ReadOnly = true;
            this.rb_allmsg.Size = new System.Drawing.Size(678, 233);
            this.rb_allmsg.TabIndex = 2;
            this.rb_allmsg.Text = "";
            // 
            // btn_send
            // 
            this.btn_send.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_send.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_send.Location = new System.Drawing.Point(990, 772);
            this.btn_send.Name = "btn_send";
            this.btn_send.Size = new System.Drawing.Size(90, 43);
            this.btn_send.TabIndex = 4;
            this.btn_send.Text = "Send Message";
            this.btn_send.UseVisualStyleBackColor = true;
            this.btn_send.Click += new System.EventHandler(this.Btn_Send_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btn_single);
            this.groupBox1.Controls.Add(this.btn_match);
            this.groupBox1.Controls.Add(this.tbn_tag);
            this.groupBox1.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(686, 587);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(297, 87);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Quick Join/Create";
            // 
            // btn_other
            // 
            this.btn_other.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_other.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_other.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_other.Location = new System.Drawing.Point(987, 608);
            this.btn_other.Name = "btn_other";
            this.btn_other.Size = new System.Drawing.Size(90, 56);
            this.btn_other.TabIndex = 0;
            this.btn_other.Text = "Host";
            this.btn_other.UseVisualStyleBackColor = true;
            this.btn_other.Click += new System.EventHandler(this.Btn_otherClick);
            // 
            // btn_clean
            // 
            this.btn_clean.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_clean.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_clean.Location = new System.Drawing.Point(990, 736);
            this.btn_clean.Name = "btn_clean";
            this.btn_clean.Size = new System.Drawing.Size(90, 30);
            this.btn_clean.TabIndex = 4;
            this.btn_clean.Text = "Clear Messages";
            this.btn_clean.UseVisualStyleBackColor = true;
            this.btn_clean.Click += new System.EventHandler(this.Btn_Clean_Click);
            // 
            // chb_closemsg
            // 
            this.chb_closemsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chb_closemsg.AutoSize = true;
            this.chb_closemsg.Location = new System.Drawing.Point(692, 749);
            this.chb_closemsg.Name = "chb_closemsg";
            this.chb_closemsg.Size = new System.Drawing.Size(118, 17);
            this.chb_closemsg.TabIndex = 5;
            this.chb_closemsg.Text = "Block All Messages";
            this.chb_closemsg.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(12, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 19);
            this.label2.TabIndex = 7;
            this.label2.Text = "GameList";
            // 
            // chb_nonane
            // 
            this.chb_nonane.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chb_nonane.AutoSize = true;
            this.chb_nonane.Location = new System.Drawing.Point(816, 749);
            this.chb_nonane.Name = "chb_nonane";
            this.chb_nonane.Size = new System.Drawing.Size(88, 17);
            this.chb_nonane.TabIndex = 5;
            this.chb_nonane.Text = "Chinese Text";
            this.chb_nonane.UseVisualStyleBackColor = true;
            // 
            // btn_join
            // 
            this.btn_join.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_join.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_join.Location = new System.Drawing.Point(990, 676);
            this.btn_join.Name = "btn_join";
            this.btn_join.Size = new System.Drawing.Size(87, 40);
            this.btn_join.TabIndex = 9;
            this.btn_join.Text = "Join Room";
            this.btn_join.UseVisualStyleBackColor = true;
            this.btn_join.Click += new System.EventHandler(this.Btn_joinClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuitem_chat,
            this.menuitem_join});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(123, 48);
            // 
            // menuitem_chat
            // 
            this.menuitem_chat.Name = "menuitem_chat";
            this.menuitem_chat.Size = new System.Drawing.Size(122, 22);
            this.menuitem_chat.Text = "Whisper";
            this.menuitem_chat.Click += new System.EventHandler(this.Menuitem_chatClick);
            // 
            // menuitem_join
            // 
            this.menuitem_join.Name = "menuitem_join";
            this.menuitem_join.Size = new System.Drawing.Size(122, 22);
            this.menuitem_join.Text = "Join Game";
            this.menuitem_join.Click += new System.EventHandler(this.Menuitem_joinClick);
            // 
            // rb_msg
            // 
            this.rb_msg.AcceptsReturn = true;
            this.rb_msg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.rb_msg.Location = new System.Drawing.Point(692, 772);
            this.rb_msg.Multiline = true;
            this.rb_msg.Name = "rb_msg";
            this.rb_msg.Size = new System.Drawing.Size(291, 43);
            this.rb_msg.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(167, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 14;
            this.label1.Visible = false;
            // 
            // btn_editdeck
            // 
            this.btn_editdeck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_editdeck.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_editdeck.Location = new System.Drawing.Point(692, 676);
            this.btn_editdeck.Name = "btn_editdeck";
            this.btn_editdeck.Size = new System.Drawing.Size(90, 37);
            this.btn_editdeck.TabIndex = 4;
            this.btn_editdeck.Text = "Deck Editor";
            this.btn_editdeck.UseVisualStyleBackColor = true;
            this.btn_editdeck.Click += new System.EventHandler(this.Btn_EditDeck_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(850, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(163, 19);
            this.label4.TabIndex = 7;
            this.label4.Text = "Online Players";
            // 
            // btn_replay
            // 
            this.btn_replay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_replay.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_replay.Location = new System.Drawing.Point(790, 677);
            this.btn_replay.Name = "btn_replay";
            this.btn_replay.Size = new System.Drawing.Size(90, 37);
            this.btn_replay.TabIndex = 4;
            this.btn_replay.Text = "View Replay";
            this.btn_replay.UseVisualStyleBackColor = true;
            this.btn_replay.Click += new System.EventHandler(this.Btn_Replay_Click);
            // 
            // btn_logout
            // 
            this.btn_logout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_logout.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_logout.Location = new System.Drawing.Point(887, 678);
            this.btn_logout.Name = "btn_logout";
            this.btn_logout.Size = new System.Drawing.Size(90, 37);
            this.btn_logout.TabIndex = 4;
            this.btn_logout.Text = "Logout";
            this.btn_logout.UseVisualStyleBackColor = true;
            this.btn_logout.Click += new System.EventHandler(this.Btn_Logout_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 194;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 824);
            this.Controls.Add(this.btn_editdeck);
            this.Controls.Add(this.btn_logout);
            this.Controls.Add(this.btn_replay);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rb_msg);
            this.Controls.Add(this.btn_join);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chb_nonane);
            this.Controls.Add(this.chb_closemsg);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_other);
            this.Controls.Add(this.btn_clean);
            this.Controls.Add(this.btn_send);
            this.Controls.Add(this.rb_allmsg);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ygopro International";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFormFormClosed);
            this.Load += new System.EventHandler(this.MainFormLoad);
            this.groupBox1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.ToolStripMenuItem menuitem_join;
		private System.Windows.Forms.ToolStripMenuItem menuitem_chat;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.DListView lv_user;
		private System.Windows.Forms.Button btn_join;
		private System.Windows.Forms.CheckBox chb_nonane;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btn_other;
		private System.Windows.Forms.CheckBox chb_closemsg;
		private System.Windows.Forms.Button btn_clean;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btn_send;
		private System.Windows.Forms.TextBox rb_msg;
		private System.Windows.Forms.RichTextBox rb_allmsg;
		private System.Windows.Forms.RoomGrid panel_rooms;
		private System.Windows.Forms.Button tbn_tag;
		private System.Windows.Forms.Button btn_match;
		private System.Windows.Forms.Button btn_single;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_editdeck;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_replay;
        private System.Windows.Forms.Button btn_logout;
    }
}
