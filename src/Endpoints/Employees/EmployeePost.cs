namespace IWantApp.Endpoints.Employees;

public class EmployeePost

{
    public static string Template => "/employees";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [Authorize(policy: "EmployeePolicy")]
    [SwaggerOperation(
       Summary = "Cria um novo colaborador",
       Description = " Esta rota permite a criação de um novo colaborador no sistema."
   )]
    public static async Task<IResult> Action(EmployeeRequest employeeRequest, HttpContext http, UserManager<IdentityUser> userManager)
    {
        // Recupera qualquer informação que esteja no token
        var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var newUser = new IdentityUser { UserName = employeeRequest.Email, Email = employeeRequest.Email };
        var result = await userManager.CreateAsync(newUser, employeeRequest.Password);

        if (!result.Succeeded)
            return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());

        var userClaims = new List<Claim>
        {
            new Claim("EmployeeCode", employeeRequest.EmployeeCode),
            new Claim("Name", employeeRequest.Name),
            new Claim("CreatedBy", userId)
        };

        var claiamResult = await userManager.AddClaimsAsync(newUser, userClaims);

        if (!claiamResult.Succeeded)
            return Results.BadRequest(result.Errors.First());


        return Results.Created($"/employees/{newUser.Id}", newUser.Id);
    }

}
