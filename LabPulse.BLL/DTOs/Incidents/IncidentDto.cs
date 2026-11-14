namespace LabPulse.BLL.DTOs.Incidents
{
    public class IncidentDto
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = null!;
        public DateTime StartedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public bool IsOpen { get; set; }
        public long DurationSeconds { get; set; }
        public string Reason { get; set; } = null!;
    }
}
