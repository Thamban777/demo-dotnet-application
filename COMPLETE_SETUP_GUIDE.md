# 🚀 Complete Setup Guide for demo-dotnet-application

## Total Setup Time: 30 minutes

---

## 📋 STEP 1: Prerequisites (5 minutes)

### Install Required Tools

**For Windows:**
```bash
# Install .NET 8 SDK
# Download from: https://dotnet.microsoft.com/download/dotnet/8.0

# Verify installation
dotnet --version
# Should show: 8.0.x or higher
```

**For Mac:**
```bash
brew install dotnet

# Verify
dotnet --version
```

**For Linux (Ubuntu/Debian):**
```bash
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0

# Verify
dotnet --version
```

**Install Docker:**
- Download from: https://www.docker.com/products/docker-desktop

---

## 📁 STEP 2: Setup Project Folder Structure (5 minutes)

Create the following folder structure in your repository:

```bash
# Create main application folder
mkdir DemoDotnetApplication
mkdir DemoDotnetApplication.Tests

# Create additional folders (optional, but recommended)
mkdir DemoDotnetApplication/Controllers
mkdir DemoDotnetApplication/Models
mkdir DemoDotnetApplication/Services
mkdir DemoDotnetApplication.Tests/Unit
mkdir DemoDotnetApplication.Tests/Integration
```

---

## 🔧 STEP 3: Copy Files to Correct Locations (5 minutes)

### Files to Copy:

1. **Copy Program.cs**
   ```
   File: Program_Complete.cs
   Location: DemoDotnetApplication/Program.cs
   ```

2. **Copy ApiTests.cs**
   ```
   File: ApiTests_Complete.cs
   Location: DemoDotnetApplication.Tests/ApiTests.cs
   ```

3. **Copy Dockerfile**
   ```
   File: Dockerfile_Complete
   Location: Dockerfile (in repository root)
   ```

4. **Copy .csproj files**
   ```
   File: DemoDotnetApplication.csproj
   Location: DemoDotnetApplication/DemoDotnetApplication.csproj
   
   File: DemoDotnetApplication.Tests.csproj
   Location: DemoDotnetApplication.Tests/DemoDotnetApplication.Tests.csproj
   ```

5. **Copy Solution File**
   ```
   File: DemoDotnetApplication.sln
   Location: DemoDotnetApplication.sln (in repository root)
   ```

6. **Copy Pipeline File**
   ```
   File: demo-dotnet-application.yml
   Location: .github/workflows/demo-dotnet-application.yml
   ```

---

## ⚙️ STEP 4: Create Configuration Files (3 minutes)

### Create appsettings.json

File: `DemoDotnetApplication/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "Jwt": {
    "Secret": "your-super-secret-key-minimum-32-characters-long-for-production-use!!",
    "Issuer": "DemoDotnetApplication",
    "Audience": "DemoDotnetApplication-Users",
    "ExpirationMinutes": 60
  },
  "Database": {
    "ConnectionString": "Server=localhost;Database=DemoDotnetApplicationDb;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "Cors": {
    "AllowedOrigins": [
      "https://yourdomain.com",
      "https://www.yourdomain.com"
    ]
  },
  "Security": {
    "RequireHttps": true,
    "MaxRequestSize": 10485760,
    "ApiRateLimitPerMinute": 100
  }
}
```

### Create appsettings.Development.json

File: `DemoDotnetApplication/appsettings.Development.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information",
      "Microsoft.Hosting.Lifetime": "Debug"
    }
  },
  "AllowedHosts": "*",
  "Jwt": {
    "Secret": "dev-secret-key-minimum-32-characters-for-local-development-testing!!",
    "Issuer": "DemoDotnetApplication-Dev",
    "Audience": "DemoDotnetApplication-Users-Dev",
    "ExpirationMinutes": 1440
  },
  "Database": {
    "ConnectionString": "Server=(localdb)\\mssqllocaldb;Database=DemoDotnetApplicationDb;Integrated Security=true;TrustServerCertificate=true;"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5000",
      "http://localhost:5173",
      "http://127.0.0.1:*"
    ]
  },
  "Security": {
    "RequireHttps": false,
    "MaxRequestSize": 52428800,
    "ApiRateLimitPerMinute": 1000
  }
}
```

### Create .dockerignore

File: `.dockerignore`

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
.github/
*.log
coverage/
reports/
```

### Create .gitignore

File: `.gitignore`

```
# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
bld/
[Bb]in/
[Oo]bj/

# Visual Studio
.vs/
*.sln.docstates
*.user
*.suo

# VS Code
.vscode/
*.code-workspace

# NuGet
packages/
.nupkg.metadata
.package.lock.json

# Test Results
TestResults/
*.trx
coverage/

# Environment
.env
.env.local
*.pem
*.key

# OS
.DS_Store
Thumbs.db

# Local user files
appsettings.local.json
launchSettings.json
```

---

## 🧪 STEP 5: Test Locally (8 minutes)

### Test 1: Restore Packages
```bash
cd demo-dotnet-application
dotnet restore
# Wait for completion... should see "Restore completed"
```

### Test 2: Build Solution
```bash
dotnet build
# Should complete without errors
# Output: Build succeeded!
```

### Test 3: Run Unit Tests
```bash
dotnet test
# Should see: Test Run Successful
# Example output:
# Passed! - Failed: 0, Passed: 24, Skipped: 0
```

### Test 4: Run Application Locally
```bash
cd DemoDotnetApplication
dotnet run
# You should see:
# info: DemoDotnetApplication[0]
#       🚀 Starting Demo Dotnet Application...
# info: Microsoft.Hosting.Lifetime[13]
#       Listening on http://localhost:5000

# Open browser: http://localhost:5000
# Should see JSON response with application info
```

**Stop the application:** Press `Ctrl+C`

### Test 5: Test API Endpoints (Optional)

```bash
# In a new terminal window

# Test health endpoint
curl http://localhost:5000/health

# Test home endpoint
curl http://localhost:5000/

# Test items endpoint
curl http://localhost:5000/api/items
```

---

## 🐳 STEP 6: Build Docker Image (5 minutes)

```bash
# Navigate to repository root
cd demo-dotnet-application

# Build Docker image
docker build -t demo-dotnet-app:latest .

# Verify image was created
docker images
# Should see: demo-dotnet-app  latest

# Run container
docker run -p 8080:8080 demo-dotnet-app:latest

# Test container
# In new terminal: curl http://localhost:8080/health

# Stop container: Press Ctrl+C
```

---

## 🔐 STEP 7: Configure GitHub Secrets (3 minutes)

1. **Go to GitHub Repository**
   - Click **Settings** tab
   - Left sidebar → **Secrets and variables** → **Actions**
   - Click **New repository secret**

2. **Add SONAR_HOST_URL**
   - Name: `SONAR_HOST_URL`
   - Value: `https://sonarqube.yourcompany.com` (or your SonarQube instance)
   - Click **Add secret**

3. **Add SONAR_TOKEN**
   - Go to your SonarQube instance
   - Login → Profile (top-right) → My Account → Security
   - Generate new token (name it "GitHub Actions")
   - Copy the token
   - In GitHub → Name: `SONAR_TOKEN`
   - Value: Paste the token
   - Click **Add secret**

4. **Add SLACK_WEBHOOK_URL (Optional)**
   - Create Slack webhook: https://api.slack.com/apps
   - Name: `SLACK_WEBHOOK_URL`
   - Value: Your webhook URL
   - Click **Add secret**

---

## 📤 STEP 8: Push to GitHub (2 minutes)

```bash
# Make sure you're in the repository root

# Check status
git status

# Add all files
git add .

# Commit with message
git commit -m "feat: Add complete DevSecOps pipeline for demo-dotnet-application

- Add Program.cs with security features
- Add comprehensive unit tests
- Add Dockerfile with multi-stage build
- Add configuration files
- Add GitHub Actions workflow"

# Push to GitHub
git push origin main

# If develop branch exists:
git push origin develop
```

---

## ✅ STEP 9: Verify Pipeline Execution (5 minutes)

1. **Go to GitHub Repository**
   - Click **Actions** tab
   - You should see the workflow running
   - It will show: "DevSecOps Pipeline - .NET Shift Left Security"

2. **Wait for Completion**
   - Each stage will execute in order
   - Takes approximately 30-45 minutes total
   - You'll see stages passing or failing

3. **Monitor Individual Stages**
   - Click on the workflow run
   - Click on each job to see detailed logs
   - Green checkmark = Stage passed ✅
   - Red X = Stage failed ❌

4. **Check Results**
   - GitHub Security tab → Shows code scanning alerts
   - SonarQube Dashboard → Code quality metrics
   - Slack (if configured) → Notifications

---

## 📊 STEP 10: Monitor Results (Ongoing)

### In GitHub:
- **Actions Tab** → All workflow runs
- **Security Tab** → Security alerts and scanning results
- **Pull Requests** → Security reports in PR comments

### In SonarQube:
- Project Dashboard → Code metrics
- Issues → Found vulnerabilities
- Coverage → Code coverage percentage

### In Slack:
- Notifications for each pipeline run
- Status updates (pass/fail)
- Summary of security findings

---

## 🎯 Success Indicators

You'll know everything is working when:

✅ **Local Tests Pass**
```
dotnet test
# Output: Passed! - Failed: 0, Passed: X, Skipped: 0
```

✅ **Docker Build Succeeds**
```
docker build -t demo-dotnet-app:latest .
# Output: Successfully tagged demo-dotnet-app:latest
```

✅ **GitHub Actions Workflow Runs**
- All stages show green checkmarks
- Pipeline completes in ~30-45 minutes

✅ **SonarQube Shows Results**
- Code metrics calculated
- Quality gate status shown

✅ **Slack Notifications Received** (if configured)
- Security scan started
- Security scan completed
- Pipeline passed/failed

---

## 🐛 Troubleshooting

### Problem: `dotnet restore` fails
```
Solution:
1. Check .NET version: dotnet --version
2. Clear NuGet cache: dotnet nuget locals all --clear
3. Restore again: dotnet restore
```

### Problem: Tests fail locally
```
Solution:
1. Verify xUnit installed: dotnet add package xunit
2. Run specific test: dotnet test --verbosity detailed
3. Check test project structure
```

### Problem: Docker build fails
```
Solution:
1. Verify Docker is running
2. Check Dockerfile syntax
3. Build with no cache: docker build --no-cache -t demo-dotnet-app:latest .
```

### Problem: GitHub Actions workflow fails
```
Solution:
1. Check GitHub Secrets are configured
2. Review workflow logs for specific error
3. Verify .sln file exists
4. Ensure solution includes all projects
```

### Problem: SonarQube integration fails
```
Solution:
1. Verify SONAR_HOST_URL is correct
2. Verify SONAR_TOKEN is valid (not expired)
3. Generate new token if needed
4. Check SonarQube instance is accessible
```

---

## 📞 Quick Reference Commands

```bash
# Clean build
dotnet clean
dotnet build

# Run specific project
dotnet run --project DemoDotnetApplication

# Run tests with coverage
dotnet test /p:CollectCoverage=true

# Build Docker image
docker build -t demo-dotnet-app:latest .

# Run Docker container
docker run -p 8080:8080 demo-dotnet-app:latest

# Check solution structure
dotnet sln DemoDotnetApplication.sln list

# Format code
dotnet format

# Check for security issues
dotnet list package --vulnerable
```

---

## ✨ Congratulations! 🎉

You now have a complete DevSecOps pipeline running 10 security stages automatically!

**Every time you push code:**
✅ Code is analyzed for vulnerabilities
✅ Dependencies are checked for security issues
✅ Docker image is built and scanned
✅ Unit tests run automatically
✅ Security report is generated
✅ Team is notified via Slack

**You're deploying secure code with confidence!** 🚀

---

**Need Help?** Check the GitHub Actions logs or the specific tool documentation:
- SonarQube: https://docs.sonarqube.org
- Docker: https://docs.docker.com
- .NET: https://learn.microsoft.com/dotnet
- GitHub Actions: https://docs.github.com/actions
