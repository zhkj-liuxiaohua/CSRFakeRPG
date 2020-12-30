/*
 * 由SharpDevelop创建。
 * 用户： BDSNetRunner
 * 日期: 2020/12/26
 * 时间: 23:01
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CSR;
using System.Web.Script.Serialization;

namespace LevelStatusUp
{
	/// <summary>
	/// 等级提升
	/// </summary>
	static class Program
	{
		const string CONFIG_DIR = @"CSR\FakeRPG";
		const string CONFIG_PATH = @"CSR\FakeRPG\config.json";
		const string PLUGIN_NAME = "LevelStatusUp";
		const string USERDATA_DIR = @"CSR\FakeRPG\save";
		const string USERDATA_PATH = @"CSR\FakeRPG\save\_LEVEL_STATUS.json";
		
		static MCCSAPI mapi;
		
		static JavaScriptSerializer ser = new JavaScriptSerializer();
		
		// 登录信息
		static readonly Hashtable players = new Hashtable();
		// 玩家存活状态
		static readonly Hashtable alives = new Hashtable();
		
		static Dictionary<string, object> config = null;
		
		static Dictionary<string, object> mcstatus = new Dictionary<string, object>();

		static LevelStatusEditForm lform;
		
#region 玩家属性调整模块

		static LSConfig cfg = null;
		
		// 检查配置文件完整性
		static bool checkConfigOK() {
			bool ret = false;
			try {
				cfg = ser.Deserialize<LSConfig>(ser.Serialize(config[PLUGIN_NAME]));
				cfg.maxlevel = cfg.maxlevel < 1 ? 1 : cfg.maxlevel;
				ret = true;
			} catch{}
			return ret;
		}
		
		static bool loadconfig() {
			try {
				Directory.CreateDirectory(CONFIG_DIR);
				Directory.CreateDirectory(USERDATA_DIR);
				config = ser.Deserialize<Dictionary<string, object>>(File.ReadAllText(CONFIG_PATH));
			}catch{}
			if (config == null || config.Count < 1 || !checkConfigOK()) {
				Console.WriteLine("[LevelStatusUp] 配置文件读取失败，使用默认配置。");
				try {
					config = new Dictionary<string, object>();
					cfg = cfg ?? new LSConfig();
					config[PLUGIN_NAME] = cfg;
					Directory.CreateDirectory(CONFIG_DIR);
					File.WriteAllText(CONFIG_PATH, ser.Serialize(config));
				}catch{}
				return checkConfigOK();
			}
			return true;
		}
		
		// 初始化当前等级状态
		static Dictionary<string, object> formatStatus(int level) {
			var vx = new Dictionary<string, object>();
			vx["maxattr"] = new Dictionary<string, object>();
			var curlevel = level > cfg.maxlevel ? cfg.maxlevel : level;
			((Dictionary<string, object>)vx["maxattr"])["maxhealth"] = 20.0f + cfg.maxhealth * curlevel / cfg.maxlevel;
			vx["attr"] = new Dictionary<string, object>();
			((Dictionary<string, object>)vx["attr"])["attack_damage"] = 1 + (cfg.maxattack * curlevel / cfg.maxlevel);
			((Dictionary<string, object>)vx["attr"])["knockback_resistance"] = (cfg.maxknockres / 100) * curlevel / cfg.maxlevel;
			return vx;
		}
		// 试图重新加载文件
		static Dictionary<string, object> reloadStat(string xuid) {
			object os;
			Dictionary<string, object> s = null;
			if (mcstatus.TryGetValue(xuid, out os)) {
				s = os as Dictionary<string, object>;
			}
			string x = "";
			if (s == null) {
				try {
					x = File.ReadAllText(USERDATA_PATH);
				} catch{}
				if (!string.IsNullOrEmpty(x)) {
					try {
						mcstatus = ser.Deserialize<Dictionary<string, object>>(x);
						return (mcstatus[xuid] as Dictionary<string, object>);
					} catch{}
				}
			}
			return s;
		}
		
		// 取玩家当前状态
		static Dictionary<string, object> getCurStat(string uuid) {
			var strs = mapi.getPlayerAttributes(uuid);
			if (!string.IsNullOrEmpty(strs)) {
				try {
					var jstatus = ser.Deserialize<Dictionary<string, object>>(strs);
					return jstatus;
				}catch{}
			}
			return null;
		}
		
		// 读取玩家状态信息至缓存
		static void loadStatus(string xuid, string uuid, Dictionary<string, object> un) {
			var xn = reloadStat(xuid);
			if (xn == null) {
				// 无效信息，重新配置
				try {
					mcstatus[xuid] = new Dictionary<string, object>();
					var pl = un["level"];
					pl = pl ?? 0;
					var playerstat = mcstatus[xuid] as Dictionary<string, object>;
					playerstat["level"] = pl;
					var fstat = formatStatus(Convert.ToInt32(pl));
					playerstat["level"] = pl;
					playerstat["health"] = ((Dictionary<string, object>)fstat["maxattr"])["maxhealth"];
					playerstat["attack_damage"] = ((Dictionary<string, object>)fstat["attr"])["attack_damage"];
					playerstat["knockback_resistance"] = ((Dictionary<string, object>)fstat["attr"])["knockback_resistance"];
					mapi.setPlayerMaxAttributes(uuid, ser.Serialize(fstat["maxattr"]));
					mapi.setPlayerTempAttributes(uuid, ser.Serialize(mcstatus[xuid]));
					playerstat["dead"] = false;
					File.WriteAllText(USERDATA_PATH, ser.Serialize(mcstatus));
					
				}catch{}
			} else {
				try {
					var playerstat = mcstatus[xuid] as Dictionary<string, object>;
					var unlv = Convert.ToInt32(un["level"]);
					var mcslv = Convert.ToInt32(playerstat["level"]);
					var lvdown = (unlv < mcslv);
					if (unlv < 1 && (bool)(playerstat["dead"]) || (mcslv < unlv)) {				// 死后、附魔消耗至1级以下
						playerstat["level"] = Math.Max(mcslv, unlv);
					} else {
						playerstat["level"] = Math.Min(mcslv, unlv);
					}
					var fstat = formatStatus(Convert.ToInt32(playerstat["level"]));
					var ftmpstat = fstat["attr"] as Dictionary<string, object>;
					playerstat["attack_damage"] = ftmpstat["attack_damage"];
					playerstat["knockback_resistance"] = ftmpstat["knockback_resistance"];
					playerstat["dead"] = false;
					playerstat["health"] = !lvdown ? Math.Max(Convert.ToInt32(playerstat["health"]), Convert.ToInt32(un["health"])) :
						un["health"];
					var mcshe = Convert.ToInt32(un["health"]);
					if (mcshe < 1) {
						playerstat["health"] = ftmpstat["health"];			// 死后重载
						File.WriteAllText(USERDATA_PATH, ser.Serialize(mcstatus));
					}
					mapi.setPlayerMaxAttributes(uuid, ser.Serialize(fstat["maxattr"]));
					mapi.setPlayerTempAttributes(uuid, ser.Serialize(mcstatus[xuid]));
				} catch{}
			}
		}
		
		// 保存玩家状态信息
		static void saveStatus(string xuid, string uuid, bool dead){
			var strs = mapi.getPlayerAttributes(uuid);
			if (strs != null) {
				try {
					var jstatus = ser.Deserialize<Dictionary<string, object>>(strs);
					if (jstatus != null) {
						mcstatus[xuid] = new Dictionary<string, object>();	// 保存玩家当前属性
						var playerstat = mcstatus[xuid] as Dictionary<string, object>;
						int orglv = Convert.ToInt32(jstatus["level"]);
						orglv = (dead) ? ((cfg.lvprotectmode != 2 && cfg.lvprotectmode != 1) ? 0 :
								orglv / (cfg.lvprotectmode == 2 ? 2 : 1)) : orglv;
						playerstat["level"] = orglv;
						var fstat = formatStatus(Convert.ToInt32(playerstat["level"]));
						var ftmpstat = fstat["attr"] as Dictionary<string, object>;
						playerstat["health"] = dead ? ((Dictionary<string, object>)fstat["maxattr"])["maxhealth"] : jstatus["health"];
						playerstat["attack_damage"] = ftmpstat["attack_damage"];
						playerstat["knockback_resistance"] = ftmpstat["knockback_resistance"];
						playerstat["dead"] = dead;
						File.WriteAllText(USERDATA_PATH, ser.Serialize(mcstatus));
					}
				} catch{}
			}
		}
		
		// 初始化为0级状态
		static void resetStatus(IntPtr p) {
			var info = players[p] as LoadNameEvent;
			if (info != null) {
				var fstat = formatStatus(0);
				mapi.setPlayerMaxAttributes(info.uuid, ser.Serialize(fstat["maxattr"]));
				mapi.setPlayerTempAttributes(info.uuid, ser.Serialize(fstat["attr"]));
			}
		}
		
		// 校验状态，一般发生在攻击时和受伤时
		static void checkStatus(string xuid, string uuid) {
			var xn = reloadStat(xuid);
			var un = getCurStat(uuid);
			if (un == null) {
				//mapi.logout("read attr Err");
				return;
			}
			if (xn == null || (xn != null && (Convert.ToInt32(xn["level"]) != Convert.ToInt32(un["level"]) ||
			   (Convert.ToSingle(un["attack_damage"]) <= 1.0f && Convert.ToInt32(un["level"]) > 1)))) {	// 初始化、等级变化、攻击力未变化
				loadStatus(xuid, uuid, un);
			} else {
				((Dictionary<string, object>)mcstatus[xuid])["health"] = un["health"];
			}
		}
		
		// 事件内校验
		static void eventToCheck(IntPtr p) {
			var info = players[p] as LoadNameEvent;
			var xuid = info != null ? info.xuid : "";
			if (!string.IsNullOrEmpty(xuid)) {
				checkStatus(xuid, new CsPlayer(mapi, p).Uuid);
			}
		}
		
#endregion

#region 状态侧边栏模块
		
		// 状态任务列表
		static Hashtable statusTasks = new Hashtable();
		
		public class StatusThread {
			public bool flag = false;
			public int freeseconds;
			public IntPtr mp;
			string makeStatusList()
			{
				var info = players[mp] as LoadNameEvent;
				ArrayList al = new ArrayList();
				if (info != null) {
					try {
						var p = new CsPlayer(mapi, mp);
						var str = mapi.getPlayerAttributes(info.uuid);
						var name = p.getName();
						var data = ser.Deserialize<Dictionary<string, object>>(str);
						al.Add("  ");
						al.Add(string.Format("{0}{1} ", cfg.snamebar, name));
						al.Add(string.Format("{0}{1} ", cfg.slevelbar, Convert.ToInt32(data["level"])));
						al.Add(string.Format("{0}{1} ", cfg.shealthbar, Convert.ToInt32(data["health"])));
						al.Add(string.Format("{0}{1} ", cfg.sattackbar, Convert.ToInt32(data["attack_damage"])));
						al.Add(string.Format("{0}{1}％ ", cfg.sknockresbar,
							(int)(Convert.ToSingle(data["knockback_resistance"]) * 100.0)));
						if (cfg.viewsecond > -1) {
							al.Add(" ");
							al.Add(cfg.slefttip.Replace("{second}", "" + freeseconds));
						}
					} catch {}
				}
				return ser.Serialize(al);
			}
			/// <summary>
			/// 重置剩余秒数
			/// </summary>
			public void resetSeconds() {
				freeseconds = 60;
			}
			/// <summary>
			/// 开启一个倒计时显示状态侧边栏任务
			/// </summary>
			/// <param name="p">玩家指针</param>
			public void start(IntPtr p) {
				mp = p;
				flag = true;
				freeseconds = cfg.viewsecond;
				var t = new Thread(() => {
					while (flag) {
						// TODO 此处设置玩家侧边栏
						string title = cfg.stitle;
						string content = makeStatusList();
						try {
							var uuid = (players[mp] as LoadNameEvent).uuid;
							mapi.setPlayerSidebar(uuid, title, content);
						} catch (Exception) {
							Console.WriteLine("[LevelStatusUp] An Exception err, exit task.");
							return;	// 发生指针读取异常时，直接结束任务
						}
						Thread.Sleep(1000);
						freeseconds -= (cfg.viewsecond > -1 ? 1 : 0);
						if (cfg.viewsecond > -1 && freeseconds < 0) {
							flag = false;
							break;
						}
					}
					statusTasks.Remove(mp);
					var pdata = players[mp] as LoadNameEvent;
					if (pdata != null) {
						mapi.removePlayerSidebar(pdata.uuid);
					}
				});
				t.Start();
			}
			public void stop() {
				flag = false;
			}
		}
		
		// 开始一个状态信息显示任务，每隔1秒更新一次侧边栏，总计更新一分钟
		public static void startStatusTask(IntPtr p) {
			if (statusTasks[p] != null) {
				StatusThread rt = (StatusThread)statusTasks[p];
				rt.resetSeconds();
				return;
			}
			StatusThread r = new StatusThread();
			statusTasks[p] = r;
			r.start(p);
		}
		// 结束一个状态信息显示任务
		public static void stopStatusTask(IntPtr p) {
			if (statusTasks[p] != null) {
				StatusThread rt = (StatusThread)statusTasks[p];
				rt.stop();
				statusTasks.Remove(p);
			}
		}
#endregion

		// 初始化从此入口开始
		public static void init(MCCSAPI api) {
			mapi = api;
			if (!loadconfig()) {
				Console.WriteLine("[LevelStatusUp] 游戏配置文件载入失败，未能成功加载插件。");
				return;
			}
			// 玩家临死状态保存信息
			api.addBeforeActListener(EventKey.onMobDie, x => {
			                         	var e = BaseEvent.getFrom(x) as MobDieEvent;
			                         	if (e != null) {
			                         		if (!string.IsNullOrEmpty(e.playername)) {
			                         			var info = players[e.mobPtr] as LoadNameEvent;
			                        			var xuid = info != null ? info.xuid : "";
			                         			if (!string.IsNullOrEmpty(xuid)) {
			                        				alives[e.mobPtr] = false;
			                         				saveStatus(xuid, new CsPlayer(api, e.mobPtr).Uuid, true);
			                         			}
			                         		}
			                         	}
			                         	return true;
			});
			// 进入游戏监听，登记登录信息
			api.addAfterActListener(EventKey.onLoadName, x => {
			                        	var e = BaseEvent.getFrom(x) as LoadNameEvent;
			                        	if (e != null)
			                        		players[e.playerPtr] = e;
			                        	return true;
			                        });
			// 离开游戏监听，撤销登记信息，保存等级状态信息
			api.addBeforeActListener(EventKey.onPlayerLeft, x => {
			                         	var e = BaseEvent.getFrom(x) as PlayerLeftEvent;
			                         	if (e != null) {
			                         		if (!string.IsNullOrEmpty(e.xuid)) {
			                         			if (alives[e.playerPtr] != null && (bool)alives[e.playerPtr])
			                         				saveStatus(e.xuid, e.uuid, false);
			                         		}
			                         		players.Remove(e.playerPtr);
			                         		alives.Remove(e.playerPtr);
			                         		stopStatusTask(e.playerPtr);
			                         	}
			                         	return true;
			                         });
			// 玩家重生监听，重载读属性表至缓存
			api.addAfterActListener(EventKey.onRespawn, x => {
			                        	var e = BaseEvent.getFrom(x) as RespawnEvent;
			                        	if (e != null) {
			                        		if (cfg.lvprotectmode != 1 && cfg.lvprotectmode != 2) {
			                        			if (alives[e.playerPtr] != null && !(bool)alives[e.playerPtr]) {
			                        				resetStatus(e.playerPtr);
			                        			}
			                        		}
			                        		eventToCheck(e.playerPtr);
			                        		alives[e.playerPtr] = true;
			                        	}
			                        	return true;
			                        });
			// 攻击前检测，状态更新
			api.addBeforeActListener(EventKey.onAttack, x => {
			                         	var e = BaseEvent.getFrom(x) as AttackEvent;
			                         	if (e != null) {
			                         		eventToCheck(e.playerPtr);
			                         	}
			                         	return true;
			                         });
			// 受伤前检测，状态更新
			api.addBeforeActListener(EventKey.onMobHurt, x => {
			                         	var e = BaseEvent.getFrom(x) as MobHurtEvent;
			                         	if (e != null) {
			                         		eventToCheck(e.mobPtr);
			                         	}
			                         	return true;
			                         });
			// 切换装备检测，状态更新
			api.addAfterActListener(EventKey.onEquippedArmor, x => {
			                        	var e = BaseEvent.getFrom(x) as EquippedArmorEvent;
			                        	if (e != null) {
			                        		eventToCheck(e.playerPtr);
			                        	}
			                        	return true;
			                        });
			// 升级检测，状态更新
			api.addAfterActListener(EventKey.onLevelUp, x => {
			                        	var e = BaseEvent.getFrom(x) as LevelUpEvent;
			                        	if (e != null) {
			                        		eventToCheck(e.playerPtr);
			                        	}
			                        	return true;
			                        });
			// TODO 指令处理
			api.addBeforeActListener(EventKey.onInputCommand, x => {
				var e = BaseEvent.getFrom(x) as InputCommandEvent;
				if (e != null) {
					var mcmd = e.cmd.Trim();
					if (string.IsNullOrEmpty(mcmd))
						return false;
					if (mcmd == "/levelstatus") {
						// TODO 开启定时显属性侧边栏
						startStatusTask(e.playerPtr);
						return false;
					}
					if (mcmd == "/levelstatus off") {
						// TODO 关闭侧边栏显示
						stopStatusTask(e.playerPtr);
						CsPlayer pe = new CsPlayer(api, e.playerPtr);
						api.removePlayerSidebar(pe.Uuid);
						return false;
					}
				}
				return true;
			});
			api.addBeforeActListener(EventKey.onServerCmd, x => {
				var e = BaseEvent.getFrom(x) as ServerCmdEvent;
				if (e != null) {
					// TODO
					var mcmd = e.cmd.Trim();
					if (mcmd == "levelstatus") {
						api.logout("[LevelStatusUp] 用法（玩家）： /levelstatus [off]，（后台）：levelstatus [reload|edit]");
						return false;
					}
					if (mcmd.IndexOf("levelstatus ") == 0) {
						var para = mcmd.Substring(12);
						if (!string.IsNullOrEmpty(para)) {
							if (para.Trim().ToLower() == "reload") {
								// TODO 此处重载配置文件
								if (!loadconfig()) {
									api.logout("[LevelStatusUp] 配置文件读写失败，请检查插件是否配置正确。");
								} else {
									api.logout("[LevelStatusUp] 已成功重载配置文件。");
								}
							} else if (para.Trim().ToLower() == "edit") {
								api.logout("[LevelStatusUp] >> 即将进入windows窗口界面编辑环境 <<");
							new Thread(() => {
								try {
									if (lform == null) {
										lform = new LevelStatusEditForm();
										lform.setDataAndListeners(cfg, new LevelStatusEditForm.OnBtCb() {
											onBtOk = (data) => {
										                          		if (data == null) {
										                          			api.logout("[LevelStatusUp] 输入项异常，请重新编辑内容。");
										                          		} else {
										                          			try {
													Directory.CreateDirectory(CONFIG_DIR);
													config = ser.Deserialize<Dictionary<string, object>>(File.ReadAllText(CONFIG_PATH));
													config[PLUGIN_NAME] = data;
													File.WriteAllText(CONFIG_PATH, ser.Serialize(config));
												} catch {}
												if (loadconfig()) {
													api.logout("[LevelStatusUp] 配置文件已更新。");
												}
										                          		}
												lform.Close();
												lform = null;
											},
											onBtCancel = () => {
												api.logout("[LevelStatusUp] 已取消配置文件表单。");
												lform.Close();
												lform = null;
											}
										});
										lform.TopMost = true;
										System.Windows.Forms.Application.Run(lform);
									} else {
										if (!lform.Visible) {
											lform.Visible = true;
											lform.Show();
										}
									}	
								} catch {}
							}).Start();
							}
						}
						return false;
					}
				}
				return true;
			});
			api.setCommandDescribe("levelstatus", "显示一个状态信息");
			api.setCommandDescribe("levelstatus off", "隐藏状态信息");
			api.logout("[LevelStatusUp] 等级属性提升插件已加载。");
		}
	}
}

namespace CSR {
	partial class Plugin {
		#region 必要接口 onStart ，由用户实现
		public static void onStart(MCCSAPI api){
			if (api.COMMERCIAL) {
				LevelStatusUp.Program.init(api);
			}
			else
				Console.WriteLine("[LevelStatusUp] 暂不适用于社区版。");
		}
		#endregion
	}
}