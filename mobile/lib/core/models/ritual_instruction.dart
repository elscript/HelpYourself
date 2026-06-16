import '../enums/instruction_type.dart';

class RitualInstruction {
  final InstructionType type;
  final String value;
  final List<int>? pattern;
  final String? description;

  const RitualInstruction({
    required this.type,
    required this.value,
    this.pattern,
    this.description,
  });

  factory RitualInstruction.fromJson(Map<String, dynamic> json) =>
      RitualInstruction(
        type: InstructionType.values.byName(
          (json['type'] as String).toLowerCase(),
        ),
        value: json['value'] as String,
        pattern: (json['pattern'] as List<dynamic>?)
            ?.map((e) => e as int)
            .toList(),
        description: json['description'] as String?,
      );
}
