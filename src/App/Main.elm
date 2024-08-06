module Main exposing (main)

import Browser
import Browser.Navigation as Nav
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events
import Http
import Json.Decode as Decode
import Log
import Page.About
import Page.Manage
import Url
import Url.Parser as Parser exposing ((</>), (<?>), Parser, oneOf, s, top)
import Url.Parser.Query as Query



-- MAIN


main : Program Decode.Value Model Msg
main =
    Browser.application
        { init = init
        , update = update
        , view = view
        , subscriptions = \_ -> Sub.none
        , onUrlChange = UrlChanged
        , onUrlRequest = LinkClicked
        }



-- MODEL


type alias Model =
    { config : Config
    , urlInput : String
    , aliasUrl : String
    , aliasType : String
    , isResultVisible : Bool
    , errorMsg : Maybe String
    , key : Nav.Key
    , url : Url.Url
    , page : Page
    }

type alias Config = 
  { host : String }

type alias AliasResponse =
    { alias : String }


type Page
    = Main
    | About Page.About.Model
    | Manage Page.Manage.Model

-- JSON DECODERS


aliasDecoder : Decode.Decoder String
aliasDecoder =
    Decode.field "id" Decode.string

configDecoder : Decode.Decoder Config
configDecoder =
    Decode.map Config
        (Decode.field "host" Decode.string)

init : Decode.Value -> Url.Url -> Nav.Key -> ( Model, Cmd Msg )
init flags url key =
    let
        initialModel =
            { config = 
                case Decode.decodeValue configDecoder flags of
                  Ok config -> config
                  Err _ -> { host = "http://err-no-host.local" }
            , urlInput = ""
            , aliasUrl = ""
            , aliasType = "none" -- short, memorable, none
            , isResultVisible = False
            , errorMsg = Nothing
            , key = key
            , url = url
            , page = Main
            }
    in
    stepUrl url initialModel



-- UPDATE


type Msg
    = UpdateUrlInput String
    | UpdateAliasType String
    | Submit
    | ReceiveResponse (Result Http.Error String)
    | NavigateToManage
    | UrlChanged Url.Url
    | LinkClicked Browser.UrlRequest
    | ManageMsg Page.Manage.Msg


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    let
        _ = Log.log "Main.update: " msg
        _ = Log.log "Main.update: " model
    in
    case msg of
        UpdateUrlInput newUrl ->
            ( { model | urlInput = newUrl, errorMsg = Nothing }, Cmd.none )

        UpdateAliasType newType ->
            ( { model | aliasType = newType, errorMsg = Nothing }, Cmd.none )

        Submit ->
            ( model, submitUrl model.config.host model.urlInput model.aliasType )

        ReceiveResponse result ->
            case result of
                Ok alias ->
                    ( { model | aliasUrl = alias, isResultVisible = True, errorMsg = Nothing }, Cmd.none )

                Err (Http.BadStatus 409) ->
                    ( { model | errorMsg = Just "Link already exists" }, Cmd.none )

                Err _ ->
                    ( { model | errorMsg = Just "Failed to create link." }, Cmd.none )

        NavigateToManage ->
            ( model, navigateTo model ("/manage?id=" ++ model.aliasUrl) )

        LinkClicked urlRequest ->
            case urlRequest of
                Browser.Internal url ->
                    -- ( model, Cmd.none )
                    ( model, Nav.pushUrl model.key (Url.toString url) )

                Browser.External href ->
                    -- ( model, Cmd.none )
                    ( model, Nav.load href )

        UrlChanged url ->
            stepUrl url model

        ManageMsg manageMsg ->
            case model.page of
                About aboutModel ->
                    ( model, Cmd.none )

                Manage manageModel ->
                    let
                        ( updatedManageModel, manageCmd ) = Page.Manage.update manageMsg manageModel
                    in
                    ( { model | page = Manage updatedManageModel }, Cmd.map ManageMsg manageCmd )
  
                Main ->
                    ( model, Cmd.none )


-- VIEW


view : Model -> Browser.Document Msg
view model =
    case model.page of
        About about ->
            Log.log "about"
                Page.About.view
                about

        Manage manage ->
            Log.log "manage"
                Page.Manage.view manage
                  |> mapDocument ManageMsg

        Main ->
            getBody model


mapDocument : (innerMsg -> outerMsg) -> Browser.Document innerMsg -> Browser.Document outerMsg
mapDocument mapMsg doc =
    { title = doc.title
    , body = List.map (Html.map mapMsg) doc.body
    }


getBody : Model -> Browser.Document Msg
getBody model =
    { title = "LinkStand | Create a Link"
    , body =
        [ div [ class "container" ]
            [ h1 [] [ text "Link Creator" ]
            , Html.form [ Html.Events.onSubmit Submit ]
                [ input
                    [ type_ "text"
                    , placeholder "https://www.example.com/"
                    , required True
                    , pattern "(https?:\\/\\/)?(www\\.)?[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}(/\\S*)?"
                    , value model.urlInput
                    , Html.Events.onInput UpdateUrlInput
                    ]
                    []
                , label [ for "aliasType" ] [ text "Type (optional):" ]
                , select [ Html.Events.onInput UpdateAliasType ]
                    [ option [ value "none" ] [ text "None" ]
                    , option [ value "short" ] [ text "Short" ]
                    , option [ value "memorable" ] [ text "Memorable" ]
                    ]
                , button [ type_ "submit" ] [ text "Get Link" ]
                ]
            , if model.isResultVisible then
                div [ id "result" ]
                    [ p [] [ text "Your Link:" ]
                    , a [ href (model.config.host ++ model.aliasUrl), target "_blank", rel "noopener noreferrer" ] [ text model.aliasUrl ]
                    , p [] []
                    , button [ Html.Events.onClick NavigateToManage ] [ text "Manage" ]
                    ]

              else
                text ""
            , case model.errorMsg of
                Just msg ->
                    p [ class "error" ] [ text msg ]

                Nothing ->
                    text ""
            , a [ class "d-block mt-4", href "/about" ] [ text "About" ]
            ]
        ]
    }



-- HTTP REQUEST


submitUrl : String -> String -> String -> Cmd Msg
submitUrl host url aliasType =
    let
        urlToFetch =
            host ++ "?url=" ++ url ++ "&type=" ++ aliasType
    in
    Http.post
        { url = urlToFetch
        , body = Http.emptyBody
        , expect = Http.expectJson ReceiveResponse aliasDecoder
        }


navigateTo : Model -> String -> Cmd Msg
navigateTo model url =
    Log.log ("navigateTo: " ++ url)
        -- Cmd.none
        -- Nav.load url
        Nav.pushUrl model.key url



-- ROUTER


stepUrl : Url.Url -> Model -> ( Model, Cmd Msg )
stepUrl url model =
    let
        parser =
            oneOf
                [ route top
                    (stepMain model)
                , route (Parser.s "about")
                    (stepAbout model Page.About.init)
                , route (Parser.s "manage" <?> Query.string "id")
                    (\maybeId ->
                        let
                            manageInit =
                                Page.Manage.init model.config.host url model.key
                        in
                        stepManage model manageInit
                    )
                ]
    in
    case Parser.parse parser url of
        Just answer ->
            answer

        Nothing ->
            ( model, Cmd.none )


stepMain : Model -> ( Model, Cmd Msg )
stepMain model =
    ( { model | page = Main }, Cmd.none )


stepAbout : Model -> ( Page.About.Model, Cmd Page.About.Msg ) -> ( Model, Cmd Msg )
stepAbout model ( otherModel, cmd ) =
    ( { model | page = About otherModel }, Cmd.none )


stepManage : Model -> ( Page.Manage.Model, Cmd Page.Manage.Msg ) -> ( Model, Cmd Msg )
stepManage model ( otherModel, cmd ) =
    ( { model | page = Manage otherModel }, Cmd.none )


route : Parser a b -> a -> Parser (b -> c) c
route parser handler =
    Parser.map handler parser
