namespace HastPiControl.AutoRemote
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Text;

    using Windows.Networking;
    using Windows.Networking.Connectivity;
    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;

    using HastPiControl.AutoRemote.Communications;

    using Newtonsoft.Json;

    public class AutoRemoteHttpServer : IDisposable
    {
        private const uint BufferSize = 8192;

        public static IPAddress[] MyLocalIPs
        {
            get
            {
                return NetworkInformation.GetHostNames()
                    .Where(h => h.Type == HostNameType.Ipv4)
                    .Select(h => new IPAddress(long.Parse(h.RawName)))
                    .ToArray();
            }
        }
        public String MyPublicIP
        {
            get
            {
                return null;
            }
        }
        public int Port { get; set; }
        private StreamSocketListener listener;
        public event Action UPNPFailed;

        public AutoRemoteHttpServer(int port)
        {
            this.Port = port;
            this.listener = new StreamSocketListener();
            this.listener.ConnectionReceived += this.ListenerOnConnectionReceived;
            this.listener.BindServiceNameAsync(port.ToString());
        }

        private async void ListenerOnConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            StringBuilder request = new StringBuilder();
            using (IInputStream input = args.Socket.InputStream)
            {
                byte[] data = new byte[BufferSize];
                IBuffer buffer = data.AsBuffer();
                uint dataRead = BufferSize;
                while (dataRead == BufferSize)
                {
                    await input.ReadAsync(buffer, BufferSize, InputStreamOptions.Partial);
                    request.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                    dataRead = buffer.Length;
                }
            }
            var lines = request.ToString().Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var autoRemoteRequest = Communication.GetRequestFromJson(lines.Last());
            this.RequestReceived?.Invoke(autoRemoteRequest);
            var autoRemoteResponse = autoRemoteRequest.ExecuteRequest();
            var jsonResponse = JsonConvert.SerializeObject(autoRemoteResponse);

            using (IOutputStream output = args.Socket.OutputStream)
            {
                using (Stream response = output.AsStreamForWrite())
                {
                    byte[] bodyArray = Encoding.UTF8.GetBytes(jsonResponse);
                    var bodyStream = new MemoryStream(bodyArray);

                    var header = "HTTP/1.1 200 OK\r\n" +
                                $"Content-Length: {bodyStream.Length}\r\n" +
                                    "Connection: close\r\n\r\n";

                    byte[] headerArray = Encoding.UTF8.GetBytes(header);
                    await response.WriteAsync(headerArray, 0, headerArray.Length);
                    await bodyStream.CopyToAsync(response);
                    await response.FlushAsync();
                }
            }
        }

        private void OnUPNPFailed()
        {
            if (this.UPNPFailed != null)
            {
                this.UPNPFailed();
            }
        }

        public void Dispose()
        { this.Stop(); }

        public void Stop()
        {
            this.listener.Dispose();
        }

        public event Action<Request> RequestReceived;
    }
}
