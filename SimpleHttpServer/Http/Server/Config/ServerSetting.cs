using System;
using System.Text.RegularExpressions;
using System.IO;
namespace Http.Server.Config{

    public  class ServerSetting {
        public static string RootDirectory{ get;private set;}


        public string ServerHttpProtocol{get;set;}        
        public string ServerIP { get; set;}
        public int ServerPort { get; set;}
        private ServerSetting(){}

        private static ServerSetting For(string address){

            string protocol = Regex.Match(address,@"http|https").ToString();
            string ip = Regex.Match(address,@"[0-9]{3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}").ToString();
            int port = Convert.ToInt32(Regex.Match(address,@":[0-9]{1,5}").ToString().Replace(":",""));
           return new ServerSetting(){
               ServerHttpProtocol = protocol,
               ServerIP = ip,
               ServerPort = port
            };
        } 

        public static implicit operator string(ServerSetting setting){
            return string.Format("{0}://{1}:{2}",setting.ServerHttpProtocol,setting.ServerIP,setting.ServerPort);
        }
        public static explicit operator ServerSetting(string address){
            return For(address);
        }
        public static void SetRootDirectory(string directory){
            RootDirectory = string.Format(@"{0}/www",directory);
            if(!Directory.Exists(RootDirectory))
                Directory.CreateDirectory(RootDirectory);
        }
    } 
}