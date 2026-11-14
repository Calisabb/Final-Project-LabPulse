namespace LabPulse.BLL.DTOs.Backup;

public class ImportResultDto
{
    public int CategoriesAdded { get; set; }
    public int CategoriesSkipped { get; set; }
    public int ServicesAdded { get; set; }
    public int ServicesSkipped { get; set; }
    public int RunbooksAdded { get; set; }
}