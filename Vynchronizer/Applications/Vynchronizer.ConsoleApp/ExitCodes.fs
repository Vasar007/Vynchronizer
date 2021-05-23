module Vynchronizer.ConsoleApp.ExitCodes

[<Struct>]
type internal ExitCode =
    | Success
    | Fail

let internal convertExitCode exitCode =
    match exitCode with
        | Success -> 0
        | Fail -> -1

let internal successExitCode = convertExitCode ExitCode.Success
let internal failExitCode = convertExitCode ExitCode.Fail
