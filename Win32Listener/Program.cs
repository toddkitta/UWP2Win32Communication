using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Win32Listener
{
    public class Program
    {
        private static readonly string[] prefixes = null;

        static Program()
        {
            prefixes = new string[] { "http://localhost:54321/" };
        }

        public static void Main(string[] args)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Nope...");
                return;
            }

            HttpListener listener = new HttpListener();
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }

            listener.Start();

            Console.CancelKeyPress += delegate
            {
                listener.Stop();
            };

            while (true)
            {
                Console.WriteLine("Listening...");

                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                string responseMessage = $"I heard you... you said '{request.Headers["MyCustomHeader"]}'";

                byte[] buffer = Encoding.UTF8.GetBytes(responseMessage);
                Console.WriteLine(responseMessage);

                response.ContentLength64 = buffer.Length;
                using (Stream output = response.OutputStream)
                {
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
            }
        }
    }
}
