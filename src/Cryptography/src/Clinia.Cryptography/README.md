# Cryptography utilities

Implements a standalone hasher with different possible implementations.
The supported implemantations are:
- PBKDF2

Note that under the hood it uses Microsoft.AspNetCore.Cryptography.KeyDerivation to compare 
an already hashed string with a provided one.

The `IHasher` interface has been adapted from `AspNet Identity`. It can be registered with different implementations
if need be as well as is not tied to any entities from the `AspNet Identity` domain model.