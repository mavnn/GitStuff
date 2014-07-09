#r "../packages/FParsec/lib/net40-client/FParsecCS.dll"
#r "../packages/FParsec/lib/net40-client/FParsec.dll"

open System
open FParsec

let timestamp = pstring "|" >>. puint32 |>> fun x -> DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(float x)
let name = pstring "|" >>. manyCharsTill anyChar newline
let file = notEmpty (restOfLine false .>> many newline)
let files = manyTill file (followedBy timestamp <|> followedBy eof)
let commit = tuple3 (timestamp .>> newline) (name .>> newline) (files)
let stream = many commit

let parseLog s =
    runParserOnStream stream () "log" s Text.Encoding.UTF8