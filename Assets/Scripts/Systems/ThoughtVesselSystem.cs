using UnityEngine;
using System;
using System.Collections.Generic;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;

namespace BecomeSisyphus.Systems
{
    public class ThoughtVesselSystem : MonoBehaviour, ISystem
    {
        [SerializeField] private int initialRows = 3;
        [SerializeField] private int initialColumns = 3;
        [SerializeField] private float loadRatioThreshold = 0.5f;
        [SerializeField] private float mentalStrengthConsumptionRate = 2f;

        private VesselCargo[,] grid;
        private SisyphusMindSystem mindSystem;
        private int rows;
        private int columns;

        public event Action<float> OnLoadRatioChanged;
        public event Action<VesselCargo> OnCargoAdded;
        public event Action<VesselCargo> OnCargoRemoved;
        public event Action<VesselCargo> OnCargoMoved;
        public event Action<VesselCargo> OnCargoRotated;

        public float LoadRatio => (float)GetOccupiedCells() / (rows * columns);

        public void Initialize()
        {
            mindSystem = GameManager.Instance.GetSystem<SisyphusMindSystem>();
            rows = initialRows;
            columns = initialColumns;
            grid = new VesselCargo[rows, columns];
        }

        public void Update()
        {
            if (LoadRatio >= loadRatioThreshold)
            {
                mindSystem.ConsumeMentalStrength(mentalStrengthConsumptionRate * Time.deltaTime);
            }
        }

        public bool AddCargo(VesselCargo cargo, Vector2Int position)
        {
            if (!IsValidPosition(position) || !CanPlaceCargo(cargo, position))
            {
                return false;
            }

            PlaceCargo(cargo, position);
            OnCargoAdded?.Invoke(cargo);
            OnLoadRatioChanged?.Invoke(LoadRatio);
            return true;
        }

        public bool MoveCargo(VesselCargo cargo, Vector2Int newPosition)
        {
            if (!IsValidPosition(newPosition) || !CanPlaceCargo(cargo, newPosition))
            {
                return false;
            }

            RemoveCargo(cargo);
            PlaceCargo(cargo, newPosition);
            OnCargoMoved?.Invoke(cargo);
            OnLoadRatioChanged?.Invoke(LoadRatio);
            return true;
        }

        public void RotateCargo(VesselCargo cargo)
        {
            cargo.Rotate();
            OnCargoRotated?.Invoke(cargo);
        }

        public bool RemoveCargo(VesselCargo cargo)
        {
            Vector2Int pos = cargo.position;
            if (IsValidPosition(pos) && grid[pos.x, pos.y] == cargo)
            {
                grid[pos.x, pos.y] = null;
                OnCargoRemoved?.Invoke(cargo);
                OnLoadRatioChanged?.Invoke(LoadRatio);
                return true;
            }
            return false;
        }

        private void PlaceCargo(VesselCargo cargo, Vector2Int position)
        {
            grid[position.x, position.y] = cargo;
            cargo.SetPosition(position);
        }

        private bool CanPlaceCargo(VesselCargo cargo, Vector2Int position)
        {
            // TODO: 实现检查是否可以放置货物的逻辑
            return true;
        }

        private bool IsValidPosition(Vector2Int position)
        {
            return position.x >= 0 && position.x < rows && position.y >= 0 && position.y < columns;
        }

        private int GetOccupiedCells()
        {
            int count = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (grid[i, j] != null)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public void ExpandVessel(int newRows, int newColumns)
        {
            if (newRows <= rows && newColumns <= columns)
            {
                return;
            }

            VesselCargo[,] newGrid = new VesselCargo[newRows, newColumns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    newGrid[i, j] = grid[i, j];
                }
            }

            grid = newGrid;
            rows = newRows;
            columns = newColumns;
            OnLoadRatioChanged?.Invoke(LoadRatio);
        }

        public void Cleanup()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    grid[i, j] = null;
                }
            }
            OnLoadRatioChanged = null;
            OnCargoAdded = null;
            OnCargoRemoved = null;
            OnCargoMoved = null;
            OnCargoRotated = null;
        }
    }
} 