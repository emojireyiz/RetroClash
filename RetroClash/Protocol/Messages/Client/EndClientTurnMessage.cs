﻿using System;
using System.Threading.Tasks;
using RetroClash.Extensions;
using RetroClash.Logic;
using RetroClash.Protocol.Messages.Server;

namespace RetroClash.Protocol.Messages.Client
{
    public class EndClientTurnMessage : PiranhaMessage
    {
        public EndClientTurnMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        public int Count { get; set; }
        public int SubTick { get; set; }

        public override void Decode()
        {
            SubTick = Reader.ReadInt32(); // Tick
            Reader.ReadInt32(); // Checksum

            Count = Reader.ReadInt32();
        }

        public override async Task Process()
        {
            if (Count <= 512 && Count >= 0)
            {
                if (Count > 0)
                    using (var reader =
                        new Reader(Reader.ReadBytes((int) (Reader.BaseStream.Length - Reader.BaseStream.Position))))
                    {
                        for (var index = 0; index < Count; index++)
                        {
                            var type = reader.ReadInt32();

                            if (LogicCommandManager.Commands.ContainsKey(type))
                                try
                                {
                                    if (Activator.CreateInstance(LogicCommandManager.Commands[type], Device, reader) is LogicCommand
                                        command)
                                    {
                                        command.SubTick = SubTick;
                                        command.Decode();
                                        await command.Process();

                                        command.Dispose();

                                        Logger.Log($"Command {type} with SubTick {SubTick} has been processed.", Enums.LogType.Debug);
                                    }
                                }
                                catch (Exception exception)
                                {
                                    Logger.Log(exception, Enums.LogType.Error);
                                }
                            else
                                Logger.Log($"Command {type} is unhandled.", Enums.LogType.Warning);
                        }
                    }
            }
            else
            {
                await Resources.Gateway.Send(new OutOfSyncMessage(Device));
            }
        }
    }
}