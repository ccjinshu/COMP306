




Create C# Model Class from exists database

Database name: lab3_movie_web

PM console:
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Design

# install AWS SDK -  DynamoDB  - for lastest version
Install-Package AWSSDK.DynamoDBv2 -Version
Install-Package AWSSDK.Extensions.NETCore.Setup -Version 3.3.100

# install AWS SDK -  S3  - for lastest version
Install-Package AWSSDK.S3 -Version 3.3.100


dotnet tool install --global dotnet-ef

dotnet ef dbcontext scaffold "YourConnectionString" Microsoft.EntityFrameworkCore.SqlServer -o Models






Scaffold-DbContext "Server=.;Database=lab3_movie_web;User ID=sa;Password=qq12345665;TrustServerCertificate=true;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models