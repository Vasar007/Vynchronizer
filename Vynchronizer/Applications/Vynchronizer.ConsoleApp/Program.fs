module Vynchronizer.ConsoleApp.Program


open System
open Vynchronizer.Core.AsyncFileReader

let asyncMain args =
    async {
        let file1 = "1.txt"
        let file2 = "2.txt"
        let size = 1000
        printfn "Comparing two files %s and %s with size blocks %i" file1 file2 size

        let! comareResult = compareFiles file1 file2 size
        printfn "Comparison result: %b" comareResult

        return 0
    }

[<EntryPoint>]
let main args =
    try
        try
            printfn "Vynchronizer started."
    
            asyncMain args |> Async.RunSynchronously // return an integer exit code.
        with
            | ex ->
                printfn "Exception occurred: %s" (ex.ToString())
                1 // return an integer exit code.
    finally
        printfn "Vynchronizer stopped."
        printfn "Press any key to close this window..."
        Console.ReadKey() |> ignore
