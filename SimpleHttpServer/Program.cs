using System;
using System.Text.RegularExpressions;
using Http.Server;
using Http.Server.Config;
using Http.Structure;

namespace SimpleHttpServer
{
    class Program
    {
        static void Main(string[] args)
        {

            string address = "http://127.0.0.1:8591";
            Server server = Server.Initialize(address);
            server.Run();
            Console.ReadKey();
        }
    }
}
z