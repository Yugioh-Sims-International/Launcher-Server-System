﻿/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/5
 * 时间: 13:43
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using YGOCore.Net;
using System.Text;
using AsyncServer;
using YGOCore.Game;
using System.Net;
using System.Net.Sockets;
using OcgWrapper;
using OcgWrapper.Enums;
using System.IO;
using System.Threading;

namespace YGOCore.Game
{
    /// <summary>
    /// Description of RoomManager.
    /// </summary>
    public class RoomManager
    {
        #region member
        private static readonly SortedList<string, GameRoom> Games = new SortedList<string, GameRoom>();
        private static readonly Queue<WinInfo> WinInfos = new Queue<WinInfo>();
        private static System.Timers.Timer WinSaveTimer;
        private static List<string> banNames = new List<string>();
        public static int Count
        {
            get { lock (Games) { return Games.Count; } }
        }
        #endregion

        #region public
        public static int OnWorldMessage(string msg, PlayerType color = PlayerType.Yellow)
        {
            List<GameRoom> rooms = new List<GameRoom>();
            int i = 0;
            lock (Games)
            {
                foreach (GameRoom room in Games.Values)
                {
                    if (room != null && room.IsOpen)
                    {
                        room.ServerMessage(msg, color);
                        i++;
                    }
                }
            }
            return i;
        }
        public static void OnWin(string roomName, int roomMode, int win, int reason, string yrpFileName,
                                 string[] names, bool force)
        {
            WinInfo info = new WinInfo(roomName, win, reason, yrpFileName, names, force);
            lock (WinInfos)
            {
                WinInfos.Enqueue(info);
            }
        }
        public static bool OnLogin(string name, string pwd)
        {
            //A long password, short
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            if (name != null && name.StartsWith("[AI]"))
            {
                //	Logger.Debug("[AI]login:"+pwd+"=="+Program.Config.AIPass+"?");
                return Program.Config.AIPass == pwd;
            }
            if (!Program.Config.isNeedAuth)
            {
                return true;
            }
            //Server interface
            return ServerApi.CheckLogin(name, pwd);
        }
        public static void init(string file)
        {
            SatrtWinTimer();
            ReadBanNames(file);
        }
        #endregion

        #region Game records
        private static void SatrtWinTimer()
        {
            //10s saved results
            if (WinSaveTimer == null)
            {
                WinSaveTimer = new System.Timers.Timer(10 * 1000);
                WinSaveTimer.AutoReset = true;
                WinSaveTimer.Enabled = true;
                WinSaveTimer.Elapsed += new System.Timers.ElapsedEventHandler(WinSaveTimer_Elapsed);
            }
            WinSaveTimer.Start();
        }
        private static void WinSaveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string[] sqls = null;
            lock (WinInfos)
            {
                if (WinInfos.Count == 0) return;
                sqls = new string[WinInfos.Count];
                int i = 0;
                while (WinInfos.Count > 0)
                {
                    WinInfo info = WinInfos.Dequeue();
                    sqls[i++] = info.GetSQL();
                }
            }
            ThreadPool.QueueUserWorkItem(new WaitCallback(
                delegate (object obj)
                {
                    //string[] sqls = (string[])obj;
                    SQLiteTool.Command(WinInfo.DB_FILE, sqls);
                    Logger.Debug("save wins record:" + sqls.Length);
                }
            ));
        }
        #endregion

        #region Disable logons
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>false=Prohibited</returns>
        public static bool CheckPlayerBan(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            name = name.Split('$')[0];
            if (Program.Config.BanMode == 0)
            {
                return true;
            }
            else if (Program.Config.BanMode == 1)
            {
                return !banNames.Contains(name);
            }
            else {
                return banNames.Contains(name);
            }
        }
        private static void ReadBanNames(string file)
        {
            //	Logger.Debug("name list="+file);
            if (File.Exists(file))
            {
                string[] lines = File.ReadAllLines(file);
                foreach (string line in lines)
                {
                    if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    {
                        continue;
                    }
                    string name = line.Trim();
                    if (!banNames.Contains(name))
                    {
                        banNames.Add(name);
                    }
                }
            }
        }
        #endregion

        #region Room
        public static void Remove(GameRoom room)
        {
            if (room == null) return;
            lock (Games)
            {
                if (room.Name != null)
                {
                    Games.Remove(room.Name);
                    ServerApi.OnRoomClose(room);
                }
            }
        }
        public static bool CheckRoomPassword(string namepwd)
        {
            string name = Password.OnlyName(namepwd);
            lock (Games)
            {
                if (Games.ContainsKey(name))
                {
                    //This room
                    GameRoom room = Games[name];
                    if (room != null && room.Config != null)
                    {
                        return namepwd == room.Config.Name;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// Create a room
        /// </summary>
        /// <param name="server"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static GameRoom CreateOrGetGame(GameConfig config)
        {
            string roomname = Password.OnlyName(config.Name);
            lock (Games)
            {
                if (Games.ContainsKey(roomname))
                {
                    return Games[roomname];
                }
                if (Games.Count >= Program.Config.MaxRoomCount)
                {
                    return null;
                }
                GameRoom room = new GameRoom(config);
                Logger.Info("create room:" + room.Name);
                Games.Add(roomname, room);
                ServerApi.OnRoomCreate(room);
                return room;
            }
        }
        /// <summary>
        /// Get a random room does not exist
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public static string NewRandomRoomName()
        {
            lock (Games)
            {
                if (Games.Count == 0)
                {
                    return GetGuidString();
                }
            }
            while (true) //keep searching till one is found!!
            {
                string roomname = GetGuidString();
                //MutexRooms.WaitOne();
                lock (Games)
                {
                    if (!Games.Keys.Contains(roomname))
                    {
                        return roomname;
                    }
                }
            }
        }

        /// <summary>
        /// Get a random room name, no password
        /// </summary>
        /// <param name="server"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static string RandomRoomName(string tag = null)
        {
            if (!string.IsNullOrEmpty(tag) && !tag.EndsWith("#"))
            {
                tag += "#";
            }
            List<GameRoom> rooms = null;
            lock (Games)
            {
                rooms = GetNoPwdRoom(Games, tag);

                if (rooms.Count == 0)
                {
                    string name = GetGuidString();
                    if (tag == null)
                    {
                        return name;
                    }
                    else
                    {
                        return tag + name;
                    }
                }
                int index = Program.Random.Next(rooms.Count);
                GameRoom room = rooms[index];
                return room.Config == null ? null : room.Config.Name;
            }
        }

        private static List<GameRoom> GetNoPwdRoom(SortedList<string, GameRoom> rooms, string tag = null)
        {
            List<GameRoom> roomList = new List<GameRoom>();
            foreach (GameRoom room in rooms.Values)
            {
                if (room != null && room.Config.Name != null && !room.Config.Name.Contains("$"))
                {
                    //Is not empty, no password
                    if (!string.IsNullOrEmpty(tag) && !room.Name.StartsWith(tag))
                    {
                        continue;
                    }
                    if ((room.IsRandom || !room.Config.Name.Contains("$")) && room.IsOpen && room.GetAvailablePlayerPos() >= 0)
                    {
                        roomList.Add(room);
                    }
                }
                else {
                    Logger.Debug("room is null?" + (room != null));
                }
            }
            return roomList;
        }

        private static string GetGuidString()
        {
            string GuidString = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            StringBuilder sb = new StringBuilder(GuidString);
            sb.Replace("=", "");
            sb.Replace("+", "");
            sb.Replace("#", "");
            sb.Replace("$", "");
            sb.Replace("/", "");
            sb.Replace("!", "");
            sb.Replace("@", "");
            sb.Replace("%", "");
            sb.Replace("^", "");
            sb.Replace("*", "");
            sb.Replace(":", "");
            sb.Replace(" ", "");
            GuidString = sb.ToString();
            if (GuidString.Length > 6)
            {
                return GuidString.Substring(0, 6);
            }
            return GuidString;
        }
        #endregion
    }
}
