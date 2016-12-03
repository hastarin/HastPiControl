namespace HastPiControl.Adafruit
{
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods for AdafruitIO.
    /// </summary>
    public static partial class AdafruitIOExtensions
    {
        private static string key;

        private static readonly Dictionary<string, List<string>> CustomHeaders = new Dictionary<string, List<string>>(); 

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public static string Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
                CustomHeaders.Clear();
                var data = new List<string> { value };
                CustomHeaders.Add("X-AIO-Key", data);
            }
        }        
    }
}
