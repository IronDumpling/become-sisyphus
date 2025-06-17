using UnityEngine;

namespace BecomeSisyphus.Core.GameStateSystem
{
    /// <summary>
    /// 外部世界状态
    /// </summary>
    public class OutsideWorldState : BaseGameState
    {
        public OutsideWorldState() : base("OutsideWorld")
        {
        }

        public override void OnEnter(IGameState previousState)
        {
            base.OnEnter(previousState);
            
            Debug.Log("OutsideWorldState: Entering outside world");
            
            // 默认进入Climbing状态
            if (CurrentSubState == null)
            {
                SwitchToSubState("Climbing");
            }
        }

        public override void OnExit(IGameState nextState)
        {
            base.OnExit(nextState);
            Debug.Log("OutsideWorldState: Exiting outside world");
        }

        public override bool CanTransitionTo(string stateName)
        {
            // 外部世界状态转换规则
            switch (stateName)
            {
                case "MountainFoot":
                case "Climbing":
                case "Perception":
                case "MountainTop":
                    return true;
                default:
                    return false;
            }
        }
    }

    /// <summary>
    /// 山脚状态
    /// </summary>
    public class MountainFootState : BaseGameState
    {
        public MountainFootState() : base("MountainFoot")
        {
        }

        public override void OnEnter(IGameState previousState)
        {
            base.OnEnter(previousState);
            Debug.Log("MountainFootState: At the foot of the mountain");
            
            // 山脚状态逻辑
            // - 显示山脚场景
            // - 准备爬山
        }

        public override void OnExit(IGameState nextState)
        {
            base.OnExit(nextState);
            Debug.Log("MountainFootState: Leaving mountain foot");
        }

        /// <summary>
        /// 开始爬山
        /// </summary>
        public void StartClimbing()
        {
            ParentState?.SwitchToSubState("Climbing");
        }
    }

    /// <summary>
    /// 爬山状态
    /// </summary>
    public class ClimbingState : BaseGameState
    {
        public ClimbingState() : base("Climbing")
        {
        }

        public override void OnEnter(IGameState previousState)
        {
            base.OnEnter(previousState);
            Debug.Log("ClimbingState: Climbing the mountain");
            
            // 爬山状态逻辑
            // - 显示爬山界面
            // - 启动爬山机制
            // - 时间推进
        }

        public override void OnExit(IGameState nextState)
        {
            base.OnExit(nextState);
            Debug.Log("ClimbingState: Stopped climbing");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            // 爬山更新逻辑
            // - 检查爬山进度
            // - 处理爬山事件
        }

        /// <summary>
        /// 使用感知技能
        /// </summary>
        public void UsePerceptionSkill()
        {
            ParentState?.SwitchToSubState("Perception");
        }

        /// <summary>
        /// 到达山顶
        /// </summary>
        public void ReachMountainTop()
        {
            ParentState?.SwitchToSubState("MountainTop");
        }

        /// <summary>
        /// 进入内部世界
        /// </summary>
        public void EnterInsideWorld()
        {
            var insideGameState = ParentState?.ParentState as InsideGameState;
            insideGameState?.SwitchToInsideWorld();
        }
    }

    /// <summary>
    /// 感知状态
    /// </summary>
    public class PerceptionState : BaseGameState
    {
        public PerceptionState() : base("Perception")
        {
        }

        public override void OnEnter(IGameState previousState)
        {
            base.OnEnter(previousState);
            Debug.Log("PerceptionState: Using perception skill");
            
            // 感知状态逻辑
            // - 显示感知界面
            // - 激活感知机制
        }

        public override void OnExit(IGameState nextState)
        {
            base.OnExit(nextState);
            Debug.Log("PerceptionState: Perception skill ended");
        }

        /// <summary>
        /// 返回爬山状态
        /// </summary>
        public void ReturnToClimbing()
        {
            ParentState?.SwitchToSubState("Climbing");
        }
    }

    /// <summary>
    /// 山顶状态
    /// </summary>
    public class MountainTopState : BaseGameState
    {
        public MountainTopState() : base("MountainTop")
        {
        }

        public override void OnEnter(IGameState previousState)
        {
            base.OnEnter(previousState);
            Debug.Log("MountainTopState: Reached the mountain top");
            
            // 山顶状态逻辑
            // - 显示山顶场景
            // - 完成爬山成就
        }

        public override void OnExit(IGameState nextState)
        {
            base.OnExit(nextState);
            Debug.Log("MountainTopState: Leaving mountain top");
        }

        /// <summary>
        /// 进入内部世界（从山顶）
        /// </summary>
        public void EnterInsideWorld()
        {
            var insideGameState = ParentState?.ParentState as InsideGameState;
            insideGameState?.SwitchToInsideWorld();
        }

        /// <summary>
        /// 重新开始爬山
        /// </summary>
        public void RestartClimbing()
        {
            ParentState?.SwitchToSubState("MountainFoot");
        }
    }
} 