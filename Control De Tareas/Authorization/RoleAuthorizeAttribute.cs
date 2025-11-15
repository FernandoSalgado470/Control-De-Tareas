using Microsoft.AspNetCore.Authorization;

namespace Control_De_Tareas.Authorization
{
    /// <summary>
    /// Atributo de autorización para el rol Administrador
    /// </summary>
    public class AdministradorAuthorize : AuthorizeAttribute
    {
        public AdministradorAuthorize()
        {
            Roles = "Administrador";
        }
    }

    /// <summary>
    /// Atributo de autorización para el rol Profesor
    /// </summary>
    public class ProfesorAuthorize : AuthorizeAttribute
    {
        public ProfesorAuthorize()
        {
            Roles = "Profesor";
        }
    }

    /// <summary>
    /// Atributo de autorización para el rol Estudiante
    /// </summary>
    public class EstudianteAuthorize : AuthorizeAttribute
    {
        public EstudianteAuthorize()
        {
            Roles = "Estudiante";
        }
    }

    /// <summary>
    /// Atributo de autorización para Profesor o Administrador
    /// </summary>
    public class ProfesorOAdministradorAuthorize : AuthorizeAttribute
    {
        public ProfesorOAdministradorAuthorize()
        {
            Roles = "Profesor,Administrador";
        }
    }
}
