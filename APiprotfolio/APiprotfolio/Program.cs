using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;              // ⬅️ إضافة
using static PROtofile.Date.AppDbcontext;

namespace PROtofile
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ===== Services =====
            // DB
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MyCon")));

            builder.Services.AddControllers();

            // CORS: اسمح للفرونت إند بالاتصال
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFront", policy =>
                {
                    policy
                        .AllowAnyOrigin()      // في الإنتاج بدّلها بـ WithOrigins("https://your-frontend.com")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // Swagger / OpenAPI
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();

            // ===== DeepSeek HttpClient =====

            {


                var app = builder.Build();

                // ===== Pipeline =====
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                    app.MapOpenApi(); // (NET 9) اختياري
                }

                app.UseHttpsRedirection();

                // فعّل CORS قبل الـ Controllers
                app.UseCors("AllowFront");

                app.UseAuthorization();

                app.MapControllers();

                app.Run();
            }
        }
    }
}