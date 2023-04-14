namespace EvoSC.Modules.Official.GeardownModule.Models;

public class MatchResult
{
  public int? id {get; set; }
  public string? result {get; set;}
  public Boolean? is_total_result {get; set;}
  public int? match_id {get;set;}
  public Boolean? pending {get; set;}
  public string? created_at { get; set; }
  public string? updated_at { get; set; }
}
