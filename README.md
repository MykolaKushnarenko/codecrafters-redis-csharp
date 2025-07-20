# CodeCrafters Redis Challenge (C#)

**⚠️ Educational Purposes Only**  
This project is intentionally poorly designed as a learning exercise for the [CodeCrafters Redis Challenge](https://codecrafters.io/challenges/redis). It demonstrates basic Redis protocol implementation but contains many anti-patterns and should never be used in production.

## 🚀 Implemented Commands

### Basic Commands
- [`PING`](https://redis.io/commands/ping/) - Simple server response test
- [`ECHO`](https://redis.io/commands/echo/) - Returns the given string
- [`SET`](https://redis.io/commands/set/) - Set key-value pair (without full options)
- [`GET`](https://redis.io/commands/get/) - Get value by key
- [`DEL`](https://redis.io/commands/del/) - Delete one or more keys
- [`EXISTS`](https://redis.io/commands/exists/) - Check if key exists
- [`TYPE`](https://redis.io/commands/type/) - Get key type (basic implementation)

### Server Commands
- [`INFO`](https://redis.io/commands/info/) - Partial server information
- [`CONFIG`](https://redis.io/commands/config/) - Minimal GET/SET support

### Replication Commands
- [`REPLCONF`](https://redis.io/commands/replconf/) - Basic replication setup
- [`PSYNC`](https://redis.io/commands/psync/) - Partial sync implementation
- [`WAIT`](https://redis.io/commands/wait/) - Fake implementation

### Stream Commands (Partial)
- [`XADD`](https://redis.io/commands/xadd/) - Basic stream entry addition
- [`XRANGE`](https://redis.io/commands/xrange/) - Simple range queries

## 🏗️ Known Anti-Patterns (Intentional)

1. **Storage Layer**:
   - Mixed concerns between different data types
   - No proper persistence
   - Race conditions in concurrent access

2. **Connection Handling**:
   - No proper connection pooling
   - Incomplete error recovery
   - Resource leaks possible

3. **Protocol Implementation**:
   - Partial RESP (Redis Serialization Protocol) support
   - Many edge cases not handled
   - No proper pipelining

## 📝 TODO List

### Core Improvements
- [ ] Implement proper thread-safe storage with granular locking
- [ ] Add complete RESP3 protocol support
- [ ] Implement TTL and key expiration
- [ ] Add AOF (Append Only File) persistence
- [ ] Proper connection lifecycle management

### Command Completion
- [ ] Full stream support (`XREAD`, `XGROUP`, etc.)
- [ ] Transactions (`MULTI`/`EXEC`)
- [ ] Pub/Sub functionality
- [ ] Lua scripting support
- [ ] Cluster mode implementation

### Code Quality
- [ ] Add comprehensive unit tests
- [ ] Implement proper logging
- [ ] Add metrics collection
- [ ] Security hardening
- [ ] Configuration system

## 🧠 Learning Resources

1. [Redis Protocol Specification](https://redis.io/docs/reference/protocol-spec/)
2. [Redis Command Reference](https://redis.io/commands/)
3. [Redis Database File Specification](https://rdb.fnordig.de/file_format.html)
4. [C# Asynchronous Programming](https://docs.microsoft.com/en-us/dotnet/csharp/async)

## ⚖️ License

This educational project is released under the [MIT License](LICENSE).