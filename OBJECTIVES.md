# Design objectives
The LightWeight protocol is built around the following assumptions;
 * The receiver is assumed to have a schema to understand a message's structure
 * Legacy receivers will co-exist with modern receives, so both forward and backwards compatibility is required

With those assumptions in mind, LightWeight has the following prioritised goals:
 * Don't be a compression algorithm or use compression algorithms - if the user desires the payload can also be compressed to further save space
 * Be as efficient as possible, producing payloads with the minimum number of bytes possible
 * Be as fast as possible, taking no longer than MsgPack or ProtoBuff
 * Be as simple as possible, the protocol must be simple enough to explain over a coffee with a senior developer
(If there is a conflict in the priorities the higher priority wins)