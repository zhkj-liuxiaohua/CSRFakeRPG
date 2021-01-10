/*
 * 由SharpDevelop创建。
 * 用户： BDSNetRunner
 * 日期: 2020/12/29
 * 时间: 16:40
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections;

namespace RandomSkillUp
{
	public class Cfg_Tips {
		
		public string ok = "[提示] 您的装备已添加赋予效果成功。";
		
		public string error = "[出错] 装备赋予效果失败。请检查命令是否正确，所持物品是否符合要求。";
		
		public string costly = "[失败] 等级或余额不符最低要求或过于昂贵，添加赋予效果失败";
		
		public string exists = "[失败] 已存在赋予效果";
		
		public string nospace = "[失败] 背包已满";
		
	}
	public class Skill {
		/// <summary>
		/// 技能编号
		/// </summary>
		public int id;
		/// <summary>
		/// 技能名称
		/// </summary>
		public string name;
		/// <summary>
		/// 技能描述
		/// </summary>
		public string describe;
		/// <summary>
		/// 技能适用目标Flag，0x0 - 无目标，0x1 - 攻击目标，0x2 - 自身，0x4 - 攻击目标附近所有其它实体，<br/>
		/// 0x8 - 自身附近所有其它实体，0x10 - 攻击目标附近所有其它玩家，0x20 - 自身附近所有其它玩家
		/// </summary>
		public int targettype;
		/// <summary>
		/// 目标选定范围格数
		/// </summary>
		public int targetarea;
		/// <summary>
		/// 技能伤害类型，0 - 物理，1 - 效果，2 - 自定义命令发动，3 - 其它
		/// </summary>
		public int hurttype;
		/// <summary>
		/// 伤害基于类型，0 - 固定伤害, 1 - 基于基础伤害，2 - 基于目标最大生命值百分比伤害
		/// </summary>
		public int basehurt;
		/// <summary>
		/// 固定物理伤害数值，或基础伤害/生命百分比值
		/// </summary>
		public int hurtcount;
		/// <summary>
		/// 应用效果随机抽取列表
		/// </summary>
		public ArrayList effectids;
		/// <summary>
		/// 应用效果增幅强度随机抽取列表
		/// </summary>
		public ArrayList effectamplifiers;
		/// <summary>
		/// 应用效果持续时间
		/// </summary>
		public int effecttime;
		/// <summary>
		/// 自定义命令列表
		/// </summary>
		public ArrayList cmds;
		/// <summary>
		/// 其它事件驱动绑定名称（需程序自行实现相关接口）
		/// </summary>
		public ArrayList eventcall;
	}
	/// <summary>
	/// 配置类
	/// </summary>
	public class RSConfig
	{
		/// <summary>
		/// 随机技能防伪标识关键字
		/// </summary>
		public string skillKey = "SKILLCONFIRM";
		/// <summary>
		/// 随机技能防伪标识关键值
		/// </summary>
		public string skillvalue = "OK";
		/// <summary>
		/// 支付类型
		/// </summary>
		public string costtype = "level";
		/// <summary>
		/// 计分板名称
		/// </summary>
		public string costsbname = "money";
		/// <summary>
		/// 至少所持最低数值
		/// </summary>
		public int atleast = 30;
		/// <summary>
		/// 消耗数值
		/// </summary>
		public int count = 3;
		/// <summary>
		/// 注释显示值
		/// </summary>
		public string lorename = "§f赋予技能：\n§e◇量子力学技能◇";
		/// <summary>
		/// 提示语
		/// </summary>
		public Cfg_Tips tips = new Cfg_Tips();
		/// <summary>
		/// 随机技能应用列表
		/// </summary>
		public ArrayList skills = new ArrayList(){
				new Skill(){
					id = 0,
					name = "无效技能",
					describe = "无效果",
					targettype = 0x0
				},
				new Skill(){
					id = 1,
					name = "吸血",
					describe = "恢复目标最大生命值的20%",
					targettype = 0x2,
					hurttype = 0,
					basehurt = 2,
					hurtcount = -20
				},
				new Skill(){
					id = 2,
					name = "顺劈",
					describe = "对目标周围所有生物和玩家造成100%真实伤害",
					targettype = 0x4 | 0x10,
					targetarea = 2,
					hurttype = 0,
					basehurt = 1,
					hurtcount = 100
				},
				new Skill(){
					id = 3,
					name = "旋风",
					describe = "对玩家周围所有生物和玩家造成50%真实伤害",
					targettype = 0x8 | 0x20,
					targetarea = 2,
					hurttype = 0,
					basehurt = 1,
					hurtcount = 50
				},
				new Skill() {
					id = 4,
					name = "随机增益",
					describe = "对玩家自身造成一个持续时间为5秒的随机增益",
					targettype = 0x2,
					hurttype = 1,
					effectids = new ArrayList(){5,8,10,11,12,23},	//力量 跳跃提升 生命恢复 抗性提升 防火 饱和 
					effectamplifiers = new ArrayList(){0, 1, 2},		// 0~3级随机抽取增幅
					effecttime = 5
				},
				new Skill() {
					id = 5,
					name = "随机减益",
					describe = "对目标造成一个持续时间为5秒的随机减益",
					targettype = 0x1,
					hurttype = 1,
					effectids = new ArrayList(){2,18,19,20,25},		// 缓慢 虚弱 中毒 凋零 剧毒
					effectamplifiers = new ArrayList(){0, 1, 2},		// 0~3级增幅
					effecttime = 5
				}
			};
	}
}
