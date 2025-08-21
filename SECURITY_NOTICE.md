# Security Notice: Sensitive Configuration Data Removed

This file previously contained sensitive credentials including:
- Google Generative AI API key
- SMTP passwords 
- OAuth client secrets
- PayPal API credentials
- Database connection strings with passwords

All sensitive data has been removed and replaced with placeholder values in `appsettings.Development.json`.

## For Developers

To configure the application for development:

1. Copy `appsettings.Development.json` to a local version with your credentials
2. Or use .NET User Secrets to store sensitive configuration:
   ```bash
   dotnet user-secrets set "GoogleGenerativeAI:ApiKey" "your-actual-api-key"
   dotnet user-secrets set "SmtpSettings:Password" "your-actual-password"
   # etc.
   ```

## Security Recommendations

- Never commit sensitive credentials to source control
- Use environment variables or secure vaults in production
- Regularly rotate API keys and secrets
- Monitor for exposed credentials in commit history

The exposed Google API key `AIzaSyCdBbtTfxOgYKBq7frKFmwOlOKSLDjxY94` should be:
1. **Revoked immediately** in the Google Cloud Console
2. **Replaced with a new API key**
3. **Configured using secure methods** (not hardcoded)