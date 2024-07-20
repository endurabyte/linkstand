module Main exposing (..)

import Browser
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import Http
import Json.Decode as Decode exposing (string)

--host = "http://localhost:8080/"
--host = "https://linkstand.fly.dev/"
host = "https://api.linkstand.net/"

-- MAIN

main =
    Browser.element { init = init, update = update, view = view, subscriptions = subscriptions }


-- MODEL

type alias Model =
    { urlInput : String
    , aliasUrl : String
    , aliasType : String
    , clickCount : Maybe Int
    , isResultVisible : Bool
    , errorMsg : Maybe String
    }

type alias AliasResponse =
    { alias : String }

-- JSON DECODERS

aliasDecoder : Decode.Decoder String
aliasDecoder =
  Decode.field "id" Decode.string

clickCountDecoder : Decode.Decoder Int
clickCountDecoder =
  Decode.field "clicks" Decode.int

init : () -> ( Model, Cmd Msg )
init _ =
    ( { urlInput = ""
      , aliasUrl = ""
      , aliasType = "none" -- short, memorable, none
      , clickCount = Nothing
      , isResultVisible = False
      , errorMsg = Nothing
      }
    , Cmd.none
    )


-- UPDATE

type Msg
    = UpdateUrlInput String
    | UpdateAliasType String
    | Submit
    | ReceiveResponse (Result Http.Error String)
    | FetchClickCount
    | ReceiveClickCount (Result Http.Error Int)

update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        UpdateUrlInput newUrl ->
            ( { model | urlInput = newUrl, errorMsg = Nothing }, Cmd.none )

        UpdateAliasType newType ->
            ( { model | aliasType = newType, errorMsg = Nothing }, Cmd.none )

        Submit ->
          ( model, submitUrl model.urlInput model.aliasType )

        ReceiveResponse result ->
            case result of
                Ok alias ->
                    ( { model | aliasUrl = alias, isResultVisible = True, errorMsg = Nothing}, Cmd.none )

                Err (Http.BadStatus 409) ->
                    ( { model | errorMsg = Just "Link already exists" }, Cmd.none )

                Err _ ->
                    ( { model | errorMsg = Just "Failed to create link." }, Cmd.none )

        FetchClickCount ->
            ( model, fetchClickCount model.aliasUrl )

        ReceiveClickCount result ->
            case result of
                Ok clicks ->
                    ( { model | clickCount = Just clicks, errorMsg = Nothing }, Cmd.none )

                Err _ ->
                    ( { model | errorMsg = Just "Failed to get click count." }, Cmd.none )


-- VIEW

view : Model -> Html Msg
view model =
    div [ class "container" ]
        [ h1 [] [ text "Link Creator" ]
        , Html.form [ onSubmit Submit ]
            [ input 
              [ type_ "text"
              , placeholder "https://www.example.com/"
              , required True
              , pattern "(https?:\\/\\/)?(www\\.)?[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}(/\\S*)?"
              , value model.urlInput
              , onInput UpdateUrlInput 
              ]
              []
            , label [ for "aliasType" ] [ text "Type (optional):" ]
            , select [ onInput UpdateAliasType ]
                [ option [ value "none" ] [ text "None" ]
                , option [ value "short" ] [ text "Short" ]
                , option [ value "memorable" ] [ text "Memorable" ]
                ]
            , button [ type_ "submit" ] [ text "Get Link" ]
            ]

        , if model.isResultVisible then
            div [ id "result" ]
                [ p [] [ text "Your Link:" ]
                , a [ href (host ++ model.aliasUrl), target "_blank", rel "noopener noreferrer" ] [ text model.aliasUrl ]
                , p [] []
                , button [ onClick FetchClickCount ] [ text "Get Click Count" ]
                , case model.clickCount of
                    Just count ->
                        p [] [ text ("Click count: " ++ String.fromInt count) ]

                    Nothing ->
                        text ""
                ]
          else
            text ""
        , case model.errorMsg of
            Just msg ->
                p [ class "error" ] [ text msg ]
            Nothing ->
                text ""
        ]


-- HTTP REQUEST

submitUrl : String -> String -> Cmd Msg
submitUrl url aliasType =
    let
        urlToFetch = host ++ "?url=" ++ url ++ "&type=" ++ aliasType
    in
    Http.post
        { url = urlToFetch
        , body = Http.emptyBody
        , expect = Http.expectJson ReceiveResponse aliasDecoder
        }

fetchClickCount : String -> Cmd Msg
fetchClickCount aliasId =
    Http.get
        { url = host ++ "clicks?id=" ++ aliasId
        , expect = Http.expectJson ReceiveClickCount clickCountDecoder
        }

-- SUBSCRIPTIONS

subscriptions : Model -> Sub Msg
subscriptions _ =
    Sub.none
