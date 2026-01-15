# Layers Are for Lasagna : Embracing Vertical Slice Architecture
Talk by Jonathan "J." Tower

Is your codebase fragile, where one small change risks breaking something unexpected? That fragility often comes from layered or “lasagna” architectures that scatter logic across controllers, services, repositories, and models. Touching one feature means touching several layers, and every change feels slow and risky. Vertical Slice Architecture takes a different approach.

Instead of organizing by technical layer, it organizes by feature. Each slice contains everything it needs, from input to persistence, and stands largely on its own. This makes features easier to find, easier to understand, and easier to change without worrying about unintended ripple effects. It also gives you the freedom to pick the right implementation for each case, whether that means EF Core, Dapper, or raw SQL.

In this talk, we will explore how to shift from rigid layered designs to feature-oriented slices, and how that shift creates codebases that grow more naturally and stay easier to maintain. Through practical .NET examples, you will see how vertical slicing encourages clarity, flexibility, and confidence when building or refactoring applications. Layers might be fine for lasagna, but your architecture deserves something better.


## Let's Talk:
https://bit.ly/th-offer
