Generic Hash Map & Algorithm Visualizer
An interactive educational tool designed to visualize the internal mechanics of a Generic Hash Map, developed to bridge the gap between theoretical Data Structures & Algorithms (DSA) and practical software implementation.
Generic Data Structure: Developed a custom MyHashMap<TKey, TValue> from scratch, independent of standard library collections.
Collision Resolution: Implemented Separate Chaining using linked lists to manage hash collisions effectively.
Manual Hashing Logic: Features a custom index calculation process to demonstrate the mapping of keys to bucket arrays.

Visualization Engine
  This project uses a custom state-machine to decompose complex operations into observable steps:
  Index Calculation: Visualizes the hash function output for a specific key.
  Bucket Traversal: Animates the search through a specific bucket, highlighting key comparisons during a collision.
  State Feedback: Utilizes color-coded UI states (Red for index selection, Orange for comparison, Green for success) to represent the internal state of the map.

Software Architecture
  Pattern: MVVM (Model-View-ViewModel) for clean separation of algorithmic logic and visualization.
  Tech Stack: C#, .NET, WPF (Windows Presentation Foundation).
How to Run:
  Clone the repository: git clone https://github.com/lilmaymo/Generic-HashMap-Visualizer.git
  Open the solution in Visual Studio.
  Build and Run (F5).
  write how long the array should be then click plus icon
  Use the AutoFill feature to observe how the map handles multiple collisions simultaneously.
  select option of insert/delete and check the autofill to show the steps
