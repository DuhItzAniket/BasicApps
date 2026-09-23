using NAudio.CoreAudioApi;
using System;
using System.Runtime.InteropServices;
using BasicApps.Interop;

namespace BasicApps.Services;

public sealed class AudioService : IDisposable
{
    private readonly MMDeviceEnumerator _enumerator;
    private MMDevice? _defaultPlaybackDevice;

    public AudioService()
    {
        _enumerator = new MMDeviceEnumerator();
        RefreshDefaultDevice();
    }

    public void RefreshDefaultDevice()
    {
        _defaultPlaybackDevice = _enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
    }

    public List<AudioDeviceInfo> GetPlaybackDevices()
    {
        var devices = _enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
        return devices.Select(d => new AudioDeviceInfo
        {
            Id = d.ID,
            Name = d.FriendlyName,
            IsDefault = d.ID == _defaultPlaybackDevice?.ID
        }).ToList();
    }

    public List<AudioDeviceInfo> GetRecordingDevices()
    {
        var devices = _enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
        return devices.Select(d => new AudioDeviceInfo
        {
            Id = d.ID,
            Name = d.FriendlyName,
            IsDefault = d.ID == _enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia)?.ID
        }).ToList();
    }

    public bool SetDefaultPlaybackDevice(string deviceId)
    {
        try
        {
            var device = _enumerator.GetDevice(deviceId);
            if (device == null) return false;

            RefreshDefaultDevice();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void CyclePlaybackDevice()
    {
        var devices = GetPlaybackDevices();
        if (devices.Count <= 1) return;

        var currentIndex = devices.FindIndex(d => d.IsDefault);
        var nextIndex = (currentIndex + 1) % devices.Count;
        SetDefaultPlaybackDevice(devices[nextIndex].Id);
    }

    public bool IsMicrophoneMuted()
    {
        try
        {
            var device = _enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
            return device?.AudioEndpointVolume.Mute ?? false;
        }
        catch
        {
            return false;
        }
    }

    public void ToggleMicrophoneMute()
    {
        try
        {
            var device = _enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
            if (device != null)
            {
                device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
            }
        }
        catch { }
    }

    public void SetMicrophoneMute(bool mute)
    {
        try
        {
            var device = _enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
            if (device != null)
            {
                device.AudioEndpointVolume.Mute = mute;
            }
        }
        catch { }
    }

    public float GetMasterVolume()
    {
        try
        {
            return _defaultPlaybackDevice?.AudioEndpointVolume.MasterVolumeLevelScalar ?? 0f;
        }
        catch
        {
            return 0f;
        }
    }

    public void SetMasterVolume(float volume)
    {
        try
        {
            if (_defaultPlaybackDevice != null)
            {
                _defaultPlaybackDevice.AudioEndpointVolume.MasterVolumeLevelScalar = Math.Clamp(volume, 0f, 1f);
            }
        }
        catch { }
    }

    public void Dispose()
    {
        _defaultPlaybackDevice?.Dispose();
        _enumerator?.Dispose();
    }
}

public sealed class AudioDeviceInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}