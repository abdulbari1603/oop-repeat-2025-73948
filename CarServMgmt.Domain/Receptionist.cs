using System.Collections.Generic;

namespace CarServMgmt.Domain;

public class Receptionist : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
} 