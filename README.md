# LabPulse ⚡ Homelab Service Hub & Health Operations Console

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 10" />
  <img src="https://img.shields.io/badge/C%23-13.0-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="C# 13" />
  <img src="https://img.shields.io/badge/EF%20Core-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="EF Core 10" />
  <img src="https://img.shields.io/badge/SQL_Server-2022-CC292B?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="SQL Server" />
  <img src="https://img.shields.io/badge/Alpine.js-3.x-8BC0D0?style=for-the-badge&logo=alpinedotjs&logoColor=black" alt="Alpine.js" />
  <img src="https://img.shields.io/badge/Architecture-Clean%20Architecture-00f2ff?style=for-the-badge" alt="Clean Architecture" />
  <img src="https://img.shields.io/badge/UI_Aesthetic-eDEX--UI%20%2F%20Tron-00ff88?style=for-the-badge" alt="eDEX-UI" />
</p>

---

## 📌 Project Overview

**LabPulse** is a specialized, centralized service monitoring hub and health operations console engineered for modern homelab infrastructure (**Homelab Operations Hub**).

It provides continuous background telemetry across **HTTP/HTTPS** web endpoints and low-level **TCP sockets** (DNS resolvers, databases, in-memory caches, storage pools), aggregates 30-day uptime sparkline matrices, automates incident logging with exact failure root-causes, and equips operators with a tactical runbook deck for rapid remediation.

The interface is built in an authentic **eDEX-UI / Tron** cyber-terminal aesthetic featuring CRT scanlines, neon signal indicators, pure **Web Audio API** sound synthesis, and a 4-zone operations layout.

---

## ✨ Key Features

### 1. 🎛️ 4-Zone Command Terminal Architecture
- **Zone 1 (Left — Telemetry Deck):**
  - Real-time digital clock ticker with date and session uptime counters.
  - Dynamic 30-second background worker sweep countdown (`SWEEP: 30s`).
  - Nominal infrastructure health gauge (`100% ALL SYSTEMS OPERATIONAL`).
  - Dynamic category cluster filters (`Network Gateways`, `Databases`, `Dev Environment`, `Media`).
- **Zone 2 (Center — Monitored Nodes & Blades):**
  - Terminal search prompt with `>_` indicator and keyboard accessibility.
  - Segmented status toggles (`ALL`, `ONLINE`, `OFFLINE`).
  - Hardware server blades displaying 30-day uptime sparkline matrices and round-trip latency.
  - Instant node actions: `[PING]`, `[INSPECT]`, `[EDIT]`, `[DEL]`.
  - Comprehensive outage stream tab (`ACTIVE_INCIDENTS`).
- **Zone 3 (Right — Network Matrix & Live Feed):**
  - Monitored port matrix (`:53`, `:443`, `:5046`, `:5432`, `:6379`, `:8096`) with status LEDs.
  - Real-time scrolling probe telemetry stream (`[04 // PROBE_FEED]`) with an interactive buffer `[FLUSH]` control.
- **Zone 4 (Bottom Console — Tactical Action Deck):**
  - **Quick Runbook Command Chips:** One-click copy for PowerShell, Docker, and Bash runbooks with visual cyan flash feedback (`COPIED // 0x01`).
  - **Tactical Operations Keypad:** Quick-action keys for adding nodes `[+ NODE]`, batch sweeps `[SWEEP ALL]`, cluster management `[CLUSTERS]`, and JSON configuration exports `[BACKUP]`.

### 2. 🔌 Dual-Stack Network Probes (HTTP & TCP)
- **HTTP/HTTPS Probes:** Status code validation (200–299), round-trip response time (RTT) benchmarking, and customizable health check paths (`/health`, `/openapi/v1.json`).
- **Raw TCP Socket Handshakes:** Asynchronous 3-way TCP handshake verification (`TcpClient.ConnectAsync`) for non-HTTP services (DNS resolvers on port 53, PostgreSQL `5432`, Redis `6379`, TrueNAS SMB `445`).

### 3. 🛡️ Intelligent Incident Ledger & Flapping Protection
- **3-Strike Failure Threshold:** Prevents false alarms from transient network jitter — outage incidents are opened only after 3 consecutive probe failures.
- Automatic outage duration calculation, root-cause diagnostics capture (`FAIL_CAUSE: Connection refused / timeout`), and automatic incident resolution upon service recovery.

### 4. 🔊 Synthesized Web Audio Sound Engine
- Zero external audio files, zero latency, 100% procedural sound synthesis powered by the native **Web Audio API**.
- Authentic tactile mechanical clicks, randomized typing ticks, double-chirp clipboard confirmations, telemetry sonar pings, frequency radar sweeps, dissonant hazard klaxons, and sub-bass purge thuds.
- Integrated **`[SFX: ON / OFF]`** toggle in the top status bar with `localStorage` persistence.

### 5. ⚠️ Cyber-Terminal Destructive Purge Modal
- Complete replacement of standard browser alerts with an obsidian HUD dialog `[0xFA // DESTRUCTIVE_PURGE_CONFIRMATION]` featuring neon ruby hazards, target metadata identification, and critical failsafe warnings.

### 6. 🌱 Idempotent Database & Demo Seeder
- Automatic database migration (`Database.MigrateAsync()`) and seeding on startup (`DataSeeder.cs`). Pre-populates 4 clusters, 8 realistic services (HTTP & TCP), and linked runbooks. Clones run out-of-the-box on any machine without manual database setup.

---

## 🏛️ Architecture & Clean Layering

The solution strictly adheres to **Clean Architecture** principles, maintaining clear separation of concerns across three distinct layers:

```
LabPulse/
├── LabPulse.API/              # Presentation & Host Layer
│   ├── Controllers/           # REST API Controllers (JWT Protected)
│   ├── Handlers/              # Global Exception Handler (RFC 7807 ProblemDetails)
│   ├── Workers/               # HealthCheckWorker (BackgroundService, 30s cycle)
│   └── wwwroot/               # Single Page Application (SPA)
│       ├── css/               # tokens.css, base.css, components.css
│       ├── js/                # sound.js, format.js, auth.js, api.js, app.js
│       └── index.html         # 4-zone eDEX operations console
├── LabPulse.BLL/              # Business Logic Layer (Application Core)
│   ├── DTOs/                  # API Contracts & Data Transfer Objects
│   ├── Mapping/               # Entity to DTO mappers
│   ├── Options/               # Strongly-typed configuration options (JwtOptions)
│   └── Services/              # Domain service implementations
│       ├── NetworkProbeService.cs # HTTP & TCP socket probe execution
│       ├── HealthCheckService.cs  # Parallel bulk health checking
│       └── BackupService.cs       # JSON configuration backup & restore
└── LabPulse.DAL/              # Data Access Layer (Infrastructure & Data)
    ├── DataContext/           # LabPulseDbContext (EF Core 10)
    │   ├── Configurations/    # EntityTypeConfiguration classes (Fluent API)
    │   └── Entities/          # Service, Category, Incident, HealthLog, Runbook, User
    ├── Migrations/            # EF Core Code-First Migrations
    ├── Repositories/          # Async Repositories (Generic & Specialized contracts)
    ├── AdminSeeder.cs         # Identity Administrator seed logic
    └── DataSeeder.cs          # Automatic services, clusters, and runbooks seeder
```

---

## 🚀 Quick Start Guide

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/) (Express, LocalDB, or Standard)

### 1. Clone the Repository
```bash
git clone https://github.com/Calisabb/Final-Project-LabPulse.git
cd Final-Project-LabPulse
```

### 2. Configure Connection String
Verify the database connection string in `LabPulse.API/appsettings.json` (defaults to local `localhost\SQLEXPRESS`):
```json
"ConnectionStrings": {
  "Default": "Server=localhost\\SQLEXPRESS;Database=LabPulseDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```

### 3. Run the Application
```bash
dotnet run --project LabPulse.API
```
> **Note:** On first startup, `DataSeeder` automatically migrates the database, registers the default administrator, and seeds all demo clusters, services, and runbooks.

### 4. Access the Operations Console
Open your browser and navigate to: **`http://localhost:5046`** (or **`https://localhost:7214`**)

- **Operator Username:** `admin`
- **Operator Password:** `MyAdmin#2026pass`

---

## 📡 REST API Specification

All endpoints (except `/api/auth/login`) require a valid **JWT Bearer Token** in the `Authorization` header.

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `POST` | `/api/auth/login` | Authenticates operator and issues JWT bearer token |
| `GET` | `/api/services` | Retrieves all monitored services (filters: status, category, search) |
| `POST` | `/api/services` | Registers a new service for telemetry monitoring |
| `GET` | `/api/services/{id}` | Retrieves detailed telemetry metrics for a service |
| `PUT` | `/api/services/{id}` | Updates service configuration and probe parameters |
| `DELETE`| `/api/services/{id}` | Permanently removes service and cascades related logs |
| `POST` | `/api/services/{id}/check` | Triggers an immediate probe check (`[PING]`) |
| `POST` | `/api/services/check-all` | Executes a batch parallel probe sweep across all nodes (`[SWEEP ALL]`) |
| `GET` | `/api/services/{id}/uptime` | Computes 30-day average uptime percentage and telemetry points |
| `GET` | `/api/services/{id}/runbooks`| Lists quick-action runbooks linked to a service |
| `POST` | `/api/runbooks` | Creates a new operational runbook command |
| `DELETE`| `/api/runbooks/{id}` | Removes a runbook command |
| `GET` | `/api/categories` | Retrieves all infrastructure cluster categories |
| `POST` | `/api/categories` | Creates a new cluster category with hex color code |
| `DELETE`| `/api/categories/{id}` | Removes a cluster category |
| `GET` | `/api/incidents` | Streams ongoing and resolved outage incident records |
| `GET` | `/api/backup/export` | Exports entire infrastructure topology to a JSON file |
| `POST` | `/api/backup/import` | Restores infrastructure configuration from a JSON file |

---

## 🛠️ Technology Stack

- **Framework:** .NET 10.0, C# 13.0
- **Database & ORM:** Entity Framework Core 10, SQL Server 2022, Code-First Migrations
- **Security & Identity:** ASP.NET Core Identity, JWT Bearer Authentication, Role-Based Access Control (`Admin`)
- **API Documentation:** OpenAPI v3, Scalar Interactive API Documentation (`/scalar/v1`)
- **Frontend SPA:** Vanilla JavaScript (ES6+), Alpine.js v3 (Reactive State), HTML5, Inline SVG Sprites
- **Styling Architecture:** Modular Vanilla CSS3, Custom Design Tokens, CRT Scanline Shaders, Blueprint Grids
- **Audio Synthesis:** Web Audio API Procedural Synthesizer (Zero external dependencies)

---

## 👨‍💻 Author

- **Developer:** Calisabb
- **Institution:** Code Academy
- **Project:** Final Graduation Project (Capstone Project)

---

<p align="center">
  <sub>LabPulse Telemetry Engine // Engineered for Resilient Homelab Infrastructure</sub>
</p>
