enum Archetype {
  fire,   // Накрывает / Взрываюсь
  water,  // Тону / Нет сил
  spiral; // Мысли по кругу

  String get label => switch (this) {
    Archetype.fire => 'Накрывает / Взрываюсь',
    Archetype.water => 'Тону / Нет сил',
    Archetype.spiral => 'Мысли по кругу',
  };

  String get subtitle => switch (this) {
    Archetype.fire => 'Сердце колотится, хочется кричать или бежать',
    Archetype.water => 'Тело ватное, мир серый, не хочу вставать',
    Archetype.spiral => 'В голове шарманка, не могу остановить внутренний диалог',
  };

  String get emoji => switch (this) {
    Archetype.fire => '🔥',
    Archetype.water => '💧',
    Archetype.spiral => '🌀',
  };
}
