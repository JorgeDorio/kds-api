namespace Kds.Models;

public class StartEndReport
{
    public StartEndReport(string stationId)
    {
        StationId = stationId;
        StartedAt = DateTime.Now;
    }

    public string StationId { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
}