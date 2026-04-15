using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using TabletPal.Actions;
using TabletPal.Models;
using TabletPal.Docking;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia;
using Material.Icons;
using Material.Icons.Avalonia;

namespace TabletPal
{
	public static class UiFactory
	{
		public static void CreateUi(LayoutModel layout, MainWindow window)
		{
			Debug.WriteLine("UI created!");
			ToggleManager.ClearButtons();
			var theme = AppState.CurrentTheme;

			window.MainCanvas.Children.Clear();

			var isDocked = AppState.Settings.DockingMode != DockingMode.None;

			if (!isDocked)
			{
				window.MainBorder.CornerRadius = new CornerRadius(theme.Rounding);
			}
			else
			{
				window.MainBorder.CornerRadius = new CornerRadius(0);
			}
			var sizes = layout.Buttons.GetSizes(AppState.Settings.DockingMode);
			var positions = Packer.Pack(sizes, layout.LayoutWidth);

			var size = Packer.GetSize(positions, sizes);


			var rotateLayout = false;
			var layoutVertical = size.Y > size.X;
			if (AppState.Settings.DockingMode != DockingMode.None)
			{
				var dockingVertical = AppState.Settings.DockingMode == DockingMode.Left
					|| AppState.Settings.DockingMode == DockingMode.Right;

				if (layoutVertical != dockingVertical)
				{
					rotateLayout = true;
				}
			}

			var titlebarHeight = TitlebarManager.GetTitlebarHeight(layout);

			var newWidth = window.Width;
			var newHeight = window.Height;

			
			if (rotateLayout)
			{
				newHeight = size.X * layout.CellSize + layout.Margin + titlebarHeight;
				newWidth = size.Y * layout.CellSize + layout.Margin;
			}
			else
			{
				newWidth = size.X * layout.CellSize + layout.Margin;
				newHeight = size.Y * layout.CellSize + layout.Margin + titlebarHeight;
			}


			var windowSizeChanged = newWidth != window.Width || newHeight != window.Height;

			var wasMinimized = TitlebarManager.Minimized;

			if (windowSizeChanged)
			{
				if (
					   AppState.Settings.DockingMode == DockingMode.Left
                    || AppState.Settings.DockingMode == DockingMode.Right
                    || AppState.Settings.DockingMode == DockingMode.None
				)
				{
					window.Width = newWidth;
				}
				if (
					   AppState.Settings.DockingMode == DockingMode.Top
					|| AppState.Settings.DockingMode == DockingMode.Bottom
                    || AppState.Settings.DockingMode == DockingMode.None
				)
				{
					if (!wasMinimized)
					{
						window.Height = newHeight;
					}
				}
                AppBarFunctions.OnChangeLayoutHeight(window, newHeight);
                AppBarFunctions.OnChangeLayoutWidth(window, newWidth);
            }

			var offset = Vector2.Zero;
			if (AppState.Settings.DockingMode != DockingMode.None)
			{
				if (AppState.Settings.DockingMode == DockingMode.Top || AppState.Settings.DockingMode == DockingMode.Bottom)
				{
                    var screen = window.Screens.ScreenCount > 1
                        ? window.Screens.ScreenFromPoint(window.Position)
                        : window.Screens.Primary;
                    if (screen != null)
                    {
                        offset.X = (float)(screen.Bounds.Width - newWidth) / 2;
                    }
                }
				else
				{
                    var screen = window.Screens.ScreenCount > 1
                        ? window.Screens.ScreenFromPoint(window.Position)
                        : window.Screens.Primary;
                    if (screen != null)
                    {
                        offset.Y = (float)(screen.Bounds.Height - newHeight) / 2;
                    }
                }
			}
			else
			{
				offset.Y = (float)titlebarHeight;
			}

			if (AppState.Settings.DockingMode != DockingMode.None)
			{
				window.MinOpacity = layout.MaxOpacity;
			}
			else
			{
				window.MinOpacity = layout.MinOpacity;
			}
			window.MaxOpacity = layout.MaxOpacity;
			window.Opacity = layout.MaxOpacity;
            // This is a code for when you hover over the panel, I believe it fades in or out.
            // It's kinda unneccessary if you ask me. Otherwise, I believe the code below isn't 
            // properly coded for Avalonia/Linux and will have to require another look.
			// if (window.IsPointerOver)
			// {
			// 	window.BeginAnimation(UIElement.OpacityProperty, window.FadeIn);
			// }
			// else
			// {
			// 	window.BeginAnimation(UIElement.OpacityProperty, window.FadeOut);
			// }
            // window.PointerEntered -= window.OnPointerEnteredFade;
            // window.PointerExited -= window.OnPointerExitedFade;

            // window.PointerEntered += window.OnPointerEnteredFade;
            // window.PointerExited += window.OnPointerExitedFade;

			Application.Current.Resources["MaterialPrimaryMidBrush"] = new SolidColorBrush(theme.PrimaryColor);
			Application.Current.Resources["MaterialPrimaryForegroundBrush"] = new SolidColorBrush(theme.SecondaryColor);
			Application.Current.Resources["MaterialPrimaryMidForegroundBrush"] = new SolidColorBrush(theme.SecondaryColor);

			Application.Current.Resources["MaterialDesignPaper"] = new SolidColorBrush(theme.BackgroundColor);
			Application.Current.Resources["MaterialDesignFont"] = new SolidColorBrush(theme.SecondaryColor);
			Application.Current.Resources["MaterialDesignBody"] = new SolidColorBrush(theme.SecondaryColor);

			window.MainBorder.Background = new SolidColorBrush(theme.BackgroundColor);

			var visibleButtons = new List<ButtonModel>();

			foreach (var button in layout.Buttons)
			{
				if (button.IsVisible(AppState.Settings.DockingMode))
				{
					visibleButtons.Add(button);
				}
			}

			for (var i = 0; i < positions.Length; i += 1)
			{
				var button = visibleButtons[i];


				if (button.Spacer)
				{
					continue;
				}
				var buttonPosition = positions[i];
				var buttonSize = sizes[i];

				if (rotateLayout)
				{
					var buffer = buttonPosition.X;
					buttonPosition.X = buttonPosition.Y;
					buttonPosition.Y = buffer;

					buffer = buttonSize.X;
					buttonSize.X = buttonSize.Y;
					buttonSize.Y = buffer;
				}

				CreateButton(layout, window, button, buttonPosition, buttonSize, offset);
			}


			TitlebarManager.CreateTitlebar(window, theme, layout, newHeight, wasMinimized);
            
        }

		private static void CreateButton(
			LayoutModel layout,
			MainWindow window,
			ButtonModel button,
			Vector2 position,
			Vector2 size,
			Vector2 offset
		)
		{
			var theme = AppState.CurrentTheme;

			Button uiButton;
			var isToggle = button.Action is ToggleAction;
			var isRepeat = button.Action is RepeatAction;

			if (isToggle)
			{
				uiButton = new ToggleButton();
			}
			else
			{
				if (isRepeat)
				{
					uiButton = new RepeatButton();
				}
				else
				{
					uiButton = new Button();
				}
			}
            
			uiButton.Width = layout.CellSize * size.X - layout.Margin;
			uiButton.Height = layout.CellSize * size.Y - layout.Margin;

			var font = button.Font;
			if (font == null)
			{
				font = AppState.CurrentTheme.DefaultFont;
			}
			var fontSize = button.FontSize;
			if (fontSize == 0)
			{
				fontSize = AppState.CurrentTheme.DefaultFontSize;
			}
			var fontWeight = button.FontWeight;
			if (fontWeight == 0)
			{
				fontWeight = AppState.CurrentTheme.DefaultFontWeight;
			}

			var text = new TextBlock();
			text.Text = button.Text;
			if (fontSize > 0)
			{
				text.FontSize = fontSize;
			}
			if (font != null)
			{
				text.FontFamily = new FontFamily(font);
			}
			if (fontWeight > 0)
			{
                text.FontWeight = (FontWeight)Math.Clamp(fontWeight, 1, 999);
            }

			uiButton.Content = text;

            if (!string.IsNullOrEmpty(button.IconPath))
            {
                var image = new Image
                {
                    Source = new Bitmap(button.IconPath),
                    Stretch = button.IconStretch
                };

                uiButton.Content = image;
            }
            else if (!string.IsNullOrEmpty(button.IconName))
            {
                if (Enum.TryParse<MaterialIconKind>(button.IconName, true, out var kind))
                {
                    var ico = new MaterialIcon
                    {
                        Kind = kind
                    };
                    uiButton.Content = ico;
                }
            }
            var toolTip = new ToolTip(){
                Content = button.Text,
                Theme = (ControlTheme)Application.Current.Resources["tool_tip"]
            };
            ToolTip.SetTip(uiButton, toolTip);
			var style = button.Style;
			if (style == null)
			{
				style = theme.DefaultStyle;
			}

			if (isToggle)
			{
                uiButton.Theme = (ControlTheme)Application.Current.Resources["toggle"];
				var key = string.Join("+",((ToggleAction)button.Action).Key);
                var keyToggle = ((ToggleAction)button.Action).Key;
				var toggle = (ToggleButton)uiButton;
				if (ToggleManager.IsHeld(keyToggle))
				{
					toggle.IsChecked = true;
				}
				ToggleManager.AddButton(key, toggle);
			}
			else
			{
				if (style == null)
				{
                    uiButton.Theme = null;
				}
				else
				{
                    uiButton.Theme = (ControlTheme)Application.Current.Resources[style];
				}
			}

			if (button.Action != null)
			{
                uiButton.Click += (_, __) => {
                    window.CaptureFocusedWindow();
                    button.Action.Invoke();
                    window.RestoreFocus();
                };


			}
			Canvas.SetTop(uiButton, layout.CellSize * position.Y + layout.Margin + offset.Y);
			Canvas.SetLeft(uiButton, layout.CellSize * position.X + layout.Margin + offset.X);
			window.MainCanvas.Children.Add(uiButton);
		}
	}
}
