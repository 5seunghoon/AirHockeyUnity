using System;

namespace seunghoon
{
    [Serializable]
    public class IpModel
    {
        public string hostIp;
        public string clientIp;

        IpModel(string hostIp, string clientIp)
        {
            this.hostIp = hostIp;
            this.clientIp = clientIp;
        }
    }
}