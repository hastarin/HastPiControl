namespace HastPiControl.IFTTT
{
    using System.Collections.Generic;
    using System.Net.Http;

    /// <summary>
    /// Handle interaction with IFTTT Maker channel
    /// </summary>
    public class Maker
    {
        private readonly string secretKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Maker(string secretKey)
        {
            this.secretKey = secretKey;
        }

        /// <summary>
        /// Sends the event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public async void SendEvent(string eventName)
        {
            using (var client = new HttpClient())
            {
                var url = "https://maker.ifttt.com/trigger/" + eventName + "/with/key/" + this.secretKey;

                var response = await client.PostAsync(url, null);
                var responseString = await response.Content.ReadAsStringAsync();
            }
        }
    }
}
