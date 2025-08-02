namespace CarServMgmt.Domain;

public class Car : BaseEntity
{
    public string RegistrationNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<ServiceRecord> ServiceRecords { get; set; } = new List<ServiceRecord>();
} 