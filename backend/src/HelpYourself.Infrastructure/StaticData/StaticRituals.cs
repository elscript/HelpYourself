using HelpYourself.Core.Enums;
using HelpYourself.Core.Models;

namespace HelpYourself.Infrastructure.StaticData;

/// <summary>
/// Fallback rituals for when LLM is unavailable or slow.
/// Curated content — not generated.
/// </summary>
public static class StaticRituals
{
    private static readonly IReadOnlyDictionary<Archetype, Ritual> Rituals = new Dictionary<Archetype, Ritual>
    {
        [Archetype.Fire] = new()
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Archetype = Archetype.Fire,
            Title = "Укроти огонь",
            KeyMetaphor = "Ты — вулкан. Ты можешь направить свою лаву, а не быть ею захлестнутым.",
            IsStatic = true,
            Phases =
            [
                new()
                {
                    Name = "Заземление",
                    DurationSec = 60,
                    Instructions =
                    [
                        new() { Type = InstructionType.Text, Value = "Поставь обе ноги на пол. Почувствуй их вес." },
                        new() { Type = InstructionType.Sensation, Value = "Найди глазами 5 красных предметов вокруг. Молча назови каждый." },
                        new() { Type = InstructionType.Breath, Pattern = [4, 0, 6], Description = "Вдох носом на 4 счета, задержи, медленный выдох ртом со звуком «фуух»." }
                    ]
                },
                new()
                {
                    Name = "Кульминация",
                    DurationSec = 90,
                    Instructions =
                    [
                        new() { Type = InstructionType.Text, Value = "Сожми кулаки изо всех сил. Держи 5 секунд." },
                        new() { Type = InstructionType.Movement, Value = "Резко брось руки вниз и потряси кистями, как будто сбрасываешь воду." },
                        new() { Type = InstructionType.Breath, Pattern = [2, 0, 2], Description = "Три резких выдоха ртом подряд — как задуваешь свечи." },
                        new() { Type = InstructionType.Movement, Value = "Встряхни всё тело от головы до ног 30 секунд. Позволь себе дрожать." }
                    ]
                },
                new()
                {
                    Name = "Интеграция",
                    DurationSec = 30,
                    Instructions =
                    [
                        new() { Type = InstructionType.Text, Value = "Положи руку на грудь. Почувствуй тепло." },
                        new() { Type = InstructionType.Voice, Value = "Прошепчи: «Я больше, чем это чувство. Я прохожу сквозь него.»" }
                    ]
                }
            ]
        },
        [Archetype.Water] = new()
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Archetype = Archetype.Water,
            Title = "Вернись на поверхность",
            KeyMetaphor = "Ты — море. Сейчас отлив, но прилив всегда возвращается.",
            IsStatic = true,
            Phases =
            [
                new()
                {
                    Name = "Заземление",
                    DurationSec = 60,
                    Instructions =
                    [
                        new() { Type = InstructionType.Text, Value = "Сядь прямо. Почувствуй спинку стула за собой." },
                        new() { Type = InstructionType.Sensation, Value = "Найди 4 предмета, которые ты можешь коснуться прямо сейчас. Коснись каждого." },
                        new() { Type = InstructionType.Breath, Pattern = [4, 0, 4], Description = "Дыши ровно: вдох на 4, выдох на 4. Просто замечай дыхание." }
                    ]
                },
                new()
                {
                    Name = "Кульминация",
                    DurationSec = 90,
                    Instructions =
                    [
                        new() { Type = InstructionType.Movement, Value = "Медленно погладь себя от груди до живота ладонью 3 раза. Без спешки." },
                        new() { Type = InstructionType.Voice, Value = "Потяни звук «Ммм» на выдохе. Три раза. Почувствуй вибрацию в груди." },
                        new() { Type = InstructionType.Movement, Value = "Очень медленно подними плечи к ушам, задержи, опусти. Повтори 5 раз." }
                    ]
                },
                new()
                {
                    Name = "Интеграция",
                    DurationSec = 30,
                    Instructions =
                    [
                        new() { Type = InstructionType.Text, Value = "Закрой глаза на 10 секунд. Просто существуй здесь." },
                        new() { Type = InstructionType.Voice, Value = "Тихо скажи себе: «Это пройдет. Я здесь.»" }
                    ]
                }
            ]
        },
        [Archetype.Spiral] = new()
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Archetype = Archetype.Spiral,
            Title = "Выйди из петли",
            KeyMetaphor = "Твои мысли — это облака. Ты — небо. Облака проходят, небо остаётся.",
            IsStatic = true,
            Phases =
            [
                new()
                {
                    Name = "Заземление",
                    DurationSec = 60,
                    Instructions =
                    [
                        new() { Type = InstructionType.Text, Value = "Назови вслух 5 вещей, которые видишь прямо сейчас." },
                        new() { Type = InstructionType.Sensation, Value = "Почувствуй 3 текстуры: то, что под твоими руками, под ногами, на коже." },
                        new() { Type = InstructionType.Breath, Pattern = [4, 4, 6], Description = "Вдох на 4, задержи на 4, выдох на 6. Это активирует тормозную систему мозга." }
                    ]
                },
                new()
                {
                    Name = "Кульминация",
                    DurationSec = 90,
                    Instructions =
                    [
                        new() { Type = InstructionType.Text, Value = "Возьми мысль, которая крутится. Представь её как поезд." },
                        new() { Type = InstructionType.Text, Value = "Ты стоишь на платформе. Поезд проезжает мимо. Ты не садишься." },
                        new() { Type = InstructionType.Voice, Value = "Скажи вслух: «Я замечаю, что у меня есть мысль о...» — и назови её. Просто замечай." },
                        new() { Type = InstructionType.Movement, Value = "Поставь руку горизонтально и медленно проведи ею перед собой слева направо, как будто провожаешь поезд." }
                    ]
                },
                new()
                {
                    Name = "Интеграция",
                    DurationSec = 30,
                    Instructions =
                    [
                        new() { Type = InstructionType.Text, Value = "Глубокий вдох. На выдохе — отпусти мысль вместе с воздухом." },
                        new() { Type = InstructionType.Voice, Value = "Прошепчи: «Я — не мои мысли. Я здесь. Я в порядке.»" }
                    ]
                }
            ]
        }
    };

    public static Ritual GetForArchetype(Archetype archetype) => Rituals[archetype];
}
