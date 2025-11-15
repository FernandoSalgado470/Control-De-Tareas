using Control_De_Tareas.Data.Entitys;
using Control_De_Tareas.Models;
using Control_De_Tareas.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Control_De_Tareas.Controllers
{
    
    [ProfesorOAdministradorAuthorize]
    public class CursosController : Controller
    {
        private Context _context;
        private ILogger<CursosController> _logger;

        public CursosController(Context context, ILogger<CursosController> logger)
        {
            _context = context;
            _logger = logger;
        }

      
        public IActionResult Index()
        {
            var viewModel = new CursosVm();

            try
            {
                var cursos = _context.Courses
                    .Include(c => c.Instructor)
                    .Where(c => !c.IsSoftDeleted) // Filtrar cursos no eliminados
                    .Select(c => new CursoDto
                    {
                        Id = c.Id,
                        Codigo = c.Codigo,
                        Nombre = c.Nombre,
                        InstructorNombre = c.Instructor != null ? c.Instructor.Instructor : "Sin instructor",
                        CantidadEstudiantes = 0,
                        Estado = c.Estado
                    })
                    .ToList();

                viewModel.Cursos = cursos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar los cursos");
                viewModel.Cursos = new List<CursoDto>();
            }

            return View(viewModel);
        }

       
        public IActionResult Crear()
        {
            return View();
        }

       
        [HttpPost]
        public IActionResult Crear(Courses curso)
        {
            if (ModelState.IsValid)
            {
                curso.Id = Guid.NewGuid();
                curso.FechaCreacion = DateTime.Now;
                curso.IsSoftDeleted = false;
                
                _context.Courses.Add(curso);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(curso);
        }

        
        public IActionResult Editar(Guid id)
        {
            var curso = _context.Courses.Find(id);
            if (curso == null || curso.IsSoftDeleted)
            {
                return NotFound();
            }
            return View(curso);
        }

        
        [HttpPost]
        public IActionResult Editar(Courses curso)
        {
            if (ModelState.IsValid)
            {
                _context.Courses.Update(curso);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(curso);
        }

        
        [AdministradorAuthorize]
        public IActionResult Eliminar(Guid id)
        {
            var curso = _context.Courses.Find(id);
            if (curso == null || curso.IsSoftDeleted)
            {
                return NotFound();
            }
            return View(curso);
        }

        
        [HttpPost, ActionName("Eliminar")]
        [AdministradorAuthorize]
        public IActionResult ConfirmarEliminacion(Guid id)
        {
            var curso = _context.Courses.Find(id);
            if (curso != null)
            {
                curso.IsSoftDeleted = true; 
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
