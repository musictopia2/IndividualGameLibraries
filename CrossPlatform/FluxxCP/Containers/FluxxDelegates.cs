using BasicGameFrameworkLibrary.Attributes;
using FluxxCP.Data;
using System;
using System.Threading.Tasks;

namespace FluxxCP.Containers
{
    [SingletonGame]
    public class FluxxDelegates
    {
        internal Func<ActionContainer, Task>? LoadProperActionScreenAsync { get; set; }
        internal Func<Task>? LoadMainScreenAsync { get; set; }
        //we may need the container for the keepers (?)
        internal Func<KeeperContainer, Task>? LoadKeeperScreenAsync { get; set; }

        internal Func<EnumActionScreen>? CurrentScreen { get; set; }
        internal Action? RefreshEnables { get; set; }
    }
}