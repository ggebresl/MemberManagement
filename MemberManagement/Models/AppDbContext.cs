using Microsoft.Extensions.DependencyInjection;
//using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MemberManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace MemberManagement
{
    //step 1. Your application DBContext class must inherit from IdentityContext

    public class AppDbContext : IdentityDbContext<User>
    //  public class AppDbContext : DbContext

    {

        public DbSet<Userlogin> userlogin { get; set; }
        public DbSet<Payment> payment { get; set; }
        public DbSet<Address> address { get; set; }
        public DbSet<Member> member { get; set; }


        //  public DbSet<Child> children { get; set; }
        // public DbSet<MemberAddress> MemberAddress { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
             base.OnModelCreating(modelBuilder);

            //Configure a one-to-many relationship with Fluent API
            /*
            modelBuilder.Entity<Child>()
                        .HasOne(c => c.Member) //navigation property in Child class - the child
                        .WithMany(m => m.Children); //navigation property in Member class - (the parent)
            */

        }
        public static async Task CreateAdminUser(IServiceProvider serviceProvider)
        {
           


            Microsoft.AspNetCore.Identity.UserManager<User> userManager = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<User>>();
            Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole> roleManager = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();

            string username = "admin";
            string password = "tigraycom";
            string roleName = "Admin";
           
            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            //If role does not exist , create it

            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(roleName));
                
            }
            
        // If username doesn't exist, create it and add it to role
         
            if( await userManager.FindByNameAsync(username) == null)
            {
               
                User user = new User { UserName = username };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }
      
        }
    }
}
