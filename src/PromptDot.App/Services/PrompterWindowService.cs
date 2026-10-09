using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using PromptDot.App.ViewModels;
using PromptDot.Core.Windowing;
using Windows.Graphics;
using Windows.Graphics.Display;

namespace PromptDot.App.Services;

internal sealed class PrompterWindowService : IPrompterWindowService
{
    private Window? window;

    public void Show(MainViewModel viewModel)
    {
        ActiveViewModel = viewModel;
        if (window is null)
        {
            window = new Window
            {
                Content = new PrompterPage(viewModel),
                Title = "PromptDot Prompter",
            };
            window.Closed += OnClosed;
            window.AppWindow.Changed += OnAppWindowChanged;
            RestoreOrCenter(viewModel, window);
        }

        ApplyAlwaysOnTop(viewModel.AlwaysOnTop);
        window.Activate();
    }

    public void Recenter()
    {
        if (window is null)
        {
            return;
        }

        var workArea = GetCurrentWorkArea(window);
        var current = ToBounds(window.AppWindow);
        MoveAndResize(window, WindowPositioning.TopCenter(workArea, current));
    }

    public void ApplyAlwaysOnTop(bool enabled)
    {
        if (window?.AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsAlwaysOnTop = enabled;
        }
    }

    public void Close()
    {
        window?.Close();
    }

    private void OnClosed(object sender, WindowEventArgs args)
    {
        window = null;
        ActiveViewModel = null;
    }

    private static WindowBounds GetCurrentWorkArea(Window targetWindow)
    {
        if (OperatingSystem.IsWindows() &&
            TryGetWindowsWorkArea(targetWindow, out var workArea))
        {
            return workArea;
        }

        var display = DisplayInformation.GetForCurrentView();
        return new WindowBounds(
            0,
            0,
            display.ScreenWidthInRawPixels,
            display.ScreenHeightInRawPixels);
    }

    private static WindowBounds ToBounds(AppWindow appWindow)
    {
        return new WindowBounds(
            appWindow.Position.X,
            appWindow.Position.Y,
            appWindow.Size.Width,
            appWindow.Size.Height);
    }

    private static void MoveAndResize(Window targetWindow, WindowBounds bounds)
    {
        var appWindow = targetWindow.AppWindow;
        var x = (int)Math.Round(bounds.X);
        var y = (int)Math.Round(bounds.Y);
        var width = (int)Math.Round(bounds.Width);
        var height = (int)Math.Round(bounds.Height);

        if (OperatingSystem.IsWindows() &&
            TryGetWin32Handle(targetWindow, out var windowHandle))
        {
            _ = SetWindowPos(
                windowHandle,
                0,
                x,
                y,
                width,
                height,
                0x0004 | 0x0010);
            return;
        }

        appWindow.Move(new PointInt32 { X = x, Y = y });
        appWindow.Resize(new SizeInt32 { Width = width, Height = height });
    }

    private static void RestoreOrCenter(MainViewModel viewModel, Window targetWindow)
    {
        var appWindow = targetWindow.AppWindow;
        var workArea = GetCurrentWorkArea(targetWindow);
        var restored = new WindowBounds(
            viewModel.WindowX ?? workArea.X,
            viewModel.WindowY ?? workArea.Y,
            viewModel.WindowWidth,
            viewModel.WindowHeight);
        var hasSavedPosition = viewModel.WindowX.HasValue && viewModel.WindowY.HasValue;
        var target = hasSavedPosition && WindowPositioning.Intersects(workArea, restored)
            ? restored
            : WindowPositioning.TopCenter(workArea, restored);
        MoveAndResize(targetWindow, target);
    }

    private void OnAppWindowChanged(AppWindow sender, AppWindowChangedEventArgs args)
    {
        if (args.DidPositionChange || args.DidSizeChange)
        {
            ActiveViewModel?.UpdateWindowGeometry(ToBounds(sender));
        }
    }

    private MainViewModel? ActiveViewModel { get; set; }

    private static bool TryGetWindowsWorkArea(
        Window targetWindow,
        out WindowBounds workArea)
    {
        if (!TryGetWin32Handle(targetWindow, out var windowHandle))
        {
            workArea = default;
            return false;
        }

        var monitor = MonitorFromWindow(windowHandle, 2);
        var info = new MonitorInfo
        {
            Size = Marshal.SizeOf<MonitorInfo>(),
        };

        if (monitor != 0 && GetMonitorInfo(monitor, ref info))
        {
            workArea = new WindowBounds(
                info.WorkArea.Left,
                info.WorkArea.Top,
                info.WorkArea.Right - info.WorkArea.Left,
                info.WorkArea.Bottom - info.WorkArea.Top);
            return true;
        }

        workArea = default;
        return false;
    }

    private static bool TryGetWin32Handle(Window targetWindow, out nint handle)
    {
        const BindingFlags InstanceMembers =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        var nativeWindow = targetWindow
            .GetType()
            .GetProperty("NativeWindow", InstanceMembers)
            ?.GetValue(targetWindow);
        var hwndSource = nativeWindow
            ?.GetType()
            .GetProperty("HwndSourceWindow", InstanceMembers)
            ?.GetValue(nativeWindow);
        var value = hwndSource
            ?.GetType()
            .GetProperty("Handle", InstanceMembers)
            ?.GetValue(hwndSource);

        handle = value switch
        {
            IntPtr pointer => pointer,
            _ => 0,
        };
        return handle != 0 && IsWindow(handle);
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindow(nint window);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetMonitorInfo(nint monitor, ref MonitorInfo monitorInfo);

    [DllImport("user32.dll")]
    private static extern nint MonitorFromWindow(nint window, uint flags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(
        nint window,
        nint insertAfter,
        int x,
        int y,
        int width,
        int height,
        uint flags);

    [StructLayout(LayoutKind.Sequential)]
    private struct MonitorInfo
    {
        public int Size;
        public NativeRectangle Monitor;
        public NativeRectangle WorkArea;
        public uint Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRectangle
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
