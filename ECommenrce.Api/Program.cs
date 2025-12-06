using System.IdentityModel.Tokens.Jwt;
using Ecommenrce.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
   options.Password.RequireNonAlphanumeric = false; 
   options.Password.RequireUppercase = false;
   options.Password.RequireLowercase = true;
   options.Password.RequireDigit = true;
   options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(options =>
{
   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
   options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
   options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; 
})
.AddJwtBearer(options =>
{
   options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
   {
       ValidateIssuerSigningKey = false,
       ValidateIssuer = false,
       ValidateAudience = false,
       ValidateLifetime = false,

       SignatureValidator = (token, parameters) =>
       {
           var handler = new JwtSecurityTokenHandler();
           return handler.ReadJwtToken(token);
       }
   };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ECommerce API",
        Version = "v1"     
    });


var securityScheme = new OpenApiSecurityScheme
{
    Name = "Authorization",
    Description = "Jwt Token Formatı: Bearer {token}",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.Http,
    Scheme = "bearer",
    BearerFormat = "JWT",
    Reference = new OpenApiReference
    {
        Type = ReferenceType.SecurityScheme,
        Id = "Bearer"
    }
};
c.AddSecurityDefinition("Bearer",securityScheme);
c.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {securityScheme, new string[]{}}
});
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = {"Admin","Seller","Buyer"};
    foreach(var role in roles)
    {
        if(!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var adminEmail = "admin@site.com";
    var admin = await userManager.FindByEmailAsync(adminEmail);
    if(admin == null){
        admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "Admin User"
        };
        await userManager.CreateAsync(admin,"Admin123*");
        await userManager.AddToRoleAsync(admin,"Admin");
    }
    if (!db.Categories.Any())
    {
        db.Categories.AddRange(
            new Category {Name = "Elektronik"},
            new Category {Name = "Kitap"}
        );
        await db.SaveChangesAsync();
    }
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStatusCodePages();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();