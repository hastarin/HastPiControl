namespace HastPiControl.AutoRemote.Communications
{
    using System;

    public class Request : Communication
    {
        public int ttl { get; set; }
        public String collapsekey { get; set; }

        /// <summary>
        /// Executes the request. Is different for every type of request. The default is to just respond with the response ResponseNoAction 
        /// </summary>
        /// <returns></returns>
        public virtual Response ExecuteRequest()
        {
            return new ResponseNoAction();
        }

        protected override string GetGCMEndpoint()
        {
            return "sendrequest";
        }
    }
}
