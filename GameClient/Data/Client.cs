﻿/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/7
 * 时间: 21:09
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Net.Sockets;

using AsyncServer;
using YGOCore;
using YGOCore.Game;
using System.IO;
using System.Windows.Forms;

namespace GameClient
{
    public delegate void OnLoginHandler();
    public delegate void OnServerChatHandler(string pname, string tname, string msg);
    public delegate void OnRoomCreateHandler(GameConfig2 config);
    public delegate void OnRoomStartHandler(RoomInfo room);
    public delegate void OnRoomCloseHandler(RoomInfo room);
    public delegate void OnRoomListHandler(List<GameConfig2> configs);
    public delegate void OnPlayerEnterEvent(string player, RoomInfo room);
    public delegate void OnPlayerLeaveEvent(string player, RoomInfo room);
    public delegate void OnGameExitedEvent();
    public delegate void OnServerCloseEvent(int port);
    public delegate void OnPlayerListEvent(List<PlayerInfo> players);
    public delegate void OnServerStopEvent();
    /// <summary>
    /// Description of Client.
    /// </summary>
    public class Client
    {
        public event OnServerCloseEvent OnServerClose;
        public event OnGameExitedEvent OnGameExited;
        public event OnLoginHandler OnLoginSuccess;
        public event OnServerChatHandler OnServerChat;
        public event OnRoomCreateHandler OnRoomCreate;
        public event OnRoomStartHandler OnRoomStart;
        public event OnRoomCloseHandler OnRoomClose;
        public event OnRoomListHandler OnRoomList;
        public event OnPlayerEnterEvent OnPlayerEnter;
        public event OnPlayerLeaveEvent OnPlayerLeave;
        public event OnPlayerListEvent OnPlayerList;
        public event OnServerStopEvent OnServerStop;
        private TcpClient client;
        public string Name = "???";
        public string Pwd = "";
        private MainForm m_parent;
        public bool IsLogin = false;
        private Random random = new Random(Environment.TickCount);
        readonly ArrayQueue<byte> ReceviceQueue = new ArrayQueue<byte>();
        //First name，Room name
        public Client()
        {
        }

        public void SetForm(MainForm parent)
        {
            this.m_parent = parent;
        }
        #region socket
        public bool Connect(ClientConfig server)
        {
            if (server == null) return false;
            if (client == null)
            {
                client = new TcpClient();
            }
            if (!client.Client.Connected)
            {
                try
                {
                    client.Close();
                }
                catch { }
                client = new TcpClient();
            }
            else {
                return true;
            }
            try
            {
                client.Connect(server.Host, server.Port);
                BeginRecevice();
                return client.Connected;
            }
            catch
            {
            }
            return false;
        }
        private void BeginRecevice()
        {
            byte[] m_buff = new byte[1024];
            try
            {
                client.Client.BeginReceive(m_buff, 0, m_buff.Length, SocketFlags.None, new AsyncCallback(EndRecevice), m_buff);
            }
            catch { }
        }
        private void EndRecevice(IAsyncResult ar)
        {
            try
            {
                byte[] buff = (byte[])ar.AsyncState;
                int len = client.Client.EndReceive(ar);
                lock (ReceviceQueue)
                {
                    ReceviceQueue.Enqueue((byte[])ar.AsyncState, 0, len);
                }
                if (len != buff.Length)
                {
                    OnRecevice();
                }
            }
            catch (Exception e)
            {
                Logger.Warn(e);
                System.Windows.Forms.MessageBox.Show("You have logged out"
#if DEBUG
				                                     +"\n"+e
#endif
                                                    );
            }
            finally
            {
                BeginRecevice();
            }
        }
        public void OnRecevice()
        {
            List<PacketReader> packets = new List<PacketReader>();
            lock (ReceviceQueue)
            {
                while (ReceviceQueue.Count > 2)
                {
                    byte[] blen = new byte[2];
                    ReceviceQueue.Dequeue(blen);
                    int len = BitConverter.ToUInt16(blen, 0);
                    byte[] data = new byte[len];
                    if (ReceviceQueue.Count >= len)
                    {
                        ReceviceQueue.Dequeue(data);
                        PacketReader packet = new PacketReader(data);
                        packets.Add(packet);
                        //Logger.Debug("add packet");
                    }
                    else {
                        break;
                    }
                }
            }
            ClientEvent.Handler(this, packets);
        }
        public void GetPlayerList()
        {
            using (PacketWriter writer = new PacketWriter(2))
            {
                writer.Write((byte)RoomMessage.PlayerList);
                Send(writer.Content);
            }
        }
        public void GetRooms(bool nolock, bool nostart)
        {
            using (PacketWriter writer = new PacketWriter(2))
            {
                writer.Write((byte)RoomMessage.RoomList);
                writer.Write(nolock);
                writer.Write(nostart);
                Send(writer.Content);
            }
        }
        public void Close(bool force = false)
        {
            try
            {
                client.Close();
            }
            catch (Exception)
            {

            }
            client = null;
            if (force)
            {
                if (m_parent != null)
                {
                    m_parent.Client_OnServerStop();
                }
            }
        }

        private void Send(byte[] data)
        {
            try
            {
                client.Client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(EndSend), data);
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("Disconnected");
            }
        }
        private void EndSend(IAsyncResult ar)
        {
            try
            {
                client.Client.EndSend(ar);
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region login/chat
        /// <summary>
        /// Network login
        /// </summary>
        public void Login(string name, string pwd,bool force = false)
        {
            Name = name;
            //Pwd = pwd;
            pwd = Tool.GetMd5(pwd);
            using (PacketWriter writer = new PacketWriter(2))
            {
                writer.Write((byte)RoomMessage.Info);
                writer.WriteUnicode(Name, 20);
                writer.WriteUnicode(pwd, 32);
                writer.Write(force);
                Send(writer.Content);
            }
        }
        /// <summary>
        /// Send a chat message
        /// </summary>
        public bool OnChat(string msg, bool hidename, string toname = "")
        {
            using (PacketWriter writer = new PacketWriter(2))
            {
                writer.Write((byte)RoomMessage.Chat);
                if (string.IsNullOrEmpty(toname))
                {
                    if (hidename)
                    {
                        if (random.Next(100) < 10)
                        {
                            writer.WriteUnicode("[Anonymous]"+ Name, 20);
                        }
                        else {
                            //90%Anonymous
                            writer.WriteUnicode("[Anonymous]", 20);
                        }
                    }
                    else {
                        writer.WriteUnicode(Name, 20);
                    }
                }
                else {
                    writer.WriteUnicode(Name, 20);
                }
                writer.WriteUnicode(toname, 20);
                writer.WriteUnicode(msg, msg.Length + 1);
                Send(writer.Content);
            }
            return false;
        }
        public void OnLoginOk()
        {
            IsLogin = true;
            if (OnLoginSuccess != null)
            {
                OnLoginSuccess();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pname">News</param>
        /// <param name="msg"></param>
        public void ServerChat(string pname, string tname, string msg)
        {
            if (OnServerChat != null)
            {
                OnServerChat(pname, tname, msg);
            }
        }
        public void JoinRoom(string room, int port, bool needauth)
        {
            string namepwd = Name;
            if (needauth)
            {
                namepwd += "$" + Pwd;
            }
#if DEBUG
			System.Windows.Forms.MessageBox.Show(Name+":"+room+":"+port);
#endif
            if (GameUtil.JoinRoom(Program.Config.Host, "" + port, namepwd, room, GameExited))
            {
                //暂停游戏
                if (Program.Config.JoinPause)
                {
                    using (PacketWriter writer = new PacketWriter(2))
                    {
                        writer.Write((byte)RoomMessage.Pause);
                        Send(writer.Content);
                    }
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("请放到游戏目录运行。");
            }
        }
        private void GameExited()
        {
            if (Program.Config.JoinPause)
            {
                GetRooms(false, true);
            }
            if (OnGameExited != null)
            {
                OnGameExited();
            }
        }
        #endregion

        #region room
        public void ServerRoomCreate(GameConfig2 config)
        {
            if (OnRoomCreate != null)
            {
                OnRoomCreate(config);
            }
        }
        public void ServerRoomClose(RoomInfo room)
        {
            if (OnRoomClose != null)
            {
                OnRoomClose(room);
            }
        }
        public void ServerRoomStart(RoomInfo room)
        {
            if (OnRoomStart != null)
            {
                OnRoomStart(room);
            }
        }

        public void ServerStop()
        {
            if (OnServerStop != null)
            {
                OnServerStop();
            }
        }
        public void ServerRoomList(List<GameConfig2> configs)
        {
            if (OnRoomList != null)
            {
                OnRoomList(configs);
            }
        }
        public void ServerPlayerList(List<PlayerInfo> players)
        {
            if (OnPlayerList != null)
            {
                OnPlayerList(players);
            }
        }
        public void ServerPlayerEnter(string player, RoomInfo room)
        {
            if (OnPlayerEnter != null)
            {
                OnPlayerEnter(player, room);
            }
        }
        public void ServerPlayerLeave(string player, RoomInfo room)
        {
            if (OnPlayerLeave != null)
            {
                OnPlayerLeave(player, room);
            }
        }
        public void ServerClose(int port)
        {
            if (OnServerClose != null)
            {
                OnServerClose(port);
            }
        }
        #endregion


    }
}
