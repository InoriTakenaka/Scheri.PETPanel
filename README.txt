Command to publish win-64:
dotnet publish -c Release -r win-64 /p:PublishAot=true /p:PublishTrimmed=true /p:TrimMode=full /p:debugType=none /p:DebugSymbols=false /p:ExcludeX86Vlc=true