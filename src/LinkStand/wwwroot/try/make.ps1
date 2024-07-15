# Stop script on any error
$ErrorActionPreference = "Stop"

$js = "elm.js"
$min = "elm.min.js"

# Compile Elm code with optimization
elm make .\src\Main.elm --optimize --output=$js $args

# Minify the JavaScript using UglifyJS
# npm install -g uglify-js
uglifyjs $js --compress 'pure_funcs=[F2,F3,F4,F5,F6,F7,F8,F9,A2,A3,A4,A5,A6,A7,A8,A9],pure_getters,keep_fargs=false,unsafe_comps,unsafe' | uglifyjs --mangle --output $min

# Get file sizes
$compiledSize = (Get-Item $js).Length
$minifiedSize = (Get-Item $min).Length

# Output file sizes
Write-Output "Compiled size: $compiledSize bytes ($js)"
Write-Output "Minified size: $minifiedSize bytes ($min)"
