# Open Questions

## Client Version And Resources

- What is the exact target client version?
- Which client resource files must be checked by size and SHA-256?
- What resource version mismatch should block startup?

## Protocol

- What is the target client's exact XOR behavior?
- Which AC0 response sequence keeps the client connected?
- What exact AC63/4 fields does the target client send?
- What exact AC63/1 character-slot structure does the target client require?
- What packet sequence is required after AC63/2 character selection?

## Testing

- What sanitized traces are available first?
- Which replay case should become the first acceptance test?
