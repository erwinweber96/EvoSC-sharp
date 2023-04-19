using EvoSC.Common.Controllers;
using EvoSC.Common.Controllers.Attributes;
using EvoSC.Common.Controllers.Context;
using EvoSC.Common.Events.Attributes;
using EvoSC.Common.Remote;
using EvoSC.Common.Remote.EventArgsModels;
using Microsoft.Extensions.Logging;

namespace EvoSC.Modules.Official.GeardownModule;

[Controller]
public class GeardownEventController : EvoScController<EventControllerContext>
{
    private readonly ILogger<GeardownEventController> _logger;
    
    public GeardownEventController(ILogger<GeardownEventController> logger)
    {
        _logger = logger;
    }
    
    [Subscribe(ModeScriptEvent.Scores)]
    public Task OnScores(object sender, ScoresEventArgs args)
    {
        _logger.LogInformation("Scores: {Scores}", args.ToString());
        return Task.CompletedTask;
    }
}
