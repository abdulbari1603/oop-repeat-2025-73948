using System;

namespace CarServMgmt.Domain;

public class ServiceRecord : BaseEntity



{
    public int CarId { get; set; }
                  public Car? Car { get; set; }
            public int MechanicId { get; set; }
                 public Mechanic? Mechanic { get; set; }
                                                 public string Description { get; set; } = string.Empty;
    public double Hours { get; set; }
                                        public bool IsComplete { get; set; }
    public DateTime DateBroughtIn { get; set; }
                                 public DateTime? DateCompleted { get; set; }

                    public decimal GetTotalCost()





    {
        var roundedHours = Math.Ceiling(Hours);
        return (decimal)roundedHours * 75m;
    }
} 