(function (global) {
    function match(input, candidates, maxResults = 5) {
        input = input.toLowerCase().trim();
        if (!input) return [];

        return candidates
            .map(candidate => {
                let lowerCandidate = candidate.toLowerCase();
                let { score, highlighted } = calculateMatchScore(input, candidate);

                return score > 0
                    ? { word: candidate, matchScore: score, highlighted }
                    : null;
            })
            .filter(Boolean) // Remove null values
            .sort((a, b) => b.matchScore - a.matchScore) // Prioritize best matches
            .slice(0, maxResults); // Limit results
    }

    function calculateMatchScore(input, candidate) {
        let matchCount = 0, penalty = 0, lastMatchIndex = -1;
        let highlighted = [];

        let lowerInput = input.toLowerCase();
        let lowerCandidate = candidate.toLowerCase();

        let inputIndex = 0;

        for (let i = 0; i < candidate.length; i++) {
            let char = candidate[i];

            if (inputIndex < lowerInput.length && char.toLowerCase() === lowerInput[inputIndex]) {
                matchCount++;

                // Apply penalty if the match is far apart
                if (lastMatchIndex !== -1 && i - lastMatchIndex > 1) {
                    penalty += (i - lastMatchIndex - 1);
                }

                lastMatchIndex = i;
                inputIndex++;
                highlighted.push({ char, matched: true });
            } else {
                highlighted.push({ char, matched: false });
            }
        }

        let finalScore = (matchCount * 3) - penalty; // Weight matches higher, penalize gaps

        return { score: finalScore, highlighted };
    }

    // Attach to global scope
    global.FuzzyMatcher = { match };
})(window);
