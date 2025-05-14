
using Microsoft.EntityFrameworkCore;
using Service2.Interface;
using Service2.Model;
using Service2.Repo;

namespace Service2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // builder.Services.AddControllers();
            builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
                   options.SuppressModelStateInvalidFilter = true);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(option =>
            {
                option.AddPolicy("MyPolicy", policy =>
                {
                    policy.AllowAnyHeader()
                    .AllowAnyOrigin()
                    .AllowAnyMethod();
                });
            });
            builder.Services.AddDbContext<Project_Context>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("cs")));

            builder.Services.AddScoped<ITransaction, transRepo>();
            builder.Services.AddScoped<generalRes>();
            builder.Services.AddHttpClient("User", client =>
            {
                client.BaseAddress = new Uri("http://localhost:5071/");
            });
            builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("MyPolicy");
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
