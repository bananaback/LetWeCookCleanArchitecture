using System.Globalization;
using System.Text;

namespace LetWeCook.Application.Utilities;

public static class FuzzySearchUtil
{
    public static List<string> FindTopMatches(
    string query,
    List<string> candidates,
    int topN = 5,
    int tolerance = 2,
    int minMatchScore = 1)
    {
        var scoredMatches = new List<(string candidate, int score)>();
        var searchWords = Normalize(query).Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var candidate in candidates)
        {
            var candidateWords = Normalize(candidate).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int matchScore = 0;

            foreach (var searchWord in searchWords)
            {
                foreach (var candidateWord in candidateWords)
                {
                    int distance = Levenshtein(searchWord, candidateWord);
                    if (distance <= tolerance)
                    {
                        matchScore += tolerance - distance + 1; // Higher score for closer match
                        break; // Only match once per search word
                    }
                }
            }

            if (matchScore >= minMatchScore) // Filter out weak matches
                scoredMatches.Add((candidate, matchScore));
        }

        return scoredMatches
            .OrderByDescending(m => m.score)
            .Take(topN)
            .Select(m => m.candidate)
            .ToList();
    }


    private static string Normalize(string input)
    {
        input = input.ToLowerInvariant().Trim();
        var normalized = input.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    private static int Levenshtein(string s, string t)
    {
        if (s == t) return 0;
        if (s.Length == 0) return t.Length;
        if (t.Length == 0) return s.Length;

        var d = new int[s.Length + 1, t.Length + 1];

        for (int i = 0; i <= s.Length; i++) d[i, 0] = i;
        for (int j = 0; j <= t.Length; j++) d[0, j] = j;

        for (int i = 1; i <= s.Length; i++)
        {
            for (int j = 1; j <= t.Length; j++)
            {
                int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }

        return d[s.Length, t.Length];
    }
}
