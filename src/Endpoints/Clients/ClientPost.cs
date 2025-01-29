using IWantApp.Domain.Users;

namespace IWantApp.Endpoints.Clients;

public class ClientPost

{
    public static string Template => "/clients";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    [SwaggerOperation(
       Summary = "Cria um novo colaborador",
       Description = " Esta rota permite a criação de um novo colaborador no sistema."
   )]
    public static async Task<IResult> Action(ClientRequest clientRequest, UserCreator userCreator)
    {
        var userClaims = new List<Claim>
        {
            new Claim("Cpf", clientRequest.Cpf),
            new Claim("Name", clientRequest.Name),
        };

        (IdentityResult identity, string userID) result = 
            await userCreator.Create(clientRequest.Email, clientRequest.Password, userClaims);

        if (!result.identity.Succeeded)
            return Results.ValidationProblem(result.identity.Errors.ConvertToProblemDetails());

        return Results.Created($"/clients/{result.userID}",result.userID);
    }

}
