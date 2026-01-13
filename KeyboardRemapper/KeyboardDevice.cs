using System;
using System.Collections.Generic;
using Linearstar.Windows.RawInput;
using Linearstar.Windows.RawInput.Native;

namespace KeyboardRemapper
{
    /// <summary>
    /// キーボードデバイスの情報を保持するクラス
    /// </summary>
    public class KeyboardDevice
    {
        public IntPtr DeviceHandle { get; set; }
        public string? DeviceName { get; set; }
        public string? DeviceId { get; set; }
        public int VendorId { get; set; }
        public int ProductId { get; set; }

        public override string ToString()
        {
            return $"{DeviceName} (VID: {VendorId:X04}, PID: {ProductId:X04})";
        }
    }

    /// <summary>
    /// キーボードデバイスマネージャー
    /// </summary>
    public class KeyboardDeviceManager
    {
        private Dictionary<IntPtr, KeyboardDevice> _devices = new();

        /// <summary>
        /// 接続されているキーボードデバイスを検出
        /// </summary>
        public List<KeyboardDevice> DetectKeyboards()
        {
            _devices.Clear();
            var detectedDevices = new List<KeyboardDevice>();

            try
            {
                var devices = RawInputDevice.GetDevices();
                
                foreach (var device in devices)
                {
                    if (device is RawInputKeyboard keyboard)
                    {
                        var deviceId = $"{keyboard.VendorId:X04}:{keyboard.ProductId:X04}";
                        var deviceName = keyboard.ProductName ?? keyboard.ManufacturerName ?? "Unknown Keyboard";
                        
                        var deviceHandle = RawInputDeviceHandle.GetRawValue(keyboard.Handle);
                        var kbDevice = new KeyboardDevice
                        {
                            DeviceHandle = deviceHandle,
                            DeviceName = deviceName,
                            DeviceId = deviceId,
                            VendorId = keyboard.VendorId,
                            ProductId = keyboard.ProductId
                        };

                        _devices[deviceHandle] = kbDevice;
                        detectedDevices.Add(kbDevice);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error detecting keyboards: {ex.Message}");
            }

            return detectedDevices;
        }



        /// <summary>
        /// デバイスハンドルからキーボードデバイスを取得
        /// </summary>
        public KeyboardDevice GetDevice(IntPtr deviceHandle)
        {
            return _devices.TryGetValue(deviceHandle, out var device) ? device : null;
        }

        /// <summary>
        /// 登録されているすべてのデバイスを取得
        /// </summary>
        public IEnumerable<KeyboardDevice> GetAllDevices()
        {
            return _devices.Values;
        }
    }
}
