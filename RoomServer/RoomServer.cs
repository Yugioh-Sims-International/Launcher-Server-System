﻿/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/12
 * 时间: 11:24
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using AsyncServer;
using System.IO;
using System.Xml;

namespace YGOCore
{
    /// <summary>
    /// Room service-side
    /// </summary>
    public class RoomServer
    {
        #region member
        public bool IsListening { get; private set; }
        private AsyncTcpListener<DuelServer> m_apilistener;
        private AsyncTcpListener<Session> m_listener;
        public readonly List<DuelServer> DuelServers = new List<DuelServer>();
        public readonly List<ServerProcess> Porcess = new List<ServerProcess>();
        public readonly SortedList<string, Session> Clients = new SortedList<string, Session>();
        public readonly Dictionary<string, Session> GameCliens = new Dictionary<string, Session>();
        public readonly RoomConfig Config = new RoomConfig();
        private System.Timers.Timer infoTimer = new System.Timers.Timer(60 * 1000);
        private int m_timers = 0;
        /// <summary>
        /// Bulletin
        /// </summary>
        public string Tip = "";
        public RoomServer()
        {
            infoTimer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Handler);

        }
        #endregion

        #region start/stop
        public bool Start()
        {
            if (IsListening) return true;
            IsListening = true;
            Config.Load();
            //本地api
            InitDeulListener();
            try
            {
                m_apilistener.Start();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            if (Config.Ports != null)
            {
                foreach (int port in Config.Ports)
                {
                    ServerProcess server = new ServerProcess(port, Config.ApiPort, Config.ServerExe, Config.Config);
                    server.Start();
                    Porcess.Add(server);
                }
            }
            else {
                Logger.Error("no configs");
            }
            InitRoomListener();
            try
            {
                m_listener.Start();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                Stop();
            }
            if (IsListening)
            {
                infoTimer.Start();
            }
            return IsListening;
        }

        public void Close(int port)
        {
            lock (Porcess)
            {
                foreach (ServerProcess p in Porcess)
                {
                    if (p.Port == port)
                    {
                        p.Close();
                    }
                }
            }
        }
        public void Stop()
        {
            //Server.Close();
            if (!IsListening) return;
            IsListening = false;
            infoTimer.Stop();
            this.OnServerStop();
            lock (DuelServers)
            {
                foreach (DuelServer server in DuelServers)
                {
                    server.Close();
                }
            }
            lock (Clients)
            {
                foreach (Session client in Clients.Values)
                {
                    client.Close();
                }
            }
            lock (Porcess)
            {
                foreach (ServerProcess p in Porcess)
                {
                    p.Close();
                }
            }
            if (m_apilistener != null)
                m_apilistener.Stop();
            if (m_listener != null)
                m_listener.Stop();
        }
        #endregion

        #region duleserver
        private void InitDeulListener()
        {
            if (m_apilistener == null)
            {
                m_apilistener = new AsyncTcpListener<DuelServer>(IPAddress.Parse("127.0.0.1"), Config.ApiPort);
                m_apilistener.OnConnect += new AsyncTcpListener<DuelServer>.ConnectEventHandler(ApiListener_OnConnect);
                m_apilistener.OnDisconnect += new AsyncTcpListener<DuelServer>.DisconnectEventHandler(ApiListener_OnDisconnect);
                m_apilistener.OnReceive += new AsyncTcpListener<DuelServer>.ReceiveEventHandler(ApiListener_OnReceive);
            }
        }
        private void ApiListener_OnReceive(Connection<DuelServer> Client)
        {
            if (Client != null && Client.Tag != null)
            {
                Client.Tag.OnRecevice();
            }
        }

        private void ApiListener_OnDisconnect(Connection<DuelServer> Client)
        {
            if (Client != null && Client.Tag != null)
            {
                this.OnServerClose(Client.Tag);
                lock (DuelServers)
                {
                    DuelServers.Remove(Client.Tag);
                }
            }
        }

        private void ApiListener_OnConnect(Connection<DuelServer> Client)
        {
            if (Client == null) return;
            if (!IsListening)
            {
                Client.Close();
                return;
            }
            DuelServer server = new DuelServer(this, Client);
            Client.Tag = server;
            lock (DuelServers)
            {
                DuelServers.Add(server);
            }
            Logger.Debug("duel server connected.");
        }
        #endregion

        #region client listener
        private void InitRoomListener()
        {
            if (m_listener == null)
            {
                m_listener = new AsyncTcpListener<Session>(IPAddress.Any, Config.Port);
                m_listener.OnConnect += new AsyncTcpListener<Session>.ConnectEventHandler(Listener_OnConnect);
                m_listener.OnDisconnect += new AsyncTcpListener<Session>.DisconnectEventHandler(Listener_OnDisconnect);
                m_listener.OnReceive += new AsyncTcpListener<Session>.ReceiveEventHandler(Listener_OnReceive);
                m_listener.OnCheckClient += new AsyncTcpListener<Session>.CheckEventHandler(Listener_OnCheck);

            }
        }
        private ConnectStatu Listener_OnCheck(Connection<Session> Client)
        {
            if (Client.Tag != null)
            {
                if (Client.Tag.OnCheck())
                {
                    return ConnectStatu.Success;
                }
            }
            return ConnectStatu.Fail;
        }
        private void Listener_OnDisconnect(Connection<Session> Client)
        {
            Logger.Debug("Listener_OnDisconnect");
            if (Client.Tag != null)
            {
                if (Client.Tag.IsLogin && Client.Tag.Name != null)
                {
                    lock (Clients)
                        Clients.Remove(Client.Tag.Name);
                }
                if (Client.Tag.ServerInfo != null)
                {
                    this.server_OnPlayerLeave(Client.Tag.ServerInfo, Client.Tag.Name, null);
                }
                Client.Tag.Close();
                Client.Tag = null;
            }
        }
        private void Listener_OnReceive(Connection<Session> Client)
        {
            if (Client.Tag != null)
            {
                Client.Tag.OnRecevice();
            }
        }

        private void Listener_OnConnect(Connection<Session> Client)
        {
            if (!IsListening)
            {
                Client.Close();
                return;
            }
            Logger.Debug("Listener_OnConnect");
            Session session = new Session(Client, this);
            //Assign the game port
        }
        #endregion

        public int GetChatPort()
        {
            return Config.Port;
        }
        public List<int> GetAllPorts()
        {
            List<int> ports = new List<int>();
            lock (DuelServers)
            {
                foreach (DuelServer srv in DuelServers)
                {
                    if (srv.Port > 0)
                        ports.Add(srv.Port);
                }
            }
            return ports;
        }
        /// <summary>
        /// Returns the minimum number of server
        /// </summary>
        public DuelServer GetMinServer()
        {
            List<int> lens = new List<int>();
            DuelServer[] servers;
            lock (DuelServers)
            {
                servers = DuelServers.ToArray();
            }
            int c = int.MaxValue;
            int index = -1;
            for (int i = 0; i < servers.Length; i++)
            {
                if (servers[i] == null) continue;
                if (c > servers[i].Count)
                {
                    c = servers[i].Count;
                    index = i;
                }
            }
            if (index >= 0 && index < servers.Length)
            {
                return servers[index];
            }
            return null;
        }
        public void Message(string msg)
        {
            this.OnChatMessage("", "", msg);
        }
        private void Timer_Handler(object sender, System.Timers.ElapsedEventArgs e)
        {
            m_timers++;
            if (m_timers % 3 == 0)
            {
                PrintServer();
            }
            if (m_timers % 15 == 0 && !string.IsNullOrEmpty(Tip))
            {
                this.OnChatMessage("", "", Tip);
            }
            if (m_timers >= int.MaxValue)
            {
                m_timers = 0;
            }
        }
        public void SendToken(string name, string tk)
        {
            using (PacketWriter token = new PacketWriter(2))
            {
                token.Write((byte)RoomMessage.Info);
                token.WriteUnicode(name, 20);
                token.WriteUnicode(tk, 32);
                lock (DuelServers)
                {
                    foreach(DuelServer srv in DuelServers)
                    {
                        srv.Send(token);
                    }
                }
            }
        }
        public void PrintServer(int index = -1)
        {
            lock (DuelServers)
            {
                if (index >= 0)
                {
                    if (index < DuelServers.Count)
                    {
                        DuelServer srv = DuelServers[index];
                        Console.WriteLine(">>" + index + ":" + srv.ToString());
                    }
                    else
                    {
                        Console.WriteLine(">>" + index + ":null");
                    }
                }
                else {
                    Console.WriteLine(">>" + DuelServers.Count);
                    int i = 0;
                    foreach (DuelServer srv in DuelServers)
                    {
                        Console.WriteLine(">>" + (i++) + ":" + srv.ToString());
                    }
                }
            }
        }
    }
}
