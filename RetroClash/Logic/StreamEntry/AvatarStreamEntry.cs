﻿using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RetroClash.Extensions;
using RetroClash.Logic.StreamEntry.Avatar;

namespace RetroClash.Logic.StreamEntry
{
    public class AvatarStreamEntry
    {
        [JsonProperty("type")]
        public int StreamEntryType { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("sender_id")]
        public long SenderAvatarId { get; set; }

        [JsonProperty("sender_name")]
        public string SenderName { get; set; }

        [JsonProperty("sender_lvl")]
        public int SenderLevel { get; set; }

        [JsonProperty("sender_league")]
        public int SenderLeagueType { get; set; }

        [JsonProperty("creation")]
        public DateTime CreationDateTime { get; set; }

        [JsonIgnore]
        public int AgeSeconds => (int) (DateTime.Now - CreationDateTime).TotalSeconds;

        public virtual async Task Encode(MemoryStream stream)
        {
            await stream.WriteLongAsync(Id); // StreamEntryId
            await stream.WriteLongAsync(SenderAvatarId); // SenderAvatarId
            await stream.WriteStringAsync(SenderName); // SenderName
            await stream.WriteIntAsync(SenderLevel); // SenderLevel
            await stream.WriteIntAsync(SenderLeagueType); // SenderLeagueType
            await stream.WriteIntAsync(AgeSeconds); // AgeSeconds
        }

        public AvatarStreamEntry CreatetreamEntryByType(int type)
        {
            AvatarStreamEntry entry;

            switch (type)
            {
                case 3:
                {
                    entry = new JoinAllianceResponseAvatarStreamEntry();
                    break;
                }

                case 4:
                {
                    entry = new AllianceInvationAvatarStreamEntry();
                    break;
                }

                case 5:
                {
                    entry = new AllianceKickOutStreamEntry();
                    break;
                }

                case 6:
                {
                    entry = new AllianceMailAvatarStreamEntry();
                    break;
                }

                case 9:
                {
                    entry = new DeviceLinkedStreamEntry();
                    break;
                }

                default:
                {
                    return null;
                }
            }

            return entry;
        }
    }
}