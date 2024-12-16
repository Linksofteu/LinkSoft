# Ask the user to enter the type of package
$package_type = Read-Host "Enter the type of package ((A)bp or (S)hared)"

# Validate the input
if ($package_type -ne "A" -and $package_type -ne "S") {
    Write-Host "Invalid package type. Please enter either 'A' for Abp or 'S' for Shared."
    exit 1
}

# Ask the user to enter the name of the package
$package_name = Read-Host "Enter the name of the package"

# Ask the user to enter the description of the package
$package_description = Read-Host "Enter the description of the package"

# Determine the target namespace and folder based on the package type
if ($package_type -eq "A") {
    $target_namespace = "LSoftTech.Abp.$package_name"
    $target_folder = "../src/Abp/$target_namespace"
} else {
    $target_namespace = "LSoftTech.$package_name"
    $target_folder = "../src/Shared/$target_namespace"
}

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
if ($package_type -eq "A") {
    New-Item -ItemType Directory -Force -Path "$target_folder/LSoftTech/Abp/$package_name"
} else {
    New-Item -ItemType Directory -Force -Path "$target_folder/LSoftTech/$package_name"
}

Write-Host "Package setup completed successfully."