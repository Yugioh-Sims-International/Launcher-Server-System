﻿/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/12
 * 时间: 17:38
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using AsyncServer;
using System.Collections.Generic;
using System.IO;
using YGOCore;

namespace YGOCore
{
    /// <summary>
    /// Client
    /// </summary>
    public class Session
    {
        static readonly Random Random = new Random(Environment.TickCount);

        #region member
        public RoomServer Server { get; private set; }
        protected Connection<Session> Client { get; private set; }
        //ygoproLogin
        public bool IsClient;
        //A message sent by a client to receive other users ' messages
        public bool CanGameChat;

        public bool IsLogin;

        private bool m_close = false;
        /// <summary>
        /// Push the pause
        /// </summary>
        public bool IsPause = false;
        /// <summary>
        /// First name
        /// </summary>
        public string Name;
        /// <summary>
        /// Room name
        /// </summary>
        public string RoomName;
        /// <summary>
        /// Current server
        /// </summary>
        public DuelServer ServerInfo;

        /// <summary>
        /// 
        /// </summary>
        public string Token { get; private set; }
        public string ip
        {
            get
            {
                if(Client != null && Client.Address!= null)
                {
                    return Client.Address.ToString();
                }
                return null;
            }
        }
        #endregion

        #region public
        public Session(Connection<Session> client, RoomServer server, int timeout = 15)
        {
            this.Client = client;
            this.Client.Tag = this;
            this.Server = server;
            MyTimer CheckTimer = new MyTimer(1000, timeout);
            CheckTimer.AutoReset = true;
            CheckTimer.Elapsed += delegate
            {
                if (!string.IsNullOrEmpty(Name))
                {
                    CheckTimer.Stop();
                    CheckTimer.Close();
                }
                if (CheckTimer.CheckStop())
                {
                    //超时自动断开
                    Close();
                    CheckTimer.Close();
                }
            };
        }
        public void Close()
        {
            if (m_close) return;
            m_close = true;
            try
            {
                Client.Close();
            }
            catch (Exception)
            {

            }
        }


        public void CreateToken(string name, string pwd)
        {
            string str = name + pwd + Environment.TickCount + Random.Next(100);
            str = Tool.GetMd5(str);
            if (str == null || str.Length < 32)
            {
                Token = Random.Next(9999).ToString("0000");
            }
            else {
                Token = str.Substring(0, 2) + str.Substring(30, 2);
            }
        }
        public void Send(PacketWriter writer, bool isNow = true)
        {
            byte[] data = writer.Content;
         //   Logger.Info("id=0x" + data[2].ToString("x"));
            Send(data, isNow);
        }
        public void Send(byte[] data, bool isNow = true)
        {
            if (Client != null && Client.Connected)
                Client.SendPackage(data, isNow);
        }
        public void PeekSend()
        {
            if (Client != null && Client.Connected)
                Client.PeekSend();
        }
            
        public bool OnCheck()
        {
            byte[] data;
            if (Client.GetPacketData(2, out data))
            {
                PacketReader packet = new PacketReader(data);
                RoomMessage msg = ClinetEvent.Handler(this, packet);
                if (msg == RoomMessage.Info || msg == RoomMessage.PlayerInfo || msg == RoomMessage.NETWORK_CLIENT_ID)
                {
                    return true;
                }
            }
            Close();
            return false;
        }
        public void OnRecevice()
        {
            if (m_close) return;
            //Threading
            bool next = true;
            while (next)
            {
                byte[] data;
                next = Client.GetPacketData(2, out data);
                if (data != null && data.Length > 0)
                {
                    //Handle game events
                    PacketReader packet = new PacketReader(data);
                    ClinetEvent.Handler(this, packet);
                }
            }
        }
        #endregion

    }
}
