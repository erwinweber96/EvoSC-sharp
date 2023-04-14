namespace EvoSC.Modules.Official.GeardownModule.Models;

public class Format
{
  public int? id { get; set; }
  public string? name { get; set; } 
  public FormatType? type_id {get; set; }
  public string? description { get; set; } 
  public List<MatchSetting>? match_settings {get;set;}
}
