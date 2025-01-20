
namespace IWantApp.Infra.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{


    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Ignore<Notification>();

        builder.Entity<Product>()
            .Property(p => p.Name).IsRequired();
        builder.Entity<Product>()
        .Property(p => p.Description).HasMaxLength(255).IsRequired(false);
        builder.Entity<Product>()
            .Property(p =>p.Price).HasColumnType("decimal(10,2)").IsRequired();

        builder.Entity<Category>()
            .Property(c => c.Name).IsRequired();
    }

    // Para criar novas convenções 
    protected override void ConfigureConventions(ModelConfigurationBuilder configuration)
    {
        configuration.Properties<string>()
            .HaveMaxLength(100);
    }
}
