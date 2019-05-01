using System;
using System.Text.RegularExpressions;
using Http.Structure;
using System.Reflection;
using System.Text;

namespace Http.Structure{
    public class HttpMessage{

        #region HttpInfo
            public string HttpVersion {get{return "HTTP/1.0";}}

            public string Method {get;set;}

            public string Status{get;set;}

            public int StatusCode{get;set;}

            public string ResourceURL{get;set;}
        #endregion

        #region Container Negotiation
            [HttpHeader("Accept-Language")]
            public string AcceptLanguage{get;set;}
            
            [HttpHeader("Accept")]
            public string Accept{get;set;}
        #endregion
        
        #region Autentication
            [HttpHeader("Authorization")]
            public string Authorization{get;set;}
        #endregion

        #region Response and Request Context
            [HttpHeader("Server")]
            public string Server{get;set;}

            [HttpHeader("Host")]
            public string Host{get;set;}

            [HttpHeader("Date")]
            public string Date{get;set;}

            [HttpHeader("Allow")]
            public string Allow{get;set;}

            [HttpHeader("User-Agent")]
            public string UserAgent{get;set;}

            public string UrlResource{get;set;}
        #endregion

        #region Message Body Content
            [HttpHeader("Content-Length")]
            public string ContentLength{get;set;}
        
            [HttpHeader("Content-Type")]
            public string ContentType{get;set;}

            public string MessageBodyResponse{get; protected set;}
        #endregion 

        #region Range Request
            [HttpHeader("Accept-Ranges")]
            public string AcceptRanges{get;set;}
            
            [HttpHeader("Range")]
            public string Range{get;set;}
        #endregion


        public static explicit operator HttpMessage(string query){
            return CastMessage(query);
        }
        public static implicit operator string(HttpMessage respMsg){
            return CastMessage(respMsg);
        }

        //cast string to HttpMessage
        private static HttpMessage CastMessage(string httpQuery){
            HttpMessage msg = new HttpMessage();
            httpQuery = httpQuery.Replace(@"\0","");
            msg.Method = Regex.Match(@httpQuery,@"GET|POST").Value;
            msg.Status = Regex.Match(@httpQuery,@"HTTP\/[0-9]\.[0-9]\s[0-9]{1,3}").Value.Replace(msg.HttpVersion,"");
            msg.UrlResource = GetResourceRequest(httpQuery);
           
            foreach(PropertyInfo pro in msg.GetType().GetProperties()){
                var attr = pro.GetCustomAttribute(typeof(HttpHeaderAttribute));
                HttpHeaderAttribute he = attr as HttpHeaderAttribute;
                if(he !=null){
                    if(!string.IsNullOrEmpty(he.headerName)){
                        var pattern = @he.headerName+@":\s(.)*(\r\n|;)";
                        var match = Regex.Match(httpQuery,pattern);
                        string value = string.IsNullOrEmpty(match.Value)?null:Regex.Replace(match.Value,@he.headerName+@":\s","");
                        if(value!=null)
                            value = value.Replace(@"\r\n","");

                        pro.SetValue(msg,value);
                    }
                }
            }
            return msg;
        }
        
        //cast HttpMessage to string
        private static string CastMessage(HttpMessage msg){
            string response = "";
             if(msg!=null){
                response = string.Format("{0} {1} {2}\r\n",msg.HttpVersion,msg.StatusCode,msg.Status);
                foreach(PropertyInfo pro in msg.GetType().GetProperties()){
                    var attr = pro.GetCustomAttribute(typeof(HttpHeaderAttribute));
                    HttpHeaderAttribute he = attr as HttpHeaderAttribute;
                    if(he !=null){
                        if(!string.IsNullOrEmpty(he.headerName)){
                            if(pro.GetValue(msg)!=null){
                                response+=string.Format("{0}:{1}\r\n",he.headerName,pro.GetValue(msg).ToString());
                            }
                        }
                    }
                }
                if(!string.IsNullOrEmpty(msg.MessageBodyResponse)){
                    response+="\r\n\r\n "+msg.MessageBodyResponse;
                }
            }
            return response;
        }
        
        // get url request in http query request
        private static string GetResourceRequest(string httpQueryRequest){
            string requestMethod = "";
            string resource = "";
            if(httpQueryRequest.Contains("GET"))
                requestMethod = "GET";
            else if(httpQueryRequest.Contains("POST"))
                requestMethod = "POST";
            httpQueryRequest = httpQueryRequest.Replace(string.Format("{0} ",requestMethod),"");
            resource = httpQueryRequest.Substring(0,httpQueryRequest.IndexOf(" "));
            return resource;
        }
    }
}