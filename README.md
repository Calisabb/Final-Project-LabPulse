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

## 📌 О проекте / Project Overview

**LabPulse** — это специализированная централизованная система мониторинга и оперативного управления серверными узлами домашней лаборатории (**Homelab Operations Hub**). 

Система обеспечивает непрерывную фоновую телеметрию сервисов по протоколам **HTTP/HTTPS** и низкоуровневым сокетам **TCP** (DNS, базы данных, кэши, файловые хранилища), отслеживает историю доступности с построением 30-дневной матрицы аптайма, регистрирует инциденты отказов и предоставляет быстрый доступ к автоматизированным ранбукам (runbooks) для моментального устранения неполадок.

Интерфейс выполнен в аутентичной эстетике **eDEX-UI / Tron** с кинематографичной CRT-сеткой, неоновой индикацией статусов, звуковым сопровождением на базе **Web Audio API** и 4-зонной компоновкой командного пункта.

---

## ✨ Ключевые возможности / Key Features

### 1. 🎛️ 4-Зонная архитектура командного терминала (4-Zone Layout)
- **Зона 1 (Слева — Telemetry Deck):**
  - Цифровые часы реального времени с микросекундной синхронизацией.
  - Динамический обратный отсчёт цикла фонового сканирования (`SWEEP: 30s`).
  - Измеритель номинального здоровья инфраструктуры (`100% ALL SYSTEMS OPERATIONAL`).
  - Фильтры кластеров (`Network Gateways`, `Databases`, `Dev Environment`, `Media`).
- **Зона 2 (Центр — Monitored Nodes & Blades):**
  - Поисковая строка терминала с префиксом `>_` и поддержкой горячих клавиш.
  - Сегментированные фильтры статуса (`ALL`, `ONLINE`, `OFFLINE`).
  - Серверные лезвия узлов с 30-дневной sparkline-матрицей аптайма и временем отклика.
  - Быстрые действия узла: `[PING]`, `[INSPECT]`, `[EDIT]`, `[DEL]`.
  - Полноэкранный поток зарегистрированных аварийных инцидентов (`ACTIVE_INCIDENTS`).
- **Зона 3 (Справа — Network Matrix & Live Feed):**
  - Матрица прослушиваемых портов (`:53`, `:443`, `:5046`, `:5432`, `:6379`, `:8096`) с цветными LED-индикаторами состояния.
  - Терминальный поток сетевых проб (`[04 // PROBE_FEED]`) с интерактивной очисткой буфера `[FLUSH]`.
- **Зона 4 (Нижняя консоль — Tactical Action Deck):**
  - **Ранбуки быстрого копирования:** Команды PowerShell, Docker и Bash с мгновенным копированием в буфер и неоновым циан-фидбеком (`COPIED // 0x01`).
  - **Тактическая клавиатура управления:** Кнопки быстрого добавления узла `[+ NODE]`, принудительного сканирования `[SWEEP ALL]`, управления кластерами `[CLUSTERS]` и создания резервной копии `[BACKUP]`.

### 2. 🔌 Двухстековое зондирование (Dual-Stack Network Probes)
- **HTTP/HTTPS Probes:** Проверка статус-кодов (200–299), замер круговой задержки (RTT) в миллисекундах и валидация путей проверки здоровья (`/health`, `/openapi/v1.json`).
- **Raw TCP Handshake Probes:** Выполнение асинхронного 3-стороннего рукопожатия TCP (`TcpClient.ConnectAsync`) для сервисов без HTTP-интерфейса (DNS-резолверы на 53 порту, PostgreSQL `5432`, Redis `6379`, TrueNAS SMB `445`).

### 3. 🛡️ Умный трекинг инцидентов (Incident Ledger)
- **Порог 3 сбоев (Failure Threshold):** Предотвращение ложных тревог и флаппинга сетевых пакетов — инцидент регистрируется только при трёх последовательных неудачных пробах.
- Автоматический расчёт длительности аварии (`Duration`), фиксация точной причины отказа (`FAIL_CAUSE: Connection refused / timeout`) и автоматическое закрытие инцидента при восстановлении связи.

### 4. 🔊 Синтезированный звуковой движок (Web Audio SFX Engine)
- Реализован на чистом **Web Audio API** — **0 байт внешних аудиофайлов**, нулевая задержка.
- Физические тактильные щелчки клавиш, двойной футуристичный сигнал при копировании команд, сонарный пинг, частотный свип радарного сканирования, аварийный диссонирующий клаксон модалки и суб-басовый импульс удаления.
- Интерактивный тумблер **`[SFX: ON / OFF]`** в шапке с сохранением состояния в `localStorage`.

### 5. ⚠️ Кибер-терминальная модалка подтверждения (Destructive Purge Modal)
- Полная замена стандартных браузерных алертов на обсидиановое окно `[0xFA // DESTRUCTIVE_PURGE_CONFIRMATION]` с неоновой рубиновой подсветкой, аварийным LED, отображением хоста цели и предупреждением о безвозвратной очистке данных.

### 6. 🌱 Идемпотентный автоматический Data Seeder
- При первом запуске приложение автоматически применяет миграции базы данных (`Database.MigrateAsync()`) и заполняет базу 4 категориями, 8 реальными сервисами (HTTP + TCP) и набором готовых ранбуков. Проект можно клонировать на любой компьютер и сразу проводить демонстрацию без ручной настройки.

---

## 🏛️ Архитектура проекта / Architecture

Проект построен по принципам **Clean Architecture** с четким разделением ответственности на три независимых слоя:

```
LabPulse/
├── LabPulse.API/              # Представление & Хостинг
│   ├── Controllers/           # REST API контроллеры (JWT Protected)
│   ├── Handlers/              # Global Exception Handler (ProblemDetails)
│   ├── Workers/               # HealthCheckWorker (BackgroundService, такт 30s)
│   └── wwwroot/               # Single Page Application (SPA)
│       ├── css/               # tokens.css, base.css, components.css
│       ├── js/                # sound.js, format.js, auth.js, api.js, app.js
│       └── index.html         # 4-зонный eDEX-терминал
├── LabPulse.BLL/              # Бизнес-логика (Application Core)
│   ├── DTOs/                  # Запросы и ответы API
│   ├── Mapping/               # Чистые мапперы сущностей
│   ├── Options/               # Strongly-typed настройки (JwtOptions)
│   └── Services/              # Реализация доменных сервисов
│       ├── NetworkProbeService.cs # HTTP и TCP сокетные пробы
│       ├── HealthCheckService.cs  # Пакетная параллельная проверка
│       └── BackupService.cs       # Импорт/экспорт конфигурации
└── LabPulse.DAL/              # Доступ к данным (Infrastructure & Data)
    ├── DataContext/           # LabPulseDbContext (EF Core 10)
    │   ├── Configurations/    # Fluent API конфигурации сущностей
    │   └── Entities/          # Service, Category, Incident, HealthLog, Runbook, User
    ├── Migrations/            # EF Core миграции схемы БД
    ├── Repositories/          # Асинхронные репозитории (Generic + Specialized)
    ├── AdminSeeder.cs         # Инициализация учётной записи администратора
    └── DataSeeder.cs          # Автоматический сидер сервисов, портов и ранбуков
```

---

## 🚀 Быстрый старт / Quick Start Guide

### Требования:
- [.NET 10.0 SDK](https://dotnet.microsoft.com/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/) (Express / LocalDB / Standard)

### 1. Клонирование репозитория
```bash
git clone https://github.com/Calisabb/Final-Project-LabPulse.git
cd Final-Project-LabPulse
```

### 2. Настройка подключения к базе данных
В файле `LabPulse.API/appsettings.json` проверьте строку подключения (по умолчанию настроена на локальный `localhost\SQLEXPRESS`):
```json
"ConnectionStrings": {
  "Default": "Server=localhost\\SQLEXPRESS;Database=LabPulseDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```

### 3. Запуск приложения
```bash
dotnet run --project LabPulse.API
```
> **Примечание:** При первом запуске `DataSeeder` автоматически создаст базу данных, накатит все миграции и заполнит демонстрационные сервисы и учётную запись администратора.

### 4. Вход в панель управления
Откройте в браузере: **`http://localhost:5046`** (или **`https://localhost:7214`**)

- **Логин:** `admin`
- **Пароль:** `MyAdmin#2026pass`

---

## 📡 Спецификация REST API / Endpoints

Все эндпоинты защищены авторизацией **JWT Bearer Token** (за исключением эндпоинта аутентификации).

| Метод | URL | Описание |
| :--- | :--- | :--- |
| `POST` | `/api/auth/login` | Аутентификация оператора и выдача JWT токена |
| `GET` | `/api/services` | Получение списка всех узлов с фильтрацией по статусу/кластеру |
| `POST` | `/api/services` | Добавление нового сервиса в мониторинг |
| `GET` | `/api/services/{id}` | Детальная телеметрия выбранного сервиса |
| `PUT` | `/api/services/{id}` | Обновление конфигурации и параметров пробы |
| `DELETE`| `/api/services/{id}` | Удаление сервиса и каскадная очистка логов |
| `POST` | `/api/services/{id}/check` | Немедленная ручная проверка узла (`[PING]`) |
| `POST` | `/api/services/check-all` | Пакетный параллельный опрос всей сети (`[SWEEP ALL]`) |
| `GET` | `/api/services/{id}/uptime` | Расчёт среднего аптайма за 30 дней и статистика |
| `GET` | `/api/services/{id}/runbooks`| Список ранбуков быстрого решения для сервиса |
| `POST` | `/api/runbooks` | Создание нового скрипта ранбука |
| `DELETE`| `/api/runbooks/{id}` | Удаление ранбука |
| `GET` | `/api/categories` | Список категорий/кластеров инфраструктуры |
| `POST` | `/api/categories` | Создание кластера с выбором цвета (#HEX) |
| `DELETE`| `/api/categories/{id}` | Удаление кластера |
| `GET` | `/api/incidents` | Поток зафиксированных аварийных инцидентов |
| `GET` | `/api/backup/export` | Экспорт всей топологии сервисов в JSON-файл |
| `POST` | `/api/backup/import` | Импорт конфигурации из JSON-файла |

---

## 🛠️ Стек технологий / Technology Stack

- **Платформа:** .NET 10.0, C# 13.0
- **ORM & Data:** Entity Framework Core 10, SQL Server, Code-First Migrations
- **Безопасность:** ASP.NET Core Identity, JWT Bearer Tokens, Role-Based Access Control (`Admin`)
- **Документация:** OpenAPI v3, Scalar Interactive API Documentation (`/scalar/v1`)
- **Фронтенд:** Vanilla JavaScript (ES6+), Alpine.js v3 (реактивное состояние), HTML5, SVG Icons
- **Стилизация:** Чистый модульный CSS3, Custom Design Tokens, CRT Scanlines, Blueprint Grid Overlay
- **Аудио:** Web Audio API Synthesizer (Zero-dependency audio)

---

## 👨‍💻 Автор / Author

- **Разработчик:** Calisabb
- **Учебное заведение:** Code Academy
- **Проект:** Final Graduation Project (Дипломный проект)

---

<p align="center">
  <sub>LabPulse Telemetry Engine // Designed for Resilient Homelab Infrastructure</sub>
</p>
