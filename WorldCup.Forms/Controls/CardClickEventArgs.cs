namespace WorldCup.Forms.Controls;

public class CardClickEventArgs : EventArgs
{
    public bool IsCtrlHeld { get; }
    public CardClickEventArgs(bool ctrlHeld) => IsCtrlHeld = ctrlHeld;
}
