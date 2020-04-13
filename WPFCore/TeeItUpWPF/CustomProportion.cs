using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
namespace TeeItUpWPF
{
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion => 1.92f; //after lots of trial/error decided on 1.92 for desktops.
    }
}