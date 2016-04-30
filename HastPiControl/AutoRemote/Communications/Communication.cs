// ***********************************************************************
// Assembly         : HastPiControl
// Author           : Jon Benson
// Created          : 20-03-2016
// 
// Last Modified By : Jon Benson
// Last Modified On : 26-03-2016
// ***********************************************************************

// ReSharper disable InconsistentNaming
namespace HastPiControl.AutoRemote.Communications
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;

    using HastPiControl.AutoRemote.Devices;

    using Newtonsoft.Json;

    /// <summary>Class Communication.</summary>
    public class Communication
    {
        /// <summary>The HTTP client</summary>
        private readonly HttpClient httpClient = new HttpClient();

        /// <summary>Initializes a new instance of the <see cref="Communication" /> class.</summary>
        public Communication()
        {
            this.communication_base_params = new CommunicationBaseParams { type = this.GetType().Name };
        }

        /// <summary>Gets or sets the key.</summary>
        public string key { get; set; }

        /// <summary>Gets the sender.</summary>
        public string sender { get; set; }

        /// <summary>Gets or sets the communication_base_params.</summary>
        public CommunicationBaseParams communication_base_params { get; set; }

        /// <summary>Gets the request from json.</summary>
        /// <param name="json">The json.</param>
        public static Request GetRequestFromJson(string json)
        {
            var comm = JsonConvert.DeserializeObject<Communication>(json);
            if (comm == null)
            {
                return new Request();
            }
            var typeString = "HastPiControl.AutoRemote.Communications." + comm.communication_base_params.type;
            Type type = Type.GetType(typeString);
            var autoRemoteRequest = (Request)JsonConvert.DeserializeObject(json, type);
            return autoRemoteRequest;
        }

        /// <summary>Sends the request. The text to send will vary with the request type.</summary>
        /// <param name="device">Device to send the request to</param>
        public async void Send(Device device)
        {
            this.key = device.key;
            if (device.port == null)
            {
                device.port = "1817";
            }
            var url = "http://" + device.localip + ":" + device.port + "/";

            this.sender = this.communication_base_params.sender;
            var dataString = JsonConvert.SerializeObject(this);
            var content = new StringContent(dataString);

            Debug.WriteLine("Sending through local ip");
            //send this as json object to localip
            Boolean success = await this.SendContent(url, content);

            //if it fails
            if (!success)
            {
                Debug.WriteLine("Couldn't send through local network. Sending through GCM");
                url = "https://autoremotejoaomgcd.appspot.com/" + this.GetGCMEndpoint();

                //To send though GCM we need to send the request as a form encoded content and add the key and sender parameters
                var postData = new List<KeyValuePair<string, string>>
                                   {
                                       new KeyValuePair<string, string>(
                                           "request",
                                           dataString),
                                       new KeyValuePair<string, string>(
                                           "key",
                                           this.key),
                                       new KeyValuePair<string, string>(
                                           "sender",
                                           this.sender)
                                   };

                var contentGCM = new FormUrlEncodedContent(postData);
                success = await this.SendContent(url, contentGCM);
                if (success)
                {
                    Debug.WriteLine("Sent through GCM");
                }
                else
                {
                    Debug.WriteLine("Couldn't send");
                }
            }
            else
            {
                Debug.WriteLine("Sent through local ip");
            }
        }

        /// <summary>Sends some content to a certain URL via HTTP POST</summary>
        /// <param name="url">Url to send to</param>
        /// <param name="content">Content to send</param>
        /// <returns>true if successful, false if not</returns>
        private async Task<Boolean> SendContent(String url, HttpContent content)
        {
            try
            {
                await this.httpClient.PostAsync(url, content);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>Gets the GCM endpoint.</summary>
        /// <returns>String.</returns>
        protected virtual string GetGCMEndpoint()
        {
            return null;
        }
    }
}