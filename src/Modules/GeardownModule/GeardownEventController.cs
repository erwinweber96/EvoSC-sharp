using EvoSC.Common.Controllers;
using EvoSC.Common.Controllers.Attributes;
using EvoSC.Common.Controllers.Context;
using EvoSC.Common.Events.Attributes;
using EvoSC.Common.Remote;
using EvoSC.Common.Remote.EventArgsModels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using EvoSC.Modules.Official.GeardownModule.Repositories;
using EvoSC.Common.Interfaces;

namespace EvoSC.Modules.Official.GeardownModule;

[Controller]
public class GeardownEventController : EvoScController<EventControllerContext>
{
    private readonly ILogger<GeardownEventController> _logger;

    private MatchRepository _matchRepository;

    private IGeardownSettings _geardownSettings;

    private readonly IServerClient _server;
    
    public GeardownEventController(ILogger<GeardownEventController> logger, MatchRepository matchRepository, IGeardownSettings geardownSettings, IServerClient server)
    {
        _logger = logger;
        _matchRepository = matchRepository;
        _geardownSettings = geardownSettings;
        _server = server;
    }
    
    [Subscribe(ModeScriptEvent.Scores)]
    public async Task OnScores(object sender, ScoresEventArgs args)
    {
        var json = JsonConvert.SerializeObject(args);
        System.Console.WriteLine(json);
        
        string? section = args.Section;

        if (section == null) {
            return; // Task.CompletedTask;
        }

        switch(section) {
            case "EndRound":
                System.Console.WriteLine("End Round Case");
                _matchRepository.OnEndRound(args);
                break;
            case "EndMap":
                System.Console.WriteLine("End Map Case");
                break;
            case "EndMatch":
                System.Console.WriteLine("End Match Case");
                _matchRepository.OnEndMatch();
                break;
            default:
                //idk
                break;
        }
        
        var mapData = await _server.Remote.GetCurrentMapInfoAsync();

        _logger.LogInformation("Scores: {Scores}", args.ToString());
        _logger.LogInformation("Current Map: {Data}", JsonConvert.SerializeObject(mapData));

        return;
        //return Task.CompletedTask;
    }
}
