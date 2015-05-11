using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using MineLib.Core;

using ProtocolTrueCraft.Packets;
using ProtocolTrueCraft.Packets.Client.Login;

namespace ProtocolTrueCraft
{
    public sealed partial class Protocol
    {
        private Dictionary<Type, Func<ISendingAsyncArgs, Task>> SendingAsyncHandlers { get; set; }

        public void RegisterSending(Type sendingAsyncType, Func<ISendingAsyncArgs, Task> func)
        {
            var any = sendingAsyncType.GetTypeInfo().ImplementedInterfaces.Any(p => p == typeof(ISendingAsync));
            if (!any)
                throw new InvalidOperationException("AsyncSending type must implement MineLib.Network.IAsyncSending");

            SendingAsyncHandlers[sendingAsyncType] = func;
        }
        
        private void RegisterSupportedSendings()
        {
            RegisterSending(typeof(ConnectToServerAsync), ConnectToServerAsync);
            RegisterSending(typeof(KeepAliveAsync), KeepAliveAsync);
        }

        public Task DoSendingAsync(Type sendingAsyncType, ISendingAsyncArgs args)
        {
            var any = sendingAsyncType.GetTypeInfo().ImplementedInterfaces.Any(p => p == typeof(ISendingAsync));
            if (!any)
                throw new InvalidOperationException("AsyncSending type must implement MineLib.Network.IAsyncSending");

            return SendingAsyncHandlers[sendingAsyncType](args);
        }

        public void DoSending(Type sendingType, ISendingAsyncArgs args)
        {
            var any = sendingType.GetTypeInfo().ImplementedInterfaces.Any(p => p == typeof(ISendingAsync));
            if (!any)
                throw new InvalidOperationException("AsyncSending type must implement MineLib.Network.IAsyncSending");

            SendingAsyncHandlers[sendingType](args).Wait();
        }



        private async Task ConnectToServerAsync(ISendingAsyncArgs args)
        {
            var data = (ConnectToServerAsyncArgs) args;

            State = ConnectionState.Joining;

            await SendPacketAsync(new HandshakePacket
            {
                Username = _minecraft.ClientUsername
            });

            await SendPacketAsync(new LoginRequestPacket { ProtocolVersion = 14, Username = "-" });
        }

        private Task KeepAliveAsync(ISendingAsyncArgs args)
        {
            var data = (KeepAliveAsyncArgs) args;

            return SendPacketAsync(new KeepAlivePacket());
        }
    }
}
