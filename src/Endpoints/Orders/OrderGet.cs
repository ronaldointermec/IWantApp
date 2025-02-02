namespace IWantApp.Endpoints.Orders;

public class OrderGet
{

    public static string Template => "/orders/{id}";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize]
    public static async Task<IResult> Action(Guid id , HttpContext http, ApplicationDbContext context, UserManager<IdentityUser> userManager )
    {
        var clientClaim = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);
        var employeeCaim = http.User.Claims.FirstOrDefault(c => c.Type == "EmployeeCode");       


        var order = context.Orders.Include(p => p.Products).FirstOrDefault(o => o.Id == id);

        if (order.ClientId != clientClaim.Value && employeeCaim.Value == null)
            Results.Forbid();

        var client = await userManager.FindByIdAsync(order.ClientId);

        var productResponse = order.Products.Select(p => new OrderProduct(p.Id, p.Name));
        var orderResponse = new OrderResponse(order.Id, client.Email,productResponse, order.DeliveryAddress);

        return Results.Ok(orderResponse);
    }
}
