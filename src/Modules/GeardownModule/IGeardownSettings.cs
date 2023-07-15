namespace EvoSC.Modules.Official.GeardownModule;


using Config.Net;
using EvoSC.Modules.Attributes;


[Settings]
public interface IGeardownSettings
{
    [Option(DefaultValue = false)]
    public bool MatchBegin { get; set; }
}
