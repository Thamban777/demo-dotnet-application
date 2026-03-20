# DevSecOps Pipeline - .NET Setup Guide

## 📋 Prerequisites

1. **GitHub Repository** with main and develop branches
2. **.NET 8.0 SDK** or later
3. **Docker** for building container images
4. **GitHub Secrets** configured

## 🔑 Required GitHub Secrets

Add these secrets to your GitHub repository (Settings → Secrets and variables → Actions):

### 1. **SonarQube Configuration**
```
SONAR_HOST_URL = https://your-sonarqube-instance.com
SONAR_TOKEN = your-sonarqube-authentication-token
```

### 2. **Slack Notifications** (Optional)
```
SLACK_WEBHOOK_URL = https://hooks.slack.com/services/YOUR/WEBHOOK/URL
```

### 3. **AWS Configuration** (If using ECR - Optional)
```
AWS_REGION = us-east-1
AWS_ACCOUNT_ID = 123456789012
```

### 4. **Container Registry** (GitHub Container Registry)
- Uses `GITHUB_TOKEN` automatically
- No additional setup needed

## 📁 Project Structure

```
.
├── .github/
│   └── workflows/
│       └── devsecops-dotnet-pipeline.yml
├── YourApp/
│   ├── YourApp.csproj
│   ├── Program.cs
│   └── [Your application files]
├── YourApp.Tests/
│   ├── YourApp.Tests.csproj
│   └── ApiTests.cs
├── Dockerfile
├── .dockerignore
├── appsettings.json
├── appsettings.Development.json
└── README.md
```

## 🚀 Quick Start

### Step 1: Create GitHub Workflow Directory
```bash
mkdir -p .github/workflows
```

### Step 2: Copy Pipeline File
```bash
cp devsecops-dotnet-pipeline.yml .github/workflows/
```

### Step 3: Update Project Names
Edit the pipeline file and replace:
- `YourApp` with your actual project name
- `your-super-secret-key-min-32-characters-long!!` with your actual secret

### Step 4: Create appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "Jwt": {
    "Secret": "your-super-secret-key-min-32-characters-long!!",
    "Issuer": "YourApp",
    "Audience": "YourApp-Users"
  },
  "AllowedOrigins": "http://localhost:3000,http://localhost:5000",
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=YourAppDb;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### Step 5: Create .dockerignore
```
.git
.gitignore
*.md
.DS_Store
bin/
obj/
.vs/
.vscode/
*.user
*.suo
*.sln.docstates
```

## 🔄 Pipeline Stages Explained

### Stage 1: SAST (SonarQube)
- Static code analysis
- Code quality checking
- Security vulnerability detection
- **Critical Stage** - blocks deployment on failure

### Stage 2: Secret Scanning (Gitleaks)
- Detects API keys, passwords, tokens in code
- **Critical Stage** - blocks deployment on failure

### Stage 3: SCA (Dependency Check + NuGet)
- Scans NuGet packages for known vulnerabilities
- Checks CVSS scores
- **Non-Critical** - can proceed with warnings

### Stage 4: SecurityCodeScan
- .NET-specific security analysis
- CWE detection
- **Non-Critical** - warnings only

### Stage 5: Container Security
- Builds Docker image
- Trivy vulnerability scanning
- Grype vulnerability scanning
- Pushes to GitHub Container Registry
- **Critical Stage** - blocks deployment on failure

### Stage 6: IaC Scanning (Checkov)
- Scans Kubernetes manifests
- Docker security best practices
- **Non-Critical** - warnings only

### Stage 7: Unit Tests
- Runs xUnit tests
- Generates code coverage reports
- **Non-Critical** - informational

### Stage 8: DAST (OWASP ZAP)
- Runtime security testing
- API endpoint scanning
- Only runs on main branch pushes
- **Non-Critical** - warnings only

### Stage 9: Security Report
- Aggregates all findings
- Generates comprehensive report
- Posts to PR comments
- Sends Slack notifications

### Stage 10: Pipeline Status
- Final enforcement gate
- Critical failures block deployment
- Non-critical issues allow merge

## ✅ Success Criteria

✅ **All Critical Stages Pass:**
- SAST (SonarQube) ✓
- Secret Scanning ✓
- Container Security ✓

✅ **Non-Critical Can Have Issues:**
- SCA (Dependency Check)
- SecurityCodeScan
- IaC Scanning
- Unit Tests
- DAST

## 🛠️ Common Issues & Solutions

### Issue 1: SonarQube Token Invalid
**Solution:**
```bash
# Verify token at SonarQube instance
# Settings → Security → Tokens
# Generate new token and update GitHub Secret
```

### Issue 2: Container Registry Authentication Failed
**Solution:**
```bash
# GitHub automatically provides GITHUB_TOKEN
# Ensure "packages: write" permission in workflow
# Check GHCR access: https://ghcr.io/your-username
```

### Issue 3: OWASP ZAP Timeout
**Solution:**
```bash
# Increase timeout-minutes: 30 → 45
# Ensure application health endpoint responds quickly
# Check logs: docker logs testapp
```

### Issue 4: Unit Tests Failing
**Solution:**
```bash
# Run locally: dotnet test
# Ensure test project references main project
# Check NuGet restore: dotnet restore
```

## 📊 Monitoring & Alerts

### View Results
1. **GitHub Actions:** Actions tab → Select workflow run
2. **GitHub Security:** Security tab → Code scanning alerts
3. **SonarQube:** Project dashboard
4. **Slack:** Configured webhook notifications

### Interpret Results
- 🟢 **Green**: All checks passed, ready to merge
- 🟡 **Yellow**: Non-critical issues, can merge with caution
- 🔴 **Red**: Critical issues, cannot merge until fixed

## 📝 Best Practices

1. **Branch Protection Rules**
   ```
   - Require status checks to pass before merging
   - Select all critical stage checks
   - Require code reviews
   - Enforce conversation resolution
   ```

2. **Secret Management**
   ```
   - Never commit secrets to Git
   - Use GitHub Secrets for sensitive data
   - Rotate tokens regularly
   - Use least privilege access
   ```

3. **Code Quality**
   ```
   - Maintain >80% code coverage
   - Keep SonarQube debt ratio <5%
   - No critical code smells
   - Regular dependency updates
   ```

4. **Security**
   ```
   - Enable branch protection
   - Require security approvals
   - Use multi-factor authentication
   - Audit logs regularly
   ```

## 🔗 Useful Links

- [SonarQube Docs](https://docs.sonarqube.org)
- [OWASP ZAP](https://www.zaproxy.org)
- [Trivy](https://github.com/aquasecurity/trivy)
- [Gitleaks](https://github.com/gitleaks/gitleaks)
- [GitHub Actions](https://docs.github.com/en/actions)
- [.NET 8 Security](https://learn.microsoft.com/en-us/dotnet/core/versions/release-notes-8-0)

## 💡 Customization Examples

### Increase Code Coverage Threshold
Edit pipeline:
```yaml
- name: Check Coverage
  run: |
    # If coverage < 80%, fail
    COVERAGE=$(cat coverage/summary.json | jq .coverage)
    if [ $COVERAGE -lt 80 ]; then exit 1; fi
```

### Skip DAST on Certain Branches
```yaml
dast-owasp-zap:
  if: github.ref == 'refs/heads/main' && github.event_name == 'push'
```

### Add Email Notifications
```yaml
- name: Email Notification
  uses: dawidd6/action-send-mail@v3
  with:
    server_address: smtp.gmail.com
    server_port: 465
    username: ${{ secrets.EMAIL_USERNAME }}
    password: ${{ secrets.EMAIL_PASSWORD }}
    subject: Security Scan Report
    to: team@example.com
    body: ${{ steps.report.outputs.overall_status }}
```

## ❓ FAQ

**Q: Can I disable a stage?**
A: Yes, comment out the job and remove from `needs:` dependencies.

**Q: How long does the pipeline take?**
A: Typically 15-25 minutes depending on code size and tools.

**Q: Can it run on PR?**
A: Yes, remove `if: github.event_name == 'push'` conditions.

**Q: What if SonarQube is not available?**
A: Remove SAST stage from pipeline, use GitHub's built-in code scanning instead.

**Q: How do I update dependencies?**
A: Run `dotnet outdated` to see updates, then `dotnet list package --outdated` and update in .csproj.

## 📞 Support

For issues:
1. Check GitHub Actions logs
2. Review tool documentation (links above)
3. Check GitHub Discussions
4. Create an Issue with full logs

---

**Last Updated:** 2024
**Version:** 1.0.0
