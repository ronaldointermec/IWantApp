using IWantApp.Domain.Users;
using IWantApp.Endpoints.Clients;
using IWantApp.Endpoints.Orders;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Criação do serviço de logs
builder.WebHost.UseSerilog((context, configuration) =>
{
    configuration
    .WriteTo.Console()
    .WriteTo.MSSqlServer(
        context.Configuration["ConnectionStrings:IWantDb"],
        sinkOptions: new MSSqlServerSinkOptions()
        {
            AutoCreateSqlTable = true,
            TableName = "LogAPI"
        }
        );
});
builder.Services.AddSqlServer<ApplicationDbContext>(
    builder.Configuration["ConnectionStrings:IWantDb"]);

builder.Services.AddIdentity<IdentityUser, IdentityRole>(
//    options =>
//{
//    options.Password.RequireNonAlphanumeric = false;
//    options.Password.RequireDigit = false;
//    options.Password.RequireLowercase = false;
//}
).AddEntityFrameworkStores<ApplicationDbContext>();

// Avisa que vai authenticar através do token
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}
).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero, // remove os 5 minutos default de tolerância 
        ValidIssuer = builder.Configuration["JwtBearerTokenSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtBearerTokenSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtBearerTokenSettings:SecretKey"]))

    };
});

// adiciona serviço de autorização para saber que está navegando pelo sistema
builder.Services.AddAuthorization(
// protege todas as rotas com o [Authorize]
    options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .Build();

    options.AddPolicy("EmployeePolicy", p =>
    p.RequireAuthenticatedUser().RequireClaim("EmployeeCode"));

    options.AddPolicy("CpfPolicy", p =>
    p.RequireAuthenticatedUser().RequireClaim("Cpf"));

    // cria uma politica para um usuário de codígo 008
    //options.AddPolicy("Employee008Policy", p =>
    //p.RequireAuthenticatedUser().RequireClaim("EmployeeCode", "008"));
}
);
// registrar a classe como serviço
builder.Services.AddScoped<QueryAllUserWithClaimName>();
builder.Services.AddScoped<UserCreator>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(
    c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "IWantApp", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    c.EnableAnnotations();
}
);

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();


app.UseHttpsRedirection();

app.MapMethods(CategoryPost.Template, CategoryPost.Methods, CategoryPost.Handle);
app.MapMethods(CategoryGetAll.Template, CategoryGetAll.Methods, CategoryGetAll.Handle);
app.MapMethods(CategoryPut.Template, CategoryPut.Methods, CategoryPut.Handle);

app.MapMethods(EmployeePost.Template, EmployeePost.Methods, EmployeePost.Handle);
app.MapMethods(EmployeeGetAll.Template, EmployeeGetAll.Methods, EmployeeGetAll.Handle);

app.MapMethods(TokenPost.Template, TokenPost.Methods, TokenPost.Handle);

app.MapMethods(ProductPost.Template, ProductPost.Methods, ProductPost.Handle);
app.MapMethods(ProductGetAll.Template, ProductGetAll.Methods, ProductGetAll.Handle);
app.MapMethods(ProductGetShowcase.Template, ProductGetShowcase.Methods, ProductGetShowcase.Handle);

app.MapMethods(ClientPost.Template, ClientPost.Methods, ClientPost.Handle);
app.MapMethods(ClientGet.Template, ClientGet.Methods, ClientGet.Handle);

app.MapMethods(OrderPost.Template, OrderPost.Methods, OrderPost.Handle);
app.MapMethods(OrderGet.Template, OrderGet.Methods, OrderGet.Handle);

app.UseExceptionHandler("/error");
// Rota para lidar com os erros da applição
app.Map("/error", (HttpContext http) =>
{
    var error = http.Features?.Get<IExceptionHandlerFeature>()?.Error;

    if (error != null)
    {
        if (error is SqlException)
            return Results.Problem(title: "Erro ao conectar ao banco de dados", statusCode: 500);
        else if (error is BadHttpRequestException)
            return Results.Problem(title: "Erro na conversão de dados. Veja todas as informações enviadas", statusCode: 500);
    }

    return Results.Problem(title: "Ocorreu um erro", statusCode: 500);
});

app.Run();
