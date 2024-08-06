#npm install --global elm elm-live@next
. .\configure.ps1

elm-live Main.elm --pushstate --hot --open "--" --output=index.js
