$config = $args[0]

if ($config -eq "release") {
  cp "config/Log.release.elm" "Log.elm"
}
else {
  cp "config/Log.debug.elm" "Log.elm"
}
