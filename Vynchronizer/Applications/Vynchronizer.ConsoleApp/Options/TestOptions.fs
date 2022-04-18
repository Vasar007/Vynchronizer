module Vynchronizer.ConsoleApp.Options.TestOptions

open CommandLine


[<Struct>]
type public TestCaseArg =
    | Comparing = 0
    | Copying = 1
    | Operator = 2

[<Struct>]
type internal TestCase =
    | Comparing
    | Copying
    | Operator

[<Verb("test", HelpText = "Executes test case.")>]
type public TestCaseOptions = {
    [<Option('c', "case", Required = true, HelpText = "Test case to execute. Valid values: Comparing (0), Copying (1), Operator (2).")>]
    Case: TestCaseArg
}

let internal convertTestCase testCase =
    match testCase with
        | TestCaseArg.Comparing -> TestCase.Comparing
        | TestCaseArg.Copying -> TestCase.Copying
        | TestCaseArg.Operator -> TestCase.Operator
        | _ -> invalidArg (nameof testCase) ($"Test case is out of range: \"{testCase.ToString()}\".")
