using HelpYourself.Core.Enums;

namespace HelpYourself.Infrastructure.LLM;

internal static class LlmPromptBuilder
{
    private const string SystemPrompt = """
        Ты — эксперт по соматической психологии и экстренной самопомощи. Твоя задача — создать короткий, действенный 3-минутный ритуал для пользователя, который находится в остром эмоциональном состоянии.

        Правила ритуала:
        1. Структура: Всегда 3 фазы: Заземление (60 сек), Кульминация (90 сек), Интеграция (30 сек).
        2. Заземление: Переключи внимание с мыслей на тело и органы чувств. Используй технику 5-4-3-2-1 или простые физические якоря.
        3. Кульминация: Для "fire" — напряжение-расслабление мышц, резкие выдохи, дрожь. Для "water" — мягкая активация через поглаживания, легкую растяжку, звук "Ммм". Для "spiral" — когнитивное разделение: представить мысли как облака или поезда.
        4. Интеграция: Синтезируй простую, поэтичную метафору ("ключ"), которая превращает состояние пользователя в образ силы.
        5. Тон: Директивный, но заботливый. Без "попробуй" — только "сделай". Без сослагательного наклонения. Без оценки чувств.
        6. Формат ответа: Строго JSON согласно схеме. Никакого текста вне JSON.

        Схема ответа:
        {
          "ritualId": "guid-string",
          "title": "string",
          "keyMetaphor": "string",
          "phases": [
            {
              "name": "string",
              "durationSec": number,
              "instructions": [
                { "type": "text|breath|movement|voice|sensation", "value": "string", "pattern": [number] | null, "description": "string | null" }
              ]
            }
          ]
        }
        """;

    public static string BuildUserPrompt(Archetype archetype, string? context)
    {
        var archetypeLabel = archetype switch
        {
            Archetype.Fire => "fire — Накрывает / Взрываюсь (Гнев, Паника, Перегрузка)",
            Archetype.Water => "water — Тону / Нет сил (Апатия, Грусть, Пустота)",
            Archetype.Spiral => "spiral — Мысли по кругу (Руминации, Тревога, Самокопание)",
            _ => throw new ArgumentOutOfRangeException(nameof(archetype))
        };

        var contextPart = string.IsNullOrWhiteSpace(context)
            ? "Контекст не указан."
            : $"Контекст: {context}";

        return $"Состояние пользователя: {archetypeLabel}. {contextPart} Сгенерируй ритуал.";
    }

    public static string GetSystemPrompt() => SystemPrompt;
}
