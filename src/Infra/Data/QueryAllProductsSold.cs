using Dapper;
namespace IWantApp.Infra.Data;

public class QueryAllProductsSold
{
    private readonly IConfiguration configuration;

    public QueryAllProductsSold(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task<IEnumerable<ProductSoldReportResponse>> Execute()
    {
        var db = new SqlConnection(configuration["ConnectionStrings:IWantDb"]);

        var query =
            @"SELECT 
                P.Id, 
                P.Name,
                COUNT(*) AS Amount
            FROM 
                Orders O 
            INNER JOIN 
                OrderProducts OP ON O.Id = OP.OrdersId
            INNER JOIN 
                Products P ON OP.ProductsId = P.Id
            GROUP BY 
                P.Id, P.Name
            ORDER BY Amount
            DESC";

        // o Dapper converte a query em uma classe, no vaso EmployeeResponse
        return await db.QueryAsync<ProductSoldReportResponse>(query);
    }
}
