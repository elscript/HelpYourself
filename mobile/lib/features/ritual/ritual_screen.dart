import 'dart:async';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:go_router/go_router.dart';
import '../../core/api/ritual_api_client.dart';
import '../../core/enums/archetype.dart';
import '../../core/enums/feedback_rating.dart';
import '../../core/models/ritual.dart';
import '../../core/models/ritual_instruction.dart';
import '../../shared/theme/app_theme.dart';

class RitualScreen extends StatefulWidget {
  final Archetype archetype;
  const RitualScreen({super.key, required this.archetype});

  @override
  State<RitualScreen> createState() => _RitualScreenState();
}

class _RitualScreenState extends State<RitualScreen> {
  Ritual? _ritual;
  bool _loading = true;
  final _api = RitualApiClient();

  int _currentPhaseIndex = 0;
  int _currentInstructionIndex = 0;
  int _remainingSec = 0;
  Timer? _timer;

  @override
  void initState() {
    super.initState();
    _loadRitual();
  }

  @override
  void dispose() {
    _timer?.cancel();
    super.dispose();
  }

  Future<void> _loadRitual() async {
    try {
      final ritual = await _api.generateRitual(widget.archetype);
      setState(() {
        _ritual = ritual;
        _loading = false;
      });
      _startPhase();
    } catch (e) {
      if (mounted) setState(() => _loading = false);
    }
  }

  void _startPhase() {
    if (_ritual == null) return;
    final phase = _ritual!.phases[_currentPhaseIndex];
    _remainingSec = phase.durationSec;
    _currentInstructionIndex = 0;

    _timer?.cancel();
    _timer = Timer.periodic(const Duration(seconds: 1), (_) {
      if (!mounted) return;
      setState(() {
        _remainingSec--;
        final elapsed = phase.durationSec - _remainingSec;
        final instrCount = phase.instructions.length;
        final secPerInstruction = phase.durationSec ~/ instrCount;
        _currentInstructionIndex = (elapsed ~/ secPerInstruction)
            .clamp(0, instrCount - 1);
      });

      if (_remainingSec <= 0) {
        _timer?.cancel();
        HapticFeedback.mediumImpact();
        _nextPhase();
      }
    });
  }

  void _nextPhase() {
    if (_ritual == null) return;
    if (_currentPhaseIndex < _ritual!.phases.length - 1) {
      setState(() => _currentPhaseIndex++);
      _startPhase();
    } else {
      _showCompletion();
    }
  }

  void _showCompletion() {
    HapticFeedback.heavyImpact();
    showModalBottomSheet(
      context: context,
      backgroundColor: AppColors.surface,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
      ),
      builder: (_) => _CompletionSheet(
        ritual: _ritual!,
        onFeedback: _onFeedback,
      ),
    );
  }

  void _onFeedback(FeedbackRating rating) {
    if (_ritual != null) {
      _api.sendFeedback(_ritual!.ritualId, rating).ignore();
    }
    context.pop();
    context.pop();
  }

  @override
  Widget build(BuildContext context) {
    if (_loading) return const _LoadingScreen();
    if (_ritual == null) return const _LoadingScreen(); // still loading

    final phase = _ritual!.phases[_currentPhaseIndex];
    final instruction = phase.instructions[_currentInstructionIndex];
    final progress = 1.0 - (_remainingSec / phase.durationSec);

    return Scaffold(
      body: SafeArea(
        child: Column(
          children: [
            _PhaseHeader(
              phaseName: phase.name,
              progress: progress,
              remainingSec: _remainingSec,
            ),
            Expanded(
              child: _InstructionCard(instruction: instruction),
            ),
            _PhaseIndicator(
              total: _ritual!.phases.length,
              current: _currentPhaseIndex,
            ),
            const SizedBox(height: 24),
          ],
        ),
      ),
    );
  }
}

class _LoadingScreen extends StatelessWidget {
  const _LoadingScreen();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const CircularProgressIndicator(color: AppColors.primary),
            const SizedBox(height: 24),
            Text(
              'Сейчас подберём ключ\nк твоему состоянию...',
              textAlign: TextAlign.center,
              style: Theme.of(context).textTheme.bodyLarge,
            ),
          ],
        ),
      ),
    );
  }
}

class _PhaseHeader extends StatelessWidget {
  final String phaseName;
  final double progress;
  final int remainingSec;

  const _PhaseHeader({
    required this.phaseName,
    required this.progress,
    required this.remainingSec,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(24, 24, 24, 0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(phaseName,
                  style: const TextStyle(
                    fontSize: 14,
                    fontWeight: FontWeight.w600,
                    color: AppColors.onSurfaceMuted,
                    letterSpacing: 1.2,
                  )),
              Text(
                '${remainingSec ~/ 60}:${(remainingSec % 60).toString().padLeft(2, '0')}',
                style: const TextStyle(
                  fontSize: 14,
                  color: AppColors.onSurfaceMuted,
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),
          ClipRRect(
            borderRadius: BorderRadius.circular(4),
            child: LinearProgressIndicator(
              value: progress,
              minHeight: 4,
              backgroundColor: AppColors.surfaceElevated,
              valueColor:
                  const AlwaysStoppedAnimation<Color>(AppColors.primary),
            ),
          ),
        ],
      ),
    );
  }
}

class _InstructionCard extends StatelessWidget {
  final RitualInstruction instruction;
  const _InstructionCard({required this.instruction});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(32),
      child: Center(
        child: AnimatedSwitcher(
          duration: const Duration(milliseconds: 500),
          child: Text(
            key: ValueKey(instruction.value),
            instruction.value,
            textAlign: TextAlign.center,
            style: const TextStyle(
              fontSize: 32,
              fontWeight: FontWeight.w700,
              color: AppColors.onBackground,
              height: 1.3,
            ),
          ),
        ),
      ),
    );
  }
}

class _PhaseIndicator extends StatelessWidget {
  final int total;
  final int current;
  const _PhaseIndicator({required this.total, required this.current});

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.center,
      children: List.generate(
        total,
        (i) => AnimatedContainer(
          duration: const Duration(milliseconds: 300),
          margin: const EdgeInsets.symmetric(horizontal: 4),
          width: i == current ? 24 : 8,
          height: 8,
          decoration: BoxDecoration(
            color: i <= current ? AppColors.primary : AppColors.surfaceElevated,
            borderRadius: BorderRadius.circular(4),
          ),
        ),
      ),
    );
  }
}

class _CompletionSheet extends StatelessWidget {
  final Ritual ritual;
  final void Function(FeedbackRating) onFeedback;
  const _CompletionSheet({required this.ritual, required this.onFeedback});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(32),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            ritual.keyMetaphor,
            textAlign: TextAlign.center,
            style: const TextStyle(
              fontSize: 18,
              fontStyle: FontStyle.italic,
              color: AppColors.onBackground,
              height: 1.5,
            ),
          ),
          const SizedBox(height: 32),
          const Text(
            'Как сейчас?',
            style: TextStyle(
              fontSize: 20,
              fontWeight: FontWeight.w600,
              color: AppColors.onBackground,
            ),
          ),
          const SizedBox(height: 20),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceEvenly,
            children: [
              _FeedbackButton(emoji: '😤', label: 'Всё ещё плохо', rating: FeedbackRating.notHelped, onTap: onFeedback),
              _FeedbackButton(emoji: '😐', label: 'Немного легче', rating: FeedbackRating.neutral, onTap: onFeedback),
              _FeedbackButton(emoji: '😌', label: 'Полегчало', rating: FeedbackRating.helped, onTap: onFeedback),
            ],
          ),
          const SizedBox(height: 16),
        ],
      ),
    );
  }
}

class _FeedbackButton extends StatelessWidget {
  final String emoji;
  final String label;
  final FeedbackRating rating;
  final void Function(FeedbackRating) onTap;
  const _FeedbackButton({required this.emoji, required this.label, required this.rating, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: () {
        HapticFeedback.lightImpact();
        onTap(rating);
      },
      child: Column(
        children: [
          Text(emoji, style: const TextStyle(fontSize: 36)),
          const SizedBox(height: 6),
          Text(label,
              style: const TextStyle(fontSize: 11, color: AppColors.onSurfaceMuted),
              textAlign: TextAlign.center),
        ],
      ),
    );
  }
}
