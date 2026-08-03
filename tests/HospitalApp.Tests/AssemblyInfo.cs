// PHASE 2

using Xunit;

// The HospitalSystem singleton is shared mutable state, and each test class
// resets it in its constructor. Run tests sequentially so those resets cannot
// race across test classes.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
