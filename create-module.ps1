param (
    [Parameter(Mandatory=$true)]
    [string]$ModuleName
)

$ErrorActionPreference = "Stop"

$rootPath = Get-Location
$moduleBaseDir = "Modules\$ModuleName"

Write-Host "Creating Module: $ModuleName in $moduleBaseDir" -ForegroundColor Cyan

# Create Directories
$subdirs = @("Domain", "Application", "Infrastructure", "Api", "Tests")
foreach ($subdir in $subdirs) {
    $path = Join-Path $moduleBaseDir $subdir
    if (-not (Test-Path $path)) {
        New-Item -ItemType Directory -Path $path | Out-Null
    }
}

# 1. Domain Project
$domainProjName = "Modules.$ModuleName.Domain"
$domainProjFile = Join-Path $moduleBaseDir "Domain\$domainProjName.csproj"
$domainContent = @"
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\..\..\BuildingBlocks\Domain\BuildingBlocks.Domain.csproj" />
  </ItemGroup>
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
"@
Set-Content -Path $domainProjFile -Value $domainContent

# 2. Application Project
$appProjName = "Modules.$ModuleName.Application"
$appProjFile = Join-Path $moduleBaseDir "Application\$appProjName.csproj"
$appContent = @"
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\..\..\BuildingBlocks\Infrastructure\BuildingBlocks.Infrastructure.csproj" />
    <ProjectReference Include="..\Domain\$domainProjName.csproj" />
    <ProjectReference Include="..\..\..\BuildingBlocks\Application\BuildingBlocks.Application.csproj" />
    <PackageReference Include="MediatR" Version="14.0.0" />
    <PackageReference Include="FluentValidation" Version="12.1.1" />
  </ItemGroup>
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
"@
Set-Content -Path $appProjFile -Value $appContent

# 3. Infrastructure Project
$infraProjName = "Modules.$ModuleName.Infrastructure"
$infraProjFile = Join-Path $moduleBaseDir "Infrastructure\$infraProjName.csproj"
$infraContent = @"
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\Application\$appProjName.csproj" />
    <ProjectReference Include="..\Domain\$domainProjName.csproj" />
    <ProjectReference Include="..\..\..\BuildingBlocks\Infrastructure\BuildingBlocks.Infrastructure.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.0" />
    <PackageReference Include="Microsoft.Extensions.Options" Version="9.0.0" />
  </ItemGroup>
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
"@
Set-Content -Path $infraProjFile -Value $infraContent

# 4. Api Project
$apiProjName = "Modules.$ModuleName.Api"
$apiProjFile = Join-Path $moduleBaseDir "Api\$apiProjName.csproj"
$apiContent = @"
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\Application\$appProjName.csproj" />
  </ItemGroup>
  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
  </ItemGroup>
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
"@
Set-Content -Path $apiProjFile -Value $apiContent

# 5. Tests Project
$testsProjName = "Modules.$ModuleName.Tests"
$testsProjFile = Join-Path $moduleBaseDir "Tests\$testsProjName.csproj"
$testsContent = @"
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\Application\$appProjName.csproj" />
    <ProjectReference Include="..\Domain\$domainProjName.csproj" />
    <ProjectReference Include="..\Infrastructure\$infraProjName.csproj" />
  </ItemGroup>
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
</Project>
"@
Set-Content -Path $testsProjFile -Value $testsContent

# Add to Solution
Write-Host "Adding projects to solution..." -ForegroundColor Yellow
dotnet sln add $domainProjFile
dotnet sln add $appProjFile
dotnet sln add $infraProjFile
dotnet sln add $apiProjFile
dotnet sln add $testsProjFile

Write-Host "Module $ModuleName created successfully!" -ForegroundColor Green
