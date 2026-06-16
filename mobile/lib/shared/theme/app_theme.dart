import 'package:flutter/material.dart';

abstract final class AppColors {
  static const background = Color(0xFF0A0A0F);
  static const surface = Color(0xFF12121A);
  static const surfaceElevated = Color(0xFF1A1A26);
  static const primary = Color(0xFF7B5EA7);       // темно-фиолетовый
  static const primaryLight = Color(0xFF9B7EC8);
  static const onPrimary = Color(0xFFFFFFFF);
  static const onBackground = Color(0xFFE8E0F0);
  static const onSurfaceMuted = Color(0xFF9090A8);
  static const fire = Color(0xFFE05C3A);
  static const water = Color(0xFF3A8FE0);
  static const spiral = Color(0xFF7A5AE0);
}

final appTheme = ThemeData(
  useMaterial3: true,
  brightness: Brightness.dark,
  scaffoldBackgroundColor: AppColors.background,
  colorScheme: const ColorScheme.dark(
    surface: AppColors.surface,
    primary: AppColors.primary,
    onPrimary: AppColors.onPrimary,
    onSurface: AppColors.onBackground,
  ),
  textTheme: const TextTheme(
    displayLarge: TextStyle(
      fontSize: 40,
      fontWeight: FontWeight.w700,
      color: AppColors.onBackground,
      height: 1.1,
    ),
    headlineMedium: TextStyle(
      fontSize: 26,
      fontWeight: FontWeight.w600,
      color: AppColors.onBackground,
    ),
    bodyLarge: TextStyle(
      fontSize: 18,
      color: AppColors.onBackground,
      height: 1.5,
    ),
    bodyMedium: TextStyle(
      fontSize: 14,
      color: AppColors.onSurfaceMuted,
    ),
  ),
);
