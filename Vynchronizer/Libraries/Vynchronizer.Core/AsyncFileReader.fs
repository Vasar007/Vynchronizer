module Vynchronizer.Core.AsyncFileReader


open System.IO

/// <summary>
/// Represents a sequence of values 'T where items
/// are generated asynchronously on-demand.
/// </summary>
type AsyncSeq<'T> = Async<AsyncSeqInner<'T>>
and AsyncSeqInner<'T> =
  | Ended
  | Item of 'T * AsyncSeq<'T>

/// <summary>
/// Reads stream in blocks of size (returns on-demand asynchronous sequence).
/// </summary>
let readInBlocks (stream: Stream) size =
    async {
        let buffer = Array.zeroCreate size

        // Returns next block as 'Item' of async seq.
        let rec nextBlock() =
            async {
                let offset  = 0
                let! count = stream.AsyncRead(buffer, offset, size)
                if count = 0 then
                    return Ended
                else 
                // Create buffer with the right size.
                    let res =
                        if count = size then buffer
                        else buffer |> Seq.take count |> Array.ofSeq
                    return Item(res, nextBlock())
            }

        return! nextBlock()
    }

/// <summary>
/// Compares two asynchronous sequences. Use this function along with <see cref="readInBlocks" />.
/// </summary>
let rec compareBlocks seq1 seq2 =
    async {
        let! item1 = seq1
        let! item2 = seq2
        match item1, item2 with
            | Item(block1, _), Item(block2, _) when block1 <> block2 -> return false
            | Item(_, nestedSeq1), Item(_, nestedSeq2) -> return! compareBlocks nestedSeq1 nestedSeq2
            | Ended, Ended -> return true
            | _ -> return failwith "Size does not match."
    }

/// <summary>
/// Compares two files using specified size of blocks.
/// </summary>
let compareFiles filePath1 filePath2 size =
    async {
        use stream1 = File.OpenRead(filePath1)
        use stream2 = File.OpenRead(filePath2)
        let seq1 = readInBlocks stream1 size
        let seq2 = readInBlocks stream2 size
        return! compareBlocks seq1 seq2
    }
