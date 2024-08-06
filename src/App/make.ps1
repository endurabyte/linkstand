$config = $args[0]

# Stop script on any error
$ErrorActionPreference = "Stop"
$compression = "pure_funcs=[F2,F3,F4,F5,F6,F7,F8,F9,A2,A3,A4,A5,A6,A7,A8,A9],pure_getters,keep_fargs=false,unsafe_comps,unsafe"
$elmFile = "Main.elm"
$htmlFile = "index.html"
$js = "index.js"
$distJs = "dist/index.js"
$distHtml = "dist/index.html"

. .\configure.ps1 $config

# Create the dist directory
Remove-Item -Recurse -Force -ErrorAction SilentlyContinue ./dist/*
New-Item -ItemType Directory -Path ./dist -Force | Out-Null
New-Item -ItemType Directory -Path ./dist/css -Force | Out-Null

# Copy static files
cp $htmlFile $distHtml

if ($config -eq "release") {
  echo "Building $elmFile in release..."

  # Install dependencies
  if ((Get-Command "uglifyjs" -ErrorAction SilentlyContinue) -eq $null) {
    echo "Installing uglifyjs"
    npm install -g uglify-js
  }

  if ((Get-Command "uglifycss" -ErrorAction SilentlyContinue) -eq $null) {
    echo "Installing uglifycss"
    npm install -g uglifycss
  }

  elm make $elmFile --optimize --output=$js

  uglifyjs $js --compress $compression | uglifyjs --mangle --output $distJs
  uglifycss "css/styles.css" --output "dist/css/styles.css"

  $compiledSize = (Get-Item $js).Length
  $minifiedSize = (Get-Item $distJs).Length

  Write-Output "Compiled size: $compiledSize bytes ($js)"
  Write-Output "Minified size: $minifiedSize bytes ($min)"
}
else {
  echo "Building $elmFile in debug..."

  elm make $elmFile --output=$js
  cp $js $distJs
  cp "css/styles.css" "dist/css/styles.css"
}
