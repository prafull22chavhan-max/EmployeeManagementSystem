EmployeeManageMentSystem

1. Restore the .NET project:
   dotnet restore EmployeeManageMentSystem\EmployeeManageMentSystem.csproj

2. Install and build the Angular frontend:
   cd ClientApp
   npm install
   npm run build
   cd ..

3. Run the application:
   dotnet run --project EmployeeManageMentSystem\EmployeeManageMentSystem.csproj

The Angular production build is emitted to the ASP.NET Core wwwroot folder.
The app uses the configured SQL Server connection string and applies EF migrations at startup.
The former MVC pages are available under /legacy.

If the Entity Framework CLI is not installed globally, use:
   dotnet tool install --global dotnet-ef
