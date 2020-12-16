﻿using Clustering.Core;
using Clustering.KMeans;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KMeansGridVisualizer : AbstractGridVisualizer
{

    #region Constructors

    /// <summary>
    /// Default constructor.
    /// </summary>
    public KMeansGridVisualizer(
        AlgorithmManagerScriptableObject algorithmManager,
        Grid gridManager)
        : base(algorithmManager, gridManager)
    {
    }

    #endregion

    #region Fields/Properties

    /// <summary>
    /// References the algorithm scriptable object.
    /// </summary>
    private KMeansAlgorithm Algorithm
    {
        get
        {
            return (KMeansAlgorithm)AlgorithmManager.CurrentAlgorithm;
        }
    }

    #endregion

    #region Color Methods

    /// <summary>
    /// Gets all cluster ids.
    /// </summary>
    protected override List<Guid> GetClusterIds()
    {
        return Algorithm.Iterations.First().Clusters.Select(x => x.Id).ToList();
    }

    #endregion

    #region Seed Methods

    /// <summary>
    /// Retrieves the list of clusters at iteration 0.
    /// </summary>
    protected override List<Item> GetSeedClusters()
    {
        return Algorithm.ClusterSeeds;
    }

    #endregion

    #region Iteration Methods

    /// <summary>
    /// Gets the number of steps that we will display.
    /// </summary>
    public override int GetIterationsCount()
    {
        return Algorithm.Iterations.Count * 2;
    }

    /// <summary>
    /// Retrieves the order of the iteration.
    /// </summary>
    protected override int GetIterationOrder(int iterationNumber)
    {
        return (int)Math.Ceiling(iterationNumber / 2f);
    }

    /// <summary>
    /// Displays the entities of an iteration.
    /// </summary>
    protected override void DisplayIteration(int iterationNumber, int order,
        Iteration previousIteration, Iteration currentIteration)
    {
        // The end
        if (previousIteration == null && currentIteration == null)
            return;

        // Display entities
        DisplayEntities(previousIteration, currentIteration, iterationNumber != order * 2);

        // Display cluster paths
        DisplayPaths(iterationNumber != order * 2 ? previousIteration : currentIteration);
    }

    /// <summary>
    /// Displays the entities of an iteration.
    /// </summary>
    private void DisplayEntities(Iteration previousIteration, Iteration currentIteration, bool keepPreviousCenters)
    {
        // Classify clusters by their ids
        Dictionary<Guid, Cluster> previousClusters = previousIteration.Clusters.ToDictionary(x => x.Id, x => x);
        Dictionary<Guid, Cluster> currentClusters = currentIteration?.Clusters.ToDictionary(x => x.Id, x => x);

        // Go through each cluster
        List<Cluster> clusters = currentIteration?.Clusters ?? previousIteration.Clusters;
        foreach (Cluster cluster in clusters)
        {
            // Display its items
            List<Vector2> seedItems = cluster.Items.Select(x => new Vector2(x.PositionX, x.PositionY)).ToList();
            GridManager.DisplayEntities(seedItems, ClusterColors[cluster.Id]);

            // Display cluster
            KMeansCluster kmeanCluster = (keepPreviousCenters ? previousClusters[cluster.Id] : cluster) as KMeansCluster;
            Vector2 clusterCenter = new Vector2(kmeanCluster.CenterX, kmeanCluster.CenterY);
            GridManager.DisplayEntities(new List<Vector2>() { clusterCenter }, ClusterColors[cluster.Id], false, 45f);

            // Display cluster lines
            if (keepPreviousCenters)
                foreach (var item in seedItems)
                    GridManager.DisplayPaths(new List<Vector2>() { clusterCenter, item });
        }
    }

    /// <summary>
    /// Displays the paths of clusters till this iteration.
    /// </summary>
    private void DisplayPaths(Iteration iteration)
    {
        // Initialize the path list
        Dictionary<Guid, List<Vector2>> clusterPaths = new Dictionary<Guid, List<Vector2>>();

        // Populate with the list of clusters
        foreach (Cluster cluster in iteration.Clusters)
            clusterPaths.Add(cluster.Id, new List<Vector2>());

        // Get the history of each cluster
        foreach (Cluster cluster in iteration.Clusters)
        {
            List<Cluster> history = new List<Cluster>();

            // Go through each iteration and find this cluster
            foreach (var it in AlgorithmManager.CurrentAlgorithm.Iterations)
                if (it.Order <= iteration.Order)
                {
                    Cluster historyCluster = it.Clusters.FirstOrDefault(x => x.Id == cluster.Id);
                    if (historyCluster != null)
                        history.Add(historyCluster);
                }

            // Calculate paths
            foreach (KMeansCluster historyCluster in history)
                clusterPaths[cluster.Id].Add(new Vector2(historyCluster.CenterX, historyCluster.CenterY));
        }

        // Display paths
        foreach (Guid clusterId in clusterPaths.Keys)
            GridManager.DisplayPaths(clusterPaths[clusterId], ClusterColors[clusterId]);
    }

    #endregion

}