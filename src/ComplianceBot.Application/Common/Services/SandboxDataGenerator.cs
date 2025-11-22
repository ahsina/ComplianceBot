using System.Text.Json;
using ComplianceBot.Domain.Enums;

namespace ComplianceBot.Application.Common.Services;

/// <summary>
/// Service for generating sandbox test data for different report types
/// </summary>
public class SandboxDataGenerator : ISandboxDataGenerator
{
    public Task<string> GenerateSampleDataAsync(ReportType reportType, Guid tenantId)
    {
        var data = reportType switch
        {
            ReportType.RBE => GenerateRBESampleData(tenantId),
            ReportType.CEDR => GenerateCEDRSampleData(tenantId),
            ReportType.FATCA => GenerateFATCASampleData(tenantId),
            ReportType.CRS => GenerateCRSSampleData(tenantId),
            ReportType.SFDR => GenerateSFDRSampleData(tenantId),
            _ => throw new NotSupportedException($"Report type {reportType} is not supported for sandbox data generation")
        };

        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return Task.FromResult(json);
    }

    public bool IsSupported(ReportType reportType)
    {
        return reportType switch
        {
            ReportType.RBE => true,
            ReportType.CEDR => true,
            ReportType.FATCA => true,
            ReportType.CRS => true,
            ReportType.SFDR => true,
            _ => false
        };
    }

    private object GenerateRBESampleData(Guid tenantId)
    {
        return new
        {
            ReportingMonth = DateTime.UtcNow.AddMonths(-1).ToString("yyyy-MM"),
            FundName = "Sample Luxembourg Fund SICAV",
            FundIdentifier = "LU1234567890",
            TotalAssetsUnderManagement = 150000000.00m,
            NetAssetValue = 148500000.00m,
            NumberOfSharesOutstanding = 1485000,
            SharePrice = 100.00m,
            Transactions = new[]
            {
                new
                {
                    TransactionDate = DateTime.UtcNow.AddMonths(-1).AddDays(-20).ToString("yyyy-MM-dd"),
                    TransactionType = "Subscription",
                    Amount = 500000.00m,
                    NumberOfShares = 5000,
                    InvestorType = "Institutional"
                },
                new
                {
                    TransactionDate = DateTime.UtcNow.AddMonths(-1).AddDays(-15).ToString("yyyy-MM-dd"),
                    TransactionType = "Redemption",
                    Amount = 250000.00m,
                    NumberOfShares = 2500,
                    InvestorType = "Retail"
                }
            },
            AssetAllocation = new
            {
                Equities = 60.0m,
                Bonds = 30.0m,
                Cash = 10.0m
            },
            GeographicExposure = new
            {
                Europe = 50.0m,
                NorthAmerica = 30.0m,
                Asia = 15.0m,
                Other = 5.0m
            }
        };
    }

    private object GenerateCEDRSampleData(Guid tenantId)
    {
        return new
        {
            ReportingPeriod = DateTime.UtcNow.AddMonths(-1).ToString("yyyy-MM"),
            EntityName = "Sample Credit Institution",
            EntityIdentifier = "LU9876543210",
            TotalLoans = 500000000.00m,
            NonPerformingLoans = 5000000.00m,
            NPLRatio = 1.0m,
            LoansByCategory = new[]
            {
                new
                {
                    Category = "Corporate",
                    Amount = 300000000.00m,
                    NumberOfLoans = 150
                },
                new
                {
                    Category = "Retail",
                    Amount = 150000000.00m,
                    NumberOfLoans = 500
                },
                new
                {
                    Category = "SME",
                    Amount = 50000000.00m,
                    NumberOfLoans = 100
                }
            ],
            Provisions = new
            {
                SpecificProvisions = 3000000.00m,
                GeneralProvisions = 2000000.00m
            }
        };
    }

    private object GenerateFATCASampleData(Guid tenantId)
    {
        return new
        {
            ReportingYear = DateTime.UtcNow.AddYears(-1).Year,
            FFIName = "Sample Foreign Financial Institution",
            GIIN = "ABC123.12345.LE.123",
            ReportableAccounts = new[]
            {
                new
                {
                    AccountNumber = "ACC-US-001",
                    AccountHolderName = "John Doe",
                    AccountHolderTIN = "123-45-6789",
                    AccountHolderType = "Individual",
                    AccountBalance = 150000.00m,
                    PaymentAmounts = new
                    {
                        Dividends = 5000.00m,
                        Interest = 2000.00m,
                        GrossProceeds = 0.00m
                    }
                },
                new
                {
                    AccountNumber = "ACC-US-002",
                    AccountHolderName = "Jane Smith",
                    AccountHolderTIN = "987-65-4321",
                    AccountHolderType = "Individual",
                    AccountBalance = 250000.00m,
                    PaymentAmounts = new
                    {
                        Dividends = 8000.00m,
                        Interest = 3500.00m,
                        GrossProceeds = 15000.00m
                    }
                }
            },
            TotalReportableAccounts = 2,
            TotalAccountValue = 400000.00m
        };
    }

    private object GenerateCRSSampleData(Guid tenantId)
    {
        return new
        {
            ReportingYear = DateTime.UtcNow.AddYears(-1).Year,
            ReportingFI = new
            {
                Name = "Sample Reporting Financial Institution",
                TIN = "LU12345678",
                ResCountryCode = "LU"
            },
            ReportableAccounts = new[]
            {
                new
                {
                    AccountNumber = "ACC-FR-001",
                    AccountHolderName = "Pierre Dupont",
                    AccountHolderTIN = "FR123456789",
                    ResidenceCountry = "FR",
                    AccountHolderType = "Individual",
                    AccountBalance = 180000.00m,
                    PaymentTypes = new
                    {
                        Interest = 3000.00m,
                        Dividends = 6000.00m
                    }
                },
                new
                {
                    AccountNumber = "ACC-DE-001",
                    AccountHolderName = "Hans Mueller",
                    AccountHolderTIN = "DE987654321",
                    ResidenceCountry = "DE",
                    AccountHolderType = "Individual",
                    AccountBalance = 220000.00m,
                    PaymentTypes = new
                    {
                        Interest = 4000.00m,
                        Dividends = 8000.00m
                    }
                }
            },
            TotalReportableJurisdictions = 2,
            TotalReportableAccounts = 2
        };
    }

    private object GenerateSFDRSampleData(Guid tenantId)
    {
        return new
        {
            ReportingPeriod = DateTime.UtcNow.AddMonths(-3).ToString("yyyy-MM"),
            ProductName = "Sample ESG Sustainable Fund",
            ProductIdentifier = "LU5555555555",
            ArticleClassification = "Article 9",
            SustainabilityIndicators = new
            {
                EnvironmentalObjectives = new[]
                {
                    new
                    {
                        Objective = "Climate Change Mitigation",
                        Percentage = 40.0m,
                        Description = "Investments in renewable energy and energy efficiency"
                    },
                    new
                    {
                        Objective = "Transition to Circular Economy",
                        Percentage = 20.0m,
                        Description = "Investments in waste reduction and recycling"
                    }
                },
                SocialObjectives = new[]
                {
                    new
                    {
                        Objective = "Adequate living standards",
                        Percentage = 15.0m,
                        Description = "Investments promoting affordable housing"
                    }
                }
            },
            PAIIndicators = new
            {
                GHGEmissions = 125.5m, // tons CO2e per million EUR invested
                CarbonFootprint = 98.2m,
                GHGIntensity = 215.3m,
                FossilFuelExposure = 5.0m, // percentage
                NonRenewableEnergy = 8.5m, // percentage
                EnergyIntensity = 0.15m
            },
            TaxonomyAlignment = new
            {
                EnvironmentallyAligned = 35.0m, // percentage
                EligibleButNotAligned = 25.0m,
                NotEligible = 40.0m
            },
            DoNoSignificantHarm = new
            {
                AssessmentPerformed = true,
                MitigationMeasures = "Strict exclusion policy for controversial sectors"
            }
        };
    }
}
