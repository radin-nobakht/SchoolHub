using Microsoft.EntityFrameworkCore;

namespace SchoolHub.Adapter
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options)
        : base(options)
        {
            Database.SetCommandTimeout(30);
        }
    }
}
