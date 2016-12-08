﻿/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/8/30
 * 时间: 17:42
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YGOCore.Game
{
	#region 服务2服务
	public enum StoSMessage:byte{
		/// <summary>
		/// 添加一个房间
		/// </summary>
		RoomCreate = 0x1,
		/// <summary>
		/// 关闭一个房间
		/// </summary>
		RoomClose  = 0x2,
		/// <summary>
		/// 更新房间信息
		/// </summary>
		RoomStart = 0x3,
		PlayerJoin= 0x4,
		PlayerLeave = 0x5,
	}
	#endregion
	
	[DataContract]
	public class RoomInfo
	{
		public RoomInfo(){
		}
		/// <summary>
		/// 房间名(需要去除$后面)
		/// </summary>
		[DataMember(Order = 0, Name="room")]
		public string Name{get; set;}
		//public string pass{get;private set;}
		/// <summary>
		/// 规则
		/// 0 ocg
		/// 1 tcg
		/// 2 ocg&tcg
		/// </summary>
		[DataMember(Order = 1, Name="rule")]
		public byte Rule{get; set;}
		/// <summary>
		/// 模式
		/// 0 single
		/// 1 match
		/// 2 tag
		/// </summary>
		[DataMember(Order = 2, Name="mode")]
		public byte Mode{get; set;}
		/// <summary>
		/// 是否需要密码
		/// </summary>
		[DataMember(Order = 3, Name="pwd")]
		public string Pwd{get; set;}
		/// <summary>
		/// 是否开始
		/// </summary>
		[DataMember(Order = 4, Name="start")]
		public bool IsStart{get; set;}
		/// <summary>
		/// 禁卡表
		/// </summary>
		[DataMember(Order = 5, Name="lflist")]
		public string Lflist{get; set;}
		/// <summary>
		/// 玩家
		/// </summary>
		[DataMember(Order = 6, Name="players")]
		public readonly string[] players = new string[4];
		/// <summary>
		/// 观战
		/// </summary>
		[DataMember(Order = 7, Name="watchs")]
		public List<string> observers = new List<string>();
		
		[DataMember(Order = 8, Name="lp")]
		public int StartLP{get; set;}
		/// <summary>
		/// 特殊规则
		/// </summary>
		[DataMember(Order = 9, Name="warring")]
		public bool Warring{get; set;}

		public bool NeedPass(){
			return !string.IsNullOrEmpty(Pwd);
		}
	}
}
