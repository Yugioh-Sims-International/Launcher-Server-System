﻿using System;
using AsyncServer;
using System.Diagnostics;

namespace YGOCore
{
	public class ServerProcess
	{
		private Process process;
		public bool isRunning{get;private set;}
        public string Name
        {
            get { return "GameServer:" + m_port; }
        }
        
		private string m_fileName;
		private int m_port;
		private int m_aptport;
		private string m_config;
        private IntPtr m_window;
		public int Port{get{return m_port;}}
		public ServerProcess(int port,int apiPort,string fileName="GameServer.exe", string config="config.txt")
		{
			this.m_port = port;
			this.m_aptport=apiPort;
			this.m_fileName = fileName;
			this.m_config = config;
		}
        private void GetWindow()
        {
            if (m_window != IntPtr.Zero) return;
            string title = Name;
            m_window = User32.FindConsoleWindow(title);
            if(m_window == IntPtr.Zero)
            {
                Logger.Warn("no find window:"+ title);
            }
        }
        public bool Show()
        {
            GetWindow();
            if (m_window != IntPtr.Zero)
            {
                return User32.ShowWindow(m_window);
            }
            return false;
        }
        public bool Hide()
        {
            GetWindow();
            if (m_window != IntPtr.Zero)
            {
                return User32.HideWindow(m_window);
            }
            return false;
        }
		public void Start(){
			if(isRunning)return;
			isRunning = true;
			if(process==null||process.HasExited){
				process=new Process();
			}
			process.StartInfo.FileName = m_fileName;
			//Set the execution of the program parameters
			process.StartInfo.Arguments = " "+m_config + " "+m_port+" "+m_aptport+" true";
			process.EnableRaisingEvents=true;
			process.StartInfo.WindowStyle=ProcessWindowStyle.Hidden;
			process.Exited+=new EventHandler(Exited);
			try{
				process.Start();
			}catch(Exception e){
				Logger.Error(e);
			}
		}
		private void Exited(object sender, EventArgs e){
			if(isRunning){
				Close();
				//Abnormal end
				Start();
			}else{
				Close();
			}
		}
		public void Close(){
			if(!isRunning)return;
			isRunning = false;
			if(process!=null){
				try{
					process.Kill();
				}catch(Exception){
					
				}finally{
					try{
						process.Close();
					}catch{}
					process = null;
				}
			}
		}
	}
}
