module Vynchronizer.Core.Local.AsyncFileReader


open System
open System.IO

/// <summary>
/// Represents a sequence of values 'TData where items are generated asynchronously on-demand.
/// </summary>
type public AsyncReadSeq<'TData> = Async<AsyncReadSeqInner<'TData>>
and public AsyncReadSeqInner<'TData> =
    | ReadingEnded
    | ReadItem of 'TData * AsyncReadSeq<'TData>

/// <summary>
/// Represents a sequence of result values 'TResult and function which accept items of type 'TData
/// are written asynchronously on-demand.
/// </summary>
type public AsyncWriteSeq<'TData, 'TResult> = Async<AsyncWriteSeqInner<'TData, 'TResult>>
and public AsyncWriteSeqInner<'TData, 'TResult> =
    | WritingEnded
    | WrittenItem of 'TResult * ('TData -> AsyncWriteSeq<'TData, 'TResult>)

/// <summary>
/// Ensures that steam readable.
/// </summary>
let private ensureStreamCanRead (stream: Stream) =
    if not stream.CanRead then
        raise (NotSupportedException("The stream does not support reading."))

/// <summary>
/// Ensures that steam writable.
/// </summary>
let private ensureStreamCanWrite (stream: Stream) =
    if not stream.CanWrite then
        raise (NotSupportedException("The stream does not support writing."))

/// <summary>
/// Reads stream in blocks of size (returns on-demand asynchronous sequence).
/// </summary>
let public readInBlocks (stream: Stream) size =
    ensureStreamCanRead stream

    async {
        let buffer = Array.zeroCreate size

        // Returns next block as "Item" of async seq.
        let rec readNextBlock() =
            async {
                let offset = 0
                let! count = stream.AsyncRead(buffer, offset, size)
                if count = 0 then
                    return ReadingEnded
                else
                    // Create buffer with the right size.
                    let result =
                        if count = size then buffer
                        else buffer |> Seq.take count |> Array.ofSeq
                    return ReadItem(result, readNextBlock())
            }

        return! readNextBlock()
    }

/// <summary>
/// Returns function that writes stream from buffer blocks (returns on-demand asynchronous
/// sequence).
/// </summary>
let public writeInBlocks (stream: Stream) =
    ensureStreamCanWrite stream

    // Writes next block and returns "Item" of async seq.
    let rec writeNextBlock (block: byte[]) =
        async {
            if block.Length = 0 then
                return WritingEnded
            else
                let offset = 0
                do! stream.AsyncWrite(block, offset, block.Length)
                // Mark that current block has been successfully written.
                return WrittenItem(true, writeNextBlock)
        }

    writeNextBlock

/// <summary>
/// Compares two asynchronous sequences. Use this function along with <see cref="readInBlocks" />.
/// </summary>
let rec internal compareBlocks readSeq1 readSeq2 =
    async {
        let! item1 = readSeq1
        let! item2 = readSeq2
        match item1, item2 with
            | ReadItem(block1, _), ReadItem(block2, _) when block1 <> block2 -> return false
            | ReadItem(_, nestedSeq1), ReadItem(_, nestedSeq2) -> return! compareBlocks nestedSeq1 nestedSeq2
            | ReadingEnded, ReadingEnded -> return true
            | _ -> return failwith "Size does not match."
    }

/// <summary>
/// Copies data between two asynchronous sequences. Use this function along with
/// <see cref="readInBlocks" /> and <see cref="writeInBlocks" />.
/// </summary>
let rec internal copyBlocks readSeq1 writeSeq2 resultSeq =
    async { 
        match! readSeq1 with
            | ReadItem(block1, nestedSeq1) ->
                let! resultSeq2 = writeSeq2 block1
                match resultSeq2 with
                    | WrittenItem(result2, nestedSeq2) ->
                        let uptatedResult = resultSeq |> Seq.append (Seq.singleton result2)
                        return! copyBlocks nestedSeq1 nestedSeq2 uptatedResult
                    | WritingEnded -> return resultSeq
            | ReadingEnded -> return resultSeq
    }

/// <summary>
/// Compares two files using specified size of blocks.
/// </summary>
let public compareFiles filePath1 filePath2 size =
    async {
        use stream1 = File.OpenRead(filePath1)
        use stream2 = File.OpenRead(filePath2)
        let readSeq1 = readInBlocks stream1 size
        let readSeq2 = readInBlocks stream2 size
        return! compareBlocks readSeq1 readSeq2
    }

/// <summary>
/// Copies data from source to target using specified size of blocks.
/// </summary>
let public copyData filePath1 filePath2 size =
    async {
        use stream1 = File.OpenRead(filePath1)
        use stream2 = File.Open(filePath2, FileMode.Truncate) // Reset content of target file.
        let readSeq1 = readInBlocks stream1 size
        let writeSeq2 = writeInBlocks stream2
        return! copyBlocks readSeq1 writeSeq2 Seq.empty
    }
