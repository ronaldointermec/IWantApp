using Flunt.Validations;
using System.Diagnostics.Contracts;

namespace IWantApp.Domain.Products;

public class Category : Entity
{
    public bool Active { get; set; }

    public Category()
    {

    }

    public Category(string nome, string createdBy, string editedBy)
    {
        var contract = new Contract<Category>()
            .IsNotNullOrEmpty(nome, "Name")
            .IsGreaterOrEqualsThan(nome, 3, "Name")
            .IsNotNullOrEmpty(createdBy, "CreatedBy")
            .IsNotNullOrEmpty(editedBy, "EditedBy");

        AddNotifications(contract);
        Active = true;
        Name = nome;
        CreatedBy = createdBy;
        EditedBy = editedBy;
        EditedOn = DateTime.Now;
        CreatedOn = DateTime.Now;

    }

}
