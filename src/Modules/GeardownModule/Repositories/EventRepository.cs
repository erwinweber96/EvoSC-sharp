namespace EvoSC.Modules.Official.GeardownModule.Repositories;

using EvoSC.Modules.Official.GeardownModule.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class EventRepository
{
  private GeardownHttpClient _client;

  public EventRepository(GeardownHttpClient geardownHttpClient)
  {
    _client = geardownHttpClient;
  }

  public async Task<bool> UpdateStatus(int eventId, EventStatus statusId)
  {
    String response = "";

    try {
      response = await _client.Put("/v1/events/status", new [] {
        new KeyValuePair<string, int>("eventId", eventId),
        new KeyValuePair<string, int>("statusId", ((int)statusId))
      });
    } catch (Exception) {
      return false;
    }
    
    dynamic data = JObject.Parse(response);

    if (data.message) {
      return true;
    }

    return false;
  }

  public async Task<List<Participant>> GetParticipants(int eventId)
  {
    String response = await _client.Get("/v1/events/participants", new [] {
      new KeyValuePair<string, string>("eventId", eventId.ToString())
    });
      
    return JsonConvert.DeserializeObject<List<Participant>>(response);
  }
}
