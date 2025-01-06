# Set the working directory to the location of the script's file
Set-Location -Path (Split-Path -Parent $MyInvocation.MyCommand.Definition)

# Ask the user to enter the name of the package
$package_name = Read-Host "Enter the name of the package"

# Ask the user to enter the description of the package
$package_description = Read-Host "Enter the description of the package"

$target_namespace = "LinkSoft.$package_name"
$target_folder = "../src/$target_namespace"

# Copy all files from the templates folder to the target folder
New-Item -ItemType Directory -Force -Path $target_folder
Copy-Item -Recurse -Path ../templates/* -Destination $target_folder

# Rename the file template.csproj to {target_namespace}.csproj
Rename-Item -Path "$target_folder/template.csproj" -NewName "$target_namespace.csproj"

# Replace {FullName} with {target_namespace} in the copied .csproj file
(Get-Content "$target_folder/$target_namespace.csproj") -replace '{FullName}', $target_namespace | Set-Content "$target_folder/$target_namespace.csproj"

# Replace {Description} with the inputted description in the copied .csproj file
(Get-Content "$target_folder/$target_namespace.csproj") -replace '{Description}', $package_description | Set-Content "$target_folder/$target_namespace.csproj"

# Create the folder structure in the same folder as the .csproj
New-Item -ItemType Directory -Force -Path "$target_folder/LinkSoft/$package_name"

# Add the newly created .csproj file to the solution file
& dotnet sln ../LinkSoft.sln add "$target_folder/$target_namespace.csproj"

Write-Host "Package setup completed successfully."