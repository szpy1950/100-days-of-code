use tokio::{
    sync::{mpsc, broadcast, Mutex, RwLock, Semaphore},
    time::{sleep, Duration, Instant},
    net::{TcpListener, TcpStream},
    fs::File,
    io::{AsyncReadExt, AsyncWriteExt},
};
use std::sync::Arc;
use std::collections::{HashMap, HashSet, BinaryHeap};

// Main async function
#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    println!("=== Tokio Examples ===\n");

    // === Basic Async Examples ===
    println!("1. Basic Async Examples:");

    // async fn example
    async fn greet(name: &str) {
        println!("Hello, {}!", name);
    }

    // async move example
    let name = String::from("Alice");
    let future = async move {
        println!("Greeting for {}", name);
    };

    // .await example
    greet("World").await;
    future.await;

    println!("\n2. Task Management Examples:");

    // tokio::spawn example
    let handle = tokio::spawn(async {
        println!("This is a spawned task");
    });
    handle.await?;

    // tokio::join example
    let (result1, result2) = tokio::join!(
        async { "First task" },
        async { "Second task" }
    );
    println!("Join results: {} and {}", result1, result2);

    // tokio::select example
    tokio::select! {
        _ = sleep(Duration::from_millis(100)) => {
            println!("Timer finished first");
        }
        _ = async { println!("This ran first") } => {
            println!("Task finished first");
        }
    }

    println!("\n3. Time Operations Examples:");

    // Duration examples
    let second = Duration::from_secs(1);
    let millisec = Duration::from_millis(500);

    // sleep example
    println!("Sleeping for 100ms...");
    sleep(Duration::from_millis(100)).await;

    // Instant example
    let start = Instant::now();
    sleep(Duration::from_millis(10)).await;
    println!("Elapsed: {:?}", start.elapsed());

    println!("\n4. Channels Examples:");

    // mpsc channel example
    let (tx, mut rx) = mpsc::channel(32);
    tokio::spawn(async move {
        tx.send("Hello through mpsc!").await.unwrap();
    });
    println!("Received: {}", rx.recv().await.unwrap());

    // broadcast channel example
    let (tx, mut rx1) = broadcast::channel(16);
    let mut rx2 = tx.subscribe();
    tx.send("Hello broadcast!").unwrap();
    println!("Receiver 1 got: {}", rx1.recv().await.unwrap());
    println!("Receiver 2 got: {}", rx2.recv().await.unwrap());

    println!("\n5. Synchronization Examples:");

    // Mutex example
    let counter = Arc::new(Mutex::new(0));
    let counter_clone = counter.clone();
    tokio::spawn(async move {
        let mut lock = counter_clone.lock().await;
        *lock += 1;
    }).await?;
    println!("Counter: {}", *counter.lock().await);

    // RwLock example
    let data = Arc::new(RwLock::new(String::from("initial")));
    {
        let r = data.read().await;
        println!("Read lock: {}", *r);
    }
    {
        let mut w = data.write().await;
        *w = String::from("modified");
    }

    // Semaphore example
    let sem = Arc::new(Semaphore::new(2));
    let permit = sem.acquire().await.unwrap();
    println!("Got semaphore permit");
    drop(permit);

    println!("\n6. File Operations Examples:");

    // Write file example
    let mut file = File::create("example.txt").await?;
    file.write_all(b"Hello, Tokio!").await?;

    // Read file example
    let mut file = File::open("example.txt").await?;
    let mut contents = Vec::new();
    file.read_to_end(&mut contents).await?;
    println!("Read: {}", String::from_utf8_lossy(&contents));

    println!("\n7. Networking Examples:");

    // TCP Listener example
    tokio::spawn(async {
        let listener = TcpListener::bind("127.0.0.1:8080").await.unwrap();
        println!("Listening on 8080");
        let (socket, _) = listener.accept().await.unwrap();
        println!("Accepted connection");
    });

    // TCP Client example
    sleep(Duration::from_millis(100)).await;
    let _stream = TcpStream::connect("127.0.0.1:8080").await?;
    println!("Connected to server");

    println!("\n8. Collections Examples:");

    // HashMap example
    let mut map = HashMap::new();
    map.insert("key", "value");
    println!("HashMap value: {:?}", map.get("key"));

    // BinaryHeap example
    let mut heap = BinaryHeap::new();
    heap.push(1);
    heap.push(5);
    heap.push(2);
    println!("Heap max: {:?}", heap.pop());

    // HashSet example
    let mut set = HashSet::new();
    set.insert("unique");
    set.insert("unique"); // Won't add duplicate
    println!("Set size: {}", set.len());

    println!("\n9. Common Patterns Examples:");

    // Error handling example
    fn fallible() -> Result<(), Box<dyn std::error::Error>> {
        Ok(())
    }
    fallible()?;

    // Shared state example
    let shared_data = Arc::new(Mutex::new(vec![1, 2, 3]));
    let data_clone = shared_data.clone();
    tokio::spawn(async move {
        let mut data = data_clone.lock().await;
        data.push(4);
    }).await?;

    // Resource cleanup example
    let resource = String::from("cleanup me");
    drop(resource); // Explicit cleanup

    println!("\nAll examples completed successfully!");
    Ok(())
}
