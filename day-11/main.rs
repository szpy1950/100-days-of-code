// === Solution 7: Async Cache with TTL ===
use std::collections::HashMap;
use tokio::sync::RwLock;
use tokio::time::{Duration, Instant};

struct CacheEntry<T> {
    value: T,
    expires_at: Instant,
}

struct AsyncCache<K, V> {
    data: Arc<RwLock<HashMap<K, CacheEntry<V>>>>,
    ttl: Duration,
}

impl<K: Eq + std::hash::Hash + Clone, V: Clone> AsyncCache<K, V> {
    async fn insert(&self, key: K, value: V) {
        let mut data = self.data.write().await;
        data.insert(key, CacheEntry {
            value,
            expires_at: Instant::now() + self.ttl,
        });
    }

    async fn get(&self, key: &K) -> Option<V> {
        let data = self.data.read().await;
        data.get(key)
        .filter(|entry| entry.expires_at > Instant::now())
        .map(|entry| entry.value.clone())
    }
}
