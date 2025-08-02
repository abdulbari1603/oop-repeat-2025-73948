using System.Collections.Generic;

namespace CarServMgmt.Domain;

public class Mechanic : BaseEntity
{
    public string Name { get; set; } = string.Empty;


                       public string Email { get; set; } = string.Empty;
               public ICollection<ServiceRecord> ServiceRecords { get; set; } = new List<ServiceRecord>();
} 