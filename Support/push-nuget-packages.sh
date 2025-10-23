#!/bin/bash

# Push NuGet packages to nuget.org
# Usage: ./push-nuget-packages.sh <api-key>

set -e

if [ -z "$1" ]; then
    echo "Error: API key parameter is required"
    echo "Usage: $0 <api-key>"
    exit 1
fi

API_KEY="$1"
PACKAGES_DIR="$(dirname "$0")/../bin/packages"

if [ ! -d "$PACKAGES_DIR" ]; then
    echo "Error: Packages directory not found: $PACKAGES_DIR"
    exit 1
fi

NUPKG_FILES=("$PACKAGES_DIR"/*.nupkg)

if [ ! -e "${NUPKG_FILES[0]}" ]; then
    echo "Error: No .nupkg files found in $PACKAGES_DIR"
    exit 1
fi

echo "Found $(find "$PACKAGES_DIR" -name "*.nupkg" | wc -l) NuGet package(s) to push..."

for package in "$PACKAGES_DIR"/*.nupkg; do
    if [ -f "$package" ]; then
        echo "Pushing $package..."
        dotnet nuget push "$package" --api-key "$API_KEY" --source https://api.nuget.org/v3/index.json
        echo "✓ Successfully pushed $(basename "$package")"
    fi
done

echo "✓ All packages pushed successfully!"
