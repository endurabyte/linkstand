. .\make.ps1 release

try {
  pushd dist
  python -m http.server
}
catch {
  throw;
}
finally {
  popd
}
