import '../enums/archetype.dart';
import 'ritual_phase.dart';

class Ritual {
  final String ritualId;
  final Archetype archetype;
  final String title;
  final String keyMetaphor;
  final List<RitualPhase> phases;
  final bool isStatic;

  const Ritual({
    required this.ritualId,
    required this.archetype,
    required this.title,
    required this.keyMetaphor,
    required this.phases,
    this.isStatic = false,
  });

  factory Ritual.fromJson(Map<String, dynamic> json, Archetype archetype) =>
      Ritual(
        ritualId: json['ritualId'] as String,
        archetype: archetype,
        title: json['title'] as String,
        keyMetaphor: json['keyMetaphor'] as String,
        phases: (json['phases'] as List<dynamic>)
            .map((e) => RitualPhase.fromJson(e as Map<String, dynamic>))
            .toList(),
        isStatic: json['isStatic'] as bool? ?? false,
      );

  int get totalDurationSec =>
      phases.fold(0, (sum, p) => sum + p.durationSec);
}
