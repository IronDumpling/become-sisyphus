using UnityEngine;

namespace BecomeSisyphus.Core.GameStateSystem
{
    /// <summary>
    /// 内部世界状态
    /// </summary>
    public class InsideWorldState : BaseGameState
    {
        public InsideWorldState() : base("InsideWorld")
        {
        }

        public override void OnEnter(IGameState previousState)
        {
            base.OnEnter(previousState);
            
            Debug.Log("InsideWorldState: Entering inside world");
            
            // Check if we should enter resting state or another state
            var gameStateManager = GameStateManager.Instance;
            if (gameStateManager != null && gameStateManager.LastInsideWorldState == "Resting")
            {
                // Default to Harbour rest for Resting state
                SwitchToSubState("Interaction");
                CurrentSubState?.SwitchToSubState("Harbour");
            }
            else if (CurrentSubState == null)
            {
                // Default to Sailing if no specific state requested
                SwitchToSubState("Sailing");
            }
        }

        public override void OnExit(IGameState nextState)
        {
            base.OnExit(nextState);
            Debug.Log("InsideWorldState: Exiting inside world");
        }

        public override bool CanTransitionTo(string stateName)
        {
            // 内部世界状态转换规则
            switch (stateName)
            {
                case "Sailing":
                case "Interaction":
                case "ThoughtBoatCabin":
                case "Telescope":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 返回外部世界
        /// </summary>
        public void ReturnToOutsideWorld()
        {
            GameStateManager.Instance?.SwitchToLastOutsideWorldState();
        }
    }

    /// <summary>
    /// 航行状态
    /// </summary>
    public class SailingState : BaseGameState
    {
        public SailingState() : base("Sailing")
        {
        }

        public override void OnEnter(IGameState previousState)
        {
            base.OnEnter(previousState);
            Debug.Log("SailingState: Sailing in the mind ocean");
            Debug.Log("SailingState: Use WASD to move boat, Press Space to go to outside world");
            
            // 航行状态逻辑
            // - 启用自由航行 WASD移动小船
            // - 时间推进
            // - 显示航行界面
            // - Space键进入外部世界
        }

        public override void OnExit(IGameState nextState)
        {
            base.OnExit(nextState);
            Debug.Log("SailingState: Stopped sailing");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            // 航行更新逻辑
            // - 处理船只移动
            // - 检测交互点
            // - 更新时间系统
        }

        /// <summary>
        /// 移动小船 (通过WASD键触发)
        /// </summary>
        public void MoveBoat(Vector2 direction)
        {
            if (direction != Vector2.zero)
            {
                Debug.Log($"SailingState: Moving boat in direction: {direction}");
                // 实际的船只移动逻辑
            }
        }

        /// <summary>
        /// 进入外部世界 (通过Space键触发)
        /// </summary>
        public void EnterOutsideWorld()
        {
            Debug.Log("SailingState: Entering outside world");
            GameStateManager.Instance?.SwitchToLastOutsideWorldState();
        }

        /// <summary>
        /// 开始交互
        /// </summary>
        public void StartInteraction(string interactionType)
        {
            ParentState?.SwitchToSubState("Interaction");
            
            // 切换到具体的交互类型
            var interactionState = ParentState?.CurrentSubState;
            interactionState?.SwitchToSubState(interactionType);
        }

        /// <summary>
        /// 打开船舱
        /// </summary>
        public void OpenCabin()
        {
            ParentState?.SwitchToSubState("ThoughtBoatCabin");
        }

        /// <summary>
        /// 打开望远镜
        /// </summary>
        public void OpenTelescope()
        {
            ParentState?.SwitchToSubState("Telescope");
        }
    }

    /// <summary>
    /// 交互状态
    /// </summary>
    public class InteractionState : BaseGameState
    {
        public InteractionState() : base("Interaction")
        {
        }

        public override void OnEnter(IGameState previousState)
        {
            base.OnEnter(previousState);
            Debug.Log("InteractionState: Starting interaction");
            
            // 交互状态逻辑
            // - 禁用自由航行
            // - 显示交互界面
        }

        public override void OnExit(IGameState nextState)
        {
            base.OnExit(nextState);
            Debug.Log("InteractionState: Ending interaction");
        }

        /// <summary>
        /// 关闭交互，返回航行
        /// </summary>
        public void CloseInteraction()
        {
            ParentState?.SwitchToSubState("Sailing");
        }
    }

    /// <summary>
    /// 船舱状态
    /// </summary>
    public class ThoughtBoatCabinState : BaseGameState
    {
        public ThoughtBoatCabinState() : base("ThoughtBoatCabin")
        {
        }

        public override void OnEnter(IGameState previousState)
        {
            base.OnEnter(previousState);
            Debug.Log("ThoughtBoatCabinState: Entered boat cabin");
            
            // 船舱状态逻辑
            // - 显示船舱界面
            // - 禁用航行控制
        }

        public override void OnExit(IGameState nextState)
        {
            base.OnExit(nextState);
            Debug.Log("ThoughtBoatCabinState: Exited boat cabin");
        }

        /// <summary>
        /// 关闭船舱
        /// </summary>
        public void CloseCabin()
        {
            ParentState?.SwitchToSubState("Sailing");
        }
    }

    /// <summary>
    /// 望远镜状态
    /// </summary>
    public class TelescopeState : BaseGameState
    {
        public TelescopeState() : base("Telescope")
        {
        }

        public override void OnEnter(IGameState previousState)
        {
            base.OnEnter(previousState);
            Debug.Log("TelescopeState: Using telescope");
            
            // 望远镜状态逻辑
            // - 显示望远镜界面
            // - 启用云朵查看/选择
        }

        public override void OnExit(IGameState nextState)
        {
            base.OnExit(nextState);
            Debug.Log("TelescopeState: Closed telescope");
        }

        /// <summary>
        /// 关闭望远镜
        /// </summary>
        public void CloseTelescope()
        {
            ParentState?.SwitchToSubState("Sailing");
        }
    }

    // 交互子状态
    public class HarbourInteractionState : BaseGameState
    {
        public HarbourInteractionState() : base("Harbour")
        {
        }

        public override void OnEnter(IGameState previousState)
        {
            base.OnEnter(previousState);
            Debug.Log("HarbourInteractionState: Resting at harbour");
            Debug.Log("HarbourInteractionState: Press Esc to start sailing, Press Space to go to outside world");
            
            // This acts as the "Resting" state
            // - Boat is docked at harbour
            // - Player can rest here
            // - Press Esc to start sailing
            // - Press Space to go to outside world
        }

        public override void OnExit(IGameState nextState)
        {
            base.OnExit(nextState);
            Debug.Log("HarbourInteractionState: Leaving harbour");
        }

        /// <summary>
        /// 开始航行 (通过Esc键触发)
        /// </summary>
        public void StartSailing()
        {
            Debug.Log("HarbourInteractionState: Starting to sail");
            // Go up two levels: Harbour -> Interaction -> InsideWorld, then switch to Sailing
            ParentState?.ParentState?.SwitchToSubState("Sailing");
        }

        /// <summary>
        /// 进入外部世界 (通过Space键触发)
        /// </summary>
        public void EnterOutsideWorld()
        {
            Debug.Log("HarbourInteractionState: Entering outside world");
            GameStateManager.Instance?.SwitchToLastOutsideWorldState();
        }

        /// <summary>
        /// 休憩（时间推进）
        /// </summary>
        public void Rest()
        {
            Debug.Log("HarbourInteractionState: Resting at harbour (time advances)");
            // 时间推进逻辑
        }
    }

    public class LighthouseInteractionState : BaseGameState
    {
        public LighthouseInteractionState() : base("Lighthouse")
        {
        }

        public override void OnEnter(IGameState previousState)
        {
            base.OnEnter(previousState);
            Debug.Log("LighthouseInteractionState: Interacting with lighthouse");
        }

        /// <summary>
        /// 休憩（时间推进）
        /// </summary>
        public void Rest()
        {
            Debug.Log("LighthouseInteractionState: Resting at lighthouse (time advances)");
            // 时间推进逻辑
        }
    }

    public class SalvageInteractionState : BaseGameState
    {
        public SalvageInteractionState() : base("Salvage")
        {
        }

        public override void OnEnter(IGameState previousState)
        {
            base.OnEnter(previousState);
            Debug.Log("SalvageInteractionState: Salvaging (time advances)");
        }
    }

    public class IslandInteractionState : BaseGameState
    {
        public IslandInteractionState() : base("Island")
        {
        }

        public override void OnEnter(IGameState previousState)
        {
            base.OnEnter(previousState);
            Debug.Log("IslandInteractionState: Exploring island");
        }
    }

    // 船舱子状态
    public class ThoughtVesselState : BaseGameState
    {
        public ThoughtVesselState() : base("ThoughtVessel")
        {
        }

        public override void OnEnter(IGameState previousState)
        {
            base.OnEnter(previousState);
            Debug.Log("ThoughtVesselState: Managing thought vessel");
        }
    }

    public class NavigationMapState : BaseGameState
    {
        public NavigationMapState() : base("NavigationMap")
        {
        }

        public override void OnEnter(IGameState previousState)
        {
            base.OnEnter(previousState);
            Debug.Log("NavigationMapState: Viewing navigation map");
        }
    }

    public class MindAbilityState : BaseGameState
    {
        public MindAbilityState() : base("MindAbility")
        {
        }

        public override void OnEnter(IGameState previousState)
        {
            base.OnEnter(previousState);
            Debug.Log("MindAbilityState: Managing mind abilities");
        }
    }
} 