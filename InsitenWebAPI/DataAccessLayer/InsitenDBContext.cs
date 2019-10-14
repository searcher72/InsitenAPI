using Microsoft.EntityFrameworkCore;

namespace InsitenWebAPI.DataAccessLayer
{
    public class InsitenDBContext : DbContext
    {
        public InsitenDBContext(DbContextOptions<InsitenDBContext> options)
        :base(options)
        { 
        }
    }
}
