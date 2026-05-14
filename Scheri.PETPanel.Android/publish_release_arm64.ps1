Set-Location $PSScriptRoot

Write-Host "Publish: Avalonia Android Target Platform (AOT + Trim)..." -ForegroundColor Cyan

dotnet publish -c Release -f net10.0-android36.0 `
  /p:AndroidPackageFormat=apk `
  /p:PublishTrimmed=true `
  /p:TrimMode=link `
  /p:RunAOTCompilation=true `
  /p:AndroidZipAlign=true `
  /p:AndroidKeyStore=true `
  /p:AndroidSigningKeyStore="Asahina.keystore" `
  /p:AndroidSigningKeyAlias="asahina" `
  /p:AndroidSigningKeyPass="123456" `
  /p:AndroidSigningStorePass="123456"

Write-Host "PUBLISH FINISHED,go get -Signed.apk under path: bin/Release/net10.0-android36.0/publish/ " -ForegroundColor Green
