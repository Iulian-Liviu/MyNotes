using Microsoft.EntityFrameworkCore;
using MyNotes.API.Database;
using MyNotes.API.Repositories;
using Newtonsoft.Json;

namespace MyNotes.API;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<MyNotesDbContext>(options => {
            options.UseLazyLoadingProxies();

            /*
            #if DEBUG
                        options.UseSqlServer(builder.Configuration.GetConnectionString("NotesDatabase"),
                            b => b.MigrationsAssembly(typeof(MyNotesDbContext).Assembly.FullName));

            #endif*/

            options.UseNpgsql(builder.Configuration.GetConnectionString("NotesDbSupabase"));
        });


        builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
        builder.Services.AddScoped<INotesRepository, NotesRepository>();
        builder.Services.AddScoped<IUsersRepository, UsersRepository>();

        builder.Services.AddControllers()
            .AddNewtonsoftJson(options => {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}