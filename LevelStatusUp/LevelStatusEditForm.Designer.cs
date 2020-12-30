/*
 * 由SharpDevelop创建。
 * 用户： BDSNetRunner
 * 日期: 12/20/2020
 * 时间: 04:01
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
namespace LevelStatusUp
{
	partial class LevelStatusEditForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Button btok;
		private System.Windows.Forms.Button btcancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox maxhealthtb;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tbstitle;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tbviewsecond;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox tbslevelbar;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox tbsnamebar;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox tbshealthbar;
		private System.Windows.Forms.TextBox maxattacktb;
		private System.Windows.Forms.TextBox maxleveltb;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.ComboBox lvprotectmodecb;
		private System.Windows.Forms.TextBox tbsknockresbar;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox maxknockrestb;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox tbsattackbar;
		private System.Windows.Forms.TextBox tbslefttip;
		
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
			this.btok = new System.Windows.Forms.Button();
			this.btcancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.maxhealthtb = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.maxknockrestb = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tbslefttip = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.tbsknockresbar = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.tbsattackbar = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.tbshealthbar = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.tbslevelbar = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.tbsnamebar = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.tbstitle = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.tbviewsecond = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.maxattacktb = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.maxleveltb = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.lvprotectmodecb = new System.Windows.Forms.ComboBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btok
			// 
			this.btok.Location = new System.Drawing.Point(247, 518);
			this.btok.Name = "btok";
			this.btok.Size = new System.Drawing.Size(75, 23);
			this.btok.TabIndex = 0;
			this.btok.Text = "确定";
			this.btok.UseVisualStyleBackColor = true;
			this.btok.Click += new System.EventHandler(this.BtokClick);
			// 
			// btcancel
			// 
			this.btcancel.Location = new System.Drawing.Point(397, 518);
			this.btcancel.Name = "btcancel";
			this.btcancel.Size = new System.Drawing.Size(75, 23);
			this.btcancel.TabIndex = 0;
			this.btcancel.Text = "取消";
			this.btcancel.UseVisualStyleBackColor = true;
			this.btcancel.Click += new System.EventHandler(this.BtcancelClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(205, 23);
			this.label1.TabIndex = 1;
			this.label1.Text = "最大临时攻击力追加：";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(205, 23);
			this.label2.TabIndex = 1;
			this.label2.Text = "最终生命上限追加：";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// maxhealthtb
			// 
			this.maxhealthtb.Location = new System.Drawing.Point(223, 44);
			this.maxhealthtb.Name = "maxhealthtb";
			this.maxhealthtb.Size = new System.Drawing.Size(236, 25);
			this.maxhealthtb.TabIndex = 3;
			this.maxhealthtb.Text = "20";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 75);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(205, 23);
			this.label3.TabIndex = 1;
			this.label3.Text = "最终抗击退百分值：";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// maxknockrestb
			// 
			this.maxknockrestb.Location = new System.Drawing.Point(223, 75);
			this.maxknockrestb.Name = "maxknockrestb";
			this.maxknockrestb.Size = new System.Drawing.Size(236, 25);
			this.maxknockrestb.TabIndex = 3;
			this.maxknockrestb.Text = "100";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.tbslefttip);
			this.groupBox1.Controls.Add(this.label13);
			this.groupBox1.Controls.Add(this.tbsknockresbar);
			this.groupBox1.Controls.Add(this.label12);
			this.groupBox1.Controls.Add(this.tbsattackbar);
			this.groupBox1.Controls.Add(this.label11);
			this.groupBox1.Controls.Add(this.tbshealthbar);
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Controls.Add(this.tbslevelbar);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.tbsnamebar);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.tbstitle);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.tbviewsecond);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Location = new System.Drawing.Point(12, 178);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(758, 334);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "侧边栏信息提示";
			// 
			// tbslefttip
			// 
			this.tbslefttip.Location = new System.Drawing.Point(211, 281);
			this.tbslefttip.Name = "tbslefttip";
			this.tbslefttip.Size = new System.Drawing.Size(541, 25);
			this.tbslefttip.TabIndex = 1;
			this.tbslefttip.Text = "{second}秒后结束显示... ";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(30, 265);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(175, 52);
			this.label13.TabIndex = 0;
			this.label13.Text = "结束显示提示语，{second}为替换项：";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbsknockresbar
			// 
			this.tbsknockresbar.Location = new System.Drawing.Point(211, 232);
			this.tbsknockresbar.Name = "tbsknockresbar";
			this.tbsknockresbar.Size = new System.Drawing.Size(541, 25);
			this.tbsknockresbar.TabIndex = 1;
			this.tbsknockresbar.Text = "抗击飞： §e";
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(30, 234);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(175, 23);
			this.label12.TabIndex = 0;
			this.label12.Text = "抗击飞栏目显示项：";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tbsattackbar
			// 
			this.tbsattackbar.Location = new System.Drawing.Point(211, 201);
			this.tbsattackbar.Name = "tbsattackbar";
			this.tbsattackbar.Size = new System.Drawing.Size(541, 25);
			this.tbsattackbar.TabIndex = 1;
			this.tbsattackbar.Text = "攻击力： §e";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(30, 203);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(175, 23);
			this.label11.TabIndex = 0;
			this.label11.Text = "攻击力栏目显示项：";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tbshealthbar
			// 
			this.tbshealthbar.Location = new System.Drawing.Point(211, 170);
			this.tbshealthbar.Name = "tbshealthbar";
			this.tbshealthbar.Size = new System.Drawing.Size(541, 25);
			this.tbshealthbar.TabIndex = 1;
			this.tbshealthbar.Text = "生命值： §e";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(30, 172);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(175, 23);
			this.label9.TabIndex = 0;
			this.label9.Text = "生命值栏目显示项：";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tbslevelbar
			// 
			this.tbslevelbar.Location = new System.Drawing.Point(211, 139);
			this.tbslevelbar.Name = "tbslevelbar";
			this.tbslevelbar.Size = new System.Drawing.Size(541, 25);
			this.tbslevelbar.TabIndex = 1;
			this.tbslevelbar.Text = "等  级： §e";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(30, 142);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(175, 23);
			this.label7.TabIndex = 0;
			this.label7.Text = "等级栏目显示项：";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tbsnamebar
			// 
			this.tbsnamebar.Location = new System.Drawing.Point(211, 105);
			this.tbsnamebar.Name = "tbsnamebar";
			this.tbsnamebar.Size = new System.Drawing.Size(541, 25);
			this.tbsnamebar.TabIndex = 1;
			this.tbsnamebar.Text = "玩家名： §e";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(30, 108);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(175, 23);
			this.label6.TabIndex = 0;
			this.label6.Text = "名字栏目显示项：";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tbstitle
			// 
			this.tbstitle.Location = new System.Drawing.Point(211, 70);
			this.tbstitle.Name = "tbstitle";
			this.tbstitle.Size = new System.Drawing.Size(541, 25);
			this.tbstitle.TabIndex = 1;
			this.tbstitle.Text = "---- 状态栏 ----";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(30, 70);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(175, 23);
			this.label5.TabIndex = 0;
			this.label5.Text = "侧边栏标题：";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tbviewsecond
			// 
			this.tbviewsecond.Location = new System.Drawing.Point(211, 35);
			this.tbviewsecond.Name = "tbviewsecond";
			this.tbviewsecond.Size = new System.Drawing.Size(541, 25);
			this.tbviewsecond.TabIndex = 1;
			this.tbviewsecond.Text = "60";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(30, 20);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(175, 50);
			this.label4.TabIndex = 0;
			this.label4.Text = "显示停留秒数(负值则为常驻)：";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// maxattacktb
			// 
			this.maxattacktb.Location = new System.Drawing.Point(223, 13);
			this.maxattacktb.Name = "maxattacktb";
			this.maxattacktb.Size = new System.Drawing.Size(236, 25);
			this.maxattacktb.TabIndex = 3;
			this.maxattacktb.Text = "10";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(12, 108);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(205, 23);
			this.label8.TabIndex = 1;
			this.label8.Text = "满级属性等级：";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// maxleveltb
			// 
			this.maxleveltb.Location = new System.Drawing.Point(223, 104);
			this.maxleveltb.Name = "maxleveltb";
			this.maxleveltb.Size = new System.Drawing.Size(236, 25);
			this.maxleveltb.TabIndex = 3;
			this.maxleveltb.Text = "80";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(12, 140);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(205, 23);
			this.label10.TabIndex = 1;
			this.label10.Text = "死亡等级保护规则：";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lvprotectmodecb
			// 
			this.lvprotectmodecb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.lvprotectmodecb.Items.AddRange(new object[] {
			"0 - 死亡等级清零",
			"1 - 死亡等级完全保留",
			"2 - 死亡等级保留一半"});
			this.lvprotectmodecb.Location = new System.Drawing.Point(223, 140);
			this.lvprotectmodecb.Name = "lvprotectmodecb";
			this.lvprotectmodecb.Size = new System.Drawing.Size(236, 23);
			this.lvprotectmodecb.TabIndex = 5;
			// 
			// LevelStatusEditForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(782, 553);
			this.Controls.Add(this.lvprotectmodecb);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.maxleveltb);
			this.Controls.Add(this.maxknockrestb);
			this.Controls.Add(this.maxattacktb);
			this.Controls.Add(this.maxhealthtb);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btcancel);
			this.Controls.Add(this.btok);
			this.Name = "LevelStatusEditForm";
			this.Text = "LevelStatus设置窗口";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}
