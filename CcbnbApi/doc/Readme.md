
更新数据库 PQSQL

PM> Add-Migration InitialCreatePqsql -OutputDir Data/Migrations_pgsql

PM> Update-Database




dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p jinshupasswd
dotnet dev-certs https --trust