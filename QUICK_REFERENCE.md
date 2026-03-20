# 📋 Quick Copy-Paste Setup Reference

Ithla files copy panna exact locations nu:

---

## 1️⃣ **Main Application File**

**File Name:** `Program_Complete.cs`  
**Copy To:** `DemoDotnetApplication/Program.cs`

```
demo-dotnet-application/
└── DemoDotnetApplication/
    └── Program.cs  ← Copy here (rename from Program_Complete.cs)
```

---

## 2️⃣ **Unit Tests File**

**File Name:** `ApiTests_Complete.cs`  
**Copy To:** `DemoDotnetApplication.Tests/ApiTests.cs`

```
demo-dotnet-application/
└── DemoDotnetApplication.Tests/
    └── ApiTests.cs  ← Copy here (rename from ApiTests_Complete.cs)
```

---

## 3️⃣ **Docker Configuration**

**File Name:** `Dockerfile_Complete`  
**Copy To:** `Dockerfile` (in repository root)

```
demo-dotnet-application/
└── Dockerfile  ← Copy here (rename from Dockerfile_Complete, remove extension)
```

---

## 4️⃣ **Project Files (.csproj)**

### Main Project:
**File Name:** `DemoDotnetApplication.csproj`  
**Copy To:** `DemoDotnetApplication/DemoDotnetApplication.csproj`

### Test Project:
**File Name:** `DemoDotnetApplication.Tests.csproj`  
**Copy To:** `DemoDotnetApplication.Tests/DemoDotnetApplication.Tests.csproj`

```
demo-dotnet-application/
├── DemoDotnetApplication/
│   └── DemoDotnetApplication.csproj  ← Copy here
└── DemoDotnetApplication.Tests/
    └── DemoDotnetApplication.Tests.csproj  ← Copy here
```

---

## 5️⃣ **Solution File**

**File Name:** `DemoDotnetApplication.sln`  
**Copy To:** Repository root (no subfolder)

```
demo-dotnet-application/
└── DemoDotnetApplication.sln  ← Copy here (in root)
```

---

## 6️⃣ **GitHub Workflow (Pipeline)**

**File Name:** `demo-dotnet-application.yml`  
**Copy To:** `.github/workflows/demo-dotnet-application.yml`

```
demo-dotnet-application/
└── .github/
    └── workflows/
        └── demo-dotnet-application.yml  ← Copy here
```

---

## 7️⃣ **Configuration Files (CREATE NEW)**

### appsettings.json
**Location:** `DemoDotnetApplication/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Jwt": {
    "Secret": "your-super-secret-key-minimum-32-characters-long-for-production-use!!",
    "Issuer": "DemoDotnetApplication",
    "Audience": "DemoDotnetApplication-Users",
    "ExpirationMinutes": 60
  },
  "Cors": {
    "AllowedOrigins": ["https://yourdomain.com"]
  },
  "Security": {
    "RequireHttps": true
  }
}
```

### appsettings.Development.json
**Location:** `DemoDotnetApplication/appsettings.Development.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information"
    }
  },
  "AllowedHosts": "*",
  "Jwt": {
    "Secret": "dev-secret-key-minimum-32-characters-for-local-development-testing!!",
    "Issuer": "DemoDotnetApplication-Dev",
    "Audience": "DemoDotnetApplication-Users-Dev",
    "ExpirationMinutes": 1440
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5000",
      "http://localhost:5173"
    ]
  },
  "Security": {
    "RequireHttps": false
  }
}
```

### .dockerignore
**Location:** `.dockerignore` (in repository root)

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
coverage/
```

### .gitignore
**Location:** `.gitignore` (in repository root)

```
# Build
[Bb]in/
[Oo]bj/
[Dd]ebug/
[Rr]elease/

# Visual Studio
.vs/
*.user
*.suo

# VS Code
.vscode/

# NuGet
packages/

# Test Results
TestResults/
coverage/

# Environment
.env
.env.local

# OS
.DS_Store
Thumbs.db
```

---

## 📁 **Complete Folder Structure After Setup**

```
demo-dotnet-application/
│
├── .github/
│   └── workflows/
│       └── demo-dotnet-application.yml      ✅ Copy here
│
├── DemoDotnetApplication/                    (Rename to match your needs if different)
│   ├── DemoDotnetApplication.csproj         ✅ Copy here
│   ├── Program.cs                           ✅ Copy here (from Program_Complete.cs)
│   ├── appsettings.json                     ✅ Create new
│   ├── appsettings.Development.json         ✅ Create new
│   ├── Controllers/                         (Create folder)
│   ├── Models/                              (Create folder)
│   └── Services/                            (Create folder)
│
├── DemoDotnetApplication.Tests/             (Rename to match your needs if different)
│   ├── DemoDotnetApplication.Tests.csproj   ✅ Copy here
│   ├── ApiTests.cs                          ✅ Copy here (from ApiTests_Complete.cs)
│   └── Unit/                                (Create folder)
│
├── DemoDotnetApplication.sln                ✅ Copy here
├── Dockerfile                               ✅ Copy here (from Dockerfile_Complete)
├── .dockerignore                            ✅ Create new
├── .gitignore                               ✅ Create new
└── README.md                                (Optional)
```

---

## ⚡ **Quick Commands to Run After Setup**

```bash
# 1. Navigate to repository
cd demo-dotnet-application

# 2. Restore dependencies
dotnet restore

# 3. Build solution
dotnet build

# 4. Run tests
dotnet test

# 5. Build Docker image
docker build -t demo-dotnet-app:latest .

# 6. Run Docker container
docker run -p 8080:8080 demo-dotnet-app:latest

# 7. Git operations
git add .
git commit -m "feat: Add complete DevSecOps pipeline"
git push origin main
```

---

## 🔐 **GitHub Secrets to Add**

Go to: **Settings** → **Secrets and variables** → **Actions**

| Secret Name | Example Value |
|------------|---|
| `SONAR_HOST_URL` | `https://sonarqube.yourcompany.com` |
| `SONAR_TOKEN` | Your SonarQube token |
| `SLACK_WEBHOOK_URL` | `https://hooks.slack.com/services/...` (Optional) |

---

## ✅ **Verification Checklist**

- [ ] All files copied to correct locations
- [ ] Configuration files created
- [ ] `dotnet restore` works
- [ ] `dotnet build` works
- [ ] `dotnet test` works
- [ ] Docker image builds
- [ ] GitHub secrets configured
- [ ] Code pushed to GitHub
- [ ] Pipeline running in GitHub Actions

---

## 🎯 **That's It!**

After copying all files and running the commands:

✅ Your project is complete  
✅ DevSecOps pipeline is active  
✅ Security checks run automatically  
✅ Every commit triggers 10 security stages  

Ready to deploy with confidence! 🚀

---

## 📞 **If You Get Stuck**

1. **dotnet restore fails?**
   ```bash
   dotnet nuget locals all --clear
   dotnet restore
   ```

2. **Tests fail?**
   ```bash
   dotnet test --verbosity detailed
   ```

3. **Docker fails?**
   ```bash
   docker build --no-cache -t demo-dotnet-app:latest .
   ```

4. **GitHub Actions fails?**
   - Check Actions tab for detailed logs
   - Verify secrets are configured
   - Ensure .sln file exists

---

**You're all set! Copy the files and push to GitHub!** 💪
