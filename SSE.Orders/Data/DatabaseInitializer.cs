using Microsoft.Data.Sqlite;

namespace SSE.Orders.Data;

public class DatabaseInitializer(IConfiguration configuration, ILogger<DatabaseInitializer> logger)
{
    private IConfiguration _configuration = configuration;
    private ILogger<DatabaseInitializer> _logger = logger;
    
    public void Initialize()
    {
        _logger.LogInformation("Initializing database...");
        var builder = new SqliteConnectionStringBuilder(_configuration.GetConnectionString("default"));
        string databaseFilePath = builder.DataSource;

        if (!File.Exists(databaseFilePath))
        {
            _logger.LogInformation("Database file not found. Creating a new database.");
            CreateDatabase();
        }
        else
        {
            _logger.LogInformation("Database file found. No need to create a new database.");
        }
    }

    private void CreateDatabase()   
    {
        try
        {
            using var connection = new SqliteConnection(_configuration.GetConnectionString("default"));
            connection.Open();
            const string createTableSql = @"
                CREATE TABLE IF NOT EXISTS Orders (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    OrderNumber VARCHAR(50) NOT NULL,
                    OrderDate DATETIME NOT NULL,
                    ProductDescription VARCHAR(100) NOT NULL,
                    ShippingAddresss VARHCAR(100) NOT NULL
                );
            ";
        
            using var command = new SqliteCommand(createTableSql, connection);
            command.ExecuteNonQuery();
        }
        catch (SqliteException e)
        {
            _logger.LogError("Error while creating the tables. ");
            _logger.LogError("{Message}",e.Message);
        }
       
    }
}