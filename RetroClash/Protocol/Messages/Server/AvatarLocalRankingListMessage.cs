﻿using System.IO;
using System.Threading.Tasks;
using RetroClash.Database;
using RetroClash.Extensions;
using RetroClash.Logic;

namespace RetroClash.Protocol.Messages.Server
{
    public class AvatarLocalRankingListMessage : Message
    {
        public AvatarLocalRankingListMessage(Device device) : base(device)
        {
            Id = 24404;
        }

        public override async Task Encode()
        {
            var count = 0;

            using (var buffer = new MemoryStream())
            {
                foreach (var player in await MySQL.GetLocalPlayerRanking(Device.Player.Language))
                {
                    await buffer.WriteLongAsync(player.AccountId);
                    await buffer.WriteStringAsync(player.Name);

                    await buffer.WriteIntAsync(count + 1);
                    await buffer.WriteIntAsync(player.Score);
                    await buffer.WriteIntAsync(200);

                    await player.AvatarRankingEntry(buffer);

                    if (count++ >= 199)
                        break;
                }

                await Stream.WriteIntAsync(count);
                await Stream.WriteBufferAsync(buffer.ToArray());
            }
        }
    }
}