DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
TOP="${DIR}/.."

dotnet pack "${TOP}/src/dotnet-versioninfo.csproj" --output "${TOP}/dist"
dotnet tool update --global dotnet-versioninfo --add-source "${TOP}/dist"
