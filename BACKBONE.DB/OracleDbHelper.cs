using Dapper;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace BACKBONE.DB
{
    public class OracleDbHelper : IDBHelper
    {
        private readonly string _connectionString;

        public OracleDbHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection()
        {
            return new OracleConnection(_connectionString);
        }

        // 1. Query<T>
        public IEnumerable<T> Query<T>(string sql, object? parameters = null, CommandType? commandType = null)
        {
            using (var connection = CreateConnection())
            {
                return connection.Query<T>(sql ?? string.Empty, parameters, commandType: commandType);
            }
        }

        // 2. QueryAsync<T>
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, CommandType? commandType = null)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<T>(sql ?? string.Empty, parameters, commandType: commandType);
            }
        }

        // 3. QueryFirstOrDefault<T>
        public T QueryFirstOrDefault<T>(string sql, object? parameters = null, CommandType? commandType = null)
        {
            using (var connection = CreateConnection())
            {
                return connection.QueryFirstOrDefault<T>(sql, parameters, commandType: commandType);
            }
        }

        // 4. QuerySingleOrDefaultAsync<T>
        public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null, CommandType? commandType = null) where T : class
        {
            try
            {
                using (IDbConnection connection = new OracleConnection(_connectionString))
                {
                    if (connection.State != ConnectionState.Open)
                        await ((OracleConnection)connection).OpenAsync();

                    return await connection.QuerySingleOrDefaultAsync<T>(sql ?? string.Empty, parameters, commandType: commandType);
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine($"Oracle Error: {ex.Message}");
                throw; // Fixed re-throw warning by using simple throw
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw; // Fixed re-throw warning by using simple throw
            }
        }

        // 5. QuerySingleOrDefault<T>
        public T? QuerySingleOrDefault<T>(string sql, object? parameters = null, CommandType? commandType = null) where T : class
        {
            try
            {
                using (IDbConnection connection = new OracleConnection(_connectionString))
                {
                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    return connection.QuerySingleOrDefault<T>(sql ?? string.Empty, parameters, commandType: commandType);
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine($"Oracle Error: {ex.Message}");
                throw; // Fixed re-throw warning by using simple throw
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw; // Fixed re-throw warning by using simple throw
            }
        }

        // 6. Execute
        public int Execute(string sql, object? parameters = null, CommandType? commandType = null)
        {
            using (var connection = CreateConnection())
            {
                return connection.Execute(sql ?? string.Empty, parameters, commandType: commandType);
            }
        }

        // 7. ExecuteAsync
        public async Task<int> ExecuteAsync(string sql, object? parameters = null, CommandType? commandType = null)
        {
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(sql ?? string.Empty, parameters, commandType: commandType);
            }
        }

        // 8. Insert<T>
        public T Insert<T>(string sql, object? parameters = null, CommandType? commandType = null)
        {
            using (var connection = CreateConnection())
            {
                return connection.QuerySingleOrDefault<T>(sql, parameters, commandType: commandType);
            }
        }

        // 9. ExecuteTransaction
        public int ExecuteTransaction(IEnumerable<SqlCommandInfo> commands)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int rowsAffected = 0;
                        foreach (var command in commands)
                        {
                            rowsAffected += connection.Execute(command.Sql, command.Parameters, transaction: transaction, commandType: command.CommandType);
                        }
                        transaction.Commit();
                        return rowsAffected;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        // 10. ExecuteTransactionAsync
        public async Task<int> ExecuteTransactionAsync(List<SqlCommandInfo> commands)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var command in commands)
                        {
                            await connection.ExecuteAsync(command.Sql, command.Parameters, transaction: transaction);
                        }
                        transaction.Commit();
                        return commands.Count;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // 11. ExecuteStoredProcedure
        public IEnumerable<IDictionary<string, object>> ExecuteStoredProcedure(string storedProcedureName, object? parameters = null)
        {
            using (var connection = CreateConnection())
            {
                return connection.Query(storedProcedureName, parameters, commandType: CommandType.StoredProcedure).Select(x => (IDictionary<string, object>)x);
            }
        }

        // 11.5. ExecuteStoredProcedureWithRefCursor - New method to handle stored procedures with REF CURSOR output parameters
        public IEnumerable<IDictionary<string, object>> ExecuteStoredProcedureWithRefCursor(string storedProcedureName, string refCursorParamName, object? parameters = null)
        {
            using (var connection = CreateConnection())
            {
                // Ensure the connection is open
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                
                // Create Oracle-specific parameters
                var oracleConnection = connection as OracleConnection;
                if (oracleConnection == null)
                {
                    throw new InvalidOperationException("Connection must be an OracleConnection for REF CURSOR operations");
                }
                
                using (var command = oracleConnection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = storedProcedureName;
                    
                    // Add input parameters if provided
                    if (parameters != null)
                    {
                        var paramProperties = parameters.GetType().GetProperties();
                        foreach (var prop in paramProperties)
                        {
                            var param = new OracleParameter(prop.Name, prop.GetValue(parameters));
                            command.Parameters.Add(param);
                        }
                    }
                    
                    // Add the REF CURSOR output parameter
                    var cursorParam = new OracleParameter(refCursorParamName, OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(cursorParam);
                    
                    // Execute the command
                    command.ExecuteNonQuery();
                    
                    // Read the results from the REF CURSOR
                    var results = new List<IDictionary<string, object>>();
                    using (var reader = ((OracleRefCursor)cursorParam.Value).GetDataReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.GetValue(i);
                            }
                            results.Add(row);
                        }
                    }
                    
                    return results;
                }
            }
        }

        // 12. ExecuteScalar
        public int ExecuteScalar(string sql, object? parameters = null, CommandType? commandType = null)
        {
            using (var connection = CreateConnection())
            {
                return connection.ExecuteScalar<int>(sql, parameters, commandType: commandType);
            }
        }

        // 13. QueryMultiple<T>
        public IEnumerable<IEnumerable<T>> QueryMultiple<T>(string sql, object? parameters = null, CommandType? commandType = null)
        {
            using (var connection = CreateConnection())
            {
                using (var multi = connection.QueryMultiple(sql, parameters, commandType: commandType))
                {
                    var resultSets = new List<IEnumerable<T>>(); 
                    while (!multi.IsConsumed)
                    {
                        resultSets.Add(multi.Read<T>());
                    }
                    return resultSets;
                }
            }
        }

        // 14. ConvertCollectionToDataTable
        public DataTable ConvertCollectionToDataTable<T>(IEnumerable<T> collection)
        {
            var dataTable = new DataTable();
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (var item in collection)
            {
                var row = dataTable.NewRow();
                foreach (var prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }
    }
}