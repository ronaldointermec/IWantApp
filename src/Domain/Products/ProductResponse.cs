namespace IWantApp.Domain.Products;

public record ProductResponse(string ane, string CategoryName, string Description, bool HasStock, bool Active);