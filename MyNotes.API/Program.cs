
using Microsoft.EntityFrameworkCore;
using MyNotes.API.Database;
using MyNotes.API.Repositories;

namespace MyNotes.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<NotesDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("NotesDatabase"),
                    b => b.MigrationsAssembly(typeof(NotesDbContext).Assembly.FullName));

            });
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
            builder.Services.AddScoped<INotesRepository, NotesRepository>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}