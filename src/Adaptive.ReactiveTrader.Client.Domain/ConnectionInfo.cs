﻿using Adaptive.ReactiveTrader.Client.Domain.Transport;

namespace Adaptive.ReactiveTrader.Client.Domain
{
    public class ConnectionInfo
    {
        public ConnectionStatus ConnectionStatus { get; }
        public string Server { get; }
        public string TransportName { get; }

        public ConnectionInfo(ConnectionStatus connectionStatus, string server, string transportName)
        {
            ConnectionStatus = connectionStatus;
            Server = server;
            TransportName = transportName;
        }

        public override string ToString()
        {
            return $"ConnectionStatus: {ConnectionStatus}, Server: {Server}, TransportName:{TransportName}";
        }
    }
}