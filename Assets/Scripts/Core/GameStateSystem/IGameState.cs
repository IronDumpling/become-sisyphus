using UnityEngine;
using System.Collections.Generic;

namespace BecomeSisyphus.Core.GameStateSystem
{
    /// <summary>
    /// 游戏状态的基础接口
    /// </summary>
    public interface IGameState
    {
        /// <summary>
        /// 状态名称
        /// </summary>
        string StateName { get; }
        
        /// <summary>
        /// 父状态（如果有的话）
        /// </summary>
        IGameState ParentState { get; set; }
        
        /// <summary>
        /// 子状态列表
        /// </summary>
        Dictionary<string, IGameState> SubStates { get; }
        
        /// <summary>
        /// 当前活跃的子状态
        /// </summary>
        IGameState CurrentSubState { get; }
        
        /// <summary>
        /// 进入状态时调用
        /// </summary>
        /// <param name="previousState">上一个状态</param>
        void OnEnter(IGameState previousState);
        
        /// <summary>
        /// 退出状态时调用
        /// </summary>
        /// <param name="nextState">下一个状态</param>
        void OnExit(IGameState nextState);
        
        /// <summary>
        /// 状态更新
        /// </summary>
        void OnUpdate();
        
        /// <summary>
        /// 状态暂停（当子状态激活时）
        /// </summary>
        void OnPause();
        
        /// <summary>
        /// 状态恢复（当子状态退出时）
        /// </summary>
        void OnResume();
        
        /// <summary>
        /// 添加子状态
        /// </summary>
        void AddSubState(IGameState subState);
        
        /// <summary>
        /// 切换到子状态
        /// </summary>
        void SwitchToSubState(string stateName);
        
        /// <summary>
        /// 检查是否可以切换到指定状态
        /// </summary>
        bool CanTransitionTo(string stateName);
        
        /// <summary>
        /// 检查当前状态或子状态是否包含指定状态名
        /// </summary>
        bool IsInState(string stateName);
        
        /// <summary>
        /// 获取当前活跃的叶子状态（最深层的活跃状态）
        /// </summary>
        IGameState GetActiveLeafState();
        
        /// <summary>
        /// 获取完整的状态路径
        /// </summary>
        string GetFullStatePath();
    }
} 