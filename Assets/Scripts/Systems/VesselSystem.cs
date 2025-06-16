using UnityEngine;
using System;
using System.Collections.Generic;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Core.Interfaces;

namespace BecomeSisyphus.Systems
{
    public class VesselSystem : ISystem
    {
        private int rows;
        private int columns;
        private VesselCargo[,] grid;
        private float loadRatio;
        private float loadRatioThreshold = 0.75f;
        private SisyphusMindSystem mindSystem;

        public event Action<float> OnLoadRatioChanged;
        public event Action<VesselCargo> OnCargoAdded;
        public event Action<VesselCargo> OnCargoRemoved;
        public event Action<VesselCargo> OnCargoMoved;
        public event Action<VesselCargo> OnCargoRotated;
        public event Action<VesselCargo, VesselCargo> OnCargoCombined;

        public void Initialize()
        {
            mindSystem = GameManager.Instance.GetSystem<SisyphusMindSystem>();
            InitializeGrid(3, 3); // 初始3x3网格
        }

        public void Update()
        {
            UpdateLoadRatio();
            CheckMentalStrain();
        }

        public bool AddCargo(VesselCargo cargo, Vector2Int position)
        {
            if (!IsValidPosition(position) || !CanPlaceCargo(cargo, position))
                return false;

            PlaceCargo(cargo, position);
            UpdateLoadRatio();
            OnCargoAdded?.Invoke(cargo);
            return true;
        }

        public bool MoveCargo(VesselCargo cargo, Vector2Int newPosition)
        {
            if (!IsValidPosition(newPosition) || !CanPlaceCargo(cargo, newPosition))
                return false;

            RemoveCargo(cargo);
            PlaceCargo(cargo, newPosition);
            OnCargoMoved?.Invoke(cargo);
            return true;
        }

        public void RotateCargo(VesselCargo cargo)
        {
            Vector2Int pos = cargo.position;
            if (CanRotateAt(cargo, pos))
            {
                RemoveCargo(cargo);
                cargo.Rotate();
                PlaceCargo(cargo, pos);
                OnCargoRotated?.Invoke(cargo);
            }
        }

        public bool TryCombineCargo(VesselCargo cargo1, VesselCargo cargo2)
        {
            if (cargo1.CanCombineWith(cargo2))
            {
                cargo1.CombineWith(cargo2);
                OnCargoCombined?.Invoke(cargo1, cargo2);
                return true;
            }
            return false;
        }

        private void InitializeGrid(int rows, int columns)
        {
            this.rows = rows;
            this.columns = columns;
            grid = new VesselCargo[rows, columns];
        }

        private void UpdateLoadRatio()
        {
            int occupied = 0;
            int total = rows * columns;
            
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    if (grid[i, j] != null)
                        occupied++;

            loadRatio = (float)occupied / total;
            OnLoadRatioChanged?.Invoke(loadRatio);
        }

        private void CheckMentalStrain()
        {
            if (loadRatio >= loadRatioThreshold && mindSystem != null)
            {
                mindSystem.ConsumeMentalStrength(Time.deltaTime, Time.deltaTime);
            }
        }

        private bool IsValidPosition(Vector2Int position)
        {
            return position.x >= 0 && position.x < rows && 
                   position.y >= 0 && position.y < columns;
        }

        private bool CanPlaceCargo(VesselCargo cargo, Vector2Int position)
        {
            if (!IsValidPosition(position))
                return false;

            // 检查目标位置是否已被占用
            if (grid[position.x, position.y] != null)
                return false;

            // 检查货物尺寸是否超出网格
            int width = cargo.isRotated ? cargo.height : cargo.width;
            int height = cargo.isRotated ? cargo.width : cargo.height;

            if (position.x + width > rows || position.y + height > columns)
                return false;

            // 检查所需空间是否都是空的
            for (int i = position.x; i < position.x + width; i++)
                for (int j = position.y; j < position.y + height; j++)
                    if (grid[i, j] != null)
                        return false;

            return true;
        }

        private bool CanRotateAt(VesselCargo cargo, Vector2Int position)
        {
            // 临时移除当前货物
            RemoveCargo(cargo);

            // 检查旋转后是否可以放置
            cargo.Rotate();
            bool canRotate = CanPlaceCargo(cargo, position);

            // 恢复原状
            cargo.Rotate();
            PlaceCargo(cargo, position);

            return canRotate;
        }

        private void PlaceCargo(VesselCargo cargo, Vector2Int position)
        {
            int width = cargo.isRotated ? cargo.height : cargo.width;
            int height = cargo.isRotated ? cargo.width : cargo.height;

            for (int i = position.x; i < position.x + width; i++)
                for (int j = position.y; j < position.y + height; j++)
                    grid[i, j] = cargo;

            cargo.SetPosition(position);
        }

        private void RemoveCargo(VesselCargo cargo)
        {
            Vector2Int pos = cargo.position;
            int width = cargo.isRotated ? cargo.height : cargo.width;
            int height = cargo.isRotated ? cargo.width : cargo.height;

            for (int i = pos.x; i < pos.x + width; i++)
                for (int j = pos.y; j < pos.y + height; j++)
                    if (IsValidPosition(new Vector2Int(i, j)) && grid[i, j] == cargo)
                        grid[i, j] = null;

            OnCargoRemoved?.Invoke(cargo);
        }

        public void Cleanup()
        {
            grid = null;
            OnLoadRatioChanged = null;
            OnCargoAdded = null;
            OnCargoRemoved = null;
            OnCargoMoved = null;
            OnCargoRotated = null;
            OnCargoCombined = null;
        }
    }
} 