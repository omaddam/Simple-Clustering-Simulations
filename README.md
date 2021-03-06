# Simple Clustering Simulations 

This project provides the implementation of three clustering algorithms in Unity3d.

` IMPORTANT: The code is not optimized for handling large number of items. Instead, it was optimized for simulation purposes by storing all the iterations and their respective data. `

| K-Means | K-Medoids | DB-Scan |
| :-----: | :-------: | :-----: |
| <img src="docs/KMeans.png" height="150" /> | <img src="docs/KMedoids.jpg" height="150" />   | <img src="docs/DBScan.jpg" height="150" /> |
| [wikipedia](https://en.wikipedia.org/wiki/K-means_clustering) | [wikipedia](https://en.wikipedia.org/wiki/K-medoids)   | [wikipedia](https://en.wikipedia.org/wiki/DBSCAN) |

# Demo

![Simulation](docs/Simulation.gif)

We created a demo that simulates and visualizes the three algorithms with two predefined samples.
A complete demo can be found on [https://omaddam.github.io/Simple-Clustering-Simulations/](https://omaddam.github.io/Simple-Clustering-Simulations/).

# Getting Started

These instructions will get you a copy of the project on your local machine for development and testing purposes.

### Prerequisites

The things you need to install before you proceed with development:

1) [Unity3d (2020.2.0f1)](https://unity3d.com/get-unity/download/archive) [required].

### Installing

A step by step guide to get you started with development.

#### Download, clone, and setup the repository

```git
git clone https://github.com/omaddam/Simple-Clustering-Simulations.git
```

#### Initialize git flow

```git
git flow init
```

# Standards

### General Standards

* Line ending: CRLF
* Case styles: Camel, Pascal, and Snake case
  * Arguments, paramters, and local variables: camel case (e.g. iterationOrder)
  * Global variables: pascal case (e.g. SeedItems)
  * Constants and static variables: snake case (ALL CAPS) (e.g. ALGORITHM_NAME)
* Methods naming convention:
  * Pascal case (e.g. GenerateSample)
  * Verbs

### Commenting Standards

* `///` Summaries: Full-usage of English grammar and punctuation. (e.g. Add periods to the end of your summaries, as if you were writing a phrase or sentence.)
*  `//` In-line comments: quick, point-form. Grammar and punctuation not needed

### Third Party Packages

* All packages should be included under Assets/ThirdParty folder.
* Contains all packages downloaded from the Unity3d store.

### Assets / App

* Contains scripts, scenes, and all resources used to create the demo application.
* Scripts are created under Assets/App/Scripts folder.

### Assets / Others

* All components should be included under Assets/\<Name> folder. (e.g. Assets/Grid)
* Each component should be isolated and under **NO CIRCUMSTANCES** referencing or using another component's scripts.
* Components are **NOT** allowed to reference or call application scripts.
* Components are allowed to reference and use ThirdParty scripts.

# Code Based Documentation

## Assets / Clustering

This folder contains all the scripts of the clustering component. The core scripts are in the *Core* folder while the rest of the scripts are in their proper implementation folders.

### Core

The core contains 4 classes as shown in the figure below.

![Simulation](docs/ClusteringCore.png)

* **AbstractAlgorithm** is an abstract class that defines the flow of all the implemented algorithms.
  * Properties
    * **Items**: list of all items that are to be clustered.
    * **Iterations**: list of all computed iterations. Gets populated upon calling the *Compute* method.
  * Methods
    * **Compute**: a public method that uses the two other methods to generate the iterations.
    * **InitializeClusters**: an abstract method that gets implemented by the different clustering algorithms. This method is used to initialize the first set of clusters used in the first iteration.
    * **ComputeNextIteration**: an abstract method that gets implemented by the different clustering algorithms. Tries to compute the next iteration. If there are no changes to the clusters, the method will return null, identifying the end.

* **Iteration**: represents a single state in an algorithm. It stores all the clusters with their items at a specific snapshot.

* **Cluster**: represents a list of items grouped by their similarities. In the case of this implementation, they are grouped by their physical location's cohesion.
  
* **Item**: represents a single point in a 2D graph.

### KMeans

In a KMeans algorithm, the initial set of clusters is defined by either creating the centers of the clusters randomly or predefining them.

* **KMeansCluster**: extends the *Cluster* class to include two additional properties and one method.
  * Properties
    * **CenterX**: the x position of the center of the cluster.
    * **CenterY**: the y position of the center of the cluster.
  * Methods
    * **RecomputeCenter**: recomputes the center of the cluster to set it at an average distance from all items in that cluster.

* **KMeansAlgorithm**: implements the *AbstractAlgorithm* class to perform the k-means approach.

### KMedoids

This algorithm is very similar to the k-means algorithm. However, rather than having an average floating point represent a cluster, one of the cluster's items is set as the centroid.

* **KMedoidsCluster**: extends the *Cluster* class to include two additional properties and one method.
  * Properties
    * **CenterId**: the id of the item representing the center of the cluster.
    * **Centroid**: the item representing the center of the cluster.
  * Methods
    * **RecomputeCenter**: finds the item that represents the center of the cluster.
  
* **KMedoidsAlgorithm**: implements the *AbstractAlgorithm* class to perform the k-medoids approach.

### DBScan

In a DB-Scan algorithm, clusters are not predefined. Instead they are created arbitrary.

* **DBScanCluster**: extends the *Cluster* class to reference items added from the previous iteration. Those items are use in the next iteration to scan for their neighbors.

* **DBScanIteration**: extends the *Iteration* class to include two additional properties.
  * Properties
    * Pending: list of items pending to be clustered. Unlike the previous implemented algorithms, not all items get clustered every iteration, instead they are clustered incrementally.
    * Noise: list of items that were processed and found not to belong to any cluster.

* **DBScanAlgorithm**: implements the *AbstractAlgorithm* class to perform the db-scan approach.

## Assets / Grid

This folder contains a grid system that allows us to visualize all the clustering algorithms. The system consists of three components.

### GridEntity

A *GridEntity* represents a 2D point on a graph. It is used as a template by the grid component when displaying the items.
* Prefab: Assets/Grid/Prefabs/GridEntity.prefab
* Script: Assets/Grid/Scripts/GridEntity.cs
  * SetColor: sets the color of the represented point. Used to set the color of the point to show the cluster it belongs to.

### GridPath

A *GridPath* is used to to draw lines and circles.. It is used as a template by the grid component when displaying the trail of the center of the clusters, the scaning circle for nearby neighbors, etc.
* Prefab: Assets/Grid/Prefabs/GridPath.prefab
* Script: Assets/Grid/Scripts/GridPath.cs
  * SetPath: draws a line between two points.
  * SetCircle: draws a circle around a point.

### Grid

A grid system that uses the two prefabs; *GridEntity* and *GridPath*; to visualize the iterations of the clustering algorithms.
* Prefab: Assets/Grid/Prefabs/Grid.prefab
* Script: Assets/Grid/Scripts/Grid.cs
  * Clear: deletes all entities and paths in the grid.
  * DisplayEntities: displays entities on the grid.
  * DisplayPaths: displays paths on the grid.
  * DisplayCircularBoundary: displays circular boundaries.