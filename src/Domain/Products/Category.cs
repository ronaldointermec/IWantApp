using Flunt.Validations;
using System.Diagnostics.Contracts;

namespace IWantApp.Domain.Products;

public class Category : Entity
{
    public string Name { get; private set; }
    public bool Active { get; private set; }

    public Category()
    {

    }

    public Category(string nome, string createdBy, string editedBy)
    {

        Active = true;
        Name = nome;
        CreatedBy = createdBy;
        EditedBy = editedBy;
        EditedOn = DateTime.Now;
        CreatedOn = DateTime.Now;

        Validate();

    }

    private void Validate()
    {
        var contract = new Contract<Category>()
            .IsNotNullOrEmpty(Name, "Name")
            .IsGreaterOrEqualsThan(Name, 3, "Name")
            .IsNotNullOrEmpty(CreatedBy, "CreatedBy")
            .IsNotNullOrEmpty(EditedBy, "EditedBy");

        AddNotifications(contract);
    }

    public void EditInfo(string name, bool active, string editedBy)
    {
        Active = active;
        Name = name;
        EditedBy = editedBy ?? string.Empty;
        EditedOn = DateTime.Now;
        Validate();
    }
}
