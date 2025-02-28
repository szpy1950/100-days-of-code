// === Solution 8: Async Event Bus ===
use tokio::sync::broadcast;
use std::collections::HashMap;

struct EventBus {
    topics: Arc<RwLock<HashMap<String, broadcast::Sender<Vec<u8>>>>>,
}

impl EventBus {
    async fn publish(&self, topic: &str, message: Vec<u8>) -> Result<(), Box<dyn std::error::Error>> {
        let topics = self.topics.read().await;
        if let Some(sender) = topics.get(topic) {
            sender.send(message)?;
        }
        Ok(())
    }

    async fn subscribe(&self, topic: &str) -> broadcast::Receiver<Vec<u8>> {
        let mut topics = self.topics.write().await;
        let sender = topics
        .entry(topic.to_string())
        .or_insert_with(|| broadcast::channel(32).0);
        sender.subscribe()
    }
}
