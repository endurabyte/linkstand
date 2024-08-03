# Stop script on any error
$ErrorActionPreference = "Stop"

# Install dependencies
npm install -g uglify-js uglifycss
$compression = "pure_funcs=[F2,F3,F4,F5,F6,F7,F8,F9,A2,A3,A4,A5,A6,A7,A8,A9],pure_getters,keep_fargs=false,unsafe_comps,unsafe"

# Create the dist directory
Remove-Item -Recurse -Force ./dist/*
New-Item -ItemType Directory -Path ./dist -Force
New-Item -ItemType Directory -Path ./dist/css -Force

# Minify CSS
uglifycss "css/styles.css" --output "dist/css/styles.css"

# Process each subdirectory in the src directory
Get-ChildItem -Path "src" -Directory | ForEach-Object {
    $name = $_.Name
    $nameLower = $name.ToLower()

    echo "Building $name..."
    pushd "src/$name"

    $elmFile = "Main.elm"
    $htmlFile = "$nameLower.html"
    $js = "../../tmp/$nameLower.js"
    $min = "../../dist/$nameLower.min.js"
    $distHtml = "../../dist/$nameLower.html"

    # Compile Elm code with optimization
    #elm make $elmFile --optimize --output=$js $args
    elm make $elmFile --output=$js $args

    # Minify the JavaScript using UglifyJS
    uglifyjs $js --compress $compression | uglifyjs --mangle --output $min

    # Get file sizes
    $compiledSize = (Get-Item $js).Length
    $minifiedSize = (Get-Item $min).Length

    # Output file sizes
    Write-Output "Compiled size: $compiledSize bytes ($js)"
    Write-Output "Minified size: $minifiedSize bytes ($min)"

    # Copy the HTML file
    cp $htmlFile $distHtml

    popd
}
