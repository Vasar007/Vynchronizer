module Vynchronizer.ConsoleApp.ExitCodes


// TODO: replace with version from Acolyte package.

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
