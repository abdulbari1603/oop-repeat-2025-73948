




## Features
- **Role-based authentication and authorization** (Admin,receptionist Mechanic, Customer)
- **CRUD operations** for Customers, Cars, and ServiceRecords
- **Service cost calculation** (€75 per hour, rounded up)
- **Sample data and user seeding**
- **Entity Framework Core with MySQL**
- **Unit test for business logic**


- ASP.NET Core MVC (.NET 8)
- Entity Framework Core 8 (Pomelo MySQL)
- ASP.NET Core Identity
- xUnit (for unit testing)
- MySQL

## How to Run
1. **Clone the repository**
2. **Configure MySQL**
   - Ensure MySQL is running on `localhost:3306` with user `root` and password `root` (or update `appsettings.json`)
3. **Restore and build**
   ```sh
   cd CarServMgmt.UI
   dotnet restore
   dotnet build
   ```
4. **Apply migrations and seed data**
   ```sh
   cd ..
   dotnet ef database update --project CarServMgmt.UI --startup-project CarServMgmt.UI
   ```
5. **Run the application**
   ```sh
    cd CarServMgmt.UI
   dotnet run 
   ```
6. **Login with sample credentials** (see below)

## Sample Logins
| Role      | Email                    | Password     |
|-----------|--------------------------|--------------|
| Admin     | admin@carservice.com     | Dorset001^   |
| Mechanic  | mechanic1@carservice.com | Dorset001^   |
| Mechanic  | mechanic2@carservice.com | Dorset001^   |
| Customer  | customer1@carservice.com | Dorset001^   |
| Customer  | customer2@carservice.com | Dorset001^   |
Receptionist	reception1@carservice.com	Dorset001^	
Receptionist	reception2@carservice.com	Dorset001^

7. **Testing** 
dotnet test CarServMgmt.tests



   
