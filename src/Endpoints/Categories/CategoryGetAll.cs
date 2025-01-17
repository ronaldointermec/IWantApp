namespace IWantApp.Endpoints.Categories;

public class CategoryGetAll
{
    public static string Template => "/categories";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action(ApplicationDbContext context)
    {
        var categories = context.Categories.ToList();
        var response = categories.Select(category => new CategoryResponse(category.Id, category.Name, category.Active));
        context.SaveChanges();

        return Results.Ok(response);
    }

}
