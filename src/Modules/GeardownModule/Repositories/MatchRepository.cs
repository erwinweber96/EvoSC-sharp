namespace EvoSC.Modules.Official.GeardownModule.Repositories;

using EvoSC.Modules.Official.GeardownModule.Models;
using EvoSC.Modules.Official.GeardownModule;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class MatchRepository
{
  private GeardownHttpClient _client;

  public MatchRepository(GeardownHttpClient geardownHttpClient)
  {
    _client = geardownHttpClient;
  }

  public async Task<List<Participant>> GetParticipants(int matchId)
  {
    String response = await _client.Get("/v1/matches/participants", new [] { 
      new KeyValuePair<string, string>("matchId", matchId.ToString()) 
    });

    return JsonConvert.DeserializeObject<List<Participant>>(response);
  }

  public async Task<bool> UpdateStatus(int matchId, MatchStatus statusId)
  {
    String response = "";

    try {
      response = await _client.Put("/v1/matches/" + matchId + "/status/" + statusId, new object {});
    } catch (Exception) {
      return false;
    }

    dynamic data = JObject.Parse(response);

    if (data.message) {
      return true;
    }

    return false;
  }

  public async Task<Match> UpdateParticipants(int matchId, List<Participant> participants)
  {
    String response = await _client.Put("/v1/matches/participants", new object [] { 
      new KeyValuePair<string, int>("matchId", matchId),
      new KeyValuePair<string, List<Participant>>("matchId", participants),
    });

    return JsonConvert.DeserializeObject<Match>(response);
  }

  public async Task<MatchResult> CreateMatchResult(int matchId, bool isTotalResult, int participantId, string result, bool pending)
  {
    String response = await _client.Post("/v1/matches/results", new object [] { 
      new KeyValuePair<string, int>("matchId", matchId),
      new KeyValuePair<string, bool>("isTotalResult", isTotalResult),
      new KeyValuePair<string, int>("participantId", participantId),
      new KeyValuePair<string, string>("result", result),
      new KeyValuePair<string, bool>("pending", pending),
    });

    return JsonConvert.DeserializeObject<MatchResult>(response);
  }

  public async Task<GameServer> AddGameServer(int matchId, string name, bool pending, string serverLink)
  {
    String response = await _client.Post("/v1/matches/game_servers", new object [] { 
      new KeyValuePair<string, int>("matchId", matchId),
      new KeyValuePair<string, string>("name", name),
      new KeyValuePair<string, bool>("pending", pending),
      new KeyValuePair<string, string>("serverLink", serverLink),
    });

    return JsonConvert.DeserializeObject<GameServer>(response);
  }

  public async Task<Match> getMatchDataByToken(string matchToken)
  {
    String response = await _client.Get("/api/matches/evo_token/"+matchToken, null);
    System.Console.WriteLine(response);
    return JsonConvert.DeserializeObject<Match>(response);
  }
}
