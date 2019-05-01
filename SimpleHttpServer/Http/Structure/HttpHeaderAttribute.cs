using System;

namespace Http.Structure{

    [AttributeUsage(AttributeTargets.Property)]
    public class HttpHeaderAttribute:Attribute{
        public string headerName{get;private set;}
        public HttpHeaderAttribute(string HeaderName){
            this.headerName = HeaderName;
        }
    }

}