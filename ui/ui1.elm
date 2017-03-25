import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onInput)

main : Program Never Model Msg
main =
  Html.beginnerProgram { model = model, view = view, update = update }


-- MODEL

type alias Model = 
 { name : String 
 , password : String
 , passwordAgain : String
 }

model : Model
model =
  Model "" "" "" 


-- UPDATE

type Msg 
    = Name String
    | Password String
    | PasswordAgain String

update : Msg -> Model -> Model
update msg model =
  case msg of
    Name name ->
        { model | name = name }
    
    Password password ->
        { model | password = password }

    PasswordAgain password ->
        { model | passwordAgain = password }

-- VIEW

view : Model -> Html Msg
view model =
    div []
        [ input [ placeholder "Text to reverse", onInput Change ] []
        , div [] [ text (String.reverse model.content) ]
        ]