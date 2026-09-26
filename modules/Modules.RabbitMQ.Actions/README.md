# RabbitMQ actions

Connect with an `amqp://` or `amqps://` URI, use the resulting **Connection** variable for subsequent actions, and close it when the flow finishes. Build URIs containing credentials from protected variables; the connection displays only the broker endpoint.

| Action | Purpose |
| --- | --- |
| Connect to RabbitMQ | Open a broker connection and a confirmed-publishing channel. |
| Publish RabbitMQ message | Send UTF-8 text with broker confirmation and mandatory routing. |
| Get RabbitMQ message | Poll an existing queue once; return **Found** and an unsettled message. |
| Acknowledge RabbitMQ message | Confirm processing succeeded. |
| Reject RabbitMQ message | Reject a delivery, optionally requeuing it. |
| Close RabbitMQ connection | Release the connection and channel. |

For the default exchange, leave **Exchange** empty and set **Routing key** to an existing queue name. Messages are marked persistent by default; durable storage also requires a durable queue. A publisher confirmation means the broker accepted the message, not that a consumer processed it.

**Get RabbitMQ message** polls once without waiting for a new arrival. If **Found** is false, **Message** is null. Its `BodyBase64` preserves arbitrary binary payloads; decode it when the flow expects text. After successfully processing a message, use **Acknowledge**. On failure, use **Reject**; **Requeue** defaults to false so a poison message does not loop indefinitely. Closing a channel requeues any still-unacknowledged deliveries. Always close the connection in the flow's cleanup path.

Polling is appropriate for occasional desktop operations, not high-throughput subscriptions. Queue and exchange declaration are deliberately outside this module.
