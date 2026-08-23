namespace SchoolHub.Dto;

public class ScoreDto
{
    public int Id {  get; set; }
     
    public double Score { get; set; }

    public string Reason { get; set; } = string.Empty;

    public DateTime Date { get; set; }
}
