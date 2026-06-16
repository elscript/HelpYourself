import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:go_router/go_router.dart';
import '../../core/enums/archetype.dart';
import '../../shared/theme/app_theme.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen>
    with SingleTickerProviderStateMixin {
  bool _showArchetypes = false;
  late AnimationController _controller;
  late Animation<double> _fadeIn;

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 600),
    );
    _fadeIn = CurvedAnimation(parent: _controller, curve: Curves.easeOut);
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  void _onMainButtonTap() {
    HapticFeedback.heavyImpact();
    setState(() => _showArchetypes = true);
    _controller.forward();
  }

  void _onArchetypeTap(Archetype archetype) {
    HapticFeedback.mediumImpact();
    context.push('/ritual', extra: archetype);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SafeArea(
        child: AnimatedSwitcher(
          duration: const Duration(milliseconds: 400),
          child: _showArchetypes
              ? _ArchetypeSelector(
                  key: const ValueKey('archetypes'),
                  animation: _fadeIn,
                  onTap: _onArchetypeTap,
                )
              : _MainButton(
                  key: const ValueKey('main'),
                  onTap: _onMainButtonTap,
                ),
        ),
      ),
    );
  }
}

class _MainButton extends StatelessWidget {
  final VoidCallback onTap;
  const _MainButton({super.key, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          GestureDetector(
            onTap: onTap,
            child: Container(
              width: 260,
              height: 260,
              decoration: BoxDecoration(
                shape: BoxShape.circle,
                color: AppColors.surfaceElevated,
                border: Border.all(
                  color: AppColors.primary.withValues(alpha: 0.6),
                  width: 2,
                ),
                boxShadow: [
                  BoxShadow(
                    color: AppColors.primary.withValues(alpha: 0.25),
                    blurRadius: 60,
                    spreadRadius: 10,
                  ),
                ],
              ),
              child: const Center(
                child: Text(
                  'Мне хреново.\nПомоги.',
                  textAlign: TextAlign.center,
                  style: TextStyle(
                    fontSize: 24,
                    fontWeight: FontWeight.w700,
                    color: AppColors.onBackground,
                    height: 1.3,
                  ),
                ),
              ),
            ),
          ),
          const SizedBox(height: 24),
          const Text(
            'Это займёт 3 минуты. Никакой регистрации.',
            style: TextStyle(
              fontSize: 13,
              color: AppColors.onSurfaceMuted,
            ),
          ),
        ],
      ),
    );
  }
}

class _ArchetypeSelector extends StatelessWidget {
  final Animation<double> animation;
  final void Function(Archetype) onTap;
  const _ArchetypeSelector({super.key, required this.animation, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return FadeTransition(
      opacity: animation,
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 40),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Что происходит?',
              style: TextStyle(
                fontSize: 28,
                fontWeight: FontWeight.w700,
                color: AppColors.onBackground,
              ),
            ),
            const SizedBox(height: 32),
            ...Archetype.values.map(
              (a) => Padding(
                padding: const EdgeInsets.only(bottom: 16),
                child: _ArchetypeCard(archetype: a, onTap: () => onTap(a)),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _ArchetypeCard extends StatelessWidget {
  final Archetype archetype;
  final VoidCallback onTap;
  const _ArchetypeCard({required this.archetype, required this.onTap});

  Color get _accentColor => switch (archetype) {
    Archetype.fire => AppColors.fire,
    Archetype.water => AppColors.water,
    Archetype.spiral => AppColors.spiral,
  };

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.all(20),
        decoration: BoxDecoration(
          color: AppColors.surfaceElevated,
          borderRadius: BorderRadius.circular(16),
          border: Border.all(
            color: _accentColor.withValues(alpha: 0.35),
            width: 1.5,
          ),
        ),
        child: Row(
          children: [
            Text(archetype.emoji, style: const TextStyle(fontSize: 32)),
            const SizedBox(width: 16),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    archetype.label,
                    style: const TextStyle(
                      fontSize: 17,
                      fontWeight: FontWeight.w600,
                      color: AppColors.onBackground,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    archetype.subtitle,
                    style: const TextStyle(
                      fontSize: 13,
                      color: AppColors.onSurfaceMuted,
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
