using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MemoryPenguin.CodeSync2.Server.Network
{
    class HttpServer
    {
        public int Port { get; private set; }
        public bool IsRunning
        {
            get
            {
                return listener.IsListening;
            }
        }

        private HttpListener listener;
        private Dictionary<string, Func<NameValueCollection, object>> routes;

        public HttpServer(int port)
        {
            Port = port;

            listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{port}/");
            routes = new Dictionary<string, Func<NameValueCollection, object>>();
        }

        public void Start()
        {
            listener.Start();

            while (listener.IsListening)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                StreamWriter writer = new StreamWriter(response.OutputStream);
                Response responseInfo = new Response(200, "OK");

                if (!request.IsLocal)
                {
                    responseInfo.StatusCode = 403;
                    responseInfo.StatusDescription = "Access denied";
                }
                else
                {
                    // Start one character after, since RawUrl starts with a /
                    string route = request.Url.LocalPath.Substring(1);

                    try
                    {
                        var responder = routes[route];

                        try
                        {
                            responseInfo.Data = responder(request.QueryString);
                            responseInfo.StatusCode = 200;
                        }
                        catch (Exception e)
                        {
                            responseInfo.StatusCode = 500;
                            responseInfo.StatusDescription = "Route errored";
                            responseInfo.Data = e.Message;
                            Console.WriteLine(e.ToString());
                        }
                    }
                    catch
                    {
                        responseInfo.StatusCode = 404;
                        responseInfo.StatusDescription = "Route not found";
                    }
                }

                response.StatusCode = responseInfo.StatusCode;
                response.StatusDescription = responseInfo.StatusDescription;
                writer.Write(JsonConvert.SerializeObject(responseInfo));

                writer.Close();
                response.Close();
            }
        }

        public void Stop()
        {
            listener.Stop();
        }

        public void Close()
        {
            listener.Close();
        }

        public void AddRoute(string route, Func<NameValueCollection, object> responder)
        {
            routes.Add(route, responder);
        }

        private struct Response
        {
            public int StatusCode { get; set; }
            public string StatusDescription { get; set; }
            public object Data { get; set; }

            public Response(int status, string response)
            {
                StatusCode = status;
                StatusDescription = response;
                Data = "";
            }
        }
    }
}
