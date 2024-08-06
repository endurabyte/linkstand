module Page.About exposing
  ( Model
  , init
  , Msg
  , update
  , view
  )

import Browser
import Html exposing (..)
import Html.Attributes exposing (..)


-- MODEL

type Status
  = Ok

type alias Model =
  { status : Status }

init : ( Model, Cmd msg )
init  =
  ( Model Ok, Cmd.none )


-- UPDATE

type Msg
  = Loaded

update : Msg -> Model -> ( Model, Cmd msg )
update msg model =
  case msg of
    Loaded ->
      ( model, Cmd.none )


-- VIEW

view : Model -> Browser.Document msg
view model =
  --Debug.log "Page.About.view"
  { title = "LinkStand | About"
  , body = 
    [ div [ class "container" ]
      [ h1 [] [ text "About" ]
      , p [] [ text "LinkStand is a web service to share URLs and track their use." ]
      , a [ href "https://github.com/endurabyte/linkstand/" ] [ text "Source Code" ]
      , a [ class "d-block mt-4", href "/" ] [ text "Back" ]
      ]
    ]
  }
