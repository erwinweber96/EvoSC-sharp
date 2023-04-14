using EvoSC.Modules.Attributes;
using EvoSC.Modules.Interfaces;

namespace EvoSC.Modules.Official.GeardownModule;

[Module(IsInternal = true)]
public class GeardownModule : EvoScModule, IToggleable
{
    public Task EnableAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisableAsync()
    {
        return Task.CompletedTask;
    }
}
