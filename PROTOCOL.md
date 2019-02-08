# LightWeight protocol

## Base algorithms
To understand the encoding of each field you must first be familiar with the
basic algorithms used in the encodings. 

### Variable length quantities (VLQ)
Variable length quantity is a technique where a number is encoded using just seven
bits out of each byte, and using the remaining spare bit as a flag to indicate if
there are more bytes to follow. We have followed the Git VLQ implmenetation which
further removes redundancy by using the prepending-redundancy technique
[described in Wikipedia](https://en.wikipedia.org/wiki/Variable-length_quantity).

## ZigZag
As described in Wikipedia:
Naively encoding a signed integer using two's complement means that −1 is 
represented as an unending sequence of ...11; for fixed length (e.g., 64-bit), 
this corresponds to an integer of maximum length. Instead, one can encode the 
numbers so that encoded 0 corresponds to 0, 1 to −1, 10 to 1, 11 to −2, 100 to 
2, etc.: counting up alternates between nonnegative (starting at 0) and negative
(since each step changes the least-significant bit, hence the sign), whence the 
name "zigzag encoding". Concretely, transform the integer as 
(n << 1) ^ (n >> k - 1) for fixed k-bit integers.

## Data types and treatments

### Booleans
Booleans are the simplest of all. `TRUE` is simply encoded as `0x01` and `FALAE`
as `0x00`. Both of these encode then naturally encode as a single byte with no
header necessary.

### UInt8 (byte) and SInt8
Since `UInt8`s and `SInt8`s are already a single byte these are stored as-is, without 
any modification. This then means that each value consumes a single byte with no
header necessary.

### UInt16, UInt32 and UInt64
Regardless of the bit length, these three unsigned integers are encoded using the
VLQ algorithm. The result is a header-less value of between one and ten bytes. At best
this is seven bytes shorter than it's decoded format, and at worst two bytes longer.

### SInt16, SInt32 and SInt64
Firstly these are encoded using the ZigZag algorithm to convert them to a managable
unsigned format, and then encoded with VLQ, similar to the unsigned integers.

### Strings
If the `string` is `NULL` the output is `0x00` by definition. If the `string` is not
`NULL` then it is encoded in UTF8, with it's length + 1 encoded in VLQ and prepended to 
the output. Strings of 126 characters or less therefore have a one-byte header.
Longer strings have a longer header, using additional bytes per the VLQ algorithm.

For example, the `string` "a" would be encoded as:
`0x0264`
`0x02` - The header, which is 1 greater than the length of the string.
`0x64` - The string "a".

### Arrays and lists
`array`s and `list`s are treated identically - so much so that it's perfectly fine 
to encode using an `array` and decode using a `list`.

If the `array`/`list` is `NULL` the output is `0x00` be definition. If it is not 
`NULL` the output first consists of the number of contained elements encoded in VLQ

Firstly the output contains the number of elements + 1, followed by the encoded
elements themselves with no separator. For example, the list [1, 2, 3] would be
encoded as:
`0x04010203`
`0x04` - Number of elements + 1
`0x01` - First element (VLQ encoded per the unsigned integer encoding)
`0x02` - Second element (VLQ encoded per the unsigned integer encoding)
`0x03` - Third element (VLQ encoded per the unsigned integer encoding)

### Maps (dictionaries)
Similarly to all nullable data types, if the value of the `map` is `NULL` the 
output is `0x00`. If not, first the output contains a header with the number
of elements in the `map` + 1 (eg, not the number of keys and pairs combined, just
the number of keys). Following this is the interleaved encoded keys and pairs.

For example {1:"a", 2:"b", 3:"c"} is encoded as:

`0x04016402650366`
`0x04` - Number of elements + 1
`0x01` - Key 1
`0x64` - Value "a"
`0x02` - Key 2
`0x65` - Value "b"
`0x03` - Key 3
`0x66` - Value "c"


## Design objectives
The LightWeight protocol is built around the following assumptions;
 * The receiver is assumed to have a schema to understand a message's structure
 * Legacy receivers will co-exist with modern receives, so both forward and backwards compatibility is required

With those assumptions in mind, LightWeight has the following prioritised goals:
 * Don't be a compression algorithm or use compression algorithms - if the user desires the payload can also be compressed to further save space
 * Be as efficient as possible, producing payloads with the minimum number of bytes possible
 * Be as simple as possible, the protocol must be simple enough to explain over a coffee with a senior developer
(If there is a conflict in the priorities the higher priority wins)
