# Toy Redis Clone

This project is a simplistic implementation of a Redis-like server in C#, created specifically for **educational purposes**. It is poorly designed to demonstrate concepts and is **not intended for production use**. The goal is to help developers understand the basics of the Redis protocol and how a simple in-memory data store could be implemented.

If you're not familiar with Redis, you can check its [official documentation here](https://redis.io/docs).

---

## Commands Implemented

This project implements the following Redis commands. Below is a brief description of each command and a link to its corresponding Redis documentation:

1. **`PING`**
   - Description: A simple command that tests connectivity.
   - Example: `PING` responds with `PONG`.
   - Documentation: [PING Command](https://redis.io/commands/ping)

2. **`SET`**
   - Description: Sets a string value to a specified key.
   - Example: `SET key value`.
   - Documentation: [SET Command](https://redis.io/commands/set)

3. **`GET`**
   - Description: Retrieves the string value of a specified key.
   - Example: `GET key`.
   - Documentation: [GET Command](https://redis.io/commands/get)

Feel free to explore and extend the functionality further to implement more complex features.

---

## How to Run the Project

To test or debug this educational Redis clone, follow these steps:

1. Ensure you have the `.NET 9.0 SDK` installed.
2. Clone this repository.
3. Run the project locally:
   ```sh
   ./your_program.sh
   ```
4. Use a Redis-compatible client (like `redis-cli`) to communicate with your server.

---

## Project TODOs

As this project is for learning purposes, there are numerous unimplemented features and areas for improvement. Here are all the TODOs present in the codebase:

- **TODO**: Add error handling for invalid commands.
- **TODO**: Implement data expiration for `SET` keys.
- **TODO**: Add support for additional Redis commands (e.g., `DEL`, `MSET`, `INCR`).
- **TODO**: Optimize memory usage for large datasets.
- **TODO**: Implement persistence to save data to disk.
- **TODO**: Support multi-threading for concurrent connections.
- **TODO**: Add proper logging for server activity.
- **TODO**: Include unit tests to improve code reliability.
- **TODO**: Replace placeholder responses with actual error messages.

---

## Disclaimer

This project is a toy implementation and has been **intentionally kept simple to focus on fundamental concepts**. As such, it is not optimized for performance, security, or reliability and is not suitable for real-world use cases.

For learning resources, you can check out:
- [Redis Documentation](https://redis.io/docs)
- [Redis Protocol Specification](https://redis.io/docs/reference/protocol-spec/)

---

Feel free to fork this repository, try out the commands, and extend the functionality further. All feedback and contributions are welcome!