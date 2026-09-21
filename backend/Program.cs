using backend.Data;
using backend.Repositories;
using backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- MySQL (Pomelo) -------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));

// --- Repositories ---------------------------------------------------------
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IInternshipRepository, InternshipRepository>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();

// --- Services -------------------------------------------------------------
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IInternshipService, InternshipService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();

// --- CORS placeholder for the Angular dev server --------------------------
const string AngularDevCors = "AngularDevCors";

builder.Services.AddCors(options =>
{
    options.AddPolicy(AngularDevCors, policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// --- Swagger --------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- MVC / Controllers ----------------------------------------------------
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(AngularDevCors);

app.MapControllers();

app.Run();
