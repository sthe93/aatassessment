string sql = "SELECT TOP 1000000 * FROM received WHERE status = 1 ORDER BY re_ref";

// List to store the received items
List<received> results = new List<received>();

// ConcurrentBag for thread-safe storage of results
ConcurrentBag<received> concurrentResults = new ConcurrentBag<received>();

// List of sql nodes to query
IEnumerable<IConfigurationSection> SqlNodes = Program.Configuration.GetSection("ConnectionStrings")
    .GetSection("SqlNodes").GetChildren();

// Create a single database connection outside the loop
using (SqlConnection connection = new SqlConnection(ConnectionString))
{
    connection.Open();

    // Use Parallel.ForEach to process SQL nodes in parallel
    Parallel.ForEach(SqlNodes, Node =>
    {
        received[] result = DBQuery<received>.Query(Node.Value, sql);

        // Add the results to the concurrentResults
        foreach (var rec in result)
        {
            concurrentResults.Add(rec);
        }
    });

    // Now we can add the concurrentResults to the results list
    results.AddRange(concurrentResults);

    // Use a StringBuilder to construct the query
    StringBuilder queryBuilder = new StringBuilder();

    foreach (received rec in results)
    {
        // Parameterized query to prevent SQL injection
        queryBuilder.Append("INSERT INTO received_total (rt_msisdn, rt_message) VALUES (@rt_msisdn, @rt_message);");

        using (SqlCommand command = new SqlCommand(queryBuilder.ToString(), connection))
        {
            // Set parameters for the current record
            command.Parameters.AddWithValue("@rt_msisdn", rec.re_fromnum);
            command.Parameters.AddWithValue("@rt_message", rec.re_message);
            command.ExecuteNonQuery();
        }

        // Clear the query builder for the next record
        queryBuilder.Clear();
    }

    connection.Close();
}
