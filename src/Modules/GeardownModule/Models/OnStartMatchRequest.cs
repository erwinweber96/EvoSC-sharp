namespace EvoSC.Modules.Official.GeardownModule.Models;

using EvoSC.Common.Remote.EventArgsModels;

public class OnStartMatchRequest
{
    public string matchToken {get; set;}
    public string join {get; set;} // Join Instructions. e.g. Club Name | Server #1
}