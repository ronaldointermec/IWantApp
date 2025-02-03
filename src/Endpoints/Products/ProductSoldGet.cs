using static System.Net.WebRequestMethods;

namespace IWantApp.Endpoints.Products;

public class ProductSoldGet
{
    public static string Template => "/products/sold";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    [SwaggerOperation(summary:"Exibe produtos mais vendidos", description: "Esta rota permite exibir os produtos mais vendidos")]
    public static async Task<IResult> Action( QueryAllProductsSold query)
    {
        var results = await query.Execute();

        return Results.Ok(results);
    }
}
