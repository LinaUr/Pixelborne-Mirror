namespace Assets.Scripts.Tools
{
    // This interface must be implemented by cameras that get used in Singleplayer and Multiplayer mode. 
    public interface ICamera
    {
        void FadeOut();
        void FadeIn();
    }
}
