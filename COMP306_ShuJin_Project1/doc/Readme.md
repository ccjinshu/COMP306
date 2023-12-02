
更新数据库 PQSQL

PM> Add-Migration InitialCreatePqsql -OutputDir Data/Migrations_pgsql

PM> Update-Database


//結構變化時更新


Add-Migration update_db -OutputDir Data/Migrations_pgsql