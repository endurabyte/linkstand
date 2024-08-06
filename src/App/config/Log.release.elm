module Log exposing (log)

log : String -> a -> a
log message value =
    value
