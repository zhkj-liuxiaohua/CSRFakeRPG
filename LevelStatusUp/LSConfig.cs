/*
 * 由SharpDevelop创建。
 * 用户： classmates
 * 日期: 2020/12/27
 * 时间: 20:14
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace LevelStatusUp
{
	/// <summary>
	/// 配置类
	/// </summary>
	public class LSConfig
	{
		/// <summary>
			/// 最终临时攻击追加值
			/// </summary>
			public float maxattack = 10;
			/// <summary>
			/// 最终生命上限追加值
			/// </summary>
			public float maxhealth = 20;
			/// <summary>
			/// 最终抗击退概率
			/// </summary>
			public float maxknockres = 100;
			/// <summary>
			/// 满级等级
			/// </summary>
			public int maxlevel = 80;
			/// <summary>
			/// 死亡等级保护规则：0 - 直接清零，1 - 不掉级，2 - 保留一半等级
			/// </summary>
			public int lvprotectmode = 2;
			/// <summary>
			/// 侧边栏显示停留秒数，如果设置为-1则表示一直停留直到指令隐藏或玩家退出
			/// </summary>
			public int viewsecond = 60;
			/// <summary>
			/// 侧边栏标题
			/// </summary>
			public string stitle = "---- 状态栏 ----";
			/// <summary>
			/// 名字栏目显示项
			/// </summary>
			public string snamebar = "玩家名： §e";
			/// <summary>
			/// 等级栏目显示项
			/// </summary>
			public string slevelbar = "等  级： §e";
			/// <summary>
			/// 生命值栏目显示项
			/// </summary>
			public string shealthbar = "生命值： §e";
			/// <summary>
			/// 攻击力栏目显示项
			/// </summary>
			public string sattackbar = "攻击力： §e";
			/// <summary>
			/// 抗击飞栏目显示项
			/// </summary>
			public string sknockresbar = "抗击飞： §e";
			/// <summary>
			/// 结束显示提示语，{second}为替换项
			/// </summary>
			public string slefttip = "{second}秒后结束显示... ";
	}
}
