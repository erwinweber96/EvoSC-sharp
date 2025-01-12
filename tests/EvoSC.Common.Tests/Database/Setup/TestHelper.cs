﻿using System.Threading.Tasks;
using EvoSC.Common.Database.Repository.Players;
using EvoSC.Common.Interfaces.Database;
using EvoSC.Common.Interfaces.Models;
using GbxRemoteNet.Structs;

namespace EvoSC.Common.Tests.Database.Setup;

public static class TestHelper
{
    public static async Task<IPlayer> AddTestPlayer(IDbConnectionFactory factory, string accountId)
    {
        var playerRepo = new PlayerRepository(factory);
        
        return await playerRepo.AddPlayerAsync(accountId, new TmPlayerDetailedInfo
        {
            NickName = "TestAccount",
            Path = "World"
        });
    }

    public static Task<IPlayer> AddTestPlayer(IDbConnectionFactory factory) => AddTestPlayer(factory, "TestAccountId");
}
