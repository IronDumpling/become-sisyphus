using System;
using System.Collections.Generic;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Core.Interfaces;

namespace BecomeSisyphus.Systems
{
    public class ThoughtVesselSystem : ISystem
    {
        private int initialRows = 3;
        private int initialColumns = 3;
        private float loadRatioThreshold = 0.5f;
        private float mentalStrengthConsumptionRate = 2f;

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

        public ThoughtVesselSystem(int initialRows, int initialColumns, float loadRatioThreshold, float mentalStrengthConsumptionRate)
        {
            this.initialRows = initialRows;
            this.initialColumns = initialColumns;
            this.loadRatioThreshold = loadRatioThreshold;
            this.mentalStrengthConsumptionRate = mentalStrengthConsumptionRate;
        }

        public void Initialize()
        {
            // mindSystem should be set externally or via a setter
            rows = initialRows;
            columns = initialColumns;
            grid = new VesselCargo[rows, columns];
        }

        public void Update() { }

        public void Update(float deltaTime)
        {
            if (mindSystem != null && LoadRatio >= loadRatioThreshold)
            {
                mindSystem.ConsumeMentalStrength(mentalStrengthConsumptionRate * deltaTime, 0f);
            }
        }

        public void SetMindSystem(SisyphusMindSystem system)
        {
            mindSystem = system;
        }

        public bool AddCargo(VesselCargo cargo, UnityEngine.Vector2Int position)
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

        public bool MoveCargo(VesselCargo cargo, UnityEngine.Vector2Int newPosition)
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
            UnityEngine.Vector2Int pos = cargo.position;
            if (IsValidPosition(pos) && grid[pos.x, pos.y] == cargo)
            {
                grid[pos.x, pos.y] = null;
                OnCargoRemoved?.Invoke(cargo);
                OnLoadRatioChanged?.Invoke(LoadRatio);
                return true;
            }
            return false;
        }

        private void PlaceCargo(VesselCargo cargo, UnityEngine.Vector2Int position)
        {
            grid[position.x, position.y] = cargo;
            cargo.SetPosition(position);
        }

        private bool CanPlaceCargo(VesselCargo cargo, UnityEngine.Vector2Int position)
        {
            // TODO: 实现检查是否可以放置货物的逻辑
            return true;
        }

        private bool IsValidPosition(UnityEngine.Vector2Int position)
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