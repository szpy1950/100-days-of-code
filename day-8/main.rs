// === Solution 4: File Processing Pipeline ===
use tokio::fs::File;
use tokio::io::{AsyncReadExt, AsyncWriteExt};
use tokio::sync::mpsc;

async fn process_file(input: &str, output: &str) -> Result<(), Box<dyn std::error::Error>> {
    let mut input_file = File::open(input).await?;
    let mut output_file = File::create(output).await?;
    let (tx, mut rx) = mpsc::channel(32);

    // Reader task
    let reader = tokio::spawn(async move {
        let mut buffer = Vec::new();
        input_file.read_to_end(&mut buffer).await?;
        tx.send(buffer).await?;
        Ok::<_, Box<dyn std::error::Error>>(())
    });

    // Processor and writer task
    while let Some(data) = rx.recv().await {
        // Process data (e.g., convert to uppercase)
        let processed = String::from_utf8_lossy(&data).to_uppercase();
        output_file.write_all(processed.as_bytes()).await?;
    }

    reader.await??;
    Ok(())
}
