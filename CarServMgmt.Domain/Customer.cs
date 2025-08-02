using System.Collections.Generic;

namespace CarServMgmt.Domain;

public class Customer : BaseEntity



{
                             
                             
                              public string Name { get; set; } = string.Empty;
  
  
  
    public string Email { get; set; } = string.Empty;
                                            
                                            
                                            
                                            public ICollection<Car> Cars { get; set; } = new List<Car>();
} 