module Vynchronizer.ConsoleApp.CommandLineArgs

open CommandLine
open Vynchronizer.Core.Resource
open Vynchronizer.Core.Target


type SourceRuleOptions = {
    [<Option('s', "storage", Required = true, HelpText = "Source storage.")>]
    StorageType: ResourceStorage

    [<Option('p', "path", Required = true, HelpText = "Resource path on source storage.")>]
    Path: string

    [<Option('b', "blocksize", Required = false, Default = 1024, HelpText = "Block size. Synchronization will copy data from source to target by blocks.")>]
    BlockSize: int
}

[<Struct>]
type public ConflictResolutionPolicyArg =
    | ReplaceAll = 0
    | ReplaceIfNewer = 1

type TargetRuleOptions = {
    [<Option('s', "storage", Required = true, HelpText = "Target storage.")>]
    StorageType: ResourceStorage

    [<Option('p', "path", Required = true, HelpText = "Resource path on target storage.")>]
    Path: string

    [<Option('r', "resolution", Required = false, Default = ConflictResolutionPolicyArg.ReplaceAll, HelpText = "Policy to resolve conflicts during synchronization.")>]
    ConflictResolution: ConflictResolutionPolicyArg
}

type RuleOptions = {
    [<Option("source", Required = true, HelpText = "Synchronization source.")>]
    Source: seq<string>

    [<Option("target", Required = true, HelpText = "Synchronization target.")>]
    Target: seq<string>
}

[<Verb("execute", HelpText = "Executes synchronization rule.")>]
type ExecuteRuleOptions = {
    Rule: RuleOptions
}

let internal convertConflictResolutionPolicy conflictResolutionPolicy =
    match conflictResolutionPolicy with
        | ConflictResolutionPolicyArg.ReplaceAll -> ConflictResolutionPolicy.ReplaceAll
        | ConflictResolutionPolicyArg.ReplaceIfNewer -> ConflictResolutionPolicy.ReplaceIfNewer
        | _ -> invalidArg (nameof conflictResolutionPolicy) ($"Conflict resolution policy is out of range: \"{conflictResolutionPolicy.ToString()}\".")
