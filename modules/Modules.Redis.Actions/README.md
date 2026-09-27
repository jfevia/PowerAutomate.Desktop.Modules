# Redis actions

Connect to Redis once, pass the resulting **Connection** variable to subsequent actions, and close it when the flow finishes. The connection is reused rather than reopened for every key operation.

| Action | Purpose |
| --- | --- |
| Connect to Redis | Connect with a StackExchange.Redis configuration string and optional database number. |
| Get Redis value | Read a string value and a separate **Found** flag. |
| Set Redis value | Store a string value, optionally expiring it after a positive number of seconds. |
| Delete Redis key | Remove a key and return whether it existed. |
| Close Redis connection | Release the connection; closing twice is safe. |

Use a credential or other protected variable when building a configuration string containing a password. The connection variable never displays that string. Use a nonnegative database number, or `-1` to use the configured default. **Expiry in seconds** defaults to zero (no expiry); negative values are rejected. These actions operate on Redis string keys, not hashes, lists, or pub/sub.
