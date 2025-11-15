using Microsoft.AspNetCore.Mvc;

namespace Control_De_Tareas.Controllers
{
    public class ErrorController : Controller
    {
        /// <summary>
        /// Acción para manejar errores 403 (Acceso Denegado)
        /// </summary>
        [Route("Error/403")]
        public IActionResult Error403()
        {
            return View();
        }

        /// <summary>
        /// Acción para manejar errores 404 (No Encontrado)
        /// </summary>
        [Route("Error/404")]
        public IActionResult Error404()
        {
            return View();
        }

        /// <summary>
        /// Acción genérica para manejar otros errores
        /// </summary>
        [Route("Error/{statusCode}")]
        public IActionResult HandleError(int statusCode)
        {
            switch (statusCode)
            {
                case 403:
                    return View("Error403");
                case 404:
                    return View("Error404");
                default:
                    return View("Error");
            }
        }
    }
}
