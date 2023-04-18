using EvoSC.Commands.Attributes;
using EvoSC.Commands.Interfaces;
using EvoSC.Common.Controllers;
using EvoSC.Common.Controllers.Attributes;
using EvoSC.Common.Controllers.Context;
using EvoSC.Common.Interfaces;
using EvoSC.Common.Interfaces.Database.Repository;
using EvoSC.Common.Interfaces.Models;
using EvoSC.Common.Interfaces.Services;
using EvoSC.Common.Util.MatchSettings;
using EvoSC.Common.Util.ServerUtils;
using EvoSC.Modules.Official.GeardownModule.Models;
using EvoSC.Modules.Official.GeardownModule.Repositories;
using EvoSC.Modules.Official.Maps.Services;

namespace EvoSC.Modules.Official.GeardownModule;

[Controller]
public class GeardownController : EvoScController<PlayerInteractionContext>
{
    private readonly IServerClient _server;
    private readonly IChatCommandManager _chatCommands;
    private readonly IPermissionManager _permissions;
    private readonly IPermissionRepository _permRepo;
    private readonly IMapRepository _mapRepo;
    private readonly IMatchSettingsService _matchSettings;
    private readonly MatchRepository _matchRepository;
    private readonly MxMapService _mapService;

    public GeardownController(IChatCommandManager cmds, IServerClient server,
        IChatCommandManager chatCommands, IPermissionManager permissions, IPermissionRepository permRepo,
        IMapRepository mapRepo, IMatchSettingsService matchSettings, MatchRepository matchRepository, MxMapService mapService)
    {
        _server = server;
        _chatCommands = chatCommands;
        _permissions = permissions;
        _permRepo = permRepo;
        _mapRepo = mapRepo;
        _matchSettings = matchSettings;
        _matchRepository = matchRepository;
        _mapService = mapService;
    }

    [ChatCommand("geardown_init", "Init match from Geardown.gg using a match token.")]
    public async Task GeardownInit(string matchToken)
    {
        Match match = await _matchRepository.getMatchDataByToken(matchToken);

        if (match.formats == null || match.formats.Count() == 0) {
            await _server.SendChatMessageAsync("Error: Please add a format to the match on geardown.gg", Context.Player);
            return;
        }

        Format format = match.formats[0];

        if (format.match_settings == null || format.match_settings.Count() == 0) {
            await _server.SendChatMessageAsync("Error: The match format does not have any match settings. Configure them in the formats menu on the geardown event page.", Context.Player);
            return;
        }

        DefaultModeScriptName modeScriptName = this.getModeScriptByFormatType(format.type_id ?? FormatType.OTHER);

        if (match.map_pool_orders == null || match.map_pool_orders.Count() == 0) {
            await _server.SendChatMessageAsync("Error: The match does not have any map pool orders. Configure the map pool orders in the geardown match page.", Context.Player);
            return;
        }

        var maps = new IMap[]{};
        IMap? map = null;

        foreach (MapPoolOrder mapPoolOrder in match.map_pool_orders)
        {
            //TODO: order by mapOrder
            if (mapPoolOrder.order == 0) {
                continue;
            }

            if (mapPoolOrder.map_pool_id == null) {
                continue;
            }

            try {
                map = await _mapService.FindAndDownloadMapAsync(mapPoolOrder.mx_map_id ?? 0, null, Context.Player);
            } catch (Exception) {
                await _server.SendChatMessageAsync("Error: Could not download map.", Context.Player);
                continue;
            }

            if (map == null) {
                System.Console.WriteLine("Map is null.");
                continue;
            }

            maps.Append(map);
        }

        await _matchSettings.CreateMatchSettingsAsync("geardown", builder => {
            builder
                .AddMap(map)
                .WithMode(modeScriptName)                
                .WithModeSettings(modeSettings =>
                    {
                        foreach (var matchSetting in format.match_settings)
                        {
                            if (matchSetting.key == null) {
                                continue;                
                            }

                            int valueAsNumber = 0;
                            bool result = int.TryParse(matchSetting.value, out valueAsNumber);

                            if (result) {
                                modeSettings[matchSetting.key] = valueAsNumber;
                            } else {
                                modeSettings[matchSetting.key] = matchSetting.value;
                            }                            
                        }
                    });
        });

        await _matchSettings.LoadMatchSettingsAsync("geardown");
        await _server.Remote.NextMapAsync();
    }

    private DefaultModeScriptName getModeScriptByFormatType(FormatType formatType)
    {
        switch (formatType) {
            case FormatType.TIME_ATTACK:
                return DefaultModeScriptName.TimeAttack;
            case FormatType.CUP:
                return DefaultModeScriptName.Cup;
            case FormatType.ROUNDS:
                return DefaultModeScriptName.Rounds;
            default:
                return DefaultModeScriptName.TimeAttack;
        }
    }
}
