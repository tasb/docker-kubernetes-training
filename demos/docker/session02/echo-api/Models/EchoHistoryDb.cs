using Microsoft.EntityFrameworkCore;

namespace echo_api.Models
{
    public class EchoHistoryDb : DbContext
    {
        public EchoHistoryDb(DbContextOptions<EchoHistoryDb> options) : base(options)
        {
        }
        public DbSet<EchoHistory> EchoLogs => Set<EchoHistory>();

    }

}