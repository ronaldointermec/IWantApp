namespace IWantApp.Endpoints.Employees;

public class EmployeeGetAll
{
    public static string Template => "/employees";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;


    // solução com dapper
    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action(int? page, int? rows, QueryAllUserWithClaimName query)

    {

        // Validate page
        if (!page.HasValue || page.Value <= 0)
        {
            return Results.BadRequest("Pagina precisa ser positivo e inteiro.");
        }

        // Validate rows
        if (!rows.HasValue || rows.Value <= 0)
        {
             Results.BadRequest("Linhas precisam ser positivo e inteiro");
        }

        var result = await query.Execute(page.Value, rows.Value);
        return Results.Ok(result);

    }

    //// solução com IF core com paginação
    //public static IResult Action(int page, int rows, UserManager<IdentityUser> userManager)
    //{
    //    var users =  userManager.Users.Skip((page - 1) * rows).Take(rows).ToList();
    //    var employees = new List<EmployeeResponse>();

    //    foreach (var user in users)
    //    {
    //        var claims = userManager.GetClaimsAsync(user).Result;
    //        var claimName = claims.FirstOrDefault(c => c.Type == "Name");
    //        var userName = claimName != null ? claimName.Value : string.Empty; 
    //        employees.Add(new EmployeeResponse(user.Email, userName));
    //    }

    //    return Results.Ok(employees);
    //}

}
