# HelpYourself — «Самопомощь»

> **Не анализируй. Действуй.**

Мобильное приложение экстренной самопомощи. За 3 минуты проводит пользователя через персонализированный ритуал — дыхание, движение, проговаривание — сгенерированный LLM на основе выбранного эмоционального состояния.

---

## Содержание

- [Концепция](#концепция)
- [Стек технологий](#стек-технологий)
- [Структура репозитория](#структура-репозитория)
- [Быстрый старт](#быстрый-старт)
- [Бэкенд](#бэкенд)
- [Мобильное приложение](#мобильное-приложение)
- [API Reference](#api-reference)
- [LLM и промпт-инжиниринг](#llm-и-промпт-инжиниринг)
- [Дорожная карта](#дорожная-карта)

---

## Концепция

Пользователь открывает приложение в момент острого эмоционального состояния. **Максимум 2 клика** — и начинается 3-минутный ритуал.

Три архетипа состояний:

| Архетип | Образ | Описание |
|---------|-------|----------|
| 🔥 **Fire** | Накрывает / Взрываюсь | Гнев, паника, сенсорная перегрузка |
| 💧 **Water** | Тону / Нет сил | Апатия, грусть, пустота |
| 🌀 **Spiral** | Мысли по кругу | Руминации, тревога, самокопание |

Каждый ритуал состоит из трёх фаз:

1. **Заземление** (60 сек) — переключение внимания с мыслей на тело и органы чувств
2. **Кульминация** (90 сек) — физическая разрядка или мягкая активация в зависимости от архетипа
3. **Интеграция** (30 сек) — поэтическая метафора («ключ»), синтезированная LLM

---

## Стек технологий

| Слой | Технология |
|------|-----------|
| Mobile | Flutter 3.x (iOS + Android) |
| Backend | C# .NET 8 Web API |
| База данных | PostgreSQL 16 |
| Кэш | Redis 7 |
| LLM | LM Studio (локально) / OpenRouter API |
| Контейнеризация | Docker Compose |

---

## Структура репозитория

```
HelpYourself/
├── backend/                          # .NET 8 решение
│   ├── HelpYourself.slnx
│   ├── src/
│   │   ├── HelpYourself.Api/         # Web API: контроллеры, сервисы, Program.cs
│   │   ├── HelpYourself.Core/        # Домен: модели, интерфейсы, DTO, перечисления
│   │   └── HelpYourself.Infrastructure/  # EF Core, Redis, LLM-клиент, репозитории
│   └── tests/
│       └── HelpYourself.Tests/       # xUnit тесты
├── mobile/                           # Flutter приложение
│   ├── lib/
│   │   ├── core/
│   │   │   ├── api/                  # HTTP-клиент к бэкенду
│   │   │   ├── config/               # URL бэкенда и константы
│   │   │   ├── enums/                # Archetype, FeedbackRating, InstructionType
│   │   │   └── models/               # Ritual, RitualPhase, RitualInstruction
│   │   ├── features/
│   │   │   ├── home/                 # Главный экран (кнопка + выбор архетипа)
│   │   │   ├── ritual/               # Экран проведения ритуала
│   │   │   └── firstaid/             # «Аптечка» — сохранённые ритуалы
│   │   ├── shared/
│   │   │   ├── theme/                # Тёмная тема, цвета, типографика
│   │   │   └── widgets/              # Переиспользуемые виджеты
│   │   └── main.dart                 # Точка входа, роутинг
│   ├── android/                      # Android-специфичные файлы
│   ├── pubspec.yaml
│   └── SETUP.md                      # Инструкция по настройке платформ
├── docker-compose.yml                # PostgreSQL + Redis
└── README.md
```

---

## Быстрый старт

### Требования

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Flutter 3.x](https://flutter.dev/docs/get-started/install)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- LM Studio (опционально, для локальной LLM)

### 1. Клонировать репозиторий

```bash
git clone https://github.com/elscript/HelpYourself.git
cd HelpYourself
```

### 2. Запустить инфраструктуру

```bash
docker compose up -d
```

Поднимает PostgreSQL на порту `5432` и Redis на `6379`.

### 3. Запустить бэкенд

```bash
cd backend
dotnet restore
dotnet run --project src/HelpYourself.Api
```

API доступен на `http://localhost:5000` / `https://localhost:7000`.  
Swagger UI: `https://localhost:7000/swagger`

### 4. Собрать мобильное приложение

```bash
cd mobile
flutter pub get
flutter run                  # запуск на подключённом устройстве/эмуляторе
flutter build apk --release  # сборка APK
```

Готовый APK: `mobile/build/app/outputs/flutter-apk/app-release.apk`

---

## Бэкенд

### Архитектура

Проект разбит на три слоя по принципу Clean Architecture:

#### `HelpYourself.Core`
Чистый доменный слой без зависимостей на инфраструктуру.

| Файл | Описание |
|------|----------|
| `Models/Ritual.cs` | Агрегат ритуала: ID, архетип, фазы, метафора |
| `Models/RitualPhase.cs` | Фаза ритуала: название, длительность, инструкции |
| `Models/RitualInstruction.cs` | Инструкция: тип (text/breath/movement/voice/sensation), текст, паттерн |
| `Models/RitualFeedback.cs` | Обратная связь от пользователя |
| `Enums/Archetype.cs` | `Fire`, `Water`, `Spiral` |
| `Enums/FeedbackRating.cs` | `Helped`, `Neutral`, `NotHelped` |
| `Enums/InstructionType.cs` | `Text`, `Breath`, `Movement`, `Voice`, `Sensation` |
| `DTOs/GenerateRitualRequest.cs` | Запрос на генерацию: архетип + опциональный контекст |
| `DTOs/GenerateRitualResponse.cs` | Ответ: ID, заголовок, метафора, фазы |
| `Interfaces/IRitualService.cs` | Контракт сервиса |
| `Interfaces/ILlmClient.cs` | Контракт LLM-клиента |
| `Interfaces/IRitualRepository.cs` | Контракт репозитория |
| `Interfaces/ICacheService.cs` | Контракт кэша |

#### `HelpYourself.Infrastructure`
Реализации инфраструктурных интерфейсов.

| Файл | Описание |
|------|----------|
| `LLM/LlmClient.cs` | HTTP-клиент к LM Studio / OpenRouter. JSON Guard: при невалидном JSON автоматически отправляет запрос на исправление |
| `LLM/LlmPromptBuilder.cs` | Системный и пользовательский промпты для генерации ритуала |
| `LLM/LlmRitualDto.cs` | Внутренние DTO для десериализации ответа LLM |
| `Cache/RedisCacheService.cs` | Кэширование ритуалов в Redis (TTL 24 часа) |
| `Data/AppDbContext.cs` | EF Core контекст, хранит фазы как JSONB в PostgreSQL |
| `Repositories/RitualRepository.cs` | CRUD для ритуалов и обратной связи |
| `StaticData/StaticRituals.cs` | 3 готовых ритуала (по одному на архетип) — используются как fallback при недоступности LLM |
| `ServiceRegistration.cs` | Extension-метод `AddInfrastructure()` для регистрации всех сервисов |

#### `HelpYourself.Api`
Web API слой.

| Файл | Описание |
|------|----------|
| `Controllers/RitualsController.cs` | Три эндпоинта: generate, feedback, instant |
| `Services/RitualService.cs` | Оркестрирует: LLM → fallback при ошибке → persist → cache |
| `Program.cs` | Конфигурация приложения, Swagger |

### Конфигурация

`backend/src/HelpYourself.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Port=5432;Database=helpyourself;Username=postgres;Password=postgres",
    "Redis": "localhost:6379"
  },
  "Llm": {
    "BaseUrl": "http://localhost:1234",
    "ChatEndpoint": "/v1/chat/completions",
    "Model": "local-model"
  }
}
```

Для использования OpenRouter замените `BaseUrl` на `https://openrouter.ai/api` и укажите нужную модель.

### Fallback-логика

```
Запрос от клиента
       ↓
  LLM доступна?
  ├── Да → Генерация → Валидация JSON → [retry при ошибке] → Сохранить в БД + Redis
  └── Нет → Вернуть статический ритуал из StaticRituals.cs (мгновенно)
```

---

## Мобильное приложение

### Экраны

#### Главный экран (`HomeScreen`)

- Большая круглая кнопка по центру: **«Мне хреново. Помоги.»**
- После нажатия — плавная анимация появления трёх карточек архетипов
- Haptic feedback на каждое нажатие

#### Экран ритуала (`RitualScreen`)

- При открытии — запрос к бэкенду (`POST /api/rituals/generate`)
- Во время загрузки: анимированный экран «Сейчас подберём ключ к твоему состоянию...»
- Во время ритуала:
  - Крупный текст инструкции на весь экран
  - Линейный прогресс-бар фазы с таймером
  - Индикатор текущей фазы (3 точки)
  - `AnimatedSwitcher` для плавной смены инструкций
- По завершении — Bottom Sheet с метафорой-«ключом» и тремя смайлами обратной связи (😤 / 😐 / 😌)

### Структура кода

```
lib/
├── core/
│   ├── api/
│   │   └── ritual_api_client.dart    # HTTP-клиент: generate, feedback, instant
│   ├── config/
│   │   └── api_config.dart           # baseUrl бэкенда
│   ├── enums/
│   │   ├── archetype.dart            # fire / water / spiral + label/subtitle/emoji
│   │   ├── feedback_rating.dart      # helped / neutral / notHelped
│   │   └── instruction_type.dart     # text / breath / movement / voice / sensation
│   └── models/
│       ├── ritual.dart               # Ritual.fromJson(), totalDurationSec
│       ├── ritual_phase.dart         # RitualPhase.fromJson()
│       └── ritual_instruction.dart   # RitualInstruction.fromJson()
├── features/
│   ├── home/
│   │   └── home_screen.dart          # Кнопка-якорь + выбор архетипа
│   ├── ritual/
│   │   └── ritual_screen.dart        # Таймер фаз, инструкции, feedback sheet
│   └── firstaid/                     # (v2) Аптечка сохранённых ритуалов
├── shared/
│   └── theme/
│       └── app_theme.dart            # Тёмная тема, AppColors
└── main.dart                         # MaterialApp.router + GoRouter
```

### Тема и дизайн

Тёмная «бархатная» тема — глубокий чёрный и тёмно-фиолетовый, создающие ощущение безопасности.

```dart
// Основные цвета (lib/shared/theme/app_theme.dart)
background       = #0A0A0F  // глубокий чёрный
surface          = #12121A  // поверхность карточек
primary          = #7B5EA7  // фиолетовый акцент
fire             = #E05C3A  // оранжево-красный
water            = #3A8FE0  // синий
spiral           = #7A5AE0  // фиолетовый
```

### Роутинг

```dart
/           → HomeScreen
/ritual     → RitualScreen(archetype: Archetype)
```

### Сборка APK

```bash
cd mobile
flutter pub get
flutter build apk --release
# → build/app/outputs/flutter-apk/app-release.apk
```

Установка на устройство:
```bash
adb install build/app/outputs/flutter-apk/app-release.apk
```

### Настройка для Android (HTTP трафик)

В `android/app/src/main/AndroidManifest.xml` уже добавлен флаг:
```xml
android:usesCleartextTraffic="true"
```
Это необходимо для HTTP-запросов к бэкенду. В продакшене следует настроить HTTPS.

### Настройка для iOS

В `ios/Runner/Info.plist` добавить:
```xml
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSAllowsArbitraryLoads</key>
    <true/>
</dict>
```

---

## API Reference

Базовый URL: `http://95.84.137.57:5117`

### `POST /api/rituals/generate`

Генерирует новый ритуал через LLM. При недоступности LLM возвращает статический ритуал.

**Request:**
```json
{
  "archetype": "Fire",
  "context": "после ссоры с начальником"
}
```

| Поле | Тип | Обязательное | Описание |
|------|-----|-------------|----------|
| `archetype` | `string` | ✅ | `Fire`, `Water` или `Spiral` |
| `context` | `string` | ❌ | Свободный текст до 500 символов |

**Response `200`:**
```json
{
  "ritualId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "Укроти огонь",
  "keyMetaphor": "Ты — вулкан, но ты можешь направить свою лаву.",
  "isStatic": false,
  "phases": [
    {
      "name": "Заземление",
      "durationSec": 60,
      "instructions": [
        {
          "type": "Text",
          "value": "Поставь обе ноги на пол. Почувствуй их вес.",
          "pattern": null,
          "description": null
        },
        {
          "type": "Breath",
          "value": "",
          "pattern": [4, 0, 6],
          "description": "Вдох носом на 4 счёта, долгий выдох ртом со звуком «фуух»."
        }
      ]
    },
    {
      "name": "Кульминация",
      "durationSec": 90,
      "instructions": [...]
    },
    {
      "name": "Интеграция",
      "durationSec": 30,
      "instructions": [...]
    }
  ]
}
```

---

### `POST /api/rituals/{ritualId}/feedback`

Сохраняет оценку пользователя после завершения ритуала.

**Request:**
```json
{
  "rating": "Helped"
}
```

| Значение | Смысл |
|----------|-------|
| `Helped` | 😌 Полегчало |
| `Neutral` | 😐 Немного легче |
| `NotHelped` | 😤 Всё ещё плохо |

**Response:** `204 No Content`

---

### `POST /api/rituals/instant/{archetype}`

Возвращает кэшированный ритуал из «Аптечки» — мгновенно, без обращения к LLM.

**Path param:** `archetype` — `Fire`, `Water` или `Spiral`

**Response `200`:** аналогичен `/generate`

---

## LLM и промпт-инжиниринг

### Системный промпт

```
Ты — эксперт по соматической психологии и экстренной самопомощи.
Твоя задача — создать короткий, действенный 3-минутный ритуал
для пользователя, который находится в остром эмоциональном состоянии.

Правила ритуала:
1. Структура: Всегда 3 фазы: Заземление (60 сек), Кульминация (90 сек), Интеграция (30 сек).
2. Заземление: Переключи внимание с мыслей на тело и органы чувств.
3. Кульминация:
   - fire → напряжение-расслабление мышц, резкие выдохи, дрожь
   - water → мягкая активация: поглаживания, растяжка, звук «Ммм»
   - spiral → когнитивное разделение: мысли как облака или поезда
4. Интеграция: Синтезируй поэтическую метафору («ключ»), которая
   превращает состояние пользователя в образ силы.
5. Тон: Директивный, но заботливый. Без «попробуй» — только «сделай».
6. Формат: Строго JSON по схеме. Никакого текста вне JSON.
```

### JSON Guard

Если LLM вернула невалидный JSON, бэкенд автоматически отправляет повторный запрос:

```
"Исправь JSON в своём предыдущем ответе.
 Верни только валидный JSON, ничего больше."
```

Максимум 2 попытки. При неудаче — fallback на статический ритуал.

### Рекомендуемые модели

| Модель | Качество | Скорость |
|--------|----------|----------|
| Llama 3.1 8B (локально) | ⭐⭐⭐ | ⚡⚡⚡ |
| Mistral 7B (локально) | ⭐⭐⭐ | ⚡⚡⚡ |
| Llama 3.3 70B (OpenRouter) | ⭐⭐⭐⭐⭐ | ⚡⚡ |

---

## Дорожная карта

### MVP (текущий статус)
- [x] Flutter: главный экран, выбор архетипа, экран ритуала, обратная связь
- [x] Backend: генерация ритуала, API, fallback на статику
- [x] LLM: системный промпт, JSON Guard, retry
- [x] Инфраструктура: PostgreSQL, Redis, Docker Compose
- [x] Android APK

### v2
- [ ] **Аптечка** — сохранение сработавших ритуалов локально (Isar DB)
- [ ] **Персонализация** — история предыдущих ключей передаётся в контекст LLM
- [ ] **TTS** — голосовое сопровождение (закрыть глаза и слушать)
- [ ] **Анимации** — Lottie/Rive фоны: огонь → вода → спираль
- [ ] **Apple Watch / Wear OS** — кнопка «Помоги» на часах + замер пульса

### v3
- [ ] **Режим «1 минута»** — только дыхание и микро-движения для публичных мест
- [ ] **RLHF** — дообучение модели на основе фидбека пользователей
- [ ] **iOS** — публикация в App Store
