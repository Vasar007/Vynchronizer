module Vynchronizer.ConsoleApp.Program

open System
open System.Reflection
open System.Text
open Acolyte.Collections
open CommandLine
open Vynchronizer.ConsoleApp
open Vynchronizer.ConsoleApp.Options.RuleOptions
open Vynchronizer.ConsoleApp.Options.TestOptions
open Vynchronizer.ConsoleApp.Executors.RunExecutor
open Vynchronizer.ConsoleApp.Executors.TestExecutor


/// <summary>
/// Loads all types with <see cref="VerbAttribute" /> from current assembly using Reflection.
/// </summary>
let private loadVerbs =
    let executingAssembly = Assembly.GetExecutingAssembly()
    let types = executingAssembly.GetTypes()
    types
        |> Array.filter (fun loadedType -> loadedType.GetCustomAttribute<VerbAttribute>() <> null)

let isImportantError (error: Error) =
    error.Tag <> ErrorType.HelpRequestedError &&
    error.Tag <> ErrorType.VersionRequestedError

let getErrorString (error: Error) =
    $"Encountered error with Tag '{error.Tag.ToString()}', StopsProcessing '{error.StopsProcessing.ToString()}'."

let constructMessage (errors: seq<Error>) =
    let errorStrings =
        errors
            |> Seq.map getErrorString
    let builder = new StringBuilder()
    builder.AppendJoin(Environment.NewLine, errorStrings).ToString()

let onNotParsed (errors: seq<Error>) =
    let filteredErrors =
        errors
            |> Seq.filter isImportantError
            |> constructMessage

    let message = filteredErrors.ToSingleString()
    failwith message

let private asyncMain (args: string[]) =
    async {
        let verbs = loadVerbs
        let result = Parser.Default.ParseArguments(args, verbs)

        match result with
            | :? CommandLine.Parsed<obj> as command ->
                match command.Value with
                    | :? ExecuteRuleOptions as options -> return! executeRunCommandAsync options
                    | :? TestCaseOptions as options -> return! executeTestCommandAsync options
                    | _ -> return invalidArg (nameof args) ($"Test case is out of range: \"{args.ToSingleString()}\".")
            | :? CommandLine.NotParsed<obj> as notParsed -> return onNotParsed notParsed.Errors
            | _ -> return invalidArg (nameof args) ($"Test case is out of range: \"{args.ToSingleString()}\".")
    }

[<EntryPoint>]
let private main args =
    try
        try
            printfn "Vynchronizer started."
    
            asyncMain args |> Async.RunSynchronously
        with
            | ex ->
                printfn $"Exception occurred: {ex}"
                ExitCodes.failExitCode
    finally
        printfn "Vynchronizer stopped."
        printfn "Press any key to close this window..."
        Console.ReadKey() |> ignore
