# ✅ SISTEMA DE AUTORIZACIÓN IMPLEMENTADO

## 📁 Archivos Creados/Modificados

### ✨ Nuevos Archivos Creados:

1. **Authorization/RoleAuthorizeAttribute.cs** - Atributos personalizados por rol
2. **Controllers/ErrorController.cs** - Manejo de errores 403/404
3. **Controllers/UsuariosController.cs** - Gestión de usuarios (Solo Admin)
4. **Views/Error/Error403.cshtml** - Página de acceso denegado
5. **Views/Error/Error404.cshtml** - Página no encontrada

### 🔧 Archivos Modificados:

1. **Program.cs** - Agregadas políticas de autorización y autenticación
2. **Controllers/TareasController.cs** - Aplicada autorización por roles
3. **Controllers/CursosController.cs** - Aplicada autorización por roles
4. **Views/Shared/_Layout.cshtml** - Muestra rol del usuario
5. **Views/Shared/_Sidebar.cshtml** - Filtra menú según rol

---

## 🎯 Funcionalidades Implementadas

### 1. ✅ Políticas de Autorización en Program.cs
- ✓ SoloAdministrador
- ✓ SoloProfesor
- ✓ SoloEstudiante
- ✓ ProfesorOAdministrador
- ✓ UsuarioAutenticado

### 2. ✅ Atributos [Authorize] Personalizados
```csharp
[AdministradorAuthorize]      // Solo administradores
[ProfesorAuthorize]            // Solo profesores
[EstudianteAuthorize]          // Solo estudiantes
[ProfesorOAdministradorAuthorize]  // Profesores o administradores
```

### 3. ✅ Autorización en Controllers

#### TareasController:
- ✓ `Index()` - Todos los autenticados
- ✓ `Crear()` - Solo Profesores
- ✓ `Editar()` - Solo Profesores
- ✓ `Eliminar()` - Solo Profesores
- ✓ `Detalles()` - Todos los autenticados
- ✓ `Entregar()` - Solo Estudiantes

#### CursosController:
- ✓ Todo el controller - Profesores y Administradores
- ✓ `Eliminar()` - SOLO Administradores

#### UsuariosController:
- ✓ Todo el controller - SOLO Administradores

### 4. ✅ Páginas de Error
- ✓ Error 403 - Acceso Denegado (diseño Bootstrap)
- ✓ Error 404 - Página No Encontrada (diseño Bootstrap)

### 5. ✅ Redirecciones
- ✓ Usuario sin permisos → Error 403
- ✓ Página no encontrada → Error 404
- ✓ Usuario no autenticado → Login (configurado en Program.cs)

### 6. ✅ Validación en Vistas
- ✓ Menú sidebar filtrado por rol
- ✓ Badge de rol en navbar
- ✓ Nombre de usuario en navbar
- ✓ Opciones de cerrar/iniciar sesión

---

## 🔐 Roles y Permisos Implementados

### Administrador:
- ✅ Acceso total al sistema
- ✅ Gestión de usuarios (crear, editar, eliminar)
- ✅ Gestión de cursos
- ✅ Eliminar cursos
- ✅ Ver auditorías

### Profesor:
- ✅ Gestión de sus cursos
- ✅ Crear, editar, eliminar tareas
- ✅ Calificar entregas
- ✅ Ver dashboard
- ✅ Publicar anuncios
- ❌ NO puede gestionar usuarios
- ❌ NO puede eliminar cursos

### Estudiante:
- ✅ Ver sus cursos
- ✅ Ver tareas asignadas
- ✅ Entregar tareas
- ✅ Ver sus calificaciones
- ✅ Ver dashboard
- ❌ NO puede crear/editar tareas
- ❌ NO puede gestionar cursos
- ❌ NO puede gestionar usuarios

---

## 🧪 Cómo Probar el Sistema

### Paso 1: Configurar la Base de Datos
Necesitas crear usuarios de prueba con diferentes roles:

```sql
-- Insertar roles (si no existen)
INSERT INTO Roles (RoleId, RoleName, Description, CreateAt, IsSoftDeleted)
VALUES 
    (NEWID(), 'Administrador', 'Control total del sistema', GETDATE(), 0),
    (NEWID(), 'Profesor', 'Gestión de cursos y tareas', GETDATE(), 0),
    (NEWID(), 'Estudiante', 'Ver y entregar tareas', GETDATE(), 0);

-- Crear usuarios de prueba
DECLARE @AdminId UNIQUEIDENTIFIER = NEWID()
DECLARE @ProfesorId UNIQUEIDENTIFIER = NEWID()
DECLARE @EstudianteId UNIQUEIDENTIFIER = NEWID()

INSERT INTO Users (UserId, UserName, Email, PasswordHash, CreateAt, IsSoftDeleted)
VALUES 
    (@AdminId, 'admin', 'admin@test.com', 'hash123', GETDATE(), 0),
    (@ProfesorId, 'profesor', 'profesor@test.com', 'hash123', GETDATE(), 0),
    (@EstudianteId, 'estudiante', 'estudiante@test.com', 'hash123', GETDATE(), 0);

-- Asignar roles
INSERT INTO UserRoles (UserId, RoleId, AssignedAt)
VALUES 
    (@AdminId, (SELECT RoleId FROM Roles WHERE RoleName = 'Administrador'), GETDATE()),
    (@ProfesorId, (SELECT RoleId FROM Roles WHERE RoleName = 'Profesor'), GETDATE()),
    (@EstudianteId, (SELECT RoleId FROM Roles WHERE RoleName = 'Estudiante'), GETDATE());
```

### Paso 2: Crear un Controller de Login (PENDIENTE)
**NOTA IMPORTANTE:** Aún necesitas crear el `AccountController` para manejar login/logout.

Aquí hay un ejemplo básico:

```csharp
// Controllers/AccountController.cs
public class AccountController : Controller
{
    private readonly Context _context;

    public AccountController(Context context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefault(u => u.Email == email && !u.IsSoftDeleted);

        if (user != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
            };

            // Agregar roles
            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.RoleName));
            }

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal);

            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Credenciales inválidas";
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Login");
    }
}
```

### Paso 3: Pruebas a Realizar

#### Prueba como Administrador:
1. Iniciar sesión como admin
2. ✅ Debe ver menú completo (Usuarios, Auditoría, etc.)
3. ✅ Debe poder acceder a `/Usuarios/Index`
4. ✅ Debe poder eliminar cursos

#### Prueba como Profesor:
1. Iniciar sesión como profesor
2. ✅ Debe ver menú de profesor (Cursos, Tareas, Calificar, Anuncios)
3. ✅ Debe poder crear/editar tareas
4. ✅ NO debe poder acceder a `/Usuarios/Index` → Error 403
5. ✅ NO debe poder eliminar cursos → Error 403

#### Prueba como Estudiante:
1. Iniciar sesión como estudiante
2. ✅ Debe ver menú de estudiante (Cursos, Mis Entregas, Calificaciones)
3. ✅ Debe poder entregar tareas
4. ✅ NO debe poder crear tareas → Error 403
5. ✅ NO debe poder acceder a `/Usuarios/Index` → Error 403

---

## 📋 Checklist de Criterios de Aceptación

### ✅ Roles aplicados correctamente
- [x] Configuradas políticas en Program.cs
- [x] Creados atributos personalizados por rol
- [x] Aplicados atributos [Authorize] en controllers

### ✅ Usuarios no autorizados no acceden a recursos
- [x] TareasController protegido
- [x] CursosController protegido
- [x] UsuariosController solo para Admin
- [ ] **PENDIENTE:** Crear usuarios de prueba en BD

### ✅ Redirección apropiada en caso de acceso denegado
- [x] Creado ErrorController con acción Error403
- [x] Creada vista Error403.cshtml
- [x] Configurado middleware en Program.cs

### ✅ Menús muestran solo opciones autorizadas
- [x] Implementado filtro por rol en _Sidebar.cshtml
- [x] Menú muestra opciones según rol
- [x] Badge de rol visible en navbar

### ✅ Políticas de autorización funcionando
- [x] Política SoloAdministrador creada
- [x] Política SoloProfesor creada
- [x] Política SoloEstudiante creada
- [x] Política ProfesorOAdministrador creada
- [ ] **PENDIENTE:** Crear AccountController para login

---

## ⚠️ Tareas Pendientes

### 1. Crear AccountController
Necesitas crear el controller de autenticación para login/logout.

### 2. Crear Vistas de Login
```
Views/Account/Login.cshtml
```

### 3. Insertar Datos de Prueba
Crear usuarios con diferentes roles en la base de datos.

### 4. Proteger HomeController (opcional)
Decidir si el Home es público o requiere autenticación.

---

## 🎉 Resumen

**Implementado:**
- ✅ Sistema de autorización por roles
- ✅ Atributos personalizados
- ✅ Políticas de autorización
- ✅ Páginas de error 403/404
- ✅ Validación en vistas
- ✅ Menú dinámico según rol
- ✅ 3 Controllers protegidos (Tareas, Cursos, Usuarios)

**Falta:**
- ⏳ AccountController (Login/Logout)
- ⏳ Vistas de Login
- ⏳ Usuarios de prueba en BD

**Próximo paso:** Crear el AccountController para poder iniciar sesión y probar todo el sistema.
