// === Solution 5: Rate-Limited API Client ===
use std::sync::Arc;
use tokio::sync::Mutex;
use tokio::time::{sleep, Duration, Instant};

struct RateLimiter {
    tokens: Arc<Mutex<u32>>,
    max_tokens: u32,
    refill_rate: Duration,
    last_refill: Arc<Mutex<Instant>>,
}

impl RateLimiter {
    async fn acquire_token(&self) -> bool {
        let mut tokens = self.tokens.lock().await;
        let mut last_refill = self.last_refill.lock().await;

        let now = Instant::now();
        let elapsed = now.duration_since(*last_refill);
        let new_tokens = (elapsed.as_secs_f32() / self.refill_rate.as_secs_f32()) as u32;

        *tokens = (*tokens + new_tokens).min(self.max_tokens);
        *last_refill = now;

        if *tokens > 0 {
            *tokens -= 1;
            true
        } else {
            false
        }
    }
}
