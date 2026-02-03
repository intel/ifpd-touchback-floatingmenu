using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace TouchDataConsole
{
    class Program
    {
        // Windows API constants
        private const int WM_POINTERDOWN = 0x0246;
        private const int WM_POINTERUP = 0x0247;
        private const int WM_POINTERUPDATE = 0x0245;
        private const int WM_CLOSE = 0x0010;

        // Pointer input types
        private const int PT_POINTER = 0x00000001;
        private const int PT_TOUCH = 0x00000002;
        private const int PT_PEN = 0x00000003;

        // Window class styles
        private const uint CS_HREDRAW = 0x0002;
        private const uint CS_VREDRAW = 0x0001;

        // Window styles
        private const int WS_OVERLAPPEDWINDOW = 0x00CF0000;
        private const int CW_USEDEFAULT = unchecked((int)0x80000000);

        // Show window constants
        private const int SW_SHOWNORMAL = 1;
        private const int IDC_ARROW = 32512;

        // Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public POINT pt;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WNDCLASSW
        {
            public uint style;
            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszMenuName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszClassName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTER_INFO
        {
            public int pointerType;
            public uint pointerId;
            public uint frameId;
            public int pointerFlags;
            public IntPtr sourceDevice;
            public IntPtr hwndTarget;
            public POINT ptPixelLocation;
            public POINT ptHimetricLocation;
            public POINT ptPixelLocationRaw;
            public POINT ptHimetricLocationRaw;
            public uint dwTime;
            public uint historyCount;
            public int inputData;
            public uint dwKeyStates;
            public ulong PerformanceCount;
            public int ButtonChangeType;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTER_TOUCH_INFO
        {
            public POINTER_INFO pointerInfo;
            public uint touchFlags;
            public uint touchMask;
            public RECT rcContact;
            public RECT rcContactRaw;
            public uint orientation;
            public uint pressure;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTER_DEVICE_PROPERTY
        {
            public int logicalMin;
            public int logicalMax;
            public int physicalMin;
            public int physicalMax;
            public uint unit;
            public uint unitExponent;
            public ushort usagePageId;
            public ushort usageId;
        }

        // Delegate for window procedure
        public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        // Windows API imports
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern ushort RegisterClassW(ref WNDCLASSW lpWndClass);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateWindowExW(
            uint dwExStyle,
            [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
            [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
            uint dwStyle,
            int x, int y, int nWidth, int nHeight,
            IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        public static extern bool TranslateMessage(ref MSG lpMsg);

        [DllImport("user32.dll")]
        public static extern IntPtr DispatchMessage(ref MSG lpMsg);

        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetPointerInfo(uint pointerId, ref POINTER_INFO pointerInfo);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetPointerTouchInfo(uint pointerId, ref POINTER_TOUCH_INFO touchInfo);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetPointerDeviceProperties(IntPtr device, ref uint propertyCount, IntPtr pProperties);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetRawPointerDeviceData(
            uint pointerId,
            uint historyCount,
            uint propertiesCount,
            IntPtr pProperties,
            IntPtr pValues);

        [DllImport("user32.dll")]
        public static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern void PostQuitMessage(int nExitCode);

        [DllImport("user32.dll")]
        public static extern bool GetGestureInfo(IntPtr hGestureInfo, ref GESTUREINFO pGestureInfo);

        [StructLayout(LayoutKind.Sequential)]
        public struct GESTUREINFO
        {
            public uint cbSize;
            public uint dwFlags;
            public uint dwID;
            public IntPtr hwndTarget;
            public POINTS ptsLocation;
            public uint dwInstanceID;
            public uint dwSequenceID;
            public ulong ullArguments;
            public uint cbExtraArgs;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTS
        {
            public short x;
            public short y;
        }

        // Windows message constants
        private const int WM_GESTURE = 0x0119;
        private const int WM_GESTURENOTIFY = 0x011A;

        // Gesture IDs
        private const int GID_BEGIN = 1;
        private const int GID_END = 2;
        private const int GID_ZOOM = 3;
        private const int GID_PAN = 4;
        private const int GID_ROTATE = 5;
        private const int GID_TWOFINGERTAP = 6;
        private const int GID_PRESSANDTAP = 7;

        private static IntPtr hwnd;
        private static bool running = true;
        private static WndProc wndProcDelegate;

        static void Main(string[] args)
        {
            Console.WriteLine("Touch Data Console Application");
            Console.WriteLine("Creating window to capture touch messages...");
            Console.WriteLine("Touch the screen to see touch data. Press Ctrl+C to exit.\n");

            Console.CancelKeyPress += (sender, e) => {
                e.Cancel = true;
                running = false;
                if (hwnd != IntPtr.Zero)
                {
                    PostQuitMessage(0);
                }
            };

            try
            {
                wndProcDelegate = WindowProc;

                WNDCLASSW wc = new WNDCLASSW
                {
                    style = CS_HREDRAW | CS_VREDRAW,
                    lpfnWndProc = Marshal.GetFunctionPointerForDelegate(wndProcDelegate),
                    cbClsExtra = 0,
                    cbWndExtra = 0,
                    hInstance = GetModuleHandle(null),
                    hIcon = IntPtr.Zero,
                    hCursor = LoadCursor(IntPtr.Zero, IDC_ARROW),
                    hbrBackground = IntPtr.Zero,
                    lpszMenuName = null,
                    lpszClassName = "TouchDataWindow"
                };

                ushort classAtom = RegisterClassW(ref wc);
                if (classAtom == 0)
                {
                    int error = Marshal.GetLastWin32Error();
                    Console.WriteLine($"Failed to register window class. Error: {error}");
                    return;
                }

                hwnd = CreateWindowExW(
                    0,
                    "TouchDataWindow",
                    "Touch Data Capture",
                    (uint)WS_OVERLAPPEDWINDOW,
                    CW_USEDEFAULT, CW_USEDEFAULT, 800, 600,
                    IntPtr.Zero, IntPtr.Zero, GetModuleHandle(null), IntPtr.Zero);

                if (hwnd == IntPtr.Zero)
                {
                    int error = Marshal.GetLastWin32Error();
                    Console.WriteLine($"Failed to create window. Error: {error}");
                    return;
                }

                ShowWindow(hwnd, SW_SHOWNORMAL);
                UpdateWindow(hwnd);

                Console.WriteLine("Window created successfully. Listening for touch events...\n");

                MSG msg;
                while (running)
                {
                    bool result = GetMessage(out msg, IntPtr.Zero, 0, 0);
                    if (!result)
                        break;

                    TranslateMessage(ref msg);
                    DispatchMessage(ref msg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                if (hwnd != IntPtr.Zero)
                {
                    DestroyWindow(hwnd);
                }
            }

            Console.WriteLine("\nApplication terminated.");
        }

        private static IntPtr WindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                switch (uMsg)
                {
                    //case WM_GESTURE:
                    //    HandleGestureMessage(lParam);
                    //    break;

                    //case WM_GESTURENOTIFY:
                    //    Console.WriteLine("Gesture notification received");
                    //    break;

                    case WM_POINTERDOWN:
                    case WM_POINTERUP:
                    case WM_POINTERUPDATE:
                        HandlePointerMessage(uMsg, wParam, lParam);
                        return IntPtr.Zero;
                    case WM_CLOSE:
                        running = false;
                        PostQuitMessage(0);
                        return IntPtr.Zero;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in WindowProc: {ex.Message}");
            }

            return DefWindowProc(hWnd, uMsg, wParam, lParam);
        }

        private static void HandlePointerMessage(uint message, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                uint pointerId = (uint)(wParam.ToInt32() & 0xFFFF);

                POINTER_INFO pointerInfo = new POINTER_INFO();
                if (!GetPointerInfo(pointerId, ref pointerInfo))
                {
                    int error = Marshal.GetLastWin32Error();
                    Console.WriteLine($"Failed to get pointer info for ID: {pointerId}, Error: {error}");
                    return;
                }

                string eventType = message switch
                {
                    WM_POINTERDOWN => "DOWN",
                    WM_POINTERUP => "UP",
                    WM_POINTERUPDATE => "UPDATE",
                    _ => "UNKNOWN"
                };

                string pointerType = pointerInfo.pointerType switch
                {
                    PT_TOUCH => "TOUCH",
                    PT_PEN => "PEN",
                    PT_POINTER => "POINTER",
                    _ => "UNKNOWN"
                };

                Console.WriteLine($"=== {eventType} Event ===");
                Console.WriteLine($"Pointer ID: {pointerId}");
                Console.WriteLine($"Type: {pointerType}");
                Console.WriteLine($"Position: ({pointerInfo.ptPixelLocation.X}, {pointerInfo.ptPixelLocation.Y})");
                Console.WriteLine($"Raw Position: ({pointerInfo.ptPixelLocationRaw.X}, {pointerInfo.ptPixelLocationRaw.Y})");
                Console.WriteLine($"Frame ID: {pointerInfo.frameId}");
                Console.WriteLine($"Time: {pointerInfo.dwTime}");
                Console.WriteLine($"History Count: {pointerInfo.historyCount}");
                Console.WriteLine($"Source Device: 0x{pointerInfo.sourceDevice.ToInt64():X}");

                // Get touch-specific info
                if (pointerInfo.pointerType == PT_TOUCH)
                {
                    POINTER_TOUCH_INFO touchInfo = new POINTER_TOUCH_INFO();
                    if (GetPointerTouchInfo(pointerId, ref touchInfo))
                    {
                        Console.WriteLine($"Touch Pressure: {touchInfo.pressure}");
                        Console.WriteLine($"Touch Orientation: {touchInfo.orientation}");
                        Console.WriteLine($"Contact Rect: ({touchInfo.rcContact.left}, {touchInfo.rcContact.top}) - ({touchInfo.rcContact.right}, {touchInfo.rcContact.bottom})");

                        int width = touchInfo.rcContact.right - touchInfo.rcContact.left;
                        int height = touchInfo.rcContact.bottom - touchInfo.rcContact.top;
                        Console.WriteLine($"Contact Size: {width} x {height}");
                    }
                }

                // Get raw device data - corrected approach
                GetRawDeviceData(pointerId, pointerInfo);

                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling pointer message: {ex.Message}");
            }
        }

        private static void GetRawDeviceData(uint pointerId, POINTER_INFO pointerInfo)
        {
            try
            {
                // First, check if we have a valid source device
                if (pointerInfo.sourceDevice == IntPtr.Zero)
                {
                    Console.WriteLine("No source device available for raw data");
                    return;
                }

                // Get the number of properties for this device
                uint propertyCount = 0;
                bool success = GetPointerDeviceProperties(pointerInfo.sourceDevice, ref propertyCount, IntPtr.Zero);

                if (!success || propertyCount == 0)
                {
                    int error = Marshal.GetLastWin32Error();
                    Console.WriteLine($"No device properties available. Error: {error}, Count: {propertyCount}");
                    return;
                }

                Console.WriteLine($"Device has {propertyCount} properties");

                // Allocate memory for properties
                int propertySize = Marshal.SizeOf<POINTER_DEVICE_PROPERTY>();
                IntPtr propertiesBuffer = Marshal.AllocHGlobal((int)(propertyCount * propertySize));

                try
                {
                    // Get the actual properties
                    success = GetPointerDeviceProperties(pointerInfo.sourceDevice, ref propertyCount, propertiesBuffer);
                    if (!success)
                    {
                        int error = Marshal.GetLastWin32Error();
                        Console.WriteLine($"Failed to get device properties. Error: {error}");
                        return;
                    }

                    // Parse and display properties
                    for (int i = 0; i < propertyCount; i++)
                    {
                        IntPtr propertyPtr = IntPtr.Add(propertiesBuffer, i * propertySize);
                        POINTER_DEVICE_PROPERTY property = Marshal.PtrToStructure<POINTER_DEVICE_PROPERTY>(propertyPtr);

                        Console.WriteLine($"Property {i}: Usage 0x{property.usagePageId:X4}:0x{property.usageId:X4}, " +
                                        $"Range: {property.logicalMin}-{property.logicalMax}");
                    }

                    // Now try to get raw data using the properties
                    uint historyCount = Math.Max(1u, pointerInfo.historyCount);
                    uint valueCount = propertyCount * historyCount;

                    if (valueCount > 0)
                    {
                        IntPtr valuesBuffer = Marshal.AllocHGlobal((int)(valueCount * sizeof(int)));
                        try
                        {
                            success = GetRawPointerDeviceData(pointerId, historyCount, propertyCount, propertiesBuffer, valuesBuffer);

                            if (success)
                            {
                                Console.WriteLine($"Raw data retrieved successfully! ({valueCount} values)");

                                // Display the raw values
                                int[] values = new int[valueCount];
                                Marshal.Copy(valuesBuffer, values, 0, (int)valueCount);

                                for (int i = 0; i < Math.Min(valueCount, 20); i++) // Show first 20 values
                                {
                                    Console.WriteLine($"  Value[{i}]: {values[i]}");
                                }

                                if (valueCount > 20)
                                {
                                    Console.WriteLine($"  ... and {valueCount - 20} more values");
                                }
                            }
                            else
                            {
                                int error = Marshal.GetLastWin32Error();
                                Console.WriteLine($"GetRawPointerDeviceData failed. Error: {error}");
                            }
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(valuesBuffer);
                        }
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(propertiesBuffer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting raw device data: {ex.Message}");
            }
        }

        private static void HandleGestureMessage(IntPtr lParam)
        {
            GESTUREINFO gi = new GESTUREINFO();
            gi.cbSize = (uint)Marshal.SizeOf(gi);

            if (GetGestureInfo(lParam, ref gi))
            {
                string gestureName = gi.dwID switch
                {
                    GID_ZOOM => "ZOOM",
                    GID_PAN => "PAN",
                    GID_ROTATE => "ROTATE",
                    GID_TWOFINGERTAP => "TWO FINGER TAP",
                    GID_PRESSANDTAP => "PRESS AND TAP",
                    GID_BEGIN => "GESTURE BEGIN",
                    GID_END => "GESTURE END",
                    _ => $"UNKNOWN ({gi.dwID})"
                };

                Console.WriteLine($"=== GESTURE DETECTED ===");
                Console.WriteLine($"Type: {gestureName}");
                Console.WriteLine($"Location: ({gi.ptsLocation.x}, {gi.ptsLocation.y})");
                Console.WriteLine($"Arguments: {gi.ullArguments}");
                Console.WriteLine($"Sequence ID: {gi.dwSequenceID}");
                Console.WriteLine();
            }
        }
    }
}