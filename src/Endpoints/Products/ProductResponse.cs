namespace IWantApp.Endpoints.Products;

public record ProductResponse(string Nane, string CategoryName, string Description, bool HasStock, decimal Price, bool Active);