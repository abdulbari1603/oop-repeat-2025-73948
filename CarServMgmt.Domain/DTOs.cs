namespace CarServMgmt.Domain;

public class CustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class CarDto
{
    public int Id { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
}

public class MechanicDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class ReceptionistDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class ServiceRecordDto
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public int MechanicId { get; set; }
    public string Description { get; set; } = string.Empty;
    public double Hours { get; set; }
    public bool IsComplete { get; set; }
    public DateTime DateBroughtIn { get; set; }
    public DateTime? DateCompleted { get; set; }
    public decimal TotalCost { get; set; }
} 