using HolmiumOS.GUI.Controls;

namespace HolmiumOS.GUI.Apps
{
    public class Terminal : AppBase
    {
        public Terminal()
            : base("Terminal")
        {

        }


        public override void Load()
        {
            Button button = new Button(
                "Test",
                20,
                50,
                80,
                30
            );


            button.OnClick = () =>
            {
                button.Text = "Clicked";
            };


            Window.AddControl(button);
        }
    }
}