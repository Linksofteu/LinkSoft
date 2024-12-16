#!/bin/bash

# Ask the user to enter the type of package
echo "Enter the type of package (A)bp or (S)hared:"
read package_type

# Validate the input
if [[ "$package_type" != "A" && "$package_type" != "S" ]]; then
  echo "Invalid package type. Please enter either 'A' for Abp or 'S' for Shared."
  exit 1
fi

# Ask the user to enter the name of the package
echo "Enter the name of the package:"
read package_name

# Ask the user to enter the description of the package
echo "Enter the description of the package:"
read package_description

# Determine the target namespace and folder based on the package type
if [[ "$package_type" == "A" ]]; then
  target_namespace="LSoftTech.Abp.$package_name"
  target_folder="../src/Abp/$target_namespace"
else
  target_namespace="LSoftTech.$package_name"
  target_folder="../src/Shared/$target_namespace"
fi

# Copy all files from the templates folder to the target folder
mkdir -p "$target_folder"
cp -r ../templates/* "$target_folder"

# Rename the file template.csproj to {target_namespace}.csproj
mv "$target_folder/template.csproj" "$target_folder/$target_namespace.csproj"

# Replace {FullName} with {target_namespace} in the copied .csproj file
sed -i "s/{FullName}/$target_namespace/g" "$target_folder/$target_namespace.csproj"

# Replace {Description} with the inputted description in the copied .csproj file
sed -i "s/{Description}/$package_description/g" "$target_folder/$target_namespace.csproj"

# Create the folder structure in the same folder as the .csproj
if [[ "$package_type" == "A" ]]; then
  mkdir -p "$target_folder/LSoftTech/Abp/$package_name"
else
  mkdir -p "$target_folder/LSoftTech/$package_name"
fi

echo "Package setup completed successfully."