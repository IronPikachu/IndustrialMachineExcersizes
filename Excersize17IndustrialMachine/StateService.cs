using IndustrialMachine.Data.Models;
using Microsoft.AspNetCore.Components;

internal class StateService
{
    public string SubTitle { get; private set; }
    public Device Device { get; private set; }

    public event Action OnChange;

    public async Task SetSubTitle(string title)
    {
        SubTitle = title;
        NotifyStateChanged();
    } 
    
    public async Task SetDevice(Device device)
    {
        Device = device;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    public void UpdateDevice(Device device)
    {
        if (device.SendData == null) return;

        device.Data = System.Text.Encoding.ASCII.GetBytes(device.SendData);
        device.SendData = null;
        NotifyStateChanged();
    }
}