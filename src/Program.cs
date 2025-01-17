using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration.GetConnectionString("ConnectionStrings:IWantDb"));
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
    // cria uma politica para um usuário de codígo 008
    //options.AddPolicy("Employee008Policy", p =>
    //p.RequireAuthenticatedUser().RequireClaim("EmployeeCode", "008"));
}
);
// registrar a classe como serviço
builder.Services.AddScoped<QueryAllUserWithClaimName>();

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

app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext http) =>
{
    var error = http.Features?.Get<IExceptionHandlerFeature>()?.Error;

    if (error != null)
    {
        if (error is SqlException)
            return Results.Problem(title: "Erro ao conectar ao banco de dados", statusCode: 500);
    }

    return Results.Problem(title: "Ocorreu um erro", statusCode: 500);
});

app.Run();
