﻿using EvoSC.Commands;
using EvoSC.Commands.Attributes;
using EvoSC.Common.Controllers;
using EvoSC.Common.Controllers.Attributes;
using EvoSC.Common.Interfaces.Models;
using EvoSC.Modules.Official.Player.Interfaces;

namespace EvoSC.Modules.Official.Player.Controllers;

[Controller]
public class PlayerCommandsController : EvoScController<CommandInteractionContext>
{
    private readonly IPlayerService _players;

    public PlayerCommandsController(IPlayerService players) => _players = players;

    [ChatCommand("kick", "Kick a player from the server.", ModPermissions.KickPlayer)]
    public Task KickPlayerAsync(IOnlinePlayer player) => _players.KickAsync(player, Context.Player);

    [ChatCommand("mute", "Mute a player from the chat.", ModPermissions.MutePlayer)]
    public Task MutePlayerAsync(IOnlinePlayer player) => _players.MuteAsync(player, Context.Player);
    
    [ChatCommand("unmute", "Un-mute player from the chat.", ModPermissions.MutePlayer)]
    public Task UnMutePlayerAsync(IOnlinePlayer player) => _players.UnmuteAsync(player, Context.Player);
    
    [ChatCommand("ban", "Ban and blacklist a player from the server.", ModPermissions.BanPlayer)]
    public Task BanPlayerAsync(IOnlinePlayer player) => _players.BanAsync(player, Context.Player);
    
    [ChatCommand("unban", "Remove a player from the ban- and blacklist.", ModPermissions.BanPlayer)]
    public Task UnbanPlayerAsync(string login) => _players.UnbanAsync(login, Context.Player);
}
