# LightWeight protocol
## Design objectives
The LightWeight protocol is built around the following assumptions;
 * The receiver is assumed to have a schema to understand a message's structure
 * Legacy receivers will co-exist with modern receives, so both forward and backwards compatibility is required

With those assumptions in mind, LightWeight has the following prioritised goals:
 * Don't be a compression algorithm or use compression algorithms - if the user desires the payload can also be compressed to further save space
 * Be as efficient as possible, producing payloads with the minimum number of bytes possible
 * Be as simple as possible, the protocol must be simple enough to explain over a coffee with a senior developer
(If there is a conflict in the priorities the higher priority wins)

## Framing
The bare minimum data any fast binary serializer needs is the length of a payload and the payload itself. This is exactly
what LightWeight uses for each field. 

At the top level, each LightWeight message looks just like this:
`<length><field>`

However given the nested nature of POCOs, each payload potentially contains more payloads:
`<length><<length><field><length><field>>`

`<length>` is a 7-bit VLQ number
`<field>` is encoded depending on the datatype as described  below.

## Field encoding

| Data type    | Range               | Encoded as             |
|--------------|---------------------|------------------------|
| Boolean      | `FALSE`             | Field omitted altogether |
|              | `TRUE`              | `0x00`                 |
| String       | `NULL`              | Field omitted altogether |
|              | Any other value     | UTF8 encoded           |
| Integer      | Fits in <=8bits     | UInt8 or SInt8         |
|              | Fits in <=16bits    | UInt16 or SInt16       |
|              | Fits in <=32bits    | UInt32 or SInt32       |
|              | Fits in <=64bits    | UInt64 or SInt64       |
| Array        | `NULL`              | Field omitted altogether |
|              | Any other value     | Sequence of values     |
| List         | `NULL`              | Field omitted altogether |
|              | Any other value     | Sequence of values     |
| Dictionary   | `NULL`              | Field omitted altogether |
|              | Any other value     | Sequence of interleaved keys and values  |
| POCO         | `NULL`              | Field omitted altogether |
|              | Any other value     | Sequence of values according to index. Omitted intermediate values have nil length. |     |