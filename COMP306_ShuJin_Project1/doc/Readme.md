
更新数据库 PQSQL

PM> Add-Migration InitialCreatePqsql -OutputDir Data/Migrations_pgsql

PM> Update-Database


//結構變化時更新



PM> Add-Migration update_db_v2 -OutputDir Data/Migrations_pgsql

PM> Update-Database