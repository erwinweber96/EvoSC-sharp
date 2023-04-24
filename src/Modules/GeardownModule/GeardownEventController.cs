using EvoSC.Common.Controllers;
using EvoSC.Common.Controllers.Attributes;
using EvoSC.Common.Controllers.Context;
using EvoSC.Common.Events.Attributes;
using EvoSC.Common.Remote;
using EvoSC.Common.Remote.EventArgsModels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using EvoSC.Modules.Official.GeardownModule.Repositories;

namespace EvoSC.Modules.Official.GeardownModule;

[Controller]
public class GeardownEventController : EvoScController<EventControllerContext>
{
    private readonly ILogger<GeardownEventController> _logger;

    private MatchRepository _matchRepository;
    
    public GeardownEventController(ILogger<GeardownEventController> logger, MatchRepository matchRepository)
    {
        _logger = logger;
        _matchRepository = matchRepository;
    }
    
    [Subscribe(ModeScriptEvent.Scores)]
    public Task OnScores(object sender, ScoresEventArgs args)
    {
        var json = JsonConvert.SerializeObject(args);
        System.Console.WriteLine(json);
        
        string? section = args.Section;

        if (section == null) {
            return Task.CompletedTask;
        }

        switch(section) {
            case "EndRound":
                System.Console.WriteLine("End Round Case");
                _matchRepository.OnEndRound(args);
                break;
            case "EndMap":
                System.Console.WriteLine("End Map Case");
                //_matchRepository.OnEndMap(); TODO: fix for case when map ends before warmup starts
                break;
            default:
                //idk
                break;
        }
        
        _logger.LogInformation("Scores: {Scores}", args.ToString());
        return Task.CompletedTask;
    }
}
