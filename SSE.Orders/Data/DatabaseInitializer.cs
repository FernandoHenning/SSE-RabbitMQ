using Microsoft.Data.Sqlite;

namespace SSE.Orders.Data;

public class DatabaseInitializer(IConfiguration configuration, ILogger<DatabaseInitializer> logger)
{
    public void Initialize()
    {
        logger.LogInformation("Initializing database...");
        var builder = new SqliteConnectionStringBuilder(configuration.GetConnectionString("default"));
        string databaseFilePath = builder.DataSource;

        if (!File.Exists(databaseFilePath))
        {
            logger.LogInformation("Database file not found. Creating a new database.");
            CreateDatabase();
        }
        else
        {
            logger.LogInformation("Database file found. No need to create a new database.");
        }
    }

    private void CreateDatabase()   
    {
        try
        {
            using var connection = new SqliteConnection(configuration.GetConnectionString("default"));
            connection.Open();
            const string createTableSql = @"
                CREATE TABLE IF NOT EXISTS Orders (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    OrderNumber VARCHAR(50) NOT NULL,
                    OrderDate DATETIME NOT NULL,
                    ProductDescription VARCHAR(100) NOT NULL,
                    ShippingAddress VARCHAR(100) NOT NULL
                );
            ";
        
            using var command = new SqliteCommand(createTableSql, connection);
            command.ExecuteNonQuery();
        }
        catch (SqliteException e)
        {
            logger.LogError("Error while creating the tables. ");
            logger.LogError("{Message}",e.Message);
        }
       
    }
}