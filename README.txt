Command to publish win-64:
1. cd to \Scheri.PETPanel.Desktop
2. run command:
   dotnet publish -c Release -r win-64 /p:PublishAot=true /p:PublishTrimmed=true /p:TrimMode=full /p:debugType=none /p:DebugSymbols=false /p:ExcludeX86Vlc=true
