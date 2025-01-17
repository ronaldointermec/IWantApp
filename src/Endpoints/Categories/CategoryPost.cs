namespace IWantApp.Endpoints.Categories;

public class CategoryPost
{
    public static string Template => "/categories";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [Authorize(policy: "EmployeePolicy")]
    [SwaggerOperation(
        Summary = "Cria uma nova categoria",
        Description = " Esta rota permite que o usuário crie uma nova categoria no sistema."
    )]
    public static async Task<IResult> Action(CategoryRequest categoryRequest,HttpContext http , ApplicationDbContext context)
    {
        // Recupera qualquer informação que esteja no token
        var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var category = new Category(categoryRequest.Name, userId, userId);

        if (!category.IsValid)
        {

            return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails());
        }
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        return Results.Created($"/categories/{category.Id}", category.Id);
    }

}
