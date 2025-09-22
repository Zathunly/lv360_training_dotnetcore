for proj in src/*/*.csproj; do
  echo ">>> $proj"
  dotnet list "$proj" reference
  echo
done
