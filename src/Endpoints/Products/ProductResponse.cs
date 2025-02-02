namespace IWantApp.Endpoints.Products;

public record ProductResponse(Guid Id, string Nane, string CategoryName, string Description, bool HasStock, decimal Price, bool Active);