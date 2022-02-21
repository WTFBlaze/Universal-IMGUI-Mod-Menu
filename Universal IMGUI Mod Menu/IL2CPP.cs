using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Universal_IMGUI_Mod_Menu
{
    // Use unhollower's ConvertToIl2Cpp Function to register it. As seen on the next line.
    // ClassInjector.RegisterTypeInIl2Cpp<IL2CPPExample>();

    public class IL2CPPExample : MonoBehaviour
    {
        // Config Values
        public KeyCode toggleKeyBind = KeyCode.Insert;
        public KeyCode previousKeybind = KeyCode.LeftArrow;
        public KeyCode nextKeybind = KeyCode.RightArrow;
        public KeyCode upKeyBind = KeyCode.UpArrow;
        public KeyCode downKeyBind = KeyCode.DownArrow;
        public bool logMenuStateChanged = true;
        public string menuTitleText = "<size=35><b><color=red>Mod Menu Base</color></b></size>";
        public string subTitleText = "<size=25><b><color=#7d0000>by WTFBlaze!</color></b></size>";
        public string highlightedOptionHex = "#7d0000";

        // Don't Change
        private Rect _rect = new Rect(0, 0, 500, Screen.height);
        private bool _menuIsOpened;
        private GUIStyle _menuStyle;
        private GUIStyle _textStyle;
        public List<UIPage> pages = new List<UIPage>();
        private bool isMainMenu = true;
        private UIPage currentPage;
        private int mmItemIndex = 1;

        public void Start()
        {
            var page1 = AddPage("Example Page");

            AddButton(page1, "Example Button", delegate
            {
                Console.WriteLine("Button Clicked!");
            });

            AddButton(page1, "View On Github", delegate
            {
                Process.Start("https://github.com/WTFBlaze/Universal-IMGUI-Mod-Menu");
            });
        }

        public void OnGUI()
        {
            _menuStyle = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    background = GeneratePixelMap(new Color(0, 0, 0, 0.6f))
                },
                stretchHeight = true,
                stretchWidth = true
            };

            _textStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    textColor = Color.white
                }
            };

            if (_menuIsOpened)
            {
                _rect = GUILayout.Window(1324231, _rect, (GUI.WindowFunction)DoMyWindow, "", _menuStyle, new GUILayoutOption[0]);
            }
        }

        public void Update()
        {
            // Toggle Menu
            if (Input.GetKeyDown(toggleKeyBind))
            {
                _menuIsOpened = !_menuIsOpened;
                if (logMenuStateChanged)
                {
                    Console.ForegroundColor = _menuIsOpened ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.WriteLine($"Menu {(_menuIsOpened ? "Opened" : "Closed")}!");
                    Console.ResetColor();
                }
            }

            if (_menuIsOpened)
            {
                // Up
                if (Input.GetKeyDown(upKeyBind))
                {
                    if (isMainMenu)
                    {
                        // decrease to go up
                        if (mmItemIndex == 1) return;
                        mmItemIndex--;
                    }
                    else
                    {
                        // decrease to go up
                        if (currentPage.selectedItem == 1) return;
                        currentPage.selectedItem--;
                    }
                }

                // Down
                if (Input.GetKeyDown(downKeyBind))
                {
                    if (isMainMenu)
                    {
                        // increase to go down
                        if (mmItemIndex == pages.Count) return;
                        mmItemIndex++;
                    }
                    else
                    {
                        // increase to go down
                        if (currentPage.selectedItem == currentPage.buttons.Count) return;
                        currentPage.selectedItem++;
                    }
                }

                // Enter
                if (Input.GetKeyDown(nextKeybind))
                {
                    if (isMainMenu)
                    {
                        isMainMenu = false;
                        currentPage = pages[mmItemIndex - 1];
                    }
                    else
                    {
                        currentPage.buttons[currentPage.selectedItem - 1].action.Invoke();
                    }
                }

                // Back
                if (Input.GetKeyDown(previousKeybind))
                {
                    if (currentPage.parentIsMainMenu)
                    {
                        isMainMenu = true;
                    }
                    else
                    {
                        currentPage = currentPage.parent;
                    }
                }
            }
        }

        private void DoMyWindow(int windowID)
        {
            if (isMainMenu)
            {
                GUILayout.Space(150f);
                GUILayout.Label(menuTitleText, _textStyle, new GUILayoutOption[0]);
                GUILayout.Label(subTitleText, _textStyle, new GUILayoutOption[0]);
                GUILayout.Space(45f);

                foreach (var p in pages)
                {
                    GUILayout.Label($"<size=30><b>{(mmItemIndex == p.pageID ? $"<color={highlightedOptionHex}>{p.label}</color>" : p.label)}</b></size>", _textStyle, new GUILayoutOption[0]);
                    GUILayout.Space(10f);
                }
            }
            else
            {
                GUILayout.Space(150f);
                GUILayout.Label($"<size=35><b><color=red>{currentPage.label}</color></b></size>", _textStyle, new GUILayoutOption[0]);
                GUILayout.Space(45f);

                foreach (var b in currentPage.buttons)
                {
                    GUILayout.Label($"<size=30><b>{(currentPage.selectedItem == b.buttonID ? $"<color={highlightedOptionHex}>{b.label}</color>" : b.label)}</b></size>", _textStyle, new GUILayoutOption[0]);
                    GUILayout.Space(10f);
                }
            }
        }

        public static Texture2D GeneratePixelMap(Color color)
        {
            Texture2D texture2D = new Texture2D(1, 1);
            texture2D.SetPixels32(new Color32[]
            {
                color
            });
            texture2D.Apply();
            return texture2D;
        }

        public UIPage AddPage(string label)
        {
            var tmp = new UIPage()
            {
                label = label,
                buttons = new List<UIButton>(),
                pageID = pages.Count + 1,
                selectedItem = 1,
                parentIsMainMenu = true,
                parent = null,
            };
            pages.Add(tmp);
            return tmp;
        }

        public UIButton AddButton(UIPage page, string label, Action action)
        {
            var tmp = new UIButton()
            {
                action = action,
                label = label,
                buttonID = page.buttons.Count + 1,
            };
            page.buttons.Add(tmp);
            return tmp;
        }

        public UIPage AddPage(UIPage page, string label)
        {
            var tmp = new UIPage()
            {
                buttons = new List<UIButton>(),
                label = label,
                parent = page,
                parentIsMainMenu = false,
                selectedItem = 1,
                pageID = -1
            };
            AddButton(page, label, delegate
            {
                currentPage = tmp;
            });
            return tmp;
        }
    }

    public class UIPage
    {
        public string label;
        public int selectedItem;
        public int pageID;
        public bool parentIsMainMenu;
        public UIPage parent;
        public List<UIButton> buttons;

        public void SetLabel(string newLabel)
        {
            label = newLabel;
        }
    }

    public class UIButton
    {
        public string label;
        public Action action;
        public int buttonID;

        public void SetLabel(string newLabel)
        {
            label = newLabel;
        }
    }
}
