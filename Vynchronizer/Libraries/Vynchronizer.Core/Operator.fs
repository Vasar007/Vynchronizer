module Vynchronizer.Core.Operator


open System
open Vynchronizer.Core.Resource
open Vynchronizer.Core.Source
open Vynchronizer.Core.Target

let private getSourceData (sourceSpec: SourceSpec) =
    match sourceSpec.StorageType with
        | ResourceStorage.Unknown -> failwith "Unknown source resource storage."
        | ResourceStorage.LocalFileSystem -> getDummyDataFromSource
        | ResourceStorage.GoogleDrive -> getDummyDataFromSource

let private getTargetWriter (targetSpec: TargetSpec) =
    match targetSpec.StorageType with
        | ResourceStorage.Unknown -> failwith "Unknown target resource storage."
        | ResourceStorage.LocalFileSystem -> writeDummyDataToTarget
        | ResourceStorage.GoogleDrive -> writeDummyDataToTarget

// TODO: add rule to specify when we should copy data.
let processSpecs sourceSpec targetSpec =
    let source = getSourceData sourceSpec
    let target = getTargetWriter targetSpec

    // TODO: compare metadata at first.
    source sourceSpec
        |> (target targetSpec)
