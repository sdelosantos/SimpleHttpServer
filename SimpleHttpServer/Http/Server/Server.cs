using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Http.Server.Config;
using System.IO;
using SimpleHttpServer.Web;
using Http.Structure;
namespace Http.Server{
    public class Server{
        public  static ServerSetting setting {get;private set;}
        private  static Socket server {get;set;}
        private Server(string address){
            server = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            setting = (ServerSetting)address;
            ServerSetting.SetRootDirectory(Directory.GetCurrentDirectory());
        }

        // set the server settings
        public static Server Initialize(string address){
            Server serv = new Server(address);
            return serv;
        }
        //start server
        public void Run(){
            server.Bind(getEndPoint(setting));
            Console.Write("\n***************************************************************************************************");
            Console.WriteLine("\nListening for connection on: {0}",(string)setting);
            Console.Write("Server root directory: {0}",ServerSetting.RootDirectory);
            Console.WriteLine("\n**********************************************************************************************");

            AcceptClient();
        }
        // startup client listening
        private void AcceptClient(){
            server.Listen(1);
            Request clientRequest = new Request(server);
            clientRequest.ClientRequestEvent+=ClientRequestHandler;
            clientRequest.WaitForClientRequest();
        }

        // when some client is connected, generate response
        protected Response ClientRequestHandler(HttpMessage request){
            Response resp = new Response();
            try{
                resp.LoadDataResourse(request.UrlResource);
            }catch(Exception ex){
                resp.ErrorResponse(ex.Message);
            }
            return resp;
        }
        
        // set ip where server is running
        private EndPoint getEndPoint(ServerSetting setting){
            return new IPEndPoint(IPAddress.Parse(setting.ServerIP),setting.ServerPort);
        }

    }
}