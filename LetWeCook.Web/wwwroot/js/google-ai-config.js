// Secure API key management module
let cachedApiKey = null;

async function getGoogleApiKey() {
    if (cachedApiKey) {
        return cachedApiKey;
    }

    try {
        const response = await fetch('/api/config/google-api-key');
        if (!response.ok) {
            throw new Error(`Failed to fetch API key: ${response.status} ${response.statusText}`);
        }
        
        const data = await response.json();
        cachedApiKey = data.apiKey;
        return cachedApiKey;
    } catch (error) {
        console.error('Error fetching Google API key:', error);
        throw error;
    }
}

// Initialize GoogleGenerativeAI with secure API key
async function initializeGoogleAI() {
    const { GoogleGenerativeAI } = await import("https://esm.sh/@google/generative-ai");
    const apiKey = await getGoogleApiKey();
    return new GoogleGenerativeAI(apiKey);
}

export { initializeGoogleAI };