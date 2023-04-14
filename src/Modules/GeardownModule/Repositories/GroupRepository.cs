namespace EvoSC.Modules.Official.GeardownModule.Repositories;

using EvoSC.Modules.Official.GeardownModule.Models;
using Newtonsoft.Json;

public class GroupRepository
{
    private GeardownHttpClient _client;

    public GroupRepository(GeardownHttpClient geardownHttpClient)
    {
      _client = geardownHttpClient;
    }

    public async Task<List<Participant>> GetParticipants(int groupId)
    {
      String response = await _client.Get("/v1/groups/participants", new [] { 
        new KeyValuePair<string, string>("groupId", groupId.ToString()) 
      });

      return JsonConvert.DeserializeObject<List<Participant>>(response);
    }

    public async Task<Group> CreateGroup(string name, int eventId, bool isTypeTree = false)
    {
      String response = await _client.Post("/v1/groups/create", new object[] { 
        new KeyValuePair<string, string>("name", name), 
        new KeyValuePair<string, int>("eventId", eventId), 
        new KeyValuePair<string, bool>("isTypeTree", isTypeTree),
      });

      return JsonConvert.DeserializeObject<Group>(response);
    }

    public async Task<Group> UpdateGroupParticipants(int groupId, List<Participant> participants)
    {
      String response = await _client.Put("/v1/groups/participants", new object[] {
        new KeyValuePair<string, int>("groupId", groupId),
        new KeyValuePair<string, List<Participant>>("participants", participants)
      });

      return JsonConvert.DeserializeObject<Group>(response);
    }
}
