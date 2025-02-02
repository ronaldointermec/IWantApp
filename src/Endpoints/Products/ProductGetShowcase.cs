namespace IWantApp.Endpoints.Products;

public class ProductGetShowcase
{
    public static string Template => "/products/showcase";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    [SwaggerOperation(summary:"Exibe produtos", description: "Esta rota permite que todos as pessoas acessem a vitrine de produtos, mesmo que não estejam logados")]
    public static async Task<IResult> Action(  ApplicationDbContext context, int page = 1, int row = 10, string orderBy = "name")
    {

        if (row > 10)
            return Results.Problem(title: "Máximo permitido de 10 linhas", statusCode: 400);

        // só vai no banco de dados quando a consulta estiver pronta
        //AsNoTracking : usar em consultas. objeto não vai ser rastreado na memória - fica mais performático 
        var queryBase = context.Products.AsNoTracking().Include(p => p.Category)
            .Where(p => p.HasStock && p.Category.Active);

        if (orderBy == "name")
            queryBase = queryBase.OrderBy(p => p.Name);
        else if (orderBy == "price")
            queryBase = queryBase.OrderBy(p => p.Price);
        else
            return Results.Problem(title: "Ordenação apenas por nome ou preço", statusCode: 400);

        var queryFilter = queryBase.Skip((page - 1) * row).Take(row);
        
       var products  = queryFilter.ToList();

        var results = products.Select(p => new ProductResponse(p.Id, p.Name, p.Category.Name, p.Description, p.HasStock, p.Price, p.Active));

        return Results.Ok(results);
    }
}
