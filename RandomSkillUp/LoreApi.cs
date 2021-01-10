/*
 * 由SharpDevelop创建。
 * 用户： BDSNetRunner
 * 日期: 2020/12/8
 * 时间: 11:06
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace RandomSkillUp
{
	/// <summary>
	/// 提供Lore对于nbt编辑的调用
	/// </summary>
	public static class LoreApi
	{
		/// <summary>
		/// 获取或添加一个value
		/// </summary>
		/// <param name="d">nbt</param>
		/// <param name="k">待读取或追加的key值，可能是 tv 或 cv，其它key则无需追加填充左侧值</param>
		/// <param name="leftk">追加时填充的左侧key值，可能是 tt 或 ck，空白则不填充此值</param>
		/// <param name="leftv">左侧value值</param>
		/// <returns></returns>
		public static T getOrAppand<T>(Dictionary<string, object> d, string k, string leftk, object leftv) where T : class, new() {
			if (d != null) {
				object o;
				if (d.TryGetValue(k, out o)) {
					return o as T;
				} else {
					if (!string.IsNullOrEmpty(leftk))
						d[leftk] = leftv;
					var on = new T();
					d[k] = on;
					return on;
				}
			}
			return default(T);
		}

		/// <summary>
		/// 找到一个cv
		/// </summary>
		/// <param name="l">包含nbt集合的列表，集合中总是由ck-cv组合构成</param>
		/// <param name="k">待查找的ck</param>
		/// <returns>cv对应的值(可能为空)</returns>
		public static T findCK<T>(ArrayList l, string k) where T : class, new() {
			if (l != null) {
				foreach(Dictionary<string, object> d in l) {
					object ock = null;
					if (d.TryGetValue("ck", out ock)) {
						if (ock as string == k) {
							object ocv = null;
							if (d.TryGetValue("cv", out ocv))
								return ocv as T;
						}
					}
				}
			}
			return default(T);
		}
		
		/// <summary>
		/// 找到或添加一个cv
		/// </summary>
		/// <param name="l">包含nbt集合的列表，集合中总是由ck-cv组合构成</param>
		/// <param name="k">待查找或追加的ck</param>
		/// <returns></returns>
		public static T findOrAppandCK<T>(ArrayList l, string k) where T : class, new() {
			if (l != null) {
				var on = findCK<T>(l, k);
				if (on == default(T)) {
					var tag = new Dictionary<string, object>();
					tag["ck"] = k;
					on = new T();
					tag["cv"] = on;
					l.Add(tag);
				}
				return on;
			}
			return default(T);
		}
		
		/// <summary>
		/// 找到并删除一个cv
		/// </summary>
		/// <param name="l">包含nbt集合的列表，集合中总是由ck-cv组合构成</param>
		/// <param name="k">待删除的ck</param>
		/// <returns></returns>
		public static object findAndRemove(ArrayList l, string k) {
			for (int m = l.Count; m > 0; --m) {
				var data = l[m - 1] as Dictionary<string, object>;
				if (data != null) {
					object ock;
					if (data.TryGetValue("ck", out ock)) {
						string ck = ock as string;
						if (ck == k) {
							// 找到
							l.Remove(data);
							return data;
						}
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// 找到物品类型
		/// </summary>
		/// <param name="item">以tt=10开头的物品nbt json</param>
		/// <returns></returns>
		public static string getItemType(Dictionary<string, object> item) {
			string nid = string.Empty;
			try {
				var tv = getOrAppand<ArrayList>(item, "tv", "tt", 10);
				if (tv != null) {
					var namedata = findOrAppandCK<Dictionary<string, object>>(tv, "Name");
					if (namedata != null) {
						object onid;
						if (namedata.TryGetValue("tv", out onid)) {
							nid = onid as string;
						}
					}
				}
			} catch {}
			return nid;
		}
		
		/// <summary>
		/// 获取关于lore标签的所有信息
		/// </summary>
		/// <param name="nbt">以tt=10开头的物品nbt json</param>
		/// <returns>lore注释集合</returns>
		public static ArrayList getLores(Dictionary<string, object> nbt) {
			ArrayList oldtxtkv = null;
			try {
				var tv = nbt["tv"] as ArrayList;
				var tagdata = findCK<Dictionary<string, object>>(tv, "tag");
				var taglist = tagdata["tv"] as ArrayList;
				var displaydata = findCK<Dictionary<string, object>>(taglist, "display");
				var displaylist = displaydata["tv"] as ArrayList;
				var loredata = findCK<Dictionary<string, object>>(displaylist, "Lore");
				var lorelist = loredata["tv"] as ArrayList;
				if (lorelist != null) {
					oldtxtkv = lorelist;
				}
			} catch{}
			if (oldtxtkv != null && oldtxtkv.Count > 0) {
				var ll = new ArrayList();
				foreach(Dictionary<string, object> mlore in oldtxtkv) {
					object olstr;
					if (mlore.TryGetValue("tv", out olstr)) {
						ll.Add(olstr as string);
					}
				}
				return ll;
			}
			return null;
		}
		
		/// <summary>
		/// 删除lore标签
		/// </summary>
		/// <param name="item">以tt=10开头的物品nbt json</param>
		public static void removeLores(Dictionary<string, object> item) {
			try {
				var tv = item["tv"] as ArrayList;
					if (tv != null) {
						var tagdata = findOrAppandCK<Dictionary<string, object>>(tv, "tag");
						if (tagdata != null) {
							var taglist = tagdata["tv"] as ArrayList;
							if (taglist != null) {
								var displaydata = findOrAppandCK<Dictionary<string, object>>(taglist, "display");
								if (displaydata != null) {
									var displaylist = displaydata["tv"] as ArrayList;
									if (displaylist != null) {
										findAndRemove(displaylist, "Lore");
									}
									if (displaylist.Count < 1)
										displaydata.Clear();
								}
								if (displaydata.Count < 1)
									findAndRemove(taglist, "display");
							}
							if (taglist.Count < 1)
								tagdata.Clear();
						}
						if (tagdata.Count < 1)
							findAndRemove(tv, "tag");
					}
			} catch{}
		}

		/// <summary>
		/// 给当前物品添加注释
		/// </summary>
		/// <param name="item">以tt=10开头的物品nbt json</param>
		/// <param name="txts">待注释文本集合，若为空集则删除lore标签</param>
		/// <returns>是否成功添加</returns>
		public static bool setLores(Dictionary<string, object> item, string [] txts) {
			if (txts == null || txts.Length < 1) {
				removeLores(item);
				return true;
			}
			try {
					var tv = getOrAppand<ArrayList>(item, "tv", "tt", 10);
					if (tv != null) {
						var tagdata = findOrAppandCK<Dictionary<string, object>>(tv, "tag");
						if (tagdata != null) {
							var taglist = getOrAppand<ArrayList>(tagdata, "tv", "tt", 10);
							if (taglist != null) {
								var displaydata = findOrAppandCK<Dictionary<string, object>>(taglist, "display");
								if (displaydata != null) {
									var displaylist = getOrAppand<ArrayList>(displaydata, "tv", "tt", 10);
									if (displaylist != null) {
										var loredata = findOrAppandCK<Dictionary<string, object>>(displaylist, "Lore");
										if (loredata != null) {
											var lorelist = getOrAppand<ArrayList>(loredata, "tv", "tt", 9);
											if (lorelist == null) {
												loredata["tv"] = new ArrayList();
												lorelist = loredata["tv"] as ArrayList;
											}
											if (lorelist != null) {
												// 创建新的lore
												lorelist.Clear();
												if (txts != null && txts.Length > 0) {
													foreach (string txt in txts) {
													var ntxt = new Dictionary<string, object>();
													ntxt["tt"] = 8;
													ntxt["tv"] = txt;
													lorelist.Add(ntxt);
													}
												}
												return true;
											}
										}
									}
								}
							}
						}
					}
				} catch {}
			return false;
		}
		
		/// <summary>
		/// 删除fakeTag标签
		/// </summary>
		/// <param name="item">以tt=10开头的物品nbt json</param>
		public static void removeFakeTag(Dictionary<string, object> item){
			try {
				var tv = item["tv"] as ArrayList;
					if (tv != null) {
						var tagdata = findOrAppandCK<Dictionary<string, object>>(tv, "tag");
						if (tagdata != null) {
							var taglist = tagdata["tv"] as ArrayList;
							if (taglist != null) {
								findAndRemove(taglist, "fakeTag");
							}
							if (taglist.Count < 1)
								tagdata.Clear();
						}
						if (tagdata.Count < 1)
							findAndRemove(tv, "tag");
					}
			} catch{}
		}
		/// <summary>
		/// 获取fakeTag标签的键值对
		/// </summary>
		/// <param name="item">以tt=10开头的物品nbt json</param>
		/// <returns>键值对</returns>
		public static KeyValuePair<string, string> getFakeTag(Dictionary<string, object> item) {
			string key = null;
			string value = null;
			try {
				var tv = item["tv"] as ArrayList;
				var tagdata = findCK<Dictionary<string, object>>(tv, "tag");
				var taglist = tagdata["tv"] as ArrayList;
				var displaydata = findCK<Dictionary<string, object>>(taglist, "fakeTag");
				var tdata = displaydata["tv"] as ArrayList;
				if (tdata != null && tdata.Count > 1) {
					var kdata = tdata[0] as Dictionary<string, object>;
					key = kdata["tv"] as string;
					var vdata = tdata[1] as Dictionary<string, object>;
					value = vdata["tv"] as string;
				}
			} catch{}
			return new KeyValuePair<string, string>(key, value);
		}
		/// <summary>
		/// 添加一个fakeTag的键和值
		/// </summary>
		/// <param name="item">以tt=10开头的物品nbt json</param>
		/// <param name="k"></param>
		/// <param name="v"></param>
		/// <returns>是否添加成功</returns>
		public static bool setFakeTag(Dictionary<string, object> item, string k, string v) {
			bool ret = false;
			try {
					var tv = getOrAppand<ArrayList>(item, "tv", "tt", 10);
					if (tv != null) {
						var tagdata = findOrAppandCK<Dictionary<string, object>>(tv, "tag");
						if (tagdata != null) {
							var taglist = getOrAppand<ArrayList>(tagdata, "tv", "tt", 10);
							if (taglist != null) {
								var displaydata = findOrAppandCK<Dictionary<string, object>>(taglist, "fakeTag");
								if (displaydata != null) {
									var tdata = getOrAppand<ArrayList>(displaydata, "tv", "tt", 9);
									if (tdata != null) {
										tdata.Clear();
										var d1 = new Dictionary<string, object>();
										d1["tt"] = 8;
										d1["tv"] = k;
										tdata.Add(d1);
										var d2 = new Dictionary<string, object>();
										d2["tt"] = 8;
										d2["tv"] = v;
										tdata.Add(d2);
										ret = true;
									}
								}
							}
						}
					}
			} catch{}
			return ret;
		}
		
		/// <summary>
		/// 获取物品数量
		/// </summary>
		/// <param name="item">以tt=10开头的物品nbt json</param>
		/// <returns>个数</returns>
		public static int getCount(Dictionary<string, object> item){
			int count = 0;
			try {
				var tv = item["tv"] as ArrayList;
				if (tv != null) {
					var tagdata = findCK<Dictionary<string, object>>(tv, "Count");
					if (tagdata != null) {
						count = Convert.ToInt32(tagdata["tv"]);
					}
				}
			} catch{}
			return count;
		}
		/// <summary>
		/// 设置物品数量
		/// </summary>
		/// <param name="item">以tt=10开头的物品nbt json</param>
		/// <param name="count">新的数量</param>
		/// <returns>是否设置成功</returns>
		public static bool setCount(Dictionary<string, object> item, int count) {
			bool setok = false;
			try {
				var tv = getOrAppand<ArrayList>(item, "tv", "tt", 10);
				if (tv != null) {
					var tagdata = findOrAppandCK<Dictionary<string, object>>(tv, "Count");
					if (tagdata != null) {
						tagdata["tv"] = count;
						setok = true;
					}
				}
			} catch{}
			return setok;
		}
		/// <summary>
		/// 设置物品格子
		/// </summary>
		/// <param name="item">以tt=10开头的物品nbt json</param>
		/// <param name="slot">新的格子位置</param>
		/// <returns>是否设置成功</returns>
		public static bool setSlot(Dictionary<string, object> item, int slot) {
			bool setok = false;
			try {
				var tv = getOrAppand<ArrayList>(item, "tv", "tt", 10);
				if (tv != null) {
					var tagdata = findOrAppandCK<Dictionary<string, object>>(tv, "Slot");
					if (tagdata != null) {
						tagdata["tt"] = 1;
						tagdata["tv"] = slot;
						setok = true;
					}
				}
			} catch{}
			return setok;
		}
		
		/// <summary>
		/// 添加效果至效果列表
		/// </summary>
		/// <param name="effc">原效果列表或空列表</param>
		/// <param name="id">效果id</param>
		/// <param name="amplifier">效果增幅强度</param>
		/// <param name="durationsec">持续秒数</param>
		/// <returns>是否添加成功</returns>
		public static bool addEffect(Dictionary<string, object>effc, int id, int amplifier, int durationsec) {
			bool setok = false;
			try {
				var lst = getOrAppand<ArrayList>(effc, "tv", "tt", 9);
				var e = new Dictionary<string, object>();
				var cps = getOrAppand<ArrayList>(e, "tv", "tt", 10);
				var Ambient = findOrAppandCK<Dictionary<string, object>>(cps, "Ambient");
				Ambient["tt"] = 1;
				Ambient["tv"] = 0;
				var Amplifier = findOrAppandCK<Dictionary<string, object>>(cps, "Amplifier");
				Amplifier["tt"] = 1;
				Amplifier["tv"] = amplifier;
				var DisplayOnScreenTextureAnimation = findOrAppandCK<Dictionary<string, object>>(cps, "DisplayOnScreenTextureAnimation");
				DisplayOnScreenTextureAnimation["tt"] = 1;
				DisplayOnScreenTextureAnimation["tv"] = 0;
				var Durationlst = new ArrayList() {findOrAppandCK<Dictionary<string, object>>(cps, "Duration"),
					findOrAppandCK<Dictionary<string, object>>(cps, "DurationEasy"),
					findOrAppandCK<Dictionary<string, object>>(cps, "DurationHard"),
					findOrAppandCK<Dictionary<string, object>>(cps, "DurationNormal")};
				foreach (Dictionary<string, object> Duration in Durationlst) {
					Duration["tt"] = 3;
					Duration["tv"] = 20 * durationsec;
				}
				var ShowParticles = findOrAppandCK<Dictionary<string, object>>(cps, "ShowParticles");
				ShowParticles["tt"] = 1;
				ShowParticles["tv"] = 1;
				var Id = findOrAppandCK<Dictionary<string, object>>(cps, "Id");
				Id["tt"] = 1;
				Id["tv"] = id;
				lst.Add(e);
				setok = true;
			} catch{};
			return setok;
		}
	}
}
