module Log exposing (log)

import Debug


log : String -> a -> a
log message value =
    value
