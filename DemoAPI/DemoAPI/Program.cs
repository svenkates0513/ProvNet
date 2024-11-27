using DemoAPI.Services;

var builder = WebApplication.CreateBuilder(args);

//Services to controllers
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient(); //Register HttpClient
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<TokenService>(); //Register TokenService

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseStaticFiles(); //Enable serving static files like HTML, CSS, JS if needed

app.UseRouting(); //Enable routing for controllers and views

app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );

app.Run();
