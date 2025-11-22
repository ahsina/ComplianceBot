# Exigences Techniques - Modules de Reporting ComplianceBot

**Version:** 1.0
**Date:** Novembre 2025
**Audience:** Développeurs de modules

---

## Table des Matières

1. [Vue d'Ensemble](#vue-densemble)
2. [Architecture Modulaire](#architecture-modulaire)
3. [Structure d'un Module](#structure-dun-module)
4. [Interfaces Communes](#interfaces-communes)
5. [Séparation des Couches](#séparation-des-couches)
6. [Conventions de Code](#conventions-de-code)
7. [Tests Requis](#tests-requis)
8. [Processus d'Intégration](#processus-dintégration)
9. [Environnement Sandbox](#environnement-sandbox)
10. [Checklist de Développement](#checklist-de-développement)

---

## Vue d'Ensemble

### Objectif

Chaque module de reporting (RBE, CEDR, FATCA, CRS, SFDR) doit être développé de manière **indépendante** mais suivre des **standards communs** pour permettre une intégration fluide dans la plateforme ComplianceBot.

### Principes Directeurs

- ✅ **Isolation:** Chaque module est autonome
- ✅ **Interopérabilité:** Interfaces communes pour intégration
- ✅ **Testabilité:** Tests unitaires + intégration obligatoires
- ✅ **Sandbox:** Environnement de test isolé
- ✅ **Documentation:** README + exemples par module

---

## Architecture Modulaire

### Schéma de Communication

```
┌─────────────────────────────────────────────────────────┐
│                   ComplianceBot.API                     │
│  ┌────────────┐  ┌────────────┐  ┌────────────┐        │
│  │ Reports    │  │ Validation │  │ Submission │        │
│  │ Controller │  │ Controller │  │ Controller │        │
│  └─────┬──────┘  └─────┬──────┘  └─────┬──────┘        │
└────────┼───────────────┼───────────────┼────────────────┘
         │               │               │
         ▼               ▼               ▼
┌─────────────────────────────────────────────────────────┐
│              ComplianceBot.Application                  │
│  Commands/Queries + MediatR Handlers                    │
└────────┬───────────────┬───────────────┬────────────────┘
         │               │               │
         ▼               ▼               ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│ Module RBE   │  │ Module CEDR  │  │ Module FATCA │  ...
│              │  │              │  │              │
│ ┌──────────┐ │  │ ┌──────────┐ │  │ ┌──────────┐ │
│ │ Models   │ │  │ │ Models   │ │  │ │ Models   │ │
│ ├──────────┤ │  │ ├──────────┤ │  │ ├──────────┤ │
│ │Validation│ │  │ │Validation│ │  │ │Validation│ │
│ ├──────────┤ │  │ ├──────────┤ │  │ ├──────────┤ │
│ │Generation│ │  │ │Generation│ │  │ │Generation│ │
│ ├──────────┤ │  │ ├──────────┤ │  │ ├──────────┤ │
│ │Submission│ │  │ │Submission│ │  │ │Submission│ │
│ └──────────┘ │  │ └──────────┘ │  │ └──────────┘ │
└──────────────┘  └──────────────┘  └──────────────┘
         │               │               │
         └───────────────┴───────────────┘
                         │
                         ▼
              ┌──────────────────────┐
              │ ComplianceBot.Shared │
              │  Common Interfaces   │
              └──────────────────────┘
```

### Dépendances

**Chaque module dépend de:**
- `ComplianceBot.Application` (CQRS, DTOs)
- `ComplianceBot.Modules.Shared` (interfaces communes)
- Packages NuGet spécifiques (ex: ClosedXML pour Excel)

**Chaque module NE dépend PAS de:**
- Autres modules (RBE ≠> CEDR)
- Infrastructure directement
- Base de données directement

---

## Structure d'un Module

### Arborescence Standard

```
ComplianceBot.Modules.{ReportType}/
├── Models/
│   ├── {ReportType}ReportData.cs       # Modèle de données métier
│   ├── {ReportType}ValidationResult.cs # Résultat de validation
│   └── {ReportType}SubmissionResult.cs # Résultat de soumission
├── Services/
│   ├── I{ReportType}ValidationService.cs    # Interface validation
│   ├── {ReportType}ValidationService.cs     # Implémentation validation
│   ├── I{ReportType}GenerationService.cs    # Interface génération
│   ├── {ReportType}GenerationService.cs     # Implémentation génération
│   ├── I{ReportType}SubmissionService.cs    # Interface soumission
│   └── {ReportType}SubmissionService.cs     # Implémentation soumission
├── Validators/
│   └── {ReportType}DataValidator.cs    # FluentValidation validators
├── Mappers/
│   └── {ReportType}Mapper.cs           # Mapping business → XML/JSON
├── Constants/
│   └── {ReportType}Constants.cs        # Constantes réglementaires
├── Tests/
│   ├── {ReportType}ValidationServiceTests.cs
│   ├── {ReportType}GenerationServiceTests.cs
│   └── {ReportType}SubmissionServiceTests.cs
├── README.md                            # Documentation module
├── CHANGELOG.md                         # Historique des versions
└── ComplianceBot.Modules.{ReportType}.csproj
```

### Exemple: Module RBE

```
ComplianceBot.Modules.RBE/
├── Models/
│   ├── RBEReportData.cs
│   ├── RBEValidationResult.cs
│   └── RBESubmissionResult.cs
├── Services/
│   ├── IRBEValidationService.cs
│   ├── RBEValidationService.cs
│   ├── IRBEGenerationService.cs
│   ├── RBEGenerationService.cs
│   ├── IRBESubmissionService.cs
│   └── RBESubmissionService.cs
├── Validators/
│   └── RBEDataValidator.cs
├── Mappers/
│   └── RBEXmlMapper.cs
├── Constants/
│   └── RBEConstants.cs
└── README.md
```

---

## Interfaces Communes

### IReportValidationService<TReportData>

**Emplacement:** `ComplianceBot.Modules.Shared/Interfaces/IReportValidationService.cs`

```csharp
public interface IReportValidationService<TReportData> where TReportData : class
{
    /// <summary>
    /// Validates report data according to regulatory rules
    /// </summary>
    Task<ValidationResult> ValidateAsync(
        TReportData data,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates report data in sandbox mode (relaxed rules)
    /// </summary>
    Task<ValidationResult> ValidateSandboxAsync(
        TReportData data,
        CancellationToken cancellationToken = default);
}
```

### IReportGenerationService<TReportData>

```csharp
public interface IReportGenerationService<TReportData> where TReportData : class
{
    /// <summary>
    /// Generates regulatory XML/JSON from report data
    /// </summary>
    Task<GenerationResult> GenerateAsync(
        TReportData data,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates preview/sample report for sandbox
    /// </summary>
    Task<GenerationResult> GenerateSandboxAsync(
        TReportData data,
        CancellationToken cancellationToken = default);
}
```

### IReportSubmissionService<TReportData>

```csharp
public interface IReportSubmissionService<TReportData> where TReportData : class
{
    /// <summary>
    /// Submits report to regulatory authority
    /// </summary>
    Task<SubmissionResult> SubmitAsync(
        TReportData data,
        string generatedContent,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Simulates submission for sandbox (no actual submission)
    /// </summary>
    Task<SubmissionResult> SubmitSandboxAsync(
        TReportData data,
        string generatedContent,
        CancellationToken cancellationToken = default);
}
```

### ValidationResult (Shared)

```csharp
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationError> Errors { get; set; } = new();
    public List<ValidationWarning> Warnings { get; set; } = new();
    public DateTime ValidatedAt { get; set; }
    public string ValidatorVersion { get; set; } = string.Empty;
}

public class ValidationError
{
    public string Field { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public SeverityLevel Severity { get; set; }
}

public enum SeverityLevel
{
    Info,
    Warning,
    Error,
    Critical
}
```

### GenerationResult (Shared)

```csharp
public class GenerationResult
{
    public bool Success { get; set; }
    public string Content { get; set; } = string.Empty; // XML/JSON
    public string ContentType { get; set; } = string.Empty; // application/xml
    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}
```

### SubmissionResult (Shared)

```csharp
public class SubmissionResult
{
    public bool Success { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public string ReceiptUrl { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public SubmissionStatus Status { get; set; }
}

public enum SubmissionStatus
{
    Pending,
    Submitted,
    Acknowledged,
    Rejected,
    Failed
}
```

---

## Séparation des Couches

### Couche 1: Models (Données Métier)

**Responsabilité:** Représenter les données du rapport

```csharp
// Exemple RBE
public class RBEReportData
{
    public string ReportingMonth { get; set; } = string.Empty;
    public decimal TotalAssetsUnderManagement { get; set; }
    public int NumberOfFunds { get; set; }
    public List<FundPosition> FundPositions { get; set; } = new();
}
```

**Règles:**
- ✅ Propriétés simples (POCOs)
- ✅ Pas de logique métier
- ✅ Annotations Data (requis pour validation)
- ❌ Pas de dépendances externes
- ❌ Pas de méthodes complexes

### Couche 2: Validation (Règles Métier)

**Responsabilité:** Valider les données selon règles réglementaires

```csharp
public class RBEValidationService : IRBEValidationService
{
    public async Task<ValidationResult> ValidateAsync(
        RBEReportData data,
        CancellationToken cancellationToken = default)
    {
        var result = new ValidationResult();

        // Validate reporting month format
        if (!Regex.IsMatch(data.ReportingMonth, @"^\d{4}-\d{2}$"))
        {
            result.Errors.Add(new ValidationError
            {
                Field = nameof(data.ReportingMonth),
                Code = "RBE001",
                Message = "Reporting month must be YYYY-MM format",
                Severity = SeverityLevel.Error
            });
        }

        // Business rules validation
        ValidateBusinessRules(data, result);

        result.IsValid = !result.Errors.Any(e =>
            e.Severity == SeverityLevel.Error ||
            e.Severity == SeverityLevel.Critical);

        return result;
    }
}
```

**Règles:**
- ✅ Validation FluentValidation OU custom
- ✅ Codes d'erreur standardisés (RBE001, CEDR042, etc.)
- ✅ Messages clairs et actionnables
- ✅ Distinction Error vs Warning
- ✅ Mode Sandbox avec règles relaxées

### Couche 3: Generation (XML/JSON)

**Responsabilité:** Transformer données → format réglementaire

```csharp
public class RBEGenerationService : IRBEGenerationService
{
    public async Task<GenerationResult> GenerateAsync(
        RBEReportData data,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var xml = new XDocument(
                new XElement("RBEReport",
                    new XAttribute("version", "1.0"),
                    new XElement("ReportingPeriod", data.ReportingMonth),
                    new XElement("TotalAuM", data.TotalAssetsUnderManagement),
                    // ... generate full XML according to CSSF schema
                )
            );

            var content = xml.ToString();

            return new GenerationResult
            {
                Success = true,
                Content = content,
                ContentType = "application/xml",
                FileName = $"RBE_{data.ReportingMonth}.xml",
                FileSizeBytes = Encoding.UTF8.GetByteCount(content),
                GeneratedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            return new GenerationResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
```

**Règles:**
- ✅ Utiliser schémas XSD officiels
- ✅ Validation schéma avant retour
- ✅ Gestion erreurs complète
- ✅ Support XML ET JSON (selon régulateur)
- ✅ Compression optionnelle (ZIP)

### Couche 4: Submission (Envoi Régulateur)

**Responsabilité:** Soumettre rapport à l'autorité réglementaire

```csharp
public class RBESubmissionService : IRBESubmissionService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public async Task<SubmissionResult> SubmitAsync(
        RBEReportData data,
        string generatedContent,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("CSSF");
        var endpoint = _configuration["CSSF:RBE:SubmissionEndpoint"];

        // Prepare submission (SOAP, REST, FTP selon régulateur)
        var request = PrepareSubmissionRequest(data, generatedContent);

        var response = await client.PostAsync(endpoint, request, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var referenceNumber = await ExtractReferenceNumber(response);

            return new SubmissionResult
            {
                Success = true,
                ReferenceNumber = referenceNumber,
                SubmittedAt = DateTime.UtcNow,
                Status = SubmissionStatus.Submitted
            };
        }

        return new SubmissionResult
        {
            Success = false,
            ErrorMessage = await response.Content.ReadAsStringAsync(),
            Status = SubmissionStatus.Failed
        };
    }

    public async Task<SubmissionResult> SubmitSandboxAsync(
        RBEReportData data,
        string generatedContent,
        CancellationToken cancellationToken = default)
    {
        // Simulate submission without actual HTTP call
        await Task.Delay(100, cancellationToken); // Simulate network delay

        return new SubmissionResult
        {
            Success = true,
            ReferenceNumber = $"SANDBOX-RBE-{Guid.NewGuid():N}",
            SubmittedAt = DateTime.UtcNow,
            Status = SubmissionStatus.Submitted,
            ReceiptUrl = "https://sandbox.compliancebot.lu/receipts/..."
        };
    }
}
```

**Règles:**
- ✅ HttpClient via IHttpClientFactory
- ✅ Retry policy (Polly)
- ✅ Timeout configuration
- ✅ Authentication (API Key, OAuth, mTLS)
- ✅ Mode Sandbox OBLIGATOIRE (pas d'appel réel)

---

## Conventions de Code

### Naming

**Classes:**
```
{ReportType}ValidationService
{ReportType}GenerationService
{ReportType}SubmissionService
{ReportType}ReportData
```

**Interfaces:**
```
I{ReportType}ValidationService
I{ReportType}GenerationService
I{ReportType}SubmissionService
```

**Codes d'Erreur:**
```
{REPORT_TYPE}{NUMBER}
Exemples:
  RBE001 - Invalid reporting month format
  RBE002 - Missing required field
  CEDR100 - Invalid company registration number
  FATCA050 - Tax ID format incorrect
```

### Constants

**Fichier:** `{ReportType}Constants.cs`

```csharp
public static class RBEConstants
{
    // Regulatory
    public const string RegulatorName = "CSSF";
    public const string RegulationReference = "Circular CSSF 13/556";

    // Deadlines
    public const int DayOfMonthDue = 15; // 15th of following month

    // Formats
    public const string ReportingPeriodFormat = "yyyy-MM";
    public const string OutputFormat = "XML";

    // Validation
    public const decimal MinAssetsUnderManagement = 0;
    public const decimal MaxAssetsUnderManagement = 999_999_999_999;

    // Endpoints (from config)
    public const string ConfigKeySubmissionEndpoint = "CSSF:RBE:SubmissionEndpoint";
    public const string ConfigKeySandboxEndpoint = "CSSF:RBE:SandboxEndpoint";
}
```

### Configuration

**appsettings.json:**

```json
{
  "Modules": {
    "RBE": {
      "Enabled": true,
      "ProductionEndpoint": "https://ssl.cssf.lu/rbe/submit",
      "SandboxEndpoint": "https://sandbox.cssf.lu/rbe/submit",
      "ApiKey": "***",
      "Timeout": 30000,
      "RetryAttempts": 3
    },
    "CEDR": {
      "Enabled": true,
      "ProductionEndpoint": "https://ssl.cssf.lu/cedr/submit",
      "SandboxEndpoint": "https://sandbox.cssf.lu/cedr/submit"
    }
  }
}
```

---

## Tests Requis

### Tests Unitaires (Minimum)

**Validation:**
```csharp
[Fact]
public async Task Validate_ValidData_ReturnsSuccess()
{
    // Arrange
    var service = new RBEValidationService();
    var data = new RBEReportData
    {
        ReportingMonth = "2024-01",
        TotalAssetsUnderManagement = 100_000_000,
        NumberOfFunds = 10
    };

    // Act
    var result = await service.ValidateAsync(data);

    // Assert
    result.IsValid.Should().BeTrue();
    result.Errors.Should().BeEmpty();
}

[Fact]
public async Task Validate_InvalidReportingMonth_ReturnsError()
{
    // Arrange
    var service = new RBEValidationService();
    var data = new RBEReportData
    {
        ReportingMonth = "2024/01", // Wrong format
        TotalAssetsUnderManagement = 100_000_000
    };

    // Act
    var result = await service.ValidateAsync(data);

    // Assert
    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => e.Code == "RBE001");
}
```

**Generation:**
```csharp
[Fact]
public async Task Generate_ValidData_ReturnsXml()
{
    // Arrange
    var service = new RBEGenerationService();
    var data = CreateValidRBEData();

    // Act
    var result = await service.GenerateAsync(data);

    // Assert
    result.Success.Should().BeTrue();
    result.Content.Should().StartWith("<?xml");
    result.ContentType.Should().Be("application/xml");
}

[Fact]
public async Task Generate_ValidData_ProducesValidSchema()
{
    // Arrange
    var service = new RBEGenerationService();
    var data = CreateValidRBEData();

    // Act
    var result = await service.GenerateAsync(data);

    // Assert
    var doc = XDocument.Parse(result.Content);
    var isValid = ValidateAgainstXsd(doc, "RBE_v1.xsd");
    isValid.Should().BeTrue();
}
```

### Tests d'Intégration

```csharp
public class RBEModuleIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    [Fact]
    public async Task FullWorkflow_ValidateGenerateSubmit_Sandbox()
    {
        // 1. Validate
        var validationService = GetService<IRBEValidationService>();
        var data = CreateTestData();
        var validationResult = await validationService.ValidateSandboxAsync(data);
        validationResult.IsValid.Should().BeTrue();

        // 2. Generate
        var generationService = GetService<IRBEGenerationService>();
        var generationResult = await generationService.GenerateSandboxAsync(data);
        generationResult.Success.Should().BeTrue();

        // 3. Submit (sandbox)
        var submissionService = GetService<IRBESubmissionService>();
        var submissionResult = await submissionService.SubmitSandboxAsync(
            data,
            generationResult.Content);
        submissionResult.Success.Should().BeTrue();
        submissionResult.ReferenceNumber.Should().StartWith("SANDBOX");
    }
}
```

### Couverture Minimale

- ✅ **Validation:** 80%+ couverture
- ✅ **Generation:** 70%+ couverture
- ✅ **Submission:** 60%+ couverture (mock HTTP)
- ✅ **E2E Sandbox:** 1 test complet par module

---

## Processus d'Intégration

### Phase 1: Développement Isolé

1. **Fork repository** ou **feature branch**
2. **Créer structure module** selon template
3. **Implémenter interfaces** communes
4. **Tests unitaires** (>80% coverage)
5. **Documentation README**
6. **Sandbox validation**

### Phase 2: Code Review

1. **Pull Request** vers `develop`
2. **Review checklist:**
   - ✅ Tests passent (CI/CD)
   - ✅ Couverture suffisante
   - ✅ Interfaces respectées
   - ✅ README complet
   - ✅ Sandbox fonctionnel
   - ✅ Pas de dépendances inter-modules
3. **Approbation:** 2 reviewers minimum

### Phase 3: Intégration

1. **Merge** vers `develop`
2. **Tests d'intégration** automatiques
3. **Déploiement staging**
4. **Tests E2E sandbox**
5. **Validation métier**

### Phase 4: Production

1. **Merge** vers `main`
2. **Tag version** (ex: `rbe-v1.0.0`)
3. **Déploiement production**
4. **Feature flag** activation progressive
5. **Monitoring**

---

## Environnement Sandbox

### Objectif

Permettre aux utilisateurs et développeurs de **tester** les modules sans risque:
- ❌ Pas de soumission réelle aux régulateurs
- ❌ Pas d'impact sur données production
- ✅ Validation relaxée (warnings au lieu d'errors)
- ✅ Données de test pré-remplies
- ✅ Simulation complète du workflow

### Activation Sandbox

**Base de données:**
```csharp
public class Report : TenantEntity
{
    // ... existing properties

    /// <summary>
    /// Indicates if this report is in sandbox mode (test)
    /// </summary>
    public bool IsSandbox { get; set; }
}
```

**API Endpoint:**
```csharp
POST /api/v1/reports/sandbox
{
  "type": "RBE",
  "reportingPeriod": "2024-01",
  "isSandbox": true  // <-- Flag sandbox
}
```

**Service Layer:**
```csharp
public async Task<ReportDto> Handle(CreateReportCommand request, ...)
{
    var report = new Report
    {
        // ...
        IsSandbox = request.IsSandbox
    };

    // Use sandbox services if flag is true
    if (report.IsSandbox)
    {
        await _validationService.ValidateSandboxAsync(data);
        await _generationService.GenerateSandboxAsync(data);
        await _submissionService.SubmitSandboxAsync(data, xml);
    }
}
```

### Données de Test

**SandboxDataGenerator:**
```csharp
public static class RBESandboxDataGenerator
{
    public static RBEReportData GenerateSampleData(string reportingMonth)
    {
        return new RBEReportData
        {
            ReportingMonth = reportingMonth,
            TotalAssetsUnderManagement = 250_000_000,
            NumberOfFunds = 15,
            NumberOfClients = 500,
            TransactionVolume = 10_000_000,
            FundPositions = new List<FundPosition>
            {
                new()
                {
                    FundCode = "LU0000000001",
                    FundName = "Sample Fund A",
                    NetAssetValue = 10_000_000,
                    NumberOfShares = 100_000,
                    ValuationDate = DateTime.UtcNow
                }
            }
        };
    }
}
```

**Endpoint:**
```http
GET /api/v1/sandbox/rbe/sample-data?month=2024-01
```

### Mode Sandbox UI

**Badge visuel:**
```
┌──────────────────────────────────┐
│  🧪 MODE SANDBOX                 │
│  Ce rapport est en mode test     │
│  Aucune soumission réelle        │
└──────────────────────────────────┘
```

**Restrictions:**
- ❌ Pas de soumission réelle aux régulateurs
- ❌ Pas de notifications email
- ✅ Génération XML/JSON autorisée
- ✅ Validation autorisée (mode relaxed)
- ✅ Archivage automatique après 30 jours

---

## Checklist de Développement

### Avant de Commencer

- [ ] Lire ce document entièrement
- [ ] Cloner repository et créer feature branch
- [ ] Configurer environnement de développement
- [ ] Accès au schéma XSD/JSON du régulateur

### Pendant le Développement

**Models:**
- [ ] Créer {ReportType}ReportData.cs
- [ ] Propriétés avec annotations Data
- [ ] Documentation XML sur chaque propriété

**Validation:**
- [ ] Implémenter I{ReportType}ValidationService
- [ ] Codes d'erreur standardisés
- [ ] Messages en français ET anglais
- [ ] Mode sandbox (règles relaxées)
- [ ] Tests unitaires (>80% coverage)

**Generation:**
- [ ] Implémenter I{ReportType}GenerationService
- [ ] Génération selon schéma officiel
- [ ] Validation XSD/JSON avant retour
- [ ] Mode sandbox (marqueur dans contenu)
- [ ] Tests unitaires + validation schéma

**Submission:**
- [ ] Implémenter I{ReportType}SubmissionService
- [ ] HttpClient configuré (retry, timeout)
- [ ] Authentication (API key, OAuth, mTLS)
- [ ] Mode sandbox (pas d'appel réel)
- [ ] Tests avec mocks

**Documentation:**
- [ ] README.md complet
- [ ] CHANGELOG.md
- [ ] Exemples de code
- [ ] Configuration requise

**Tests:**
- [ ] Tests unitaires (validation, génération, soumission)
- [ ] Tests d'intégration (workflow complet sandbox)
- [ ] Coverage >70%
- [ ] Tous les tests passent

### Avant la Pull Request

- [ ] Code formaté (dotnet format)
- [ ] Pas de warnings compilation
- [ ] Tous les tests passent
- [ ] Documentation à jour
- [ ] Branch rebased sur develop
- [ ] Sandbox validé manuellement

### Après Intégration

- [ ] Tests E2E passent
- [ ] Déployé sur staging
- [ ] Validation métier OK
- [ ] Performance acceptable (<2s génération)
- [ ] Monitoring configuré

---

## Support & Questions

**Slack:** `#compliancebot-modules`
**Email:** `dev@compliancebot.lu`
**Documentation:** `/docs/modules/`
**Exemples:** `ComplianceBot.Modules.RBE` (référence)

---

**Bonne chance dans le développement de votre module !** 🚀
