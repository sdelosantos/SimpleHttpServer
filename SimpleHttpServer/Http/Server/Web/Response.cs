using System;
using System.Net.Sockets;
using Http.Structure;
using Http.Server.Config;
using System.IO;
using System.Text;

namespace SimpleHttpServer.Web{
    public class Response:HttpMessage{
        
        // 
        public void LoadDataResourse(string resourceUrl){
            string contentType = "";
            this.MessageBodyResponse = readResource(resourceUrl,ref contentType);
            if(this.MessageBodyResponse!=null){
                this.StatusCode = 200;
                this.Status = "OK";
                this.ContentType = contentType;
                this.ContentLength = this.MessageBodyResponse.Length.ToString();
            }
            else{
                this.StatusCode = 404;
                this.Status = "Not Found";
                this.MessageBodyResponse = "Resource not found";
                this.ContentType = "text/html";
                this.ContentLength = "Resource not found".Length.ToString();
            }
        }
        //response error response
        public void ErrorResponse(string msg){
            this.MessageBodyResponse = msg;
            this.StatusCode = 500;
            this.Status = "Error";
        }
        // response status
        public void SetStatus(int statusCode,string statusMsg){
            this.MessageBodyResponse = null;
            this.StatusCode = statusCode;
            this.Status = statusMsg;
        }
        // read file request(Only html)
        private string readResource(string resourceUrl,ref string contenType){
            if(resourceUrl[resourceUrl.Length-1]=='/'){
                resourceUrl+="index.html";
            }
            string url = string.Format(@"{0}{1}",ServerSetting.RootDirectory,resourceUrl);
            if(File.Exists(url)){
                contenType = "text/html";
                return File.ReadAllText(url);
            }
            else
            return null;
        }
        // cast response to byte array
        public static byte[] GetByResponse(HttpMessage msg){
            return Encoding.ASCII.GetBytes((string)msg);
        }
    }
}