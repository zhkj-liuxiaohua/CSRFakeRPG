/*
 * 由SharpDevelop创建。
 * 用户： BDSNetRunner
 * 日期: 2021/1/5
 * 时间: 16:50
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CSR;
using System.Web.Script.Serialization;

namespace RandomSkillUp
{
	/// <summary>
	/// 技能处理器
	/// </summary>
	public class AtkSkillAdapter
	{
		public static Hashtable eventcbs = new Hashtable();
		
		MCCSAPI mapi;
		MCCSAPI.EventCab atk = null;
		ArrayList skilllist;
		readonly JavaScriptSerializer ser = new JavaScriptSerializer();
		Random r;
		
		public AtkSkillAdapter(MCCSAPI api, ArrayList skills)
		{
			mapi = api;
			skilllist = skills;
		}
		
		private KeyValuePair<int, int> [] makeRandList() {
			ArrayList al = new ArrayList(skilllist);
			ArrayList indexs = new ArrayList();
			ArrayList pers = new ArrayList();
			r = r ?? new Random();
			for(int l = skilllist.Count; l > 0; --l) {
				int d = r.Next(0, l);
				indexs.Add(al[d]);
				al.RemoveAt(d);
			}
			int per = 101;
			int i = 0, len = indexs.Count;
			// 前len-1项为随机值，最后一项为剩余点数
			for(; i < len - 1; i++) {
				int iper = r.Next(0, per);
				int index = skilllist.IndexOf(indexs[i]);
				pers.Add(new KeyValuePair<int, int>(index, iper));
				per -= iper;
			}
			per -= 1;
			per = (per < 0) ? 0 : per;
			pers.Add(new KeyValuePair<int, int>(skilllist.IndexOf(indexs[i]), per));
			return (KeyValuePair<int, int> [])pers.ToArray(typeof(KeyValuePair<int, int> []));
		}
		private int getSkillFromRandList() {
			var perlst = makeRandList();
			Random r = new Random();
			int nr = r.Next(0, 101);
			int index = -1;
			do {
				++index;
				nr -= perlst[index].Value;
			} while(nr > 0 && index < perlst.Length);
			index -= (index == perlst.Length ? 1 : 0);
			return index;
		}
		
		private bool addOne(ArrayList a, object b) {
			if (!a.Contains(b)) {
				a.Add(b);
				return true;
			}
			return false;
		}
		private bool addSome(ArrayList a, ArrayList b) {
			var r = false;
			for(int i = 0, l = b.Count; i < l; i++) {
				r |= addOne(a, b[i]);
			}
			return r;
		}
		private ArrayList selectTargets(IntPtr p, IntPtr a, Skill s) {
			ArrayList al = new ArrayList();
			int d = s.targettype;
			CsPlayer cp = new CsPlayer(mapi, p);
			CsActor ca = new CsActor(mapi, a);
			Vec3 va =  ser.Deserialize<Vec3>(ca.Position);
			Vec3 vp = ser.Deserialize<Vec3>(cp.Position);
			int area = s.targetarea;
			// 判断目标
			if ((d & 0x1) == 0x1) {		// 包含目标
				addOne(al, a);
			}
			if ((d & 0x2) == 0x2) {		// 包含自身
				addOne(al, p);
			}
			if ((d & 0x4) == 0x4) {		// 包含目标附近所有其它实体
				var lst = CsActor.getsFromAABB(mapi, ca.DimensionId, va.x - area, va.y - 1, va.z - area,
				                               va.x + area, va.y + 1, va.z + area);
				lst.Remove(a);
				addSome(al, lst);
			}
			if ((d & 0x8) == 0x8) {		// 包含玩家附近所有其它实体
				var lst = CsActor.getsFromAABB(mapi, cp.DimensionId, vp.x - area, vp.y - 1, vp.z - area,
				                               vp.x + area, vp.y + 1, vp.z + area);
				lst.Remove(a);
				addSome(al, lst);
			}
			if ((d & 0x10) == 0x10) {	// 包含目标附近所有其它玩家
				var lst = CsPlayer.getplFromAABB(mapi, ca.DimensionId, va.x - area, va.y - 1, va.z - area,
				                               va.x + area, va.y + 1, va.z + area);
				lst.Remove(p);
				addSome(al, lst);
			}
			if ((d & 0x20) == 0x20) {	// 包含玩家附近所有其它玩家
				var lst = CsPlayer.getplFromAABB(mapi, cp.DimensionId, vp.x - area, vp.y - 1, vp.z - area,
				                               vp.x + area, vp.y + 1, vp.z + area);
				lst.Remove(p);
				addSome(al, lst);
			}
			return al;
		}
		
		private object getRandOne(ArrayList ids) {
			if (ids != null && ids.Count > 0) {
				r = (r ?? new Random());
				var index = r.Next(0, ids.Count);
				return ids[index];
			}
			return null;
		}
		// 治愈
		private void recoverHealth(CsActor a, double count) {
			var n = a.Attributes;
			var mn = a.MaxAttributes;
			var dn = ser.Deserialize<Dictionary<string, object>>(n);
			var dmn = ser.Deserialize<Dictionary<string, object>>(mn);
			int nhealth = (int)(Convert.ToInt32(dn["health"]) - count);
			int maxhealth = Convert.ToInt32(dmn["maxhealth"]);
			nhealth = nhealth > maxhealth ? maxhealth : nhealth;
			a.Attributes = ser.Serialize(new { health = nhealth });
		}
		// 执行伤害处理
		private void doHurt(IntPtr p, IntPtr ap, float count) {
			bool isdamage = (count >= 0);
			CsActor a = new CsActor(mapi, ap);
			if ((a.TypeId & 0x100) == 0x100) {	// 是生物
			if (!isdamage)	// 反向伤害，治疗
				recoverHealth(a, count);
			else
				a.hurt(p, ActorDamageCause.EntityAttack, (int)count, true, false);
			}
		}
		// 执行效果处理
		private void doEffect(IntPtr p, int id, int am, int sec) {
			CsActor a = new CsActor(mapi, p);
			if ((a.TypeId & 0x100) == 0x100) {	// 是生物
				var str = a.Effects;
				Dictionary<string, object> effs = (string.IsNullOrEmpty(str) ? new Dictionary<string, object>() :
				                                   ser.Deserialize<Dictionary<string, object>>(str));
				LoreApi.addEffect(effs, id, am, sec);
				str = ser.Serialize(effs);
				a.Effects = str;
			}
		}
		
		/// <summary>
		/// 返回一个处理技能用的监听器
		/// </summary>
		/// <returns></returns>
		public MCCSAPI.EventCab makeListener() {
			if (atk == null) {
				atk = x => {
					var e = BaseEvent.getFrom(x) as AttackEvent;
					if (e != null) {
						int len = (skilllist != null ? skilllist.Count : 0);
						if (len > 0) {
							// 进入技能选择器
							var p = new CsPlayer(mapi, e.playerPtr);
							//int i = getSkillFromRandList();
							Skill s;
							var os = getRandOne(skilllist);
							s = os as Skill;
							//Skill s = getRandOne(skilllist) as Skill; //skilllist[i] as Skill;
							if (s != null) {
								// 处理技能效果
								if (s.targettype == 0)
									return true;
								ArrayList targets = selectTargets(e.playerPtr, e.attackedentityPtr, s);
								switch(s.hurttype) {
									case 0:		// 物理伤害
										{
											switch(s.basehurt) {
												case 0:		// 固定伤害
													{
														int count = s.hurtcount;
														bool isdamage = (count >= 0);
														foreach(IntPtr ap in targets) {
															doHurt(e.playerPtr, ap, count);
														}
													}
													break;
												case 1:		// 基础伤害，需要发起伤害监听
													{
														MCCSAPI.EventCab hurtlis = null;
														hurtlis = (hx) => {
															var he = BaseEvent.getFrom(hx) as MobHurtEvent;
															if (he != null) {
																if (he.mobPtr == e.attackedentityPtr) {
																	// 进入target循环检索
																	mapi.removeBeforeActListener(EventKey.onMobHurt, hurtlis);
																	bool ret = true;
																	int count = (int)((float)(he.dmcount) * (float)(s.hurtcount) / 100.0f);
																	//bool isdamage = (count >= 0);
																	foreach (IntPtr ap in targets) {
																		if (ap == he.mobPtr) {
																			ret = false;
																			new Thread(() => {
																				Thread.Sleep(20);	// 拦截旧伤害并延时发动对目标的新伤害
																				doHurt(e.playerPtr, ap, count);
																			}).Start();
																		} else {
																			doHurt(e.playerPtr, ap, count);
																		}
																	}
																	return ret;
																}
															}
															return true;
														};
														mapi.addBeforeActListener(EventKey.onMobHurt, hurtlis);
														new Thread(() => {				// 延时1秒移除本次伤害监听
															Thread.Sleep(1000);
															if (mapi.removeBeforeActListener(EventKey.onMobHurt, hurtlis)) {
															}
														}).Start();
													}
													break;
												case 2:		// 基于目标最大生命值百分比伤害
													{
														foreach(IntPtr ap in targets) {
															float count = (float)(s.hurtcount) / 100.0f;
															CsActor a = new CsActor(mapi, ap);
															var hel = ser.Deserialize<Dictionary<string, object>>(a.Health);
															count *= Convert.ToSingle(hel["max"]);
															doHurt(e.playerPtr, ap, count);
														}
													}
													break;
											}
										}
										break;
									case 1:		// 效果应用
										{
											object oid = getRandOne(s.effectids);
											if (oid != null) {
												int id = (int)oid;
												object oam = getRandOne(s.effectamplifiers);
												int am = (int)(oam ?? 0);
												foreach(IntPtr ap in targets) {
													doEffect(ap, id, am, s.effecttime);
												}
											}
										}
										break;
									case 2:		// 指令执行
										{
											foreach (string cmd in s.cmds) {
												mapi.runcmd(cmd);
											}
										}
										break;
									case 3:		// 自定义事件处理
										{
											foreach (string cbstr in s.eventcall) {
												ArrayList al = eventcbs[cbstr] as ArrayList;
												if (al != null || al.Count > 0) {
													bool ret = true;
													foreach (MCCSAPI.EventCab cb in al) {
														ret = ret && cb(x);
													}
													return ret;
												}
											}
										}
										break;
								}
							}
						}
					}
					return true;
				};
			}
			return atk;
		}
		
		public delegate bool ADDSKILLEVENTCB(string cbstr, MCCSAPI.EventCab cb);
		
		//-------- 静态方法区 --------//
		/// <summary>
		/// 增加一个技能处理关键字对应事件
		/// </summary>
		/// <param name="cbstr">技能自定义关键字</param>
		/// <param name="cb">事件回调处理函数</param>
		/// <returns>true -添加成功，false - 事件已存在</returns>
		public readonly static ADDSKILLEVENTCB addSkillEventCb = (string cbstr, MCCSAPI.EventCab cb) => {
			ArrayList al = eventcbs[cbstr] as ArrayList;
			al = (al ?? new ArrayList());
			if (al.Contains(cb))
				return false;
			al.Add(cb);
			eventcbs[cbstr] = al;
			return true;
		};
		/// <summary>
		/// 移除一个技能处理关键字对应事件
		/// </summary>
		/// <param name="cbstr">技能自定义关键字</param>
		/// <param name="cb">事件回调处理函数</param>
		/// <returns>是否移除成功</returns>
		public readonly static ADDSKILLEVENTCB removeSkillEventCb = (string cbstr, MCCSAPI.EventCab cb) => {
			ArrayList al = eventcbs[cbstr] as ArrayList;
			al = (al ?? new ArrayList());
			if (al.Contains(cb)) {
				al.Remove(cb);
				if (al.Count < 1)
					eventcbs.Remove(cbstr);
				else
					eventcbs[cbstr] = al;
				return true;
			}
			return false;
		};
	}
}
