# API con Autenticación JWT - Práctica 5

API desarrollada con **.NET 8** que implementa autenticación mediante JWT para la gestión de usuarios, productos, categorías y proveedores.

## 📋 Requisitos Previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server LocalDB](https://docs.microsoft.com/es-es/sql/database-engine/configure-windows/sql-server-express-localdb) (incluido con Visual Studio)
- [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/es-es/sql/ssms/download-sql-server-management-studio-ssms) (opcional, para ver la base de datos)
- Postman (opcional) o Swagger UI

## 🚀 Configuración Rápida

### 1. Clonar y configurar

```bash
git clone https://github.com/tuusuario/Practica5.git
cd Practica5/Practica5
```

### 2. Configurar la base de datos

En `appsettings.json`, verifica la cadena de conexión:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=UsuariosDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "EstaEsUnaClaveSuperSecretaParaJWT2025Con32Caracteres!",
    "Issuer": "Practica5API",
    "Audience": "Practica5Client",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  }
}
```

### 3. Crear y actualizar la base de datos

Ejecuta los siguientes comandos en la **Consola del Administrador de Paquetes** de Visual Studio (Tools → NuGet Package Manager → Package Manager Console):

```powershell
# Asegúrate que el proyecto seleccionado es "Practica5"
Update-Database
```

Esto creará automáticamente la base de datos `UsuariosDb` con las siguientes tablas:
- **Usuarios** - Gestión de usuarios y autenticación
- **Categorias** - Clasificación de productos
- **Proveedores** - Información de proveedores
- **Productos** - Artículos con relaciones a categorías y proveedores

### 4. Ejecutar la API

```bash
dotnet run
```

O presiona **F5** en Visual Studio.

La API estará disponible en:
- HTTP: http://localhost:5074
- HTTPS: https://localhost:7005
- Swagger UI: https://localhost:7005/swagger

## 📚 Endpoints

### Autenticación (Públicos)

#### Registrar usuario
**POST** `/api/auth/register`

```json
{
  "nombre": "Juan Pérez",
  "correo": "juan@example.com",
  "passwordHash": "123456"
}
```

#### Iniciar sesión
**POST** `/api/auth/login`

```json
{
  "correo": "juan@example.com",
  "password": "123456"
}
```

**Respuesta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "x7dG8hK9lM2nP4qR5sT6uV8wY0zA1bC3dE5fG7hI9jK...",
  "expiration": "2025-03-13T16:45:00Z",
  "nombre": "Juan Pérez",
  "correo": "juan@example.com"
}
```

#### Refrescar token
**POST** `/api/auth/refresh`

```json
{
  "refreshToken": "x7dG8hK9lM2nP4qR5sT6uV8wY0zA1bC3dE5fG7hI9jK..."
}
```

### CRUDs (Todos protegidos con JWT)

#### Categorías
- `GET /api/categorias` - Listar todas las categorías
- `GET /api/categorias/{id}` - Obtener categoría por ID
- `POST /api/categorias` - Crear nueva categoría
- `PUT /api/categorias/{id}` - Actualizar categoría
- `DELETE /api/categorias/{id}` - Eliminar categoría

#### Proveedores
- `GET /api/proveedores` - Listar todos los proveedores
- `GET /api/proveedores/{id}` - Obtener proveedor por ID
- `POST /api/proveedores` - Crear nuevo proveedor
- `PUT /api/proveedores/{id}` - Actualizar proveedor
- `DELETE /api/proveedores/{id}` - Eliminar proveedor

#### Productos
- `GET /api/productos` - Listar todos los productos (incluye categoría y proveedor)
- `GET /api/productos/{id}` - Obtener producto por ID
- `POST /api/productos` - Crear nuevo producto
- `PUT /api/productos/{id}` - Actualizar producto
- `DELETE /api/productos/{id}` - Eliminar producto

### Endpoints de Agregación (Productos)

| Endpoint | Método | Descripción |
|----------|--------|-------------|
| `/api/productos/estadisticas` | GET | Devuelve: precio más alto, más bajo, suma total y promedio |
| `/api/productos/por-categoria/{id}` | GET | Lista productos de una categoría específica |
| `/api/productos/por-proveedor/{id}` | GET | Lista productos de un proveedor específico |
| `/api/productos/cantidad-total` | GET | Devuelve la cantidad total de productos y el stock total |

**Ejemplo respuesta `/api/productos/estadisticas`:**
```json
{
  "precioMasAlto": {
    "precio": 1500.99,
    "producto": "Laptop Gamer"
  },
  "precioMasBajo": {
    "precio": 15.50,
    "producto": "Mouse Pad"
  },
  "sumaTotalPrecios": 12580.45,
  "precioPromedio": 425.75,
  "cantidadTotalProductos": 25
}
```

## 🔌 Conexión a SQL Server Management Studio

Para ver la base de datos en SSMS:

1. Abre **SQL Server Management Studio**
2. En "Nombre del servidor", escribe: `(localdb)\MSSQLLocalDB`
3. Usa "Windows Authentication"
4. Haz clic en "Conectar"

La base de datos `UsuariosDb` aparecerá en el Explorador de objetos con todas sus tablas.


## ✅ Validaciones implementadas

| Entidad | Campo | Validaciones |
|---------|-------|--------------|
| Usuario | Nombre | Required, MaxLength(100), MinLength(2) |
| Usuario | Correo | Required, EmailAddress, MaxLength(200), Unique |
| Usuario | PasswordHash | Required, MinLength(6), MaxLength(100) |
| Categoría | Nombre | Required, MaxLength(100), Unique |
| Proveedor | Nombre | Required, MaxLength(150), Unique |
| Proveedor | Contacto | MaxLength(100) |
| Producto | Nombre | Required, MaxLength(200) |
| Producto | Precio | Required, Range(0.01, double.MaxValue) |
| Producto | Stock | Required, Range(0, int.MaxValue) |

## 🔐 Autenticación JWT

- Las contraseñas se almacenan hasheadas con **SHA-256**
- Los Access Tokens expiran en **15 minutos**
- Los Refresh Tokens expiran en **7 días**
- Cada usuario solo puede modificar/eliminar su propio perfil

## 📦 Paquetes NuGet utilizados

```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.5.1" />
```


## 📁 Estructura del Proyecto

```
Practica5/
├── Controllers/
│   ├── AuthController.cs
│   ├── UsuariosController.cs
│   ├── CategoriasController.cs
│   ├── ProveedoresController.cs
│   └── ProductosController.cs
├── Models/
│   ├── Usuario.cs
│   ├── Categoria.cs
│   ├── Proveedor.cs
│   ├── Producto.cs
│   ├── LoginModel.cs
│   ├── RefreshTokenModel.cs
│   └── TokenResponse.cs
├── Services/
│   └── TokenService.cs
├── Data/
│   └── AppDbContext.cs
├── Migrations/
├── appsettings.json
└── Program.cs
```

