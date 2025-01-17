namespace IWantApp.Endpoints.Categories;

public class CategoryPut
{
    public static string Template => "/categories/{id:guid}";
    public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action([FromRoute] Guid id, CategoryRequest categoryRequest, HttpContext http, ApplicationDbContext context)

    {
        // Recupera qualquer informação que esteja no token
        var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        //var maneira_ansinada_no_curso= context.Categories.FirstOrDefault(c => c.Id == id);
        var category = context.Categories.Where(c => c.Id == id).FirstOrDefault();


        if (category == null)
            return Results.NotFound();


        category.EditInfo(categoryRequest.Name, categoryRequest.Active, userId);

        if (!category.IsValid)        
            return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails());
        

        context.SaveChanges();

        return Results.Ok();
    }

}
