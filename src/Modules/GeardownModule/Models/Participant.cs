namespace EvoSC.Modules.Official.GeardownModule.Models;

public class Participant
{
  public int? id { get; set; }
  public string? type { get; set; } //user|page
  public int? user_id { get; set; } 
  public int? page_id { get; set; } 
  public int event_id { get; set; }
  public string? created_at { get; set; }
  public string? updated_at { get; set; }
  public User? user { get; set; } //null in team formats
  public Page? page { get; set; } //null in solo mode
}
