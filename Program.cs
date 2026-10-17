using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using StudentRecordDraft.Data;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages + role-based access control by folder/page.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");       // every page under /Admin
    options.Conventions.AuthorizePage("/MyRecord", "StudentOnly");
})
// Stops MVC treating every non-nullable string/list as [Required]; we validate explicitly instead.
.AddMvcOptions(o => o.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

// SQLite database accessed through Entity Framework Core.
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite("Data Source=students.db"));

// Cookie authentication: the cookie stores the user's name, role and StudentId claims.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Login";
        o.AccessDeniedPath = "/AccessDenied";
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", p => p.RequireRole("Admin"))
    .AddPolicy("StudentOnly", p => p.RequireRole("Student"));

var app = builder.Build();

// Create the database (if needed) and insert demo data on start-up.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    SeedData.Initialise(db);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();   // who are you?
app.UseAuthorization();    // are you allowed here?

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
