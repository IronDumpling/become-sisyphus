using UnityEngine;

namespace BecomeSisyphus.Core.GameStateSystem
{
    /// <summary>
    /// 主菜单状态
    /// </summary>
    public class MainMenuState : BaseGameState
    {
        public MainMenuState() : base("MainMenu")
        {
        }

        public override void OnEnter(IGameState previousState)
        {
            base.OnEnter(previousState);
            
            // 主菜单进入逻辑
            Debug.Log("MainMenuState: Setting up main menu UI");
            
            // TEMPORARY: Auto-transition to game for prototype
            Debug.Log("MainMenuState: Auto-transitioning to OutsideWorld for prototype");
            StartGame();
            
            // 可以在这里添加：
            // - 显示主菜单UI
            // - 播放背景音乐
            // - 重置游戏数据等
        }

        public override void OnExit(IGameState nextState)
        {
            base.OnExit(nextState);
            
            // 主菜单退出逻辑
            Debug.Log("MainMenuState: Cleaning up main menu");
            
            // 可以在这里添加：
            // - 隐藏主菜单UI
            // - 停止背景音乐
            // - 开始游戏准备等
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            // 主菜单更新逻辑
            // 可以添加动画、特效等
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            Debug.Log("MainMenuState: Starting new game");
            GameStateManager.Instance.SwitchToState("InsideGame/OutsideWorld/MountainFoot");
        }

        /// <summary>
        /// 继续游戏
        /// </summary>
        public void ContinueGame()
        {
            Debug.Log("MainMenuState: Continuing saved game");
            // 这里可以加载保存的游戏状态
            GameStateManager.Instance.SwitchToRootState("InsideGame");
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("MainMenuState: Quitting game");
            Application.Quit();
        }
    }
} 