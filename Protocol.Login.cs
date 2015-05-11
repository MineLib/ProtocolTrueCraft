using MineLib.Core;

namespace ProtocolTrueCraft
{
    public sealed partial class Protocol
    { 
        public bool Login(string login, string password)
        {
            throw new ProtocolException("Connection: Login not supported.");
        }
        public bool Logout()
        {
            throw new ProtocolException("Connection: Logout not supported.");
        }
    }
}
