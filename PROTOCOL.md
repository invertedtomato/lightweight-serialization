# LightWeight protocol

## Data types and treatments
| Data type    | Summary                                                                                                                 |
|--------------|-------------------------------------------------------------------------------------------------------------------------|
| [Bool](#Booleans)                  | `TRUE` encodes as `0x01`, `FALSE` encodes as `0x00`.                                                                    |
| [UInt8/SInt8](#Bytes)    | Raw single byte.                                                                                                        |
| [UInt16/32/64](#Unsigned-integers) | Encoded using [VLQ](#VLQ-encoding).                                                                                     |
| [SInt16/32/64](#Signed-integers)   | Encoded using [ZigZag](#ZigZag-encoding) then [VLQ](#VLQ-encoding).                                                     |
| [String](#Strings)                 | Encoded using UTF-8, prefixed with [VLQ](#VLQ-encoding)-encoded length.                                                 |
| [Arrays/Lists](#Arrays-and-lists)  | Number of elements encoded using [VLQ](#VLQ-encoding), followed by each element, with no separator.                     |
| [Maps](#Maps)                      | Number of elements encoded using [VLQ](#VLQ-encoding), followed by each element's keys and values interleaved, with no separator. |
| [Nullable](#Nullable)              | As a variation to the above, the initial value (length etc) is incremented by 1, and `0x00` is used to express `NULL`.  |

### Booleans
Booleans are the simplest of all. `TRUE` is simply encoded as `0x01` and `FALSE`
as `0x00`. Both of these encode then naturally encode as a single byte with no
header necessary.

### Bytes
Since `UInt8`s and `SInt8`s are already a single byte these are stored as-is, without 
any modification. This then means that each value consumes a single byte with no
header necessary. If you're thinking about a byte array, see [Arrays and lists](#Arrays-and-lists).

### Unsigned integers
Regardless of the bit length, these three unsigned integers are encoded using the
[VLQ](#VLQ-encoding) algorithm. The result is a header-less value of between one and ten bytes. At best
this is seven bytes shorter than it's decoded format, and at worst two bytes longer.

### Signed integers
Firstly these are encoded using the [ZigZag](#ZigZag-encoding) algorithm to convert them to a managable
unsigned format, and then encoded with [VLQ](#VLQ-encoding), similar to the unsigned integers.

### Strings
The string is encoded in [UTF-8](https://en.wikipedia.org/wiki/UTF-8), with it's length encoded in [VLQ](#VLQ-encoding) and prepended to 
the output. Strings of 127 characters or less therefore have a one-byte header.
Longer strings have a longer header, using additional bytes per [VLQ](#VLQ-encoding).

For example, the `string` "a" would be encoded as:
`0x0164`
`0x01` - The header, which is 1 greater than the length of the string.
`0x64` - The string "a".

### Arrays and lists
`array`s and `list`s are treated identically to each other - so much so that it's perfectly fine 
to encode using an `array` and decode using a `list`.

The output first consists of the number of contained elements encoded in [VLQ](#VLQ-encoding)

Firstly the output contains the number of elements, followed by the encoded
elements themselves with no separator. For example, the list [1, 2, 3] would be
encoded as:
`0x03010203`
`0x03` - Number of elements
`0x01` - First element (VLQ encoded per the unsigned integer encoding)
`0x02` - Second element (VLQ encoded per the unsigned integer encoding)
`0x03` - Third element (VLQ encoded per the unsigned integer encoding)

### Maps (dictionaries)
The output contains a header with the number of elements in the `map` (eg, not the number of keys and pairs combined, just
the number of keys). Following this is the interleaved encoded keys and pairs.

For example {1:"a", 2:"b", 3:"c"} is encoded as:

`0x03016402650366`
`0x03` - Number of elements
`0x01` - Key 1
`0x64` - Value "a"
`0x02` - Key 2
`0x65` - Value "b"
`0x03` - Key 3
`0x66` - Value "c"

### Nullables
Any of the above data types are adapted to being nullable by incrementing the initial number by one, and using `0x00` to represent `NULL`. That is, a string of a single character would start with `0x02` instead of `0x01`, and `0x00` is used for `NULL` values.


## Supporting algorithms
To understand the encoding of each field you must first be familiar with the
supporting algorithms used in the encodings. 

### VLQ encoding
Variable length quantity is a technique where a number is encoded using just seven
bits out of each byte, and using the remaining spare bit as a flag to indicate if
there are more bytes to follow. We have followed the Git VLQ implmenetation which
further removes redundancy by using the prepending-redundancy technique
[described in Wikipedia](https://en.wikipedia.org/wiki/Variable-length_quantity).

## ZigZag encoding
[As described in Wikipedia](https://en.wikipedia.org/wiki/Variable-length_quantity#Zigzag_encoding):
> Naively encoding a signed integer using two's complement means that −1 is 
> represented as an unending sequence of ...11; for fixed length (e.g., 64-bit), 
> this corresponds to an integer of maximum length. Instead, one can encode the 
> numbers so that encoded 0 corresponds to 0, 1 to −1, 10 to 1, 11 to −2, 100 to 
> 2, etc.: counting up alternates between nonnegative (starting at 0) and negative
> (since each step changes the least-significant bit, hence the sign), whence the 
> name "zigzag encoding". Concretely, transform the integer as 
> (n << 1) ^ (n >> k - 1) for fixed k-bit integers.
