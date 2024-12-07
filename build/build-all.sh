#!/bin/bash

# Navigate to the source directory
cd src || { echo "Source directory not found"; exit 1; }

# Find and build all .NET projects (assuming each project has a .csproj file)
find . -name "*.csproj" | while read -r csproj; do
  dir=$(dirname "$csproj")
  echo "Building .NET project in $dir"
  (cd "$dir" && dotnet build)
done
