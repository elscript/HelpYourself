import 'ritual_instruction.dart';

class RitualPhase {
  final String name;
  final int durationSec;
  final List<RitualInstruction> instructions;

  const RitualPhase({
    required this.name,
    required this.durationSec,
    required this.instructions,
  });

  factory RitualPhase.fromJson(Map<String, dynamic> json) => RitualPhase(
        name: json['name'] as String,
        durationSec: json['durationSec'] as int,
        instructions: (json['instructions'] as List<dynamic>)
            .map((e) => RitualInstruction.fromJson(e as Map<String, dynamic>))
            .toList(),
      );
}
