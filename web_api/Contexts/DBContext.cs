using Microsoft.EntityFrameworkCore;

namespace web_api.Contexts
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions options) : base(options)
        {
            
        }
    }
}

