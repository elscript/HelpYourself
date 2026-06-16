import 'dart:convert';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../enums/archetype.dart';
import '../enums/feedback_rating.dart';
import '../models/ritual.dart';

class RitualApiClient {
  final http.Client _client;

  RitualApiClient({http.Client? client}) : _client = client ?? http.Client();

  Future<Ritual> generateRitual(Archetype archetype, {String? context}) async {
    final body = <String, dynamic>{
      'archetype': _archetypeName(archetype),
      if (context != null && context.isNotEmpty) 'context': context,
    };

    final response = await _client
        .post(
          Uri.parse('${ApiConfig.baseUrl}${ApiConfig.ritualsPath}/generate'),
          headers: {'Content-Type': 'application/json'},
          body: jsonEncode(body),
        )
        .timeout(const Duration(minutes: 2));

    if (response.statusCode != 200) {
      throw ApiException(response.statusCode, response.body);
    }

    final json = jsonDecode(response.body) as Map<String, dynamic>;
    return Ritual.fromJson(json, archetype);
  }

  Future<void> sendFeedback(String ritualId, FeedbackRating rating) async {
    await _client
        .post(
          Uri.parse(
              '${ApiConfig.baseUrl}${ApiConfig.ritualsPath}/$ritualId/feedback'),
          headers: {'Content-Type': 'application/json'},
          body: jsonEncode({'rating': _ratingName(rating)}),
        )
        .timeout(const Duration(seconds: 10));
  }

  Future<Ritual> getInstantRitual(Archetype archetype) async {
    final response = await _client
        .post(
          Uri.parse(
              '${ApiConfig.baseUrl}${ApiConfig.ritualsPath}/instant/${_archetypeName(archetype)}'),
          headers: {'Content-Type': 'application/json'},
        )
        .timeout(const Duration(seconds: 10));

    if (response.statusCode != 200) {
      throw ApiException(response.statusCode, response.body);
    }

    final json = jsonDecode(response.body) as Map<String, dynamic>;
    return Ritual.fromJson(json, archetype);
  }

  static String _archetypeName(Archetype a) => a.name[0].toUpperCase() + a.name.substring(1);

  static String _ratingName(FeedbackRating r) => switch (r) {
        FeedbackRating.helped => 'Helped',
        FeedbackRating.neutral => 'Neutral',
        FeedbackRating.notHelped => 'NotHelped',
      };
}

class ApiException implements Exception {
  final int statusCode;
  final String body;
  const ApiException(this.statusCode, this.body);

  @override
  String toString() => 'ApiException($statusCode): $body';
}
