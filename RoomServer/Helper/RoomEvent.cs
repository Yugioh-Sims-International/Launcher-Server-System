﻿/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/13
 * 时间: 10:45
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using AsyncServer;
using YGOCore.Game;
using System.IO;

namespace YGOCore
{
    /// <summary>
    /// Description of RoomEvent.
    /// </summary>
    public static class RoomEvent
    {
        public static void OnSendServerInfo(this RoomServer roomServer, Session session)
        {
            using (PacketWriter writer = new PacketWriter(2))
            {
                writer.Write((byte)RoomMessage.Info);
                writer.Write(roomServer.GetChatPort());
                DuelServer srv = roomServer.GetMinServer();
                lock (srv.AsyncLock)
                {
                    srv.Count++;
                }
                if (srv != null)
                {
                    session.ServerInfo = srv;
                    writer.Write(srv.Port);
                    writer.Write(srv.NeedAuth);
                }
                else
                {
                    writer.Write(0);
                    writer.Write((byte)0);
                }
                writer.WriteUnicode(session.Token, 32);
                session.Send(writer.Content);
            }
        }

        public static void OnServerClose(this RoomServer roomServer, DuelServer server)
        {
            using (PacketWriter writer = new PacketWriter(2))
            {
                writer.Write((byte)RoomMessage.ServerClose);
                writer.Write(server.Port);
                DuelServer srv = roomServer.GetMinServer();
                lock (srv.AsyncLock)
                {
                    srv.Count = 0;
                }
                if (srv != null)
                {
                    //Special handling
                    writer.Write(srv.Port);
                    writer.Write(srv.NeedAuth);
                    //session.ServerInfo = srv;
                    lock (roomServer.Clients)
                    {
                        foreach (Session client in roomServer.Clients.Values)
                        {
                            if (client.ServerInfo != null && client.ServerInfo.Port == server.Port)
                            {
                                client.ServerInfo = srv;
                                client.Send(writer.Content);
                            }
                        }
                    }
                }
                             
            }
        }


        #region room list
        public static HostInfo ToHostInfo(this GameConfig config)
        {
            HostInfo info = new HostInfo();
            if (config != null)
            {
                info.LfList = (uint)config.LfList;
                info.Rule = (byte)config.Rule;
                info.Mode = (byte)config.Mode;
                info.EnablePriority = (byte)(config.EnablePriority ? 1 : 0);
                info.NoCheckDeck = (byte)(config.NoCheckDeck ? 1 : 0);
                info.NoShuffleDeck = (byte)(config.NoShuffleDeck ? 1 : 0);
                info.StartLp = (uint)config.StartLp;
                info.StartHand = (byte)config.StartHand;
                info.DrawCount = (byte)config.DrawCount;
                info.GameTimer = (ushort)config.GameTimer;
            }
            return info;
        }
        public static void SendRoomList(this RoomServer roomServer, Session session)
        {

            lock (roomServer.DuelServers)
            {
                foreach (DuelServer srv in roomServer.DuelServers)
                {
                    lock (srv.Rooms)
                    {
                        foreach (GameConfig game in srv.Rooms.Values)
                        {
                            using (PacketWriter packet = new PacketWriter(2))
                            {
                                packet.Write((ushort)RoomMessage.NETWORK_CLIENT_ID);
                                StocHostPacket hp = new StocHostPacket();
                                hp.port = (ushort)srv.Port;
                                hp.host = game.ToHostInfo();
                                hp.name = new char[20];
                                char[] name = game.HasPassword() ? game.RoomString.ToCharArray() : game.Name.ToCharArray();
                                Array.Copy(name, hp.name, Math.Min(20, name.Length));
                                if (game.IsStart)
                                {
                                    int i = 20;
                                    while (--i > 4) hp.name[i] = hp.name[i - 5];
                                    hp.name[0] = '【';
                                    hp.name[1] = '对';
                                    hp.name[2] = '战';
                                    hp.name[3] = '中';
                                    hp.name[4] = '】';
                                }
                                packet.Write(StructTransformer.StructToBytes(hp));
                                session.Send(packet.Content, false);
                            }
                        }
                    }
                    session.PeekSend();
                }
            }

        }
        public static void OnRoomList(this RoomServer roomServer, Session session, bool nolock = false, bool nostart = false)
        {
#if DEBUG
			Logger.Debug("roomlist");
#endif
            using (PacketWriter wrtier = new PacketWriter(20))
            {
                int count = 0;
                wrtier.Write((byte)RoomMessage.RoomList);
                wrtier.Write(0);
                lock (roomServer.DuelServers)
                {
                    foreach (DuelServer srv in roomServer.DuelServers)
                    {
                        lock (srv.Rooms)
                        {
                            foreach (GameConfig config in srv.Rooms.Values)
                            {
                                count++;
                                wrtier.Write(srv.Port);
                                wrtier.Write(srv.NeedAuth);
                                wrtier.WriteUnicode(config.Name, 20);
                                wrtier.WriteUnicode(config.BanList, 20);
                                wrtier.WriteUnicode(config.RoomString, 20);
                                wrtier.Write(config.IsStart);
                            }
                        }
                    }
                }
                //Override the length
                wrtier.SetPosition(1);
                wrtier.Write(count);
                session.Send(wrtier.Content);
            }
        }
        #endregion

        #region msg
        public static void SendServerMsg(this Session session, string msg)
        {
            using (PacketWriter writer = new PacketWriter(2))
            {
                writer.Write((byte)RoomMessage.Chat);
                writer.WriteUnicode("", 20);
                writer.WriteUnicode("", 20);
                writer.WriteUnicode(msg, msg.Length + 1);
                session.Send(writer);
            }
        }
        public static void OnChatMessage(this RoomServer roomServer, string name, string toname, string msg)
        {
            using (PacketWriter writer = new PacketWriter(2))
            {
                writer.Write((byte)RoomMessage.OnChat);
                writer.Write((short)0x11);
                writer.WriteUnicode(name + ":" + msg);
                if (string.IsNullOrEmpty(toname))
                {
                    Logger.Debug("send to client");
                    roomServer.SendAllClient(writer.Content, name: name);
                }
            }
            using (PacketWriter writer = new PacketWriter(2))
            {
                writer.Write((byte)RoomMessage.Chat);
                writer.WriteUnicode(name, 20);
                writer.WriteUnicode(toname, 20);
                writer.WriteUnicode(msg, msg.Length + 1);
                if (!string.IsNullOrEmpty(toname))
                {
                    lock (roomServer.Clients)
                    {
                        Session sender = null;
                        if (roomServer.Clients.TryGetValue(name, out sender))
                        {
                            sender.Send(writer.Content, true);
                        }
                        else
                        {
#if DEBUG
							Console.WriteLine("no find "+name);
#endif
                        }
                        if (name != toname)
                        {
                            Session recevicer = null;
                            if (roomServer.Clients.TryGetValue(toname, out recevicer))
                            {
                                recevicer.Send(writer.Content, true);
                            }
                            else
                            {
                                if (sender != null)
                                {
                                    sender.SendError("[err]" + toname + " Not online。");
                                }
#if DEBUG
								Console.WriteLine("no find "+toname);
#endif
                            }
                        }
                    }
                }
                else
                {
                    roomServer.SendAll(writer.Content, true, true);
                }
            }
        }
        public static void OnServerStop(this RoomServer roomServer)
        {
            using (PacketWriter writer = new PacketWriter(2))
            {
                writer.Write((byte)RoomMessage.ServerStop);
                roomServer.SendAll(writer.Content);
            }
        }
        public static void OnError(this RoomServer roomServer, string err)
        {
            using (PacketWriter writer = new PacketWriter(2))
            {
                writer.Write((byte)RoomMessage.Error);
                writer.WriteUnicode(err, err.Length + 1);
                roomServer.SendAll(writer.Content);
            }
        }
        public static void SendError(this Session session, string err)
        {
            using (PacketWriter writer = new PacketWriter(2))
            {
                writer.Write((byte)RoomMessage.Error);
                writer.WriteUnicode(err, err.Length + 1);
                session.Send(writer.Content);
            }
        }
        private static void SendAllClient(this RoomServer roomServer, byte[] data, bool isNow = true, bool Force = false, string name = null)
        {
            lock (roomServer.GameCliens)
            {
                foreach (Session client in roomServer.GameCliens.Values)
                {
                    if (client.CanGameChat)
                    {
                        if (string.IsNullOrEmpty(name) || (!string.IsNullOrEmpty(name) && client.Name != name))
                        {
                            client.Send(data, isNow);
                        }
                    }

                }
            }
        }
        private static void SendAll(this RoomServer roomServer, byte[] data, bool isNow = true, bool Force = false)
        {
            lock (roomServer.Clients)
            {
                foreach (Session client in roomServer.Clients.Values)
                {
                    if (Force || !client.IsPause)
                    {
                        client.Send(data, isNow);
                    }
                }
            }
        }

        #endregion

        #region room
        public static void server_OnRoomCreate(this RoomServer roomServer, DuelServer server, string name, string banlist, string gameinfo)
        {
            using (PacketWriter writer = new PacketWriter(2))
            {
                writer.Write((byte)RoomMessage.RoomCreate);
                writer.Write(server.Port);
                writer.Write(server.NeedAuth);
                writer.WriteUnicode(name, 20);
                writer.WriteUnicode(banlist, 20);
                writer.WriteUnicode(gameinfo, gameinfo.Length + 1);
                //start duel
                writer.Write(false);
                roomServer.SendAll(writer.Content);
            }
        }

        public static void server_OnRoomStart(this RoomServer roomServer, DuelServer server, string name)
        {
            using (PacketWriter writer = new PacketWriter(2))
            {
                writer.Write((byte)RoomMessage.RoomStart);
                writer.Write(server.Port);
                writer.WriteUnicode(name, 20);
                roomServer.SendAll(writer.Content);
            }
        }


        public static void server_OnRoomClose(this RoomServer roomServer, DuelServer server, string name)
        {
            using (PacketWriter writer = new PacketWriter(2))
            {
                writer.Write((byte)RoomMessage.RoomClose);
                writer.Write(server.Port);
                writer.WriteUnicode(name, 20);
                roomServer.SendAll(writer.Content);
            }
        }
        #endregion

        #region player
        public static void OnPlayerList(this Session client)
        {
            lock (client.Server.Clients)
            {
                using (PacketWriter writer = new PacketWriter(2))
                {
                    writer.Write((byte)RoomMessage.PlayerList);
                    writer.Write(client.Server.Clients.Count);
                    foreach (Session session in client.Server.Clients.Values)
                    {
                        writer.Write(session.ServerInfo == null ? 0 : session.ServerInfo.Port);
                        writer.WriteUnicode(session.Name, 20);
                        writer.WriteUnicode(session.RoomName, 20);
                    }
                    client.Send(writer.Content);
                }
            }
        }
        public static void server_OnPlayerLeave(this RoomServer roomServer, DuelServer server, string name, string room)
        {
            if (server != null && string.IsNullOrEmpty(room))
            {
                lock (server.AsyncLock)
                {
                    server.Count--;
                }
            }
            if (string.IsNullOrEmpty(name)) return;

            lock (roomServer.Clients)
            {
                Session player = null;
                if (roomServer.Clients.TryGetValue(name, out player))
                    player.RoomName = null;
            }
            using (PacketWriter writer = new PacketWriter(2))
            {
                writer.Write((byte)RoomMessage.PlayerLeave);
                writer.Write(server == null ? 0 : server.Port);
                writer.WriteUnicode(name, 20);
                writer.WriteUnicode(room, 20);
                roomServer.SendAll(writer.Content);
            }
        }

        public static void server_OnPlayerJoin(this RoomServer roomServer, DuelServer server, string name, string room)
        {
            if (string.IsNullOrEmpty(name)) return;
            lock (roomServer.Clients)
            {
                Session player = null;
                if (roomServer.Clients.TryGetValue(name, out player))
                    player.RoomName = room;
            }
            using (PacketWriter writer = new PacketWriter(2))
            {
                writer.Write((byte)RoomMessage.PlayerEnter);
                writer.Write(server.Port);
                writer.WriteUnicode(name, 20);
                writer.WriteUnicode(room, 20);
                roomServer.SendAll(writer.Content);
            }
        }
        #endregion
    }
}
