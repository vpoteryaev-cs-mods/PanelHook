using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ColossalFramework.Globalization;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using UnityEngine;

namespace PanelHook.UI
{
    internal readonly struct HookAction
    {
        internal MouseEventHandler Handler { get; }
        internal UIComponent Sender { get; }
        internal UIMouseEventParameter EventParam { get; }

        internal HookAction(MouseEventHandler handler, UIComponent sender, UIMouseEventParameter eventParam)
        {
            Handler = handler;
            Sender = sender;
            EventParam = eventParam;
        }
    }

    public class HooksPanel: UIPanel
    {
        private const int windowWidth = 400;
        private const int captionHeight = 40;
        private readonly Vector2 closeButtonSize = new Vector2(32, 32);
        new private readonly RectOffset padding = new RectOffset(0, 0, 0, 0);

        private const int bottomPanelHeight = 30; //but maybe need on-the-fly calculation if buttons used
        private UIDropDown actionsDropDown;

        internal HookAction[] actions;
        internal string[] dropDownItems;

        public HooksPanel()
        {
#if DEBUG
            Debug.Log("PanelHook: HooksPanel.HooksPanel()");
#endif
        }

        public override void Awake()
        {
            base.Awake();
#if DEBUG
            Debug.Log("PanelHook: HooksPanel.Awake()");
#endif
        }
        public override void Start()
        {
            base.Start();
#if DEBUG
            Debug.LogFormat("PanelHook: HooksPanel.Start() - position {0}", position);
#endif
            Init();
            Hide();
        }

        public override void Update()
        {
            base.Update();
            if (!isVisible) return;
            //seems nothing to do
        }

        private void Init()
        {
            backgroundSprite = "MenuPanel2"; //????????????
            opacity = 0.9f;
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;

            var caption = AddUIComponent<UIDragHandle>();
            caption.size = new Vector2(windowWidth, captionHeight);

            var captionLabel = caption.AddUIComponent<UILabel>();
            captionLabel.anchor = UIAnchorStyle.CenterVertical | UIAnchorStyle.CenterHorizontal;
            captionLabel.text = "Choose action";

            var closeButton = caption.AddUIComponent<UIButton>();
            closeButton.size = closeButtonSize;
            closeButton.normalBgSprite = "buttonclose";
            closeButton.hoveredBgSprite = "buttonclosehover";
            closeButton.pressedBgSprite = "buttonclosepressed";
            closeButton.relativePosition = new Vector3(windowWidth - closeButtonSize.x, 0, 0);
            closeButton.anchor = UIAnchorStyle.Top | UIAnchorStyle.Right;
            closeButton.eventClicked += (sender, e) =>
            {
                Hide();
                ClearActions();
            };

            var bottomPanel = AddUIComponent<UIPanel>();
            bottomPanel.size = new Vector2(windowWidth, bottomPanelHeight);
            bottomPanel.autoLayout = true;
            bottomPanel.autoLayoutDirection = LayoutDirection.Vertical;
            bottomPanel.autoLayoutStart = LayoutStart.TopLeft;
            bottomPanel.autoLayoutPadding = padding;

            actionsDropDown = UI_Utils.CreateDropDown(bottomPanel);
            actionsDropDown.autoSize = false;
            actionsDropDown.size = new Vector2(windowWidth, 30f);
            actionsDropDown.eventSelectedIndexChanged += new PropertyChangedEventHandler<int>((c, idx) =>
            {
                OnActionChoosed(idx);
            });

            ClearActions();

            FitChildren(new Vector2(2, 2));

            canFocus = true;
            //note: possible add the panel hiding when focus lost
            //eventLostFocus += (sender, param) =>
            //{
            //    Hide();
            //    ClearActions();
            //};

            isInteractive = true;
        }

        internal void AddAction(MouseEventHandler handler, string description, UIComponent sender, UIMouseEventParameter eventParam)
        {
            int idx = actions.Length;
            Array.Resize(ref actions, idx + 1);
            actions[idx] = new HookAction(handler, sender, eventParam);
            Array.Resize(ref dropDownItems, idx + 1);
            dropDownItems[idx] = description;
            actionsDropDown.items = dropDownItems;
        }

        internal void ClearActions()
        {
            actions = (HookAction[])Array.CreateInstance(typeof(HookAction), 1);
            dropDownItems = new string[] { "None" };
            actionsDropDown.items = dropDownItems;
            actionsDropDown.selectedIndex = 0;
        }

        private void OnActionChoosed(int idx)
        {
#if DEBUG
            Debug.LogFormat("PanelHook: HooksPanel.OnActionChoosed - {0}", idx);
#endif
            if (isVisible && idx < 1) return;
            if (idx > 0)
            {
#if DEBUG
                Debug.LogFormat("PanelHook: HooksPanel.OnActionChoosed - action needed {0}", idx);
#endif
                //Action< HookManager.pHandlerPointer, UIComponent, UIMouseEventParameter> act = (ptr1, sdr1, ep1) =>
                //{ ptr1(sdr1, ep1)};
                //act(actions[idx].Handler, actions[idx].Sender, actions[idx].EventParam);
                HookAction action = actions[idx];
                action.Handler(action.Sender, action.EventParam);

                Hide();
            }
            ClearActions();
        }
    }
}
