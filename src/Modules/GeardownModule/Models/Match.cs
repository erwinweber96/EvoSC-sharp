namespace EvoSC.Modules.Official.GeardownModule.Models;

public class Match
{
  public int? id { get; set; }
  public string? name { get; set; } 
  public int? status_id {get; set; }
  public string? status {get; set; }
  public int? group_id { get; set; } //not used
  public int? map_pool_id { get; set; } //not used
  public string? date { get; set; }
  public string? created_at { get; set; }
  public string? updated_at { get; set; }
  public List<Participant>? participants { get; set; }
  public List<MatchResult>? results {get; set; }
  public List<Format>? formats {get; set; }
  public GameServer? selectedGameServer {get; set; }
  public List<MapPoolOrder>? map_pool_orders {get; set; }
}
