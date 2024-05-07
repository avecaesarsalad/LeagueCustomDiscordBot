﻿using DSharpPlus.Entities;

namespace LeagueCustomBot
{
    public class ChannelManager
    {
        private static ChannelManager? _instance;
        public ulong? RedTeamChannelId;
        public ulong? BlueTeamChannelId;
        
        public static ChannelManager GetInstance()
        {
            return _instance ??= new ChannelManager();
        }
        
    }
}