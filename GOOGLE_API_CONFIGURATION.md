# Google Generative AI Configuration

## Security Notice

The Google Generative AI API key has been moved from hardcoded client-side JavaScript to secure server-side configuration to prevent unauthorized access and potential security breaches.

**IMPORTANT**: The API key `AIzaSyCdBbtTfxOgYKBq7frKFmwOlOKSLDjxY94` that was previously exposed should be **revoked immediately** and replaced with a new one.

## Configuration

### Development Environment

1. The API key should be configured in `appsettings.Development.json` (which is gitignored):
```json
{
  "GoogleGenerativeAI": {
    "ApiKey": "your-google-generative-ai-api-key-here"
  }
}
```

### Production Environment

For production deployments, configure the Google Generative AI API key using one of these methods:

1. **Environment Variable:**
   ```bash
   export GoogleGenerativeAI__ApiKey="your-production-api-key"
   ```

2. **Azure App Settings** (for Azure deployments):
   Add a new application setting:
   - Key: `GoogleGenerativeAI:ApiKey`
   - Value: `your-production-api-key`

3. **User Secrets** (for development):
   ```bash
   dotnet user-secrets set "GoogleGenerativeAI:ApiKey" "your-api-key"
   ```

## How It Works

1. The client-side JavaScript files import a secure configuration module: `google-ai-config.js`
2. This module fetches the API key from the server endpoint: `/api/config/google-api-key`
3. The server-side `ConfigController` serves the API key from the secure configuration
4. The API key is cached in memory on the client side to avoid repeated requests

## Security Benefits

- ✅ API key is no longer exposed in client-side JavaScript
- ✅ API key is served only from secure server-side configuration
- ✅ Development settings are properly gitignored
- ✅ Supports secure deployment patterns (environment variables, user secrets)

## Getting Your Google Generative AI API Key

1. Go to [Google AI Studio](https://makersuite.google.com/app/apikey)
2. Create a new API key
3. Configure it in your application settings as described above
4. **Never commit API keys to source control**

## Immediate Action Required

⚠️ **The exposed API key must be revoked and replaced:**
1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Navigate to APIs & Services > Credentials
3. Find and delete the compromised API key: `AIzaSyCdBbtTfxOgYKBq7frKFmwOlOKSLDjxY94`
4. Create a new API key
5. Update your application configuration with the new key