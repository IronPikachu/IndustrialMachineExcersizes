namespace IndustrialMachine.Data.Models;

public class Device
{
    public string Name { get; set; }
    public Guid Id { get; set; }
    public bool Status { get; set; }
    public byte[] Data { get; set; }
    public string? SendData { get; set; }

    public Device()
    {
        Id = Guid.NewGuid();
    }

}
