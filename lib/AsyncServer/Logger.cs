using System;
using System.IO;
using System.Threading;

namespace System {
	public enum LogLevel:byte {
		Debug=0,
		Info=1,
		Warn=3,
		Error=4
	}
	
	public class Logger{
		static readonly byte[] _lock = new byte[0];
		static string ErrFile = "errors.log";
		static LogLevel sLevel = LogLevel.Debug;
		public static void SetLogLevel(int level){
			if(LogLevel.IsDefined(typeof(LogLevel), (byte)level)){
				sLevel = (LogLevel)level;
			}else{
				sLevel = LogLevel.Info;
			}
		}
		public static void SetLogLevel(LogLevel level){
			sLevel = level;
		}
		public static void SetErrorFile(string file){
			ErrFile = file;
		}
		public static void Debug(object obj, bool ignoreLevel=false){
			if(sLevel <= LogLevel.Debug || ignoreLevel){
				WriteLine(""+obj, ConsoleColor.Gray);
			}
		}
		public static void Info(object obj, bool ignoreLevel=false){
			if(sLevel <= LogLevel.Info || ignoreLevel){
				WriteLine(""+obj, ConsoleColor.White);
			}
		}
		public static void Warn(object obj, bool ignoreLevel=false){
			if(sLevel <= LogLevel.Warn || ignoreLevel){
				WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") +" "+obj, ConsoleColor.Yellow);
			}
		}
		public static void Error(object obj, bool ignoreLevel=false){
			if(sLevel <= LogLevel.Error || ignoreLevel){
				string str =DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") +" "+obj;
				WriteLine(str, ConsoleColor.Red);
				ThreadPool.QueueUserWorkItem(new WaitCallback(
					(object o)=>{
						lock(_lock)
							File.AppendAllText(ErrFile, str+"\n");
					}), str);
				
			}
		}
		private static void WriteLine(object obj, ConsoleColor color=ConsoleColor.White){
			ConsoleColor old=Console.ForegroundColor;
			Console.ForegroundColor=color;
			Console.WriteLine("" + obj);
			Console.ForegroundColor=old;
		}
	}
}
