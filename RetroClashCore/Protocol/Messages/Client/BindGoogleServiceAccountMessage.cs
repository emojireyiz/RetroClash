﻿using System.Threading.Tasks;
using RetroClashCore.Helpers;
using RetroClashCore.Logic;
using RetroClashCore.Protocol.Messages.Server;

namespace RetroClashCore.Protocol.Messages.Client
{
    public class BindGoogleServiceAccountMessage : PiranhaMessage
    {
        public BindGoogleServiceAccountMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        public override void Decode()
        {
            Reader.ReadString();
        }

        public override async Task Process()
        {
            await Resources.Gateway.Send(new GoogleServiceAccountBoundMessage(Device));
        }
    }
}