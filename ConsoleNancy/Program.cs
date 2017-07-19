using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Hosting.Self;
using Newtonsoft.Json;
using System.Net;

namespace Nancy
{
    class Program
    {
        private string _url = "http://localhost";
        private int _port = 12345;
        private NancyHost _nancy;

        public Program()
        {
            IPHostEntry host;
            string hostName = System.Environment.MachineName;
            hostName = Dns.GetHostName();
  
            host = Dns.GetHostEntry(hostName);

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    //System.Diagnostics.Debug.WriteLine("LocalIPadress: " + ip);
                    Console.WriteLine(ip.ToString());
                }
            }

            var configuration = new HostConfiguration()
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true }
            };

            var uri = new Uri($"{_url}:{ _port}/");
            _nancy = new NancyHost(configuration, uri);
        }

        private void Start()
        {

            _nancy.Start();
            Console.WriteLine($"Started listenning port {_port}");
            Console.ReadKey();
            _nancy.Stop();
        }

        static void Main(string[] args)
        {
            var p = new Program();
            p.Start();

        }
    }

    public class SimpleModule : NancyModule
    {
        public SimpleModule()
        {
            Get["/"] = _ => "Received GET request";
            Post["/"] = _ =>
            {
                //return "Received POST request";
                var id = this.Request.Body;
                var length = this.Request.Body.Length;
                var data = new byte[length];
                id.Read(data, 0, (int)length);
                var body = System.Text.Encoding.Default.GetString(data);

                var request = JsonConvert.DeserializeObject<SimpleRequest>(body);

                return 200;
            };
            Get["/user/{id}"] = parameters =>
            {
                if (((int)parameters.id) == 666)
                {
                    return $"ALL hail user #{parameters.id}! \\m/";
                }
                else
                {
                    return "Just a regular user!";
                }
            };
        }
    }

    public class SimpleRequest
    {
        public string username { get; set; }

        public string isAdmin { get; set; }
    }
}
