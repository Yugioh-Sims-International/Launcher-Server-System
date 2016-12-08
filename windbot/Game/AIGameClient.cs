﻿using System;
using System.Net;
using System.Text;
using System.IO;
using WindBot.Game;
using WindBot.Game.AI;
using WindBot.Game.Data;
using WindBot.Game.Network;
using OcgWrapper.Enums;

namespace WindBot.Game
{
	public class AIGameClient
	{
		public AIGameConnection Connection { get; private set; }
		public string Username;
		public string Deck_;

		private string _serverHost;
		private int _serverPort;
		private string _roomInfos;

		private GameBehavior _behavior;

		public AIGameClient(string username, int serverPort, string roomInfos)
			:this(username, "localhost", serverPort,roomInfos)
		{
		}
		public AIGameClient(string pass, string serverHost, int serverPort, string roomInfos)
		{
			_serverHost = serverHost;
			_serverPort = serverPort;
			_roomInfos = roomInfos;
			Random random=new Random(Environment.TickCount);
			DirectoryInfo dir=new DirectoryInfo("Decks/");
			FileInfo[] files=dir.GetFiles("*.ydk");
			string deck;
			if(files.Length>0){
				int index=random.Next(files.Length);
				deck=files[index].Name;
			}else{
				deck = "";
			}
			Deck_=deck.Replace(".ydk", "");
			Username = "[AI]"+Deck_;
			if(!string.IsNullOrEmpty(pass)){
				int i = pass.IndexOf("$");
				if(i >= 0){
					pass = pass.EndsWith("$")?"":pass.Substring(i+1);
				}
				Username += "$"+pass;
			}
			Console.WriteLine(Username+" use deck is "+Deck_);
		}

		public void Start()
		{
			Connection = new AIGameConnection(IPAddress.Parse(_serverHost), _serverPort);
			_behavior = new GameBehavior(this);

			GameClientPacket packet = new GameClientPacket(CtosMessage.PlayerInfo);
			packet.Write(Username, 20);
			Connection.Send(packet);

			byte[] junk = { 0xCC, 0xCC, 0x00, 0x00, 0x00, 0x00 };
			packet = new GameClientPacket(CtosMessage.JoinGame);
			packet.Write((short)Program.ProVersion);
			packet.Write(junk);

			packet.Write(_roomInfos, 30);

			
			Deck deck = Deck.Load(Deck_);
			if(deck == null) {
				Logger.WriteLine("read deck fail.");
				Connection.Close();
				return;
			}

			packet.Write((byte)CtosMessage.UpdateDeck);

			packet.Write(deck.Cards.Count);
			packet.Write(deck.SideCards.Count);

			foreach (var k in deck.Cards) {
				packet.Write(k.Id);
			}

			foreach (var k in deck.SideCards) {
				packet.Write(k.Id);
			}

			packet.Write((byte)CtosMessage.HsReady);
			Connection.Send(packet);
			Logger.WriteLine("send deck");
		}

		public void Tick()
		{
			if (!Connection.IsConnected)
			{
				return;
			}
			while (Connection.HasPacket())
			{
				GameServerPacket packet = Connection.Receive();
				_behavior.OnPacket(packet);
			}
		}

		public void Chat(string message)
		{
			byte[] content = Encoding.Unicode.GetBytes(message + "\0");
			GameClientPacket chat = new GameClientPacket(CtosMessage.Chat);
			chat.Write(content);
			Connection.Send(chat);
		}
	}
}