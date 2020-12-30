/*
 * 由SharpDevelop创建。
 * 用户： BDSNetRunner
 * 日期: 12/20/2020
 * 时间: 04:01
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Windows.Forms;

namespace LevelStatusUp
{
	/// <summary>
	/// Lore配置文件编辑框
	/// </summary>
	public partial class LevelStatusEditForm : Form
	{
		private OnBtCb cb;
		private LSConfig cfg;
		
		public LevelStatusEditForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
		}
		
		public void setDataAndListeners(LSConfig cdata, OnBtCb mcb) {
			cb = mcb;
			cfg = cdata;
			
			if (cfg != null) {
				try {
					maxattacktb.Text = "" + cfg.maxattack;
					maxhealthtb.Text = "" + cfg.maxhealth;
					maxknockrestb.Text = "" + cfg.maxknockres;
					maxleveltb.Text = "" + cfg.maxlevel;
					lvprotectmodecb.SelectedIndex = ((cfg.lvprotectmode != 1 && cfg.lvprotectmode != 2) ? 0 : cfg.lvprotectmode);
					tbviewsecond.Text = "" + cfg.viewsecond;
					tbstitle.Text = cfg.stitle;
					tbsnamebar.Text = cfg.snamebar;
					tbslevelbar.Text = cfg.slevelbar;
					tbshealthbar.Text = cfg.shealthbar;
					tbsattackbar.Text = cfg.sattackbar;
					tbsknockresbar.Text = cfg.sknockresbar;
					tbslefttip.Text = cfg.slefttip;
				} catch{}
			}
		}
		
		void BtcancelClick(object sender, EventArgs e)
		{
			if (cb != null) {
				if (cb.onBtCancel != null)
					cb.onBtCancel();
			}
		}
		void BtokClick(object sender, EventArgs e)
		{
			if (cb != null) {
				if (cb.onBtOk != null) {
					LSConfig od = null;
					try {
						od = new LSConfig();
						od.maxattack = float.Parse(maxattacktb.Text);
						od.maxhealth = float.Parse(maxhealthtb.Text);
						od.maxknockres = float.Parse(maxknockrestb.Text);
						od.maxlevel = int.Parse(maxleveltb.Text);
						od.lvprotectmode = lvprotectmodecb.SelectedIndex;
						od.viewsecond = int.Parse(tbviewsecond.Text);
						od.stitle = tbstitle.Text;
						od.snamebar = tbsnamebar.Text;
						od.slevelbar = tbslevelbar.Text;
						od.sattackbar = tbsattackbar.Text;
						od.shealthbar = tbshealthbar.Text;
						od.sknockresbar = tbsknockresbar.Text;
						od.slefttip = tbslefttip.Text;
					} catch(Exception){
						od = null;	// 数据组装存在异常时，本次编辑数据无效
					}
					cb.onBtOk(od);
				}
			}
		}
		/// <summary>
		/// 按钮回调
		/// </summary>
		public class OnBtCb {
			public delegate void ONBTOK(LSConfig data);
			public delegate void ONBTCANCEL();
			/// <summary>
			/// 按下确定按钮后回调
			/// </summary>
			public ONBTOK onBtOk;
			/// <summary>
			/// 按下取消按钮回调
			/// </summary>
			public ONBTCANCEL onBtCancel;
		}
	}
}
