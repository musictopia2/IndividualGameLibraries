namespace SorryCP.Data
{
    public class MoveInfo
    {
        public int SpaceFrom { get; set; }
        public int SpaceTo { get; set; }
        public bool IsOptional { get; set; } //for the 11s
        public int NumberUsed { get; set; } //if not 0; then this means additional move is used for the 7's
        public bool IsBackwards { get; set; } //is used for the 10s that can be backwards
    }
}