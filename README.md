# HelpYourself — «Самопомощь»

> **Не анализируй. Действуй.**

Мобильное приложение экстренной самопомощи. За 3 минуты проводит пользователя через персонализированный ритуал (дыхание, движение, проговаривание), сгенерированный LLM на основе выбранного эмоционального состояния.

## Стек

| Слой | Технология |
|------|-----------|
| Mobile | Flutter (iOS + Android) |
| Backend | C# .NET 8 Web API |
| БД | PostgreSQL 16 |
| Кэш | Redis 7 |
| LLM | LM Studio (локально) / OpenRouter (старт) |

## Структура репозитория

```
HelpYourself/
├── backend/                   # .NET 8 решение
│   ├── src/
│   │   ├── HelpYourself.Api/          # Web API, контроллеры, сервисы
│   │   ├── HelpYourself.Core/         # Домен: модели, интерфейсы, DTO
│   │   └── HelpYourself.Infrastructure/  # EF Core, Redis, LLM клиент
│   └── tests/
│       └── HelpYourself.Tests/
├── mobile/                    # Flutter приложение
├── docker-compose.yml         # PostgreSQL + Redis
└── README.md
```

## Быстрый старт

### 1. Поднять инфраструктуру

```bash
docker compose up -d
```

### 2. Запустить бэкенд

```bash
cd backend
dotnet run --project src/HelpYourself.Api
```

API будет доступен на `https://localhost:7000`, документация: `https://localhost:7000/scalar`.

### 3. Настроить LLM

В `appsettings.json` укажи адрес своего LM Studio:

```json
"Llm": {
  "BaseUrl": "http://localhost:1234",
  "Model": "название-вашей-модели"
}
```

Если LLM недоступна — автоматически используются встроенные статические ритуалы.

## API

| Метод | Endpoint | Описание |
|-------|---------|---------|
| POST | `/api/rituals/generate` | Сгенерировать ритуал через LLM |
| POST | `/api/rituals/{id}/feedback` | Отправить оценку ритуала |
| POST | `/api/rituals/instant/{archetype}` | Мгновенный ритуал из «Аптечки» |

### Архетипы состояний

| Ключ | Состояние |
|------|----------|
| `Fire` | Накрывает / Взрываюсь (Гнев, Паника) |
| `Water` | Тону / Нет сил (Апатия, Грусть, Пустота) |
| `Spiral` | Мысли по кругу (Руминации, Тревога) |

### Пример запроса

```bash
curl -X POST https://localhost:7000/api/rituals/generate \
  -H "Content-Type: application/json" \
  -d '{"archetype": "Fire", "context": "после ссоры с начальником"}'
```
