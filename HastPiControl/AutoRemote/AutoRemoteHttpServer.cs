// ***********************************************************************
// Assembly         : HastPiControl
// Author           : Jon Benson
// Created          : 20-03-2016
// 
// Last Modified By : Jon Benson
// Last Modified On : 26-03-2016
// ***********************************************************************

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierWordIsNotInDictionary
namespace HastPiControl.AutoRemote
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Text;

    using Windows.Networking;
    using Windows.Networking.Connectivity;
    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;

    using HastPiControl.AutoRemote.Communications;

    using Newtonsoft.Json;

    /// <summary>Class AutoRemoteHttpServer.</summary>
    /// <seealso cref="System.IDisposable" />
    /// <remarks>Based on https://github.com/AutoApps/AutoRemote and converted for UWP usage.</remarks>
    public class AutoRemoteHttpServer : IDisposable
    {
        /// <summary>The buffer size</summary>
        private const uint BufferSize = 8192;

        /// <summary>The listener</summary>
        private StreamSocketListener listener;

        /// <summary>Initializes a new instance of the <see cref="AutoRemoteHttpServer" /> class.</summary>
        /// <param name="port">The port to listen on.</param>
        public AutoRemoteHttpServer(int port)
        {
            this.Port = port;
        }

        /// <summary>Gets my local IPs.</summary>
        public static IPAddress[] MyLocalIPs { get; } = NetworkInformation.GetHostNames()
            .Where(h => h.Type == HostNameType.Ipv4)
            .Select(h => new IPAddress(long.Parse(h.RawName)))
            .ToArray();

        /// <summary>Gets my public IP.</summary>
        public string MyPublicIP => null;

        /// <summary>Gets or sets the port to listen on.</summary>
        public int Port { get; set; }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            this.Stop();
        }
        /// <summary>Starts this instance listening on the <see cref="Port"/>.</summary>
        public async void Start()
        {
            this.listener = new StreamSocketListener();
            this.listener.ConnectionReceived += this.ListenerOnConnectionReceived;
            await this.listener.BindServiceNameAsync(this.Port.ToString());
        }

        /// <summary>Stops this instance.</summary>
        public void Stop()
        {
            this.listener.Dispose();
        }

        private async void ListenerOnConnectionReceived(
            StreamSocketListener sender,
            StreamSocketListenerConnectionReceivedEventArgs args)
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
            var autoRemoteResponse = new Response();
            try
            {
                var jsonData = lines.Last();
                var autoRemoteRequest = Communication.GetRequestFromJson(jsonData);
                this.RequestReceived?.Invoke(autoRemoteRequest,args.Socket.Information.RemoteAddress);
                autoRemoteResponse = autoRemoteRequest.ExecuteRequest();
            }
            catch (Exception)
            {
                autoRemoteResponse.responseError = "Unknown request type";
            }
            var jsonResponse = JsonConvert.SerializeObject(autoRemoteResponse);

            using (IOutputStream output = args.Socket.OutputStream)
            {
                using (Stream response = output.AsStreamForWrite())
                {
                    byte[] bodyArray = Encoding.UTF8.GetBytes(jsonResponse);
                    var bodyStream = new MemoryStream(bodyArray);

                    var header = "HTTP/1.1 200 OK\r\n" + $"Content-Length: {bodyStream.Length}\r\n"
                                 + "Connection: close\r\n\r\n";

                    byte[] headerArray = Encoding.UTF8.GetBytes(header);
                    await response.WriteAsync(headerArray, 0, headerArray.Length);
                    await bodyStream.CopyToAsync(response);
                    await response.FlushAsync();
                }
            }
        }

        /// <summary>Occurs when [request received].</summary>
        public event Action<Request,HostName> RequestReceived;
    }
}