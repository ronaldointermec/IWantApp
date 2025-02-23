﻿using Flunt.Validations;
using IWantApp.Domain.Orders;

namespace IWantApp.Domain.Products;

public class Product : Entity
{
    public string Name { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; }
    public string Description { get; private set; }
    public bool HasStock { get; private set; }
    public bool Active { get; private set; } = true;
    public decimal Price { get; private set; }
    
    //criado apenas para estabelecer relacionando N pra N com o Entity Order (o que chamamos de sujar a entidade)
    public ICollection<Order> Orders { get; private set; }
    public Product()
    {
    }

    public Product(string name, Category catetory, string description, bool hasStock, decimal price, string createdBy)
    {
        Name = name;
        Category = catetory;
        Description = description;
        HasStock = hasStock;
        Price = price;

        CreatedBy = createdBy;
        EditedBy = createdBy;
        CreatedOn   = DateTime.Now;
        EditedOn = DateTime.Now;

        Validate();
    }

    private void Validate()
    {
        var contract = new Contract<Product>()
            .IsNotNullOrEmpty(Name, "Name")
            .IsGreaterOrEqualsThan(Name, 3, "Name")
            .IsNotNull(Category, "Category", "Categoria não encontrada")
            .IsNotNullOrEmpty(Description, "Description")
            .IsGreaterOrEqualsThan(Description, 3, "Description")
            .IsGreaterOrEqualsThan(Price, 1, "Price")
            .IsNotNullOrEmpty(CreatedBy, "CreatedBy")
            .IsNotNullOrEmpty(EditedBy, "EditedBy");

        AddNotifications(contract);
    }


}