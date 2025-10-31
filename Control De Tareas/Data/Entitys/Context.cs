using Microsoft.EntityFrameworkCore;
using Control_De_Tareas.Data.Entitys;

namespace Control_De_Tareas.Data.Entitys
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }
      //  public DbSet<Usuario> Usuarios { get; set; }
    }
}
