# iCredito API

**iCredito API** es un backend RESTful para la gestión de tarjetas de crédito, diseñado como un **caso práctico realista** para demostrar **Arquitectura Hexagonal**, **Monolito Modular** y el patrón **BFF (Backend for Frontend)** usando **.NET 8**.

> **Objetivo del proyecto**
> Construir un **backend profesional**, con reglas de negocio claras, límites bien definidos y un dominio independiente de la infraestructura, listo para escalar y fácil de testear.

---

## Tabla de Contenidos

* [Descripción General](#-descripción-general)
* [Qué Resuelve iCredito](#-qué-resuelve-icredito)
* [Decisiones de Diseño](#-decisiones-de-diseño)
* [Arquitectura](#-arquitectura)

  * [Arquitectura Hexagonal](#arquitectura-hexagonal)
  * [Monolito Modular](#monolito-modular)
  * [Patrón BFF](#patrón-bff)
* [Dashboard & KPIs](#-dashboard--kpis)
* [Estructura del Proyecto](#-estructura-del-proyecto)
* [Patrones de Diseño](#-patrones-de-diseño)
* [Módulos del Sistema](#-módulos-del-sistema)
* [API Endpoints](#-api-endpoints)
* [Configuración e Instalación](#-configuración-e-instalación)
* [Base de Datos](#-base-de-datos)
* [Seguridad](#-seguridad)
* [Flujos de Negocio](#-flujos-de-negocio)
* [Tecnologías Utilizadas](#-tecnologías-utilizadas)
* [Licencia](#-licencia)
* [Autor](#-autor)

---

## Descripción General

**iCredito API** simula el core de un sistema de tarjetas de crédito real. No es un CRUD básico: el foco está en **modelar correctamente el dominio financiero**, separar responsabilidades y mantener el código preparado para crecer.

### Características Clave

* Autenticación de usuarios con JWT
* Gestión completa de tarjetas de crédito
* Procesamiento de pagos (simulado)
* Registro e historial de transacciones
* Dashboard agregado optimizado para frontend
* KPIs financieros y operativos

---

## Qué Resuelve iCredito

* Registro y autenticación segura de usuarios
* Emisión y administración de tarjetas
* Cargos, pagos y reembolsos
* Consistencia transaccional
* Visualización agregada de métricas de negocio

Todo el flujo está pensado como en un sistema real: con **estados**, **reglas**, **eventos de dominio** y **límites claros entre capas**.

---

## Decisiones de Diseño

Este proyecto responde preguntas comunes en equipos reales y entrevistas técnicas:

* ¿Cómo evitar que el dominio dependa de Entity Framework?
* ¿Cómo organizar un monolito sin que se vuelva inmanejable?
* ¿Cómo modelar reglas de negocio sin lógica en controllers?
* ¿Cómo escalar sin saltar directamente a microservicios?

Los pilares elegidos son:

* Arquitectura Hexagonal
* Monolito Modular
* Patrón BFF

---

## Arquitectura

### Arquitectura Hexagonal

La lógica de negocio vive en el **centro del sistema** y no conoce detalles como HTTP, EF Core, JWT o la base de datos.

Beneficios:

* Dominio altamente testeable
* Infraestructura intercambiable
* Bajo acoplamiento
* Código que envejece bien

Capas principales:

* **Domain**: entidades, value objects, reglas y eventos
* **Application**: casos de uso y orquestación
* **Infrastructure**: persistencia, servicios externos y controllers

---

### Monolito Modular

iCredito es un **monolito bien organizado**, dividido por módulos funcionales:

* Auth
* Cards
* Payments
* Transactions

Cada módulo:

* Tiene su propio dominio
* Define sus propios puertos
* No accede directamente a otros módulos

Esto permite escalar el código y migrar módulos a microservicios si algún día es necesario.

---

### Patrón BFF

El **Backend for Frontend** agrega y transforma datos de múltiples módulos para entregar respuestas optimizadas al frontend.

Ventajas:

* Menos llamadas HTTP
* ViewModels listos para UI
* Mejor performance
* Menor acoplamiento frontend-backend

---

## Dashboard & KPIs

El proyecto incluye un **dashboard unificado** (no separado) que combina información operativa y analítica.

### KPIs Incluidos

**Financieros**

* Gasto total del mes actual
* Comparación con mes anterior
* Variación porcentual
* Balance total y crédito disponible

**Tarjetas**

* Total de tarjetas
* Tarjetas activas vs bloqueadas
* Distribución por marca
* Distribución por tipo

**Transacciones**

* Total de transacciones
* Compras vs pagos
* Ticket promedio

**Tendencias**

* Evolución mensual de gastos
* Patrones de uso

El BFF expone un endpoint agregado:

```
GET /api/bff/dashboard
```

El frontend consume una sola llamada y renderiza todo el dashboard.

---

## Estructura del Proyecto

```
iCredito.Api/
├── Shared/
│   ├── Domain/
│   ├── Application/
│   └── Infrastructure/
│
├── Modules/
│   ├── Auth/
│   ├── Cards/
│   ├── Payments/
│   └── Transactions/
│
├── BFF/
│   ├── Controllers/
│   ├── ViewModels/
│   └── Services/
│
├── API/
│   └── Controllers/
│
├── Program.cs
└── appsettings.json
```

---

## Patrones de Diseño

* Result Pattern
* Value Objects
* Aggregate Root
* Repository Pattern
* Domain Events
* Dependency Injection por módulo

Todos aplicados con foco en claridad y mantenibilidad.

---

## Módulos del Sistema

### Auth

* Registro y login
* Hashing con BCrypt
* Generación de JWT

### Cards

* Emisión y administración de tarjetas
* Validación Luhn
* Control de estados

### Payments

* Flujo de pagos con estados
* Procesador simulado
* Consistencia transaccional

### Transactions

* Registro inmutable de movimientos
* Consultas paginadas y optimizadas

---

##  API Endpoints

### Auth

```
POST   /api/auth/register
POST   /api/auth/login
GET    /api/auth/profile
```

### Cards

```
GET    /api/cards
POST   /api/cards
PUT    /api/cards/{id}
DELETE /api/cards/{id}
POST   /api/cards/{id}/block
POST   /api/cards/{id}/activate
```

### Payments

```
POST   /api/payments
GET    /api/payments
POST   /api/payments/{id}/refund
```

### Transactions

```
GET /api/transactions
GET /api/transactions/card/{cardId}
```

### BFF

```
GET /api/bff/dashboard
```

---

## Configuración e Instalación

### Requisitos

* .NET 8 SDK
* PostgreSQL 14+

### Pasos

```bash
git clone https://github.com/tu-usuario/icredito_api.git
cd iCredito.Api
dotnet restore
dotnet ef database update
dotnet run
```

---

## Base de Datos

* PostgreSQL
* Migraciones con EF Core
* Índices optimizados por UserId y fechas

---

## Seguridad

* Autenticación JWT Bearer
* Hashing de contraseñas con BCrypt
* Validaciones de propiedad de recursos
* Tokens con expiración configurable

---

## Flujos de Negocio

Ejemplo: **Realizar una compra**

1. Validar tarjeta y usuario
2. Crear Payment (Pending)
3. Procesar pago (simulado)
4. Cargar a la tarjeta
5. Registrar transacción
6. Confirmar pago

---

## Tecnologías Utilizadas

* .NET 8 / ASP.NET Core
* Entity Framework Core 8
* PostgreSQL
* JWT Bearer Authentication
* BCrypt
* FluentValidation
* Swagger / OpenAPI

---

## Licencia

Proyecto de uso educativo y demostrativo.

---

##  Autor

**Jose Alexander Ríos Trespalacios**
Caso práctico de arquitectura de software con .NET 8.
