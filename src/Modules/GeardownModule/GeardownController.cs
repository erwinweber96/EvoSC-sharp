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
using EvoSC.Modules.Official.GeardownModule.Services;
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
    private readonly IMapService _mapService;
    private readonly GeardownMapService _geardownMapService;
    private IGeardownSettings _geardownSettings;

    public GeardownController(IChatCommandManager cmds, IServerClient server,
        IChatCommandManager chatCommands, IPermissionManager permissions, IPermissionRepository permRepo,
        IMapRepository mapRepo, IMatchSettingsService matchSettings, MatchRepository matchRepository, IMapService mapService,
        GeardownMapService geardownMapService, IGeardownSettings geardownSettings)
    {
        _server = server;
        _chatCommands = chatCommands;
        _permissions = permissions;
        _permRepo = permRepo;
        _mapRepo = mapRepo;
        _matchSettings = matchSettings;
        _matchRepository = matchRepository;
        _mapService = mapService;
        _geardownMapService = geardownMapService;
        _matchRepository = matchRepository;
        _geardownSettings = geardownSettings;
    }

    [ChatCommand("geardown_init", "Init match from Geardown.gg using a match token.")]
    public async Task GeardownInit(string matchToken)
    {
        _geardownSettings.MatchBegin = false;
        Match match = await _matchRepository.getMatchDataByToken(matchToken);

        if (match.formats == null || match.formats.Count() == 0)
        {
            await _server.SendChatMessageAsync("Error: Please add a format to the match on geardown.gg", Context.Player);
            return;
        }

        Format format = match.formats[0];

        if (format.match_settings == null || format.match_settings.Count() == 0)
        {
            await _server.SendChatMessageAsync("Error: The match format does not have any match settings. Configure them in the formats menu on the geardown event page.", Context.Player);
            return;
        }

        DefaultModeScriptName modeScriptName = this.getModeScriptByFormatType(format.type_id ?? FormatType.OTHER);

        if (match.map_pool_orders == null || match.map_pool_orders.Count() == 0)
        {
            await _server.SendChatMessageAsync("Error: The match does not have any map pool orders. Configure the map pool orders in the geardown match page.", Context.Player);
            return;
        }

        var maps = new List<IMap> {};
        IMap? map = null;

        foreach (MapPoolOrder mapPoolOrder in match.map_pool_orders)
        {
            if (mapPoolOrder.order == 0)
            {
                continue;
            }

            if (mapPoolOrder.map_pool_id == null)
            {
                continue;
            }

            var metadata = await _geardownMapService.getMap(mapPoolOrder.mx_map_id ?? 0, null);

            if (metadata == null)
            {
                continue;
            }

            //check if already exists
            map = await _mapService.GetMapByUidAsync(metadata.MapUid);

            //if exists, just add to list
            if (map == null) {
                try
                {
                    var mapStream = await _geardownMapService.FindAndDownloadMapAsync(mapPoolOrder.mx_map_id ?? 0, null, Context.Player);
                    
                    if (mapStream == null) {
                        continue;
                    }

                    map = await _mapService.AddMapAsync(mapStream);
                }
                catch (Exception)
                {
                    await _server.SendChatMessageAsync("Error: Could not download map.", Context.Player);
                    continue;
                }
            }

            if (map == null) {
                continue;
            } 

            maps.Add(map);
        }

        var rand = new Random();
        var shuffledList = maps.OrderBy(_ => rand.Next()).ToList();
        await _matchSettings.CreateMatchSettingsAsync("geardown", builder =>
            {
                builder
                    .AddMaps(shuffledList)
                    .WithMode(modeScriptName)
                    .WithModeSettings(modeSettings =>
                        {
                            foreach (var matchSetting in format.match_settings)
                            {
                                if (matchSetting.key == null)
                                {
                                    continue;
                                }

                                int valueAsNumber = 0;
                                bool result = int.TryParse(matchSetting.value, out valueAsNumber);

                                if (result)
                                {
                                    modeSettings[matchSetting.key] = valueAsNumber;
                                }
                                else
                                {
                                    modeSettings[matchSetting.key] = matchSetting.value;
                                }
                            }
                        });
            }
        );

        await _matchSettings.LoadMatchSettingsAsync("geardown");
        //await _server.Remote.NextMapAsync();
        
        //TODO: load from config or make sure server name is descriptive as the where it can be found.
        string serverName = await _server.Remote.GetServerNameAsync(); 
        _matchRepository.OnStartMatch(serverName);
    }

    private DefaultModeScriptName getModeScriptByFormatType(FormatType formatType)
    {
        switch (formatType)
        {
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
