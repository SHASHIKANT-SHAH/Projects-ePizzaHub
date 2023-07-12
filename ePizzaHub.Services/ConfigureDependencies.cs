using ePizzaHub.Core;
using ePizzaHub.Core.Entities;
using ePizzaHub.Repositories.Implementations;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Implementations;
using ePizzaHub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.Services
{
    public class ConfigureDependencies
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            //databse
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DbConnection"));
            });

            //Repositories
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IRepository<Item>, Repository<Item>>();
            services.AddScoped<IRepository<Cart>,Repository<Cart>>();
            services.AddScoped<IRepository<CartItem>,Repository<CartItem>>(); 
            services.AddScoped<IRepository<PaymentDetail>,Repository<PaymentDetail>>(); 
            services.AddScoped<IRepository<Order>,Repository<Order>>();

            services.AddScoped<ICartRepository, CartRepository>();  

            //Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IPaymentService,PaymentService>();
            services.AddScoped<IOrderService, OrderService>();  

            services.AddScoped<IService<Item>, Service<Item>>();
            services.AddScoped<IService<PaymentDetail>, Service<PaymentDetail>>();  
        }
    }
}
