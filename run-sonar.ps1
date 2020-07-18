# instructions from: https://docs.sonarqube.org/latest/analysis/scan/sonarscanner-for-msbuild/
# as of writing, install the dotnet-sonarscanner tool first:
# dotnet tool install --global dotnet-sonarscanner --version 4.8.0
# coverlet: dotnet tool install --global coverlet.console

dotnet sonarscanner begin /key:"Puffin" /d:sonar.cs.opencover.reportsPaths=Puffin.Core.UnitTests/coverage.opencover.xml,Puffin.UI.UnitTests/coverage.opencover.xml /d:sonar.coverage.exclusions="**Test*.cs"
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
dotnet sonarscanner end

