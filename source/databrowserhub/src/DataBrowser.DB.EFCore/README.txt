Install required dependecies:
	dotnettool install --global dotnet-ef    
	dotnetadd package Microsoft.EntityFrameworkCore.Design

Generate Migration Script:
	dotnet ef migrations add NameScriptToCreate --context DatabaseContext --project DataBrowser.DB.EFCore\DataBrowser.DB.EFCore.csproj --startup-project WSHUB\WSHUB.csproj -v



We recommend that production apps not call Database.Migrate at application startup. Migrate shouldn't be called from an app that is deployed to a server farm. If the app is scaled out to multiple server instances, it's hard to ensure database schema updates don't happen from multiple servers or conflict with read/write access.

Database migration should be done as part of deployment, and in a controlled way. Production database migration approaches include:

Using migrations to create SQL scripts and using the SQL scripts in deployment.
Running dotnet ef database update from a controlled environment.