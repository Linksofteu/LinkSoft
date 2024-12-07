# Navigate to the source directory
Set-Location -Path "src" -ErrorAction Stop

# Find and build all .NET projects (assuming each project has a .csproj file)
Get-ChildItem -Recurse -Filter *.csproj | ForEach-Object {
    $dir = $_.DirectoryName
    Write-Output "Building .NET project in $dir"
    Set-Location -Path $dir
    dotnet build
    Set-Location -Path "src"
}