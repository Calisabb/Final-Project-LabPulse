namespace LabPulse.DAL.DataContext.Entities
{
    public class Incident : BaseEntity
    {
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;
        public DateTime StartedAt { get; set; }
        public DateTime ResolvedAt { get; set; }
        public string Reason { get; set; } = null!;
    }
}
