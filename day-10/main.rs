// === Solution 6: Distributed Counter with Multiple Workers ===
use tokio::sync::{mpsc, Mutex};
use std::sync::Arc;

struct Coordinator {
    counter: Arc<Mutex<i64>>,
    workers: Vec<mpsc::Sender<i64>>,
}

impl Coordinator {
    async fn new(worker_count: usize) -> Self {
        let counter = Arc::new(Mutex::new(0));
        let mut workers = Vec::new();

        for _ in 0..worker_count {
            let (tx, mut rx) = mpsc::channel(32);
            let counter = counter.clone();

            tokio::spawn(async move {
                while let Some(value) = rx.recv().await {
                    let mut count = counter.lock().await;
                    *count += value;
                }
            });

            workers.push(tx);
        }

        Self { counter, workers }
    }
}
