/*
 * 由SharpDevelop创建。
 * 用户： BDSNetRunner
 * 日期: 2020/12/29
 * 时间: 0:18
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using CSR;
using System.Web.Script.Serialization;


namespace RandomSkillUp
{
	/// <summary>
	/// 自定义武器赋予技能
	/// </summary>
	static class Program
	{
		const string CONFIG_DIR = @"plugins\FakeRPG";
		const string CONFIG_PATH = @"plugins\FakeRPG\rsconfig.json";
		const string PLUGIN_NAME = "RandomSkillUp";
		const string USERDATA_DIR = @"plugins\FakeRPG\save";
		const string USERDATA_PATH = @"plugins\FakeRPG\save\_RANDSKILL_MAXID.txt";
		
		static MCCSAPI mapi;
		static readonly JavaScriptSerializer ser = new JavaScriptSerializer();
		
		static RSConfig cfg = null;
		
		/// <summary>
		/// 防伪唯一自增长标识
		/// </summary>
		static int fakeMaxId = 0;
		/// <summary>
		/// 防伪自增长ID文件读写接口
		/// </summary>
		static int FakeCurId {
			get {
				int s = 0;
				try {
					var str = File.ReadAllText(USERDATA_PATH);
					s = int.Parse(str);
				} catch{}
				return s;
			}
			set {
				try {
					Directory.CreateDirectory(USERDATA_DIR);
					File.WriteAllText(USERDATA_PATH, "" + value);
				} catch{}
			}
		}
		
		// 检查是否可支付注释费用
		static bool checkCanpay(CsPlayer p) {
			bool can = false;
			switch(cfg.costtype) {
				case "level":
					try {
							var plvl = ser.Deserialize<Dictionary<string, object>>(mapi.getPlayerAttributes(p.Uuid));
							var curlvl = Convert.ToInt32(plvl["level"]);
							can = (curlvl >= cfg.atleast) && (curlvl - cfg.count >= 0);
					} catch{}
					break;
				case "scoreboard":
					try {
						int d = mapi.getscoreboard(p.Uuid, cfg.costsbname);
						can = (d >= cfg.atleast) && (d - cfg.count >= 0);
					}catch{}
					break;
			}
			return can;
		}
		// 支付等级或计分板数值
		static bool pay(CsPlayer p) {
			bool can = false;
			switch(cfg.costtype) {
				case "level":
					try {
						var plvl = ser.Deserialize<Dictionary<string, object>>(mapi.getPlayerAttributes(p.Uuid));
						can = (Convert.ToInt32(plvl["level"])) - cfg.count >= 0;
						if (can) {
							var lvl = new Dictionary<string, object>();
							lvl["level"] = (Convert.ToInt32(plvl["level"])) - cfg.count;
							return mapi.setPlayerTempAttributes(p.Uuid, ser.Serialize(lvl));
						}
					} catch {}
					break;
				case "scoreboard":
					try {
						int d = mapi.getscoreboard(p.Uuid, cfg.costsbname);
						int nd = d - cfg.count;
						can = nd >= 0;
						if (can) {
							return mapi.setscoreboard(p.Uuid, cfg.costsbname, nd);
						}
					}catch{}
					break;
			}
			return can;
		}
		
		static int encodeTagCurV(string k, string v, int id) {
			byte[] al = Encoding.ASCII.GetBytes(k);
			byte[] bl = Encoding.ASCII.GetBytes(v);
			var alen = al.Length;
			var blen = bl.Length;
			for(int i = 0, l = Math.Max(alen, blen); i < l; i++) {
				var a = al[i % alen];
				var b = bl[i % blen];
				id += (a ^ b);
			}
			return id;
		}
		static bool isFakeTagOk(string k, string v) {
			string [] vvs = v.Split(',');
			if (vvs.Length == 2) {
				try {
					var vv = vvs[1];
					var countselect = int.Parse(vvs[0]);
					var mincount = encodeTagCurV(k, vv, 0);
					var maxcount = encodeTagCurV(k, vv, fakeMaxId);
					if (countselect >= mincount && countselect <= maxcount)
						return true;
				} catch{}
			}
			return false;
		}
		
		/// <summary>
		/// 给玩家当前手持书本添加注释
		/// </summary>
		/// <param name="p">玩家类的实例</param>
		/// <returns></returns>
		static int skillbook(CsPlayer p)
		{
			// TODO 此处检查装备是否允许添加注释
			int retcode = 0;		// 返回值：0 - err，1 - ok，2 - too costly, 3 - already taged，4 - bag no space
			Dictionary<string, object> curitem = null;
			string uuid = p.Uuid;
			var curitemstr = mapi.getPlayerSelectedItem(uuid);
			try {
				curitem = ser.Deserialize<Dictionary<string, object>>(curitemstr);
			} catch {}
			if (curitem != null && curitem.Count > 0) {
				bool isitemok = false;
				try {
					var data = LoreApi.getOrAppand<Dictionary<string, object>>(curitem, "selecteditem", "", "");
					string nid = LoreApi.getItemType(data);
					isitemok = (nid == "minecraft:book");
				} catch {}
				if (!isitemok) {
					return 0;
				}
				// TODO 此处进行扣分检查操作
				if (!checkCanpay(p)) {
					return 2;
				}
				
				// TODO 此处进行是否已具有防伪标识检查操作
				isitemok = true;
				try {
					var data = LoreApi.getOrAppand<Dictionary<string, object>>(curitem, "selecteditem", "", "");
					var kv = LoreApi.getFakeTag(data);
					if (kv.Key == cfg.skillKey && isFakeTagOk(kv.Key, kv.Value))
						isitemok = false;
				} catch {}
				if (!isitemok)
					return 3;
				// TODO 此处进行背包空间检查
				isitemok = false;
				try {
					var data = ser.Deserialize<ArrayList>(p.InventoryContainer);
					foreach (Dictionary<string, object> i in data) {
						if (Convert.ToInt32(i["id"]) == 0) {
							isitemok = true;
							break;
						}
					}
				} catch {}
				if (!isitemok)
					return 4;
				// TODO 此处进行注释操作，成功后进行扣分操作
				bool loreok = false;
				// 局部操作
				{
					Dictionary<string, object> data = null, ndata = null;
					try {
						curitem = ser.Deserialize<Dictionary<string, object>>(curitemstr);
						data = LoreApi.getOrAppand<Dictionary<string, object>>(curitem, "selecteditem", "", "");
						ndata = ser.Deserialize<Dictionary<string, object>>(ser.Serialize(data));
						LoreApi.setCount(data, LoreApi.getCount(data) - 1);
						LoreApi.setCount(ndata, 1);
						var fakevalue = "" + encodeTagCurV(cfg.skillKey, cfg.skillvalue, fakeMaxId) + "," + cfg.skillvalue;
						loreok = LoreApi.setFakeTag(ndata, cfg.skillKey, fakevalue);
						if (loreok) {
							var lores = LoreApi.getLores(ndata);
							lores = (lores ?? new ArrayList());
							if (lores.Count < 1 || (lores.Count > 0 && (lores[0] as string) != cfg.lorename)) {
								lores.Insert(0, cfg.lorename);
								loreok = LoreApi.setLores(ndata, (string[])lores.ToArray(typeof(string)));
							}
						}
					} catch (Exception e) {Console.WriteLine(e.StackTrace);}
					if (loreok) {
						if (pay(p)) {
							int slot = Convert.ToInt32(curitem["selectedslot"]);
							if (resetItemToSlot(uuid, data, slot) && mapi.addPlayerItemEx(uuid, ser.Serialize(ndata))) {
								retcode = 1;
								++fakeMaxId;
								FakeCurId = fakeMaxId;
							}
						}
					}
				}
			}
			return retcode;
		}
		// 判断是否已赋予
		static bool isSkilled(Dictionary<string, object> item) {
			var data = LoreApi.getFakeTag(item);
			return data.Key == cfg.skillKey && isFakeTagOk(data.Key, data.Value);
		}
		// 确认或追加一个赋予信息
		static bool apandSkillLore(Dictionary<string, object> item) {
			var loreok = false;
			var lores = LoreApi.getLores(item);
			lores = (lores ?? new ArrayList());
			if (lores.Count < 1 || (lores.Count > 0 && (lores[0] as string) != cfg.lorename)) {
				lores.Insert(0, cfg.lorename);
				loreok = LoreApi.setLores(item, (string[])lores.ToArray(typeof(string)));
			}
			return loreok;
		}
		// 将物品重设入指定格内
		static bool resetItemToSlot(string uuid, Dictionary<string, object>item, int slot) {
			LoreApi.setSlot(item, slot);
			var nitmdata = new Dictionary<string, object>();
			var d = LoreApi.getOrAppand<ArrayList>(nitmdata, "tv", "tt", 9);
			d.Add(item);
			var nitms = new Dictionary<string, object>();
			nitms["Inventory"] = nitmdata;
			return mapi.setPlayerItems(uuid, ser.Serialize(nitms));
		}
		/// <summary>
		/// 移除玩家当前物品的随机技能
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		static int unskill(CsPlayer p) {
			int retcode = 0;
			var uuid = p.Uuid;
			try {
				var curitem = ser.Deserialize<Dictionary<string, object>>(mapi.getPlayerSelectedItem(uuid));
				var data = LoreApi.getOrAppand<Dictionary<string, object>>(curitem, "selecteditem", "", "");
				if (isSkilled(data)) {
					var lores = LoreApi.getLores(data);
					lores = (lores ?? new ArrayList());
					if (lores.Count > 0 && (lores[0] as string) == cfg.lorename) {
						lores.RemoveAt(0);
						LoreApi.setLores(data, (string[])lores.ToArray(typeof(string)));
					}
					LoreApi.removeFakeTag(data);
					int slot = Convert.ToInt32(curitem["selectedslot"]);
					if (resetItemToSlot(uuid, data, slot)) {
						retcode = 1;
					}
				}
			} catch {}
			return retcode;
		}
		
		// 根据返回码打印提示消息
		static void tips(CsPlayer p, int code) {
			switch(code) {
				case 0:
					mapi.sendText(p.Uuid, cfg.tips.error);
					break;
				case 1:
					mapi.sendText(p.Uuid, cfg.tips.ok);
					break;
				case 2:
					mapi.sendText(p.Uuid, cfg.tips.costly);
					break;
				case 3:
					mapi.sendText(p.Uuid, cfg.tips.exists);
					break;
				case 4:
					mapi.sendText(p.Uuid, cfg.tips.nospace);
					break;
				default:
					// do nothing
					break;
			}
		}
		
		static Hashtable useitemlock = new Hashtable();
		// 锁定使用物品监听1秒
		static bool lockUse(IntPtr p) {
			if (useitemlock[p] == null || !(bool)useitemlock[p]) {
				useitemlock[p] = true;
				new Thread(() => {
				           	Thread.Sleep(1000);
				           	useitemlock.Remove(p);
				           }).Start();
				return true;
			}
			return false;
		}
		// 消耗技能书并给武器赋予技能
		static int fakeTagSword(CsPlayer p, int bookslot, int swordslot) {
			int retcode = 0;
			try {
				var uuid = p.Uuid;
				var data = ser.Deserialize<Dictionary<string, object>>(mapi.getPlayerItems(uuid));
				var items = (data["Inventory"] as Dictionary<string, object>)["tv"] as ArrayList;
				var bookitem = items[bookslot] as Dictionary<string, object>;
				var sworditem = items[swordslot] as Dictionary<string, object>;
				if ((LoreApi.getItemType(bookitem) == "minecraft:book") && LoreApi.getItemType(sworditem).IndexOf("sword") >= 0) {
					if (isSkilled(bookitem)) {
						if (!isSkilled(sworditem)) {
							var kv = LoreApi.getFakeTag(bookitem);
							LoreApi.setCount(bookitem, 0);
							var loreok = LoreApi.setFakeTag(sworditem, kv.Key, kv.Value);
							if (loreok) {
								loreok = apandSkillLore(sworditem);
							}
							if (loreok) {
								var nitmdata = new Dictionary<string, object>();
								var d = LoreApi.getOrAppand<ArrayList>(nitmdata, "tv", "tt", 9);
								d.Add(bookitem);
								d.Add(sworditem);
								var nitms = new Dictionary<string, object>();
								nitms["Inventory"] = nitmdata;
								if (mapi.setPlayerItems(uuid, ser.Serialize(nitms))) {
									retcode = 1;
								}
							}
						} else {
							retcode = 3;
						}
					}
				}
			} catch{}
			return retcode;
		}
		// 本地技能事件处理
		static MCCSAPI.EventCab skillEventCb = null;
		
		static RSConfig loadconfig() {
			try {
				Directory.CreateDirectory(CONFIG_DIR);
				return ser.Deserialize<RSConfig>(File.ReadAllText(CONFIG_PATH));
			}catch{}
			return null;
		}
		
		static Hashtable itemnames = new Hashtable();
		// 反查物品id-标识符对应表
		public static string getItemRName(int id) {
			string rname = itemnames[id] as string;
			if (string.IsNullOrEmpty(rname)) {
				rname = mapi.getItemRawname(id);
				itemnames[id] = rname;
				return rname;
			}
			return rname;
		}
		
		readonly static ArrayList SWORDSLIST = new ArrayList(new []{"wooden_sword", "stone_sword", "iron_sword",
			"golden_sword", "diamond_sword", "netherite_sword"});
		readonly static ArrayList TXTSWORDS = new ArrayList(new []{"木剑", "石剑", "铁剑", "金剑", "钻石剑", "合金剑"});
		
		// 主程序入口从此进入
		public static void init(MCCSAPI api){
			fakeMaxId = FakeCurId;
			mapi = api;
			cfg = loadconfig();
			if (cfg == null) {
				Console.WriteLine("[RandomSkillUp] 配置文件读取失败，使用预设配置项。");
				cfg = new RSConfig();
				try {
					File.WriteAllText(CONFIG_PATH, ser.Serialize(cfg));
				} catch {}
			}
			// 指令添加书本赋予及指令删除赋予
			api.addBeforeActListener(EventKey.onInputCommand, x => {
			                         	var e = BaseEvent.getFrom(x) as InputCommandEvent;
			                         	if (e != null) {
			                         		var p = new CsPlayer(api, e.playerPtr);
			                         		var uuid = p.Uuid;
			                         		var cmd = e.cmd.Trim();
			                         		if (cmd == "/randskill") {
			                         			try {
			                         				tips(p, skillbook(p));
			                         			} catch(Exception err){Console.WriteLine(err.StackTrace);}
			                         			return false;
			                         		} else if (cmd == "/randskill off") {
			                         			try {
			                         				tips(p, unskill(p));
			                         			} catch(Exception err){Console.WriteLine(err.StackTrace);}
			                         			return false;
			                         		}
			                         	}
			                         	return true;
			                         });
			// 使用赋予效果书本打开gui界面
			api.addBeforeActListener(EventKey.onUseItem, x => {
			                         	var e = BaseEvent.getFrom(x) as UseItemEvent;
			                         	if (e != null) {
			                         		if (getItemRName(e.itemid) == "book") {		 // 此ID可能会变动
			                         			var p = new CsPlayer(api, e.playerPtr);
			                         			var uuid = p.Uuid;
			                         			// 检查书本是否是赋予之书
			                         			var tagged = false;
			                         			var curslot = -1;
			                         			try {
			                         				var curitem = ser.Deserialize<Dictionary<string, object>>(api.getPlayerSelectedItem(uuid));
			                         				var data = LoreApi.getOrAppand<Dictionary<string, object>>(curitem, "selecteditem", "", "");
			                         				curslot = Convert.ToInt32(curitem["selectedslot"]);
													tagged |= isSkilled(data);
			                         			} catch{}
			                         			if (tagged) {
			                         				if (lockUse(e.playerPtr)) {
			                         					// 弹窗GUI
			                         					var bts = new ArrayList();
			                         					var slots = new ArrayList();
			                         					try {
			                         						var hotsitm = p.HotbarContainer;
			                         						var hotitems = ser.Deserialize<ArrayList>(p.HotbarContainer);
			                         						for(int i = 0; i < hotitems.Count; i++) {
			                         							var d = hotitems[i] as Dictionary<string, object>;
			                         							if (d != null) {
			                         								object orawname;
			                         								string rawname = "";
			                         								if (d.TryGetValue("rawnameid", out orawname)) {
			                         									rawname = orawname as string;
			                         								}
			                         								if (!string.IsNullOrEmpty(rawname)) {
			                         									int index = SWORDSLIST.IndexOf(rawname);
			                         									if (index >= 0) {
			                         										var slot = Convert.ToInt32(d["Slot"]);
			                         										var bt = string.Format("{0}、 {1}({2})", slot, d["item"] as string,
			                         										                       TXTSWORDS[index]);
			                         										bts.Add(bt);
			                         										slots.Add(slot);
			                         									}
			                         								}
			                         							}
			                         						}
			                         					} catch{}
			                         					var gui = new SimpleGUI(api, uuid, "武器赋予", "请选择需要赋予的武器：",
			                         					                        bts);
			                         					gui.send(30000, selected => {
			                         					         	if (selected == "null")
			                         					         		return;
			                         					         	try {
			                         					         		var i = int.Parse(selected);
			                         					         		if (i < bts.Count) {
			                         					         			var slot = (int)slots[i];
			                         					         			tips(p, fakeTagSword(p, curslot, slot));
			                         					         		}
			                         					         	}catch{}
			                         					         },
			                         					         () => {});
			                         					return false;
			                         				}
			                         			}
			                         		}
			                         	}
			                         	return true;
			                         });
			// 当玩家切换手头装备为剑系武器时，自动添加lore
			api.addAfterActListener(EventKey.onEquippedArmor, x => {
			                        	var e = BaseEvent.getFrom(x) as EquippedArmorEvent;
			                        	if (e != null) {
			                        		if (e.slottype == 1) {
			                        			if (e.slot == 0) {
			                        				var p = new CsPlayer(api, e.playerPtr);
			                        				var uuid = p.Uuid;
			                        				var id = e.itemid;
			                        				// 剑，ID可能会变化
			                        				if (SWORDSLIST.IndexOf(getItemRName(id)) >= 0) {
			                        					var str = api.getPlayerSelectedItem(uuid);
			                        					try {
			                        						var curitem = ser.Deserialize<Dictionary<string, object>>(str);
			                        						var item = LoreApi.getOrAppand<Dictionary<string, object>>(curitem, "selecteditem", "", "");
			                        						if (isSkilled(item)) {
			                        							if (apandSkillLore(item))
				                        							resetItemToSlot(uuid, item, Convert.ToInt32(curitem["selectedslot"]));
			                        						}
			                        					} catch{}
			                        				}
			                        			}
			                        		}
			                        	}
			                        	return true;
			                         });
			// TODO 技能处理
			api.addBeforeActListener(EventKey.onAttack, x => {
				var e = BaseEvent.getFrom(x) as AttackEvent;
				if (e != null) {
					var p = new CsPlayer(api, e.playerPtr);
					var uuid = p.Uuid;
					var hand = ser.Deserialize<ArrayList>(p.HandContainer);
					if (hand != null && hand.Count > 0) {
						var mainhand = hand[0] as Dictionary<string, object>;
						if (mainhand != null) {
							object oid;
							if (mainhand.TryGetValue("id", out oid)) {
								int id = Convert.ToInt32(oid);		// 剑，ID可能会变化
								if (SWORDSLIST.IndexOf(getItemRName(id)) >= 0) {
									var str = api.getPlayerSelectedItem(uuid);
									try {
										var curitem = ser.Deserialize<Dictionary<string, object>>(str);
										var item = LoreApi.getOrAppand<Dictionary<string, object>>(curitem, "selecteditem", "", "");
										if (isSkilled(item)) {
											skillEventCb = (skillEventCb ?? new AtkSkillAdapter(api, cfg.skills).makeListener());
											return skillEventCb(x);
										}
									} catch {}
								}
							}
						}
					}
				}
				return true;
			});
			api.setSharePtr("addSkillEventCb", Marshal.GetFunctionPointerForDelegate(AtkSkillAdapter.addSkillEventCb));
			api.setSharePtr("removeSkillEventCb", Marshal.GetFunctionPointerForDelegate(AtkSkillAdapter.removeSkillEventCb));
			api.setCommandDescribe("randskill", "给予手持书本一个随机赋予");
			api.setCommandDescribe("randskill off", "移除手持物品的随机赋予技能");
			Console.WriteLine("[RandomSkillUp] 随机武器技能赋予插件已加载。用法：手持书本，输入/randskill添加书本的赋予；\n" +
			                  "\t使用赋予之书右键点地打开武器赋予选择界面；\n\t手持赋予之书或赋予之剑，输入/randskill off移除武器赋予。");
		}
	}
}

namespace CSR{
	partial class Plugin{
		public static void onStart(MCCSAPI api){
			if (api.COMMERCIAL) {
				RandomSkillUp.Program.init(api);
			} else {
				Console.WriteLine("[RandomSkillUp] 暂不支持社区版。");
			}
		}
	}
}