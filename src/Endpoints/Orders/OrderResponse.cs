namespace IWantApp.Endpoints.Orders;

public record OrderResponse(Guid Id, string ClientEmail , IEnumerable<OrderProduct> Products, String DeliveryAddress);
public record OrderProduct(Guid Id, string Name);
