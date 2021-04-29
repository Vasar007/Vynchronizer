module Vynchronizer.Core.Operator


open System
open Vynchronizer.Core.Resource
open Vynchronizer.Core.Source
open Vynchronizer.Core.Target

// TODO: add rule to specify when we should copy data.
let copyData source sourceSpec target targetSpec =
    // TODO: compare metadata at first.
    source sourceSpec
        |> (target targetSpec)
