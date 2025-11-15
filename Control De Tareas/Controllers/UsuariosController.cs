using Control_De_Tareas.Data.Entitys;
using Control_De_Tareas.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Control_De_Tareas.Controllers
{
    /// <summary>
    /// Controller para gestión de usuarios
    /// Acceso: SOLO Administradores
    /// </summary>
    [AdministradorAuthorize]
    public class UsuariosController : Controller
    {
        private readonly Context _context;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(Context context, ILogger<UsuariosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Listar todos los usuarios - Solo Administrador
        /// </summary>
        public IActionResult Index()
        {
            try
            {
                var usuarios = _context.Users
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .Where(u => !u.IsSoftDeleted)
                    .ToList();

                return View(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar usuarios");
                return View(new List<Users>());
            }
        }

        /// <summary>
        /// Crear nuevo usuario - Solo Administrador
        /// </summary>
        public IActionResult Crear()
        {
            // Cargar roles disponibles para el dropdown
            ViewBag.Roles = _context.Roles.Where(r => !r.IsSoftDeleted).ToList();
            return View();
        }

        /// <summary>
        /// Crear nuevo usuario (POST) - Solo Administrador
        /// </summary>
        [HttpPost]
        public IActionResult Crear(Users usuario, Guid roleId)
        {
            if (ModelState.IsValid)
            {
                usuario.UserId = Guid.NewGuid();
                usuario.CreateAt = DateTime.Now;
                usuario.IsSoftDeleted = false;

                _context.Users.Add(usuario);
                
                // Asignar rol al usuario
                var userRole = new UserRoles
                {
                    UserId = usuario.UserId,
                    RoleId = roleId,
                    AssignedAt = DateTime.Now
                };
                _context.UserRoles.Add(userRole);

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Roles = _context.Roles.Where(r => !r.IsSoftDeleted).ToList();
            return View(usuario);
        }

        /// <summary>
        /// Editar usuario - Solo Administrador
        /// </summary>
        public IActionResult Editar(Guid id)
        {
            var usuario = _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefault(u => u.UserId == id);

            if (usuario == null)
            {
                return NotFound();
            }

            ViewBag.Roles = _context.Roles.Where(r => !r.IsSoftDeleted).ToList();
            return View(usuario);
        }

        /// <summary>
        /// Editar usuario (POST) - Solo Administrador
        /// </summary>
        [HttpPost]
        public IActionResult Editar(Users usuario, Guid roleId)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Update(usuario);

                // Actualizar rol si cambió
                var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == usuario.UserId);
                if (userRole != null && userRole.RoleId != roleId)
                {
                    userRole.RoleId = roleId;
                    userRole.AssignedAt = DateTime.Now;
                }

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Roles = _context.Roles.Where(r => !r.IsSoftDeleted).ToList();
            return View(usuario);
        }

        /// <summary>
        /// Eliminar usuario (soft delete) - Solo Administrador
        /// </summary>
        public IActionResult Eliminar(Guid id)
        {
            var usuario = _context.Users.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        /// <summary>
        /// Confirmar eliminación de usuario - Solo Administrador
        /// </summary>
        [HttpPost, ActionName("Eliminar")]
        public IActionResult ConfirmarEliminacion(Guid id)
        {
            var usuario = _context.Users.Find(id);
            if (usuario != null)
            {
                usuario.IsSoftDeleted = true;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Ver detalles de usuario - Solo Administrador
        /// </summary>
        public IActionResult Detalles(Guid id)
        {
            var usuario = _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefault(u => u.UserId == id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }
    }
}
