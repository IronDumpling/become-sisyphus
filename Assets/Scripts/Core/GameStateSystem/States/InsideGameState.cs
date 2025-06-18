using UnityEngine;

namespace BecomeSisyphus.Core.GameStateSystem
{
    /// <summary>
    /// 游戏内状态（包含OutsideWorld和InsideWorld）
    /// </summary>
    public class InsideGameState : BaseGameState
    {
        public InsideGameState() : base("InsideGame")
        {
        }

        public override void OnEnter(IGameState previousState)
        {
            base.OnEnter(previousState);
            
            Debug.Log("InsideGameState: Entering game");
            
            // 游戏开始逻辑
            // - 初始化游戏系统
            // - 加载游戏数据
            // - 启动游戏循环等
            
            // 默认进入OutsideWorld/Climbing状态
            if (CurrentSubState == null)
            {
                SwitchToSubState("OutsideWorld");
            }
        }

        public override void OnExit(IGameState nextState)
        {
            base.OnExit(nextState);
            
            Debug.Log("InsideGameState: Exiting game");
            
            // 游戏结束逻辑
            // - 保存游戏数据
            // - 清理资源等
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            // 游戏全局更新逻辑
            // - 时间系统更新
            // - 全局状态检查等
        }

        /// <summary>
        /// 切换到外部世界
        /// </summary>
        public void SwitchToOutsideWorld()
        {
            SwitchToSubState("OutsideWorld");
        }

        /// <summary>
        /// 切换到内部世界
        /// </summary>
        public void SwitchToInsideWorld()
        {
            SwitchToSubState("InsideWorld");
        }

        /// <summary>
        /// 返回主菜单
        /// </summary>
        public void ReturnToMainMenu()
        {
            GameStateManager.Instance.SwitchToRootState("MainMenu");
        }
    }
} 