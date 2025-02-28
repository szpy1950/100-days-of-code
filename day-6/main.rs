
// === Solution 2: Concurrent Counter ===
async fn count() {
    for i in 1..=3 {
        println!("Counter: {}", i);
        sleep(Duration::from_millis(500)).await;
    }
}

#[tokio::main]
async fn main() {
    let (t1, t2) = tokio::join!(
        tokio::spawn(count()),
                                tokio::spawn(count())
    );
}
