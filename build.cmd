SET PATH=%PATH%;C:\Program Files (x86)\MSBuild\14.0\Bin

rd /Q .\output
mkdir .\output

msbuild .\src\IisExpressTestKit.sln /p:Configuration=Release
.\tools\nuget.exe pack .\src\IisExpressTestKit\IisExpressTestKit.csproj -Prop Configuration=Release -BasePath .\src -OutputDirectory .\output