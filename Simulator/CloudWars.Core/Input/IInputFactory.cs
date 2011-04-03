using CloudWars.Core.Settings;

namespace CloudWars.Core.Input
{
    public interface IInputFactory {
        IInputHandler NewInputHandler(CloudType type);
    }
}