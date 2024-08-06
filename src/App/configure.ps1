$config = $args[0]

if ($config -eq "release") {
  cp "config/Log.release.elm" "Log.elm"
  cp "config/config.release.json" "config.json"
}
else {
  cp "config/Log.debug.elm" "Log.elm"
  cp "config/config.debug.json" "config.json"
}
