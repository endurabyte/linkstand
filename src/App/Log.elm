module Log exposing (log)

import Debug


isEnabled : Bool
isEnabled =
    True


log : String -> a -> a
log message value =
    if isEnabled then
        value
        -- Uncomment if you need logging
        -- Debug.log message value

    else
        value
