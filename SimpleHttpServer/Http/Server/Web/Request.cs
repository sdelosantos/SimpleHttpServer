using System;
using System.Net.Sockets;
using System.Text;
using Http.Structure;

namespace SimpleHttpServer.Web{
    public class Request{
        public delegate Response ClientRequest(HttpMessage msg);
        private Socket server{get;set;}
        public Request(Socket Server){
            server = Server;
        }

        //wait for client connection
        public void WaitForClientRequest(){
            while(true){
                Socket client = server.Accept();
                byte[] buffer = new byte[client.ReceiveBufferSize];
                client.Receive(buffer);
                if(ClientRequestEvent!=null){
                    try{
                         HttpMessage msg = (HttpMessage)GetMessage(buffer);
                         Response resp = ClientRequestEvent(msg);
                         client.Send(Response.GetByResponse(resp));
                    }catch(Exception ex){
                        var err = new Response();
                        err.ErrorResponse(ex.Message);
                        client.Send(Response.GetByResponse(err));
                    }
                    finally{
                        client.Close();
                    }
                }
             }
        }

        // get request message
        private string GetMessage(byte[] message){
            string msg = Encoding.ASCII.GetString(message);
            return msg;
        }

        // events
        public event ClientRequest ClientRequestEvent;
    }
}
