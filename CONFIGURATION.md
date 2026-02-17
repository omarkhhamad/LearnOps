# Configuration and Secrets Management

## Important Security Notice

**Never commit secrets to the repository!** This includes:

- Database connection strings
- JWT secrets
- OAuth client IDs and secrets
- API keys
- Passwords

## How to Configure Secrets

### For Development

1. **Use `appsettings.Development.json`** (already gitignored)
   - This file contains your actual secrets for local development
   - It overrides values from `appsettings.json`
   - Never commit this file to Git

2. **Current Configuration Files:**
   - `appsettings.json` - Contains placeholder values (committed to Git)
   - `appsettings.Development.json` - Contains actual secrets (gitignored)

### For Production

Use environment variables or a secure configuration provider:

```bash
# Example: Setting environment variables
export ConnectionStrings__DefaultConnection="Server=...;Database=...;"
export JwtSettings__Secret="your-jwt-secret"
export Authentication__Google__ClientId="your-client-id"
export Authentication__Google__ClientSecret="your-client-secret"
```

Or use Azure Key Vault, AWS Secrets Manager, or similar services.

## Configuration Structure

The application expects the following configuration sections:

### ConnectionStrings

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER; Database=YOUR_DATABASE; User Id=YOUR_USER; Password=YOUR_PASSWORD; Encrypt=True; TrustServerCertificate=True;"
  }
}
```

### JwtSettings

```json
{
  "JwtSettings": {
    "Secret": "YOUR_JWT_SECRET_KEY_HERE",
    "Issuer": "MyApp",
    "Audience": "MyAppUsers",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  }
}
```

### Authentication (Google OAuth)

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "YOUR_GOOGLE_CLIENT_ID",
      "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
    }
  }
}
```

## What to Do If You Accidentally Commit Secrets

If you accidentally commit secrets to Git:

1. **Immediately rotate/revoke the exposed secrets**
   - Change database passwords
   - Generate new JWT secrets
   - Revoke and regenerate OAuth credentials
   - Rotate any other exposed keys

2. **Remove secrets from the repository:**

   ```bash
   # Edit the file to remove secrets
   # Then amend the commit
   git add <file>
   git commit --amend --no-edit
   git push origin <branch> --force
   ```

3. **Consider using tools like `git-filter-repo` or `BFG Repo-Cleaner` if secrets are in older commits**

## Best Practices

1. ✅ Use `appsettings.Development.json` for local development secrets
2. ✅ Use environment variables or secure vaults for production
3. ✅ Keep `appsettings.json` with placeholder values only
4. ✅ Review `.gitignore` to ensure secret files are excluded
5. ✅ Enable GitHub secret scanning (already active on this repo)
6. ❌ Never commit files containing real secrets
7. ❌ Never share secrets in chat, email, or documentation

## Current .gitignore Rules

The following files are already excluded from Git:

- `appsettings.Development.json`
- `appsettings.Production.json` (if created)
- Any file matching `*.user`, `*.suo`, etc.
