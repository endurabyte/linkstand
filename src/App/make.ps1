$config = $args[0]

# Stop script on any error
$ErrorActionPreference = "Stop"

# Install dependencies
npm install -g uglify-js uglifycss
$compression = "pure_funcs=[F2,F3,F4,F5,F6,F7,F8,F9,A2,A3,A4,A5,A6,A7,A8,A9],pure_getters,keep_fargs=false,unsafe_comps,unsafe"
. .\configure.ps1 $config

# Create the dist directory
Remove-Item -Recurse -Force -ErrorAction SilentlyContinue ./dist/*
New-Item -ItemType Directory -Path ./dist -Force | Out-Null
New-Item -ItemType Directory -Path ./dist/css -Force | Out-Null

# Minify CSS
uglifycss "css/styles.css" --output "dist/css/styles.css"

$elmFile = "Main.elm"
$htmlFile = "index.html"
$js = "index.js"
$distJs = "dist/index.js"
$distHtml = "dist/index.html"

# Copy the HTML file
cp $htmlFile $distHtml

echo "Building $elmFile..."

# Compile Elm code with optimization
if ($config -eq "release") {
  elm make $elmFile --optimize --output=$js

  # Minify the JavaScript using UglifyJS
  uglifyjs $js --compress $compression | uglifyjs --mangle --output $distJs

  # Get file sizes
  $compiledSize = (Get-Item $js).Length
  $minifiedSize = (Get-Item $distJs).Length

  # Output file sizes
  Write-Output "Compiled size: $compiledSize bytes ($js)"
  Write-Output "Minified size: $minifiedSize bytes ($min)"
}
else {
  elm make $elmFile --output=$js
  cp $js $distJs
}
