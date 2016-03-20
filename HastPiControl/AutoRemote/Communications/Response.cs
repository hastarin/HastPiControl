namespace HastPiControl.AutoRemote.Communications
{
    using System;

    public class Response : Communication
    {
        //If the request resulted in error, set the error here
        public String responseError { get; set; }
        protected override string GetGCMEndpoint()
        {
            return "sendresponse";
        }
    }
}
