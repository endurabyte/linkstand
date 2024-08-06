module Page.Manage exposing
    ( Model
    , Msg
    , init
    , update
    , view
    )

import Browser
import Browser.Navigation as Nav
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import Http
import Json.Decode as Decode exposing (string)
import Log
import Url exposing (Url)
import Url.Parser exposing (Parser, map)
import Url.Parser.Query



-- MODEL


type alias Model =
    { host : String
    , linkId : String
    , clickCount : Maybe Int
    , events : List Event
    , errorMsg : Maybe String
    }


type alias Event =
    { id : String
    , aliasId : String
    , ip : String
    , timestamp : String
    }



-- JSON DECODERS


clickCountDecoder : Decode.Decoder Int
clickCountDecoder =
    Decode.field "clicks" Decode.int


eventDecoder : Decode.Decoder Event
eventDecoder =
    Decode.map4 Event
        (Decode.field "id" string)
        (Decode.field "aliasId" string)
        (Decode.field "ip" string)
        (Decode.field "timestamp" string)


init : String -> Url -> Nav.Key -> ( Model, Cmd Msg )
init host url key =
    ( { host = host
      , linkId =
            case extractSearchArgument "id" url of
                Just linkId ->
                    linkId

                Nothing ->
                    ""
      , clickCount = Nothing
      , events = []
      , errorMsg = Nothing
      }
    , Cmd.none
    )



-- UPDATE


type Msg
    = UpdateLinkId String
    | FetchStats
    | ReceiveClickCount (Result Http.Error Int)
    | ReceiveEvents (Result Http.Error (List Event))
    | UrlChanged Url
    | LinkClicked Browser.UrlRequest
    | NoOp


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    let
        _ = Log.log "Manage.update: msg = " msg
        _ = Log.log "Manage.update: model = " model
    in
    case msg of
        UpdateLinkId newLinkId ->
            ( { model | linkId = newLinkId, errorMsg = Nothing }, Cmd.none )

        FetchStats ->
            ( model, Cmd.batch [ fetchClickCount model.host model.linkId, fetchEvents model.host model.linkId ] )

        ReceiveClickCount result ->
            case result of
                Ok count ->
                    ( { model | clickCount = Just count, errorMsg = Nothing }, Cmd.none )

                Err _ ->
                    ( { model | errorMsg = Just "Failed to get click count." }, Cmd.none )

        ReceiveEvents result ->
            case result of
                Ok events ->
                    ( { model | events = events, errorMsg = Nothing }, Cmd.none )

                Err _ ->
                    ( { model | errorMsg = Just "Failed to get events." }, Cmd.none )

        UrlChanged url ->
              case extractSearchArgument "id" url of
                  Just linkId ->
                      ( { model | linkId = linkId }, Cmd.none )

                  Nothing ->
                      ( model, Cmd.none )

        LinkClicked _ ->
            ( model, Cmd.none )

        NoOp ->
            ( model, Cmd.none )


extractSearchArgument : String -> Url -> Maybe String
extractSearchArgument key location =
    { location | path = "" }
        |> Url.Parser.parse (Url.Parser.query (Url.Parser.Query.string key))
        |> Maybe.withDefault Nothing



-- VIEW


view : Model -> Browser.Document Msg
view model =
    { title = "LinkStand | Manage"
    , body =
        [ div [ class "container" ]
            [ h1 [] [ text "Link Stats" ]
            , input [ type_ "text", placeholder "Enter Link ID", value model.linkId, onInput UpdateLinkId ] []
            , button [ onClick FetchStats ] [ text "Get Stats" ]
            , case model.clickCount of
                Just count ->
                    p [] [ text ("Click count: " ++ String.fromInt count) ]

                Nothing ->
                    text ""
            , if not (List.isEmpty model.events) then
                table []
                    [ tr [] [ th [] [ text "ID" ], th [] [ text "IP" ], th [] [ text "Timestamp" ] ]
                    , List.map eventRow model.events |> Html.ul []
                    ]

              else
                text ""
            , case model.errorMsg of
                Just msg ->
                    p [ class "error" ] [ text msg ]

                Nothing ->
                    text ""
            , a [ class "d-block mt-4", href "/" ] [ text "Back" ]
            ]
        ]
    }


eventRow : Event -> Html Msg
eventRow event =
    tr []
        [ td [] [ text event.id ]
        , td [] [ text event.ip ]
        , td [] [ text event.timestamp ]
        ]



-- HTTP REQUEST


fetchClickCount : String -> String -> Cmd Msg
fetchClickCount host linkId =
        Http.get
        { url = host ++ "clicks?id=" ++ linkId
        , expect = Http.expectJson ReceiveClickCount clickCountDecoder
        }


fetchEvents : String -> String -> Cmd Msg
fetchEvents host linkId =
    Http.get
        { url = host ++ "events?id=" ++ linkId
        , expect = Http.expectJson ReceiveEvents (Decode.field "events" (Decode.list eventDecoder))
        }
