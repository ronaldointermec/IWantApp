using Dapper;
using IWantApp.Endpoints.Employees;
using Microsoft.Data.SqlClient;

namespace IWantApp.Infra.Data;

public class QueryAllUserWithClaimName
{
    private readonly IConfiguration configuration;

    public QueryAllUserWithClaimName(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task<IEnumerable<EmployeeResponse>> Execute(int page, int rows)
    {
        var db = new SqlConnection(configuration["ConnectionStrings:IWantDb"]);

        var query =
            @"SELECT 
            Email, 
            ClaimValue AS Name
            FROM AspNetUsers U INNER JOIN AspNetUserClaims C 
            ON U.Id = C.UserId AND C.ClaimType = 'Name'
            ORDER BY name  asc
            OFFSET (@page -1) * @rows ROWS FETCH NEXT @rows ROWS ONLY";

        // o Dapper converte a query em uma classe, no vaso EmployeeResponse
        return  await db.QueryAsync<EmployeeResponse>(
       query,
       new { page, rows } // estes parâmetros vão ser usados na query (objeto anônimo)
       );
    }
}
