# Tokio Cheatsheet

## Essential Imports
```rust
use tokio::time::{sleep, Duration, Instant};  // Time-related functionality
use tokio::sync::{mpsc, broadcast, Mutex, RwLock};  // Synchronization primitives
use tokio::net::{TcpListener, TcpStream};  // Networking
use tokio::fs::File;  // File operations
use tokio::io::{AsyncReadExt, AsyncWriteExt};  // I/O traits
```

## Core Concepts & Keywords

### Basic Async
- `async fn` - Declares an async function
- `async move` - Creates an async block that takes ownership
- `.await` - Waits for an async operation to complete
- `#[tokio::main]` - Macro for async main function

### Task Management
- `tokio::spawn()` - Spawns a new async task
- `tokio::join!` - Waits for multiple tasks to complete
- `tokio::select!` - Waits for one of multiple tasks to complete

### Time Operations
- `Duration::from_secs()` - Creates duration from seconds
- `Duration::from_millis()` - Creates duration from milliseconds
- `sleep()` - Async sleep
- `Instant::now()` - Gets current time point

### Channels
- `mpsc::channel()` - Multi-producer, single-consumer channel
- `broadcast::channel()` - Multi-producer, multi-consumer channel
- `Sender::send()` - Sends message through channel
- `Receiver::recv()` - Receives message from channel

### Synchronization
- `Mutex` - Mutual exclusion (async-aware)
- `RwLock` - Reader-writer lock (async-aware)
- `Arc` - Atomic reference counting
- `Semaphore` - Limits concurrent access

### File Operations
- `File::open()` - Opens file for reading
- `File::create()` - Creates/opens file for writing
- `read_to_end()` - Reads entire file to buffer
- `write_all()` - Writes entire buffer to file

### Networking
- `TcpListener::bind()` - Creates TCP server
- `listener.accept()` - Accepts incoming connection
- `TcpStream::connect()` - Creates TCP client connection

### Collections & Data Structures
- `HashMap` - Key-value storage
- `BinaryHeap` - Priority queue
- `Vec` - Dynamic array
- `HashSet` - Unique item collection

## Common Patterns

### Error Handling
```rust
Result<(), Box<dyn std::error::Error>>  // Common error return type
```

### Shared State
```rust
Arc<Mutex<T>>  // Thread-safe shared mutable state
Arc<RwLock<T>>  // Thread-safe shared state with multiple readers
```

### Resource Cleanup
```rust
drop(resource)  // Explicitly cleanup resource
```

## Tips
- Use `Arc` when sharing data between tasks
- Prefer `RwLock` over `Mutex` when you have multiple readers
- Always handle task errors from `spawn()`
- Use channels for communication between tasks
- Remember to `.await` async operations
- Use `select!` for handling multiple async operations with timeouts
