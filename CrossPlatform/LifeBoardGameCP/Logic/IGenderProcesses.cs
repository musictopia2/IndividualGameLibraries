using LifeBoardGameCP.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LifeBoardGameCP.Logic
{
    public interface IGenderProcesses
    {
        //void Init();
        Action<string>? SetTurn { get; set; }
        Action<string>? SetInstructions { get; set; } //hopefully this simple.
        Task ChoseGenderAsync(EnumGender gender);
    }
}