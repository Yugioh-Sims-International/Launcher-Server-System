﻿using System;
using System.Threading;
using WindBot.Game;
using WindBot.Game.AI;
using WindBot.Game.Data;
using System.IO;
using System.Diagnostics;
using OcgWrapper.Managers;

namespace WindBot
{
	public class Program
	{
		public static int ProVersion = 0x1337;

		public static Random Rand;
		public static bool Replay{get;private set;}

		public static void Main(string[] args)
		{
			//Does not turn on
			Replay=false;
			if(args.Length < 3)
			{
				Console.WriteLine("String pass, String serverIP, int serverPort,int version,String room,String deck");
				return;
			}
			try
			{
				if(args.Length==3){
					Run(args[0], args[1], int.Parse(args[2]), ProVersion);
				}else if(args.Length==4){
					Run(args[0], args[1], int.Parse(args[2]), int.Parse(args[3]));
				}else if(args.Length==5){
					Run(args[0], args[1], int.Parse(args[2]), int.Parse(args[3]), args[4]);
				}else{
					Run(args[0], args[1], int.Parse(args[2]), int.Parse(args[3]), args[4], args[5]);
				}
			}
			catch (Exception ex)
			{
				Logger.WriteLine("Error: " + ex);
			}
		}

		private static void Run(String pass, String serverIP, int serverPort,int version,String room="", string cdb="cards.cdb")
		{
			ProVersion = version;
			if(pass=="--"){
				pass="";
			}
			if(room=="[null]"){
				room="";
			}
			Console.WriteLine(pass+" "+serverIP+":"+serverPort+" 0x"+version.ToString("x")+" room:"+room);
			Rand = new Random();
			CardsManager.Init(cdb);
			DecksManager.Init();

			// Start two clients and connect them to the same room. Which deck is gonna win?
			AIGameClient clientA = new AIGameClient(pass, serverIP, serverPort, room);
			clientA.Start();
			while (clientA.Connection.IsConnected)
			{
				clientA.Tick();
				Thread.Sleep(1);
			}
			//Thread.Sleep(3000);
		}
	}
}
