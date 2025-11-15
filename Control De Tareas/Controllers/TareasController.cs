using Control_De_Tareas.Data.Entitys;
using Control_De_Tareas.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Control_De_Tareas.Controllers
{
    [Authorize] // Requiere que el usuario esté autenticado
    public class TareasController : Controller
    {
        private Context _context;
        private ILogger<TareasController> _logger;

        public TareasController(Context context, ILogger<TareasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Todos los usuarios autenticados pueden ver sus tareas
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Solo los profesores pueden crear tareas
        /// </summary>
        [ProfesorAuthorize]
        public IActionResult Crear()
        {
            return View();
        }

        /// <summary>
        /// Solo los profesores pueden crear tareas (POST)
        /// </summary>
        [HttpPost]
        [ProfesorAuthorize]
        public IActionResult Crear(Tareas tarea)
        {
            if (ModelState.IsValid)
            {
                // Lógica para crear la tarea
                _context.Tareas.Add(tarea);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tarea);
        }

        /// <summary>
        /// Solo los profesores pueden editar tareas
        /// </summary>
        [ProfesorAuthorize]
        public IActionResult Editar(Guid id)
        {
            var tarea = _context.Tareas.Find(id);
            if (tarea == null)
            {
                return NotFound();
            }
            return View(tarea);
        }

        /// <summary>
        /// Solo los profesores pueden editar tareas (POST)
        /// </summary>
        [HttpPost]
        [ProfesorAuthorize]
        public IActionResult Editar(Tareas tarea)
        {
            if (ModelState.IsValid)
            {
                _context.Tareas.Update(tarea);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tarea);
        }

        /// <summary>
        /// Solo los profesores pueden eliminar tareas
        /// </summary>
        [ProfesorAuthorize]
        public IActionResult Eliminar(Guid id)
        {
            var tarea = _context.Tareas.Find(id);
            if (tarea == null)
            {
                return NotFound();
            }
            return View(tarea);
        }

        /// <summary>
        /// Solo los profesores pueden eliminar tareas (POST)
        /// </summary>
        [HttpPost, ActionName("Eliminar")]
        [ProfesorAuthorize]
        public IActionResult ConfirmarEliminacion(Guid id)
        {
            var tarea = _context.Tareas.Find(id);
            if (tarea != null)
            {
                tarea.IsSoftDeleted = true; // Soft delete
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Todos pueden ver detalles de una tarea
        /// </summary>
        public IActionResult Detalles(Guid id)
        {
            var tarea = _context.Tareas.Find(id);
            if (tarea == null)
            {
                return NotFound();
            }
            return View(tarea);
        }

        /// <summary>
        /// Solo los estudiantes pueden entregar tareas
        /// </summary>
        [EstudianteAuthorize]
        public IActionResult Entregar(Guid id)
        {
            var tarea = _context.Tareas.Find(id);
            if (tarea == null)
            {
                return NotFound();
            }
            return View(tarea);
        }

        /// <summary>
        /// Solo los estudiantes pueden entregar tareas (POST)
        /// </summary>
        [HttpPost]
        [EstudianteAuthorize]
        public IActionResult Entregar(Guid id, string contenido)
        {
            // Lógica para guardar la entrega del estudiante
            var submission = new Submissions
            {
                TaskId = id,
                SubmissionContent = contenido,
                SubmittedAt = DateTime.Now
                // Agregar UserId del estudiante actual
            };

            _context.Submissions.Add(submission);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
