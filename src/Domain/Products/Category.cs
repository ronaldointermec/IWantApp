using Flunt.Validations;
using System.Diagnostics.Contracts;

namespace IWantApp.Domain.Products;

public class Category : Entity
{
    public bool Active { get; set; }

    public Category()
    {
       
    }

    public Category( string nome)
    {
        var contract = new Contract<Category>()
            .IsNotNullOrEmpty(nome, "Name");
        AddNotifications(contract);
        Active = true;
        Name = nome;
       
    }

}
