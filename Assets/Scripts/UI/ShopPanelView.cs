using System;
using CookieClicker.Common;
using UnityEngine;
using UnityEngine.UI;

namespace CookieClicker.UI
{
    /// <summary>
    /// The shop window: opens from the HUD shop button, closes from its close button or a tap outside,
    /// and switches between its tabs (Stand, Staff, ...).
    /// </summary>
    public class ShopPanelView : MonoBehaviour
    {
        [Serializable]
        private class Tab
        {
            public Button button;
            public Image background;
            public GameObject page;
        }

        [SerializeField] private Button openButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button backdrop;
        [Tooltip("The backdrop and window. Hidden while the shop is closed.")]
        [SerializeField] private GameObject content;
        [SerializeField] private PopAnimation windowPop;
        [SerializeField] private Tab[] tabs = new Tab[0];
        [SerializeField] private Color selectedTabColor = Color.white;
        [SerializeField] private Color idleTabColor = new Color32(0xE8, 0xD3, 0xBD, 0xFF);

        private int selectedTab;

        public bool IsOpen => content.activeSelf;

        private void Awake()
        {
            content.SetActive(false);
            SelectTab(0);
        }

        private void OnEnable()
        {
            openButton.onClick.AddListener(Open);
            closeButton.onClick.AddListener(Close);
            backdrop.onClick.AddListener(Close);
            for (int i = 0; i < tabs.Length; i++)
            {
                int index = i;
                tabs[i].button.onClick.AddListener(() => SelectTab(index));
            }
        }

        private void OnDisable()
        {
            openButton.onClick.RemoveListener(Open);
            closeButton.onClick.RemoveListener(Close);
            backdrop.onClick.RemoveListener(Close);
            foreach (Tab tab in tabs)
            {
                tab.button.onClick.RemoveAllListeners();
            }
        }

        public void Open()
        {
            content.SetActive(true);
            SelectTab(selectedTab);
            windowPop.Play();
        }

        public void Close()
        {
            content.SetActive(false);
        }

        public void SelectTab(int index)
        {
            selectedTab = Mathf.Clamp(index, 0, Mathf.Max(0, tabs.Length - 1));
            for (int i = 0; i < tabs.Length; i++)
            {
                bool selected = i == selectedTab;
                tabs[i].page.SetActive(selected);
                tabs[i].background.color = selected ? selectedTabColor : idleTabColor;
            }
        }
    }
}
