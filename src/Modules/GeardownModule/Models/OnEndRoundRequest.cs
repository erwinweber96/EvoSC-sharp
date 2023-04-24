namespace EvoSC.Modules.Official.GeardownModule.Models;

using EvoSC.Common.Remote.EventArgsModels;

public class OnEndRoundRequest
{
    public string matchToken {get; set;}
    public ScoresEventArgs eventData {get; set;}
}