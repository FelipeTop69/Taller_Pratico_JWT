
# Taller Seguridad .NET - Autenticación con JWT y OAuth 2.0

Este taller implementa una API ASP.NET Core utilizando arquitectura en capas (N-Capas) y principios SOLID para resolver escenarios de autenticación moderna. Se aplican diferentes flujos de OAuth 2.0 y validación de tokens JWT.

---

## 🔧 Arquitectura del proyecto

```
/API_JWT.Security.API         → Controladores y configuración API
/API_JWT.Security.Business    → Servicios y lógica de autenticación
/API_JWT.Security.Data        → Repositorios y acceso a base de datos
/API_JWT.Security.Entity      → Modelos, DTOs, ApplicationDbContext
```

---

## ✅ Ejercicios resueltos

### 1. **Generación de JWT**
- Endpoint: `/api/auth/ejercicio1/`
- Entrada: `username` + `password`
- Salida: JWT con claims `name` y `role`, expiración de 30 minutos

### 2. **Autenticación básica**
- Endpoint: `/api/auth/ejercicio2/`
- Verifica credenciales del usuario
- Respuesta: mensaje de bienvenida personalizado

### 3. **OAuth 2.0 Authorization Code Grant**
- Endpoints: `/api/oauth/authorize/`, `/api/oauth/token/`
- Simula redirección con código
- Intercambio de código por token de acceso

---

## ✅ Ejercicio 5: Comparación de Flujos de OAuth 2.0

### Flujos implementados

#### ✅ Authorization Code Grant
- Implementado: `/authorize/` + `/token/`
- Seguridad alta, ideal para aplicaciones web
- Requiere redirección y código de autorización

#### 🆕 Client Credentials Grant
- Implementado: `/client/token/`
- Ideal para comunicación entre servicios backend
- No involucra usuario, seguridad alta

#### 🆕 Password Grant (Resource Owner)
- Implementado: `/password/token/`
- Simple pero menos seguro
- Enviar usuario y contraseña directo al cliente

### Comparación

| Flujo                        | Uso ideal                | Seguridad     | Requiere UI | Involucra usuario |
|-----------------------------|--------------------------|---------------|-------------|-------------------|
| Authorization Code (✅)     | Web apps, SPAs           | 🔒 Alta        | ✅ Sí        | ✅ Sí              |
| Client Credentials (🆕)      | Servicios/M2M            | 🔒 Alta        | ❌ No        | ❌ No              |
| Password Grant (🆕)          | Apps internas, legacy    | ⚠️ Baja        | ❌ No        | ✅ Sí              |

---

## 🛡️ Validación de JWT personalizada (Ejercicio 4)

Middleware personalizado (`JwtValidationMiddleware.cs`) intercepta solicitudes:
- Verifica si el endpoint requiere autorización (`[Authorize]`)
- Valida que el token sea válido y tenga el rol requerido
- Devuelve errores 401/403 con mensajes claros si no cumple

---

## 🔐 Configuración de seguridad

```json
// appsettings.Secrets.json
"Jwt": {
  "Key": "YouWillNeverWalkAloneFromColombiaToMexico",
  "Issuer": "Taller.API.JWT",
  "Audience": "Taller.Client"
}
```

Registrado con:
```csharp
builder.Services.AddJwtAuthentication(builder.Configuration);
```

---

## 📂 Ejemplo de llamada a endpoints

### Obtener token:
```http
POST /api/auth/ejercicio1/
{
  "username": "admin",
  "password": "1234"
}
```

### Usar token:
```http
GET /api/oauth/protected/hello/
Authorization: Bearer <access_token>
```
