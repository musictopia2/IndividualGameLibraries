using BasicGameFramework.Attributes;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace FroggiesCP
{
    [SingletonGame]
    public class FroggiesGameBoardCP : ObservableObject
    {
        internal FroggiesViewModel ThisMod;
        private readonly RandomGenerator _rs;
        private readonly EventAggregator _thisE;
        public FroggiesGameBoardCP(FroggiesViewModel thisMod, RandomGenerator rs, EventAggregator thisE)
        {
            ThisMod = thisMod;
            _rs = rs;
            _thisE = thisE;
        }
        private Dictionary<string, LilyPadCP>? _col_Pads;
        private Dictionary<string, LilyPadCP>? _col_TargetPads = new Dictionary<string, LilyPadCP>();
        private CustomBasicList<LilyPadCP>? _col_RedoPads;
        private LilyPadCP? _obj_SelectedPad;
        public CustomBasicList<LilyPadCP> GetCompleteLilyList()
        {
            return (from items in _col_Pads
                    select items.Value).ToCustomBasicList();
        }

        public int NumberOfFrogs
        {
            get
            {
                return ThisMod.NumberOfFrogs;
            }
            set
            {
                ThisMod.NumberOfFrogs = value;
            }
        }

        public int NumberOfMoves()
        {
            return MovesLeft();
        }

        private void NeedsToDraw()
        {
            SubscribeGameBoardEventModel temps = new SubscribeGameBoardEventModel();
            temps.DrawCategory = EnumDrawCategory.Redraw;
            _thisE.Publish(temps);
        }

        private void NewLilyList()
        {
            SubscribeGameBoardEventModel temps = new SubscribeGameBoardEventModel();
            temps.DrawCategory = EnumDrawCategory.NewLilyList;
            _thisE.Publish(temps);
        }

        public void DrawBoard(SKCanvas thisCanvas, float Width, float Height) // i think when drawing the board, it needs to recalculate so it can draw the pads as well.
        {
            SKRect bounds = SKRect.Create(0, 0, Width, Height);
            SKPaint ThisPaint = new SKPaint();
            ThisPaint.IsAntialias = true;
            ThisPaint.FilterQuality = SKFilterQuality.High;
            SKPoint FirstPoint;
            SKPoint SecondPoint;
            FirstPoint = new SKPoint(0, 0);
            SecondPoint = new SKPoint(0, Height);
            SKColor[] Colors;
            Colors = new SKColor[2];
            SKColor FirstColor;
            SKColor SecondColor;
            FirstColor = SKColors.LightBlue;
            SecondColor = SKColors.DarkBlue;
            Colors[0] = FirstColor;
            Colors[1] = SecondColor;
            ThisPaint.Shader = SKShader.CreateLinearGradient(FirstPoint, SecondPoint, Colors, null, SKShaderTileMode.Clamp);
            thisCanvas.DrawRect(bounds, ThisPaint);
        }
        private void CreateLilyPads()
        {
            LilyPadCP? obj_TempPad1 = null;
            LilyPadCP? obj_TempPad2 = null;
            LilyPadCP obj_CurrentPad;
            int int_RowCount;
            int int_ColCount;
            int int_FrogCount;
            int int_TryCount = 0;
            int int_RandomNumber;
            int int_RandomIndex;
            int int_NewX = 0;
            int int_NewY = 0;
            int int_NewX2 = 0;
            int int_NewY2 = 0;
            bool bln_Pad1OK;
            bool bln_Pad2OK;
            _col_Pads = new Dictionary<string, LilyPadCP>();
            _col_TargetPads = new Dictionary<string, LilyPadCP>();
            _obj_SelectedPad = null;
            int_ColCount = 4;
            int_RowCount = 4;

            // *** This will be the final pad in the puzzle
            obj_CurrentPad = new LilyPadCP(int_ColCount, int_RowCount, true);
            _col_Pads.Add($"{int_ColCount}, {int_RowCount}", obj_CurrentPad);
            int_FrogCount = 1;
            while (int_FrogCount < NumberOfFrogs)
            {

                // *** Get a random occupied pad
                int_RandomIndex = GetRandomInteger(_col_Pads.Count - 1);
                obj_CurrentPad = _col_Pads.ElementAt(int_RandomIndex).Value;
                // *** Only find a new path if the pad has a frog on it
                if (obj_CurrentPad.HasFrog)
                {
                    int_RandomNumber = GetRandomInteger(4);

                    // *** Pick a random direction in which to move
                    switch (int_RandomNumber)
                    {
                        case 0: // move up
                            {
                                int_NewX = obj_CurrentPad.Column;
                                int_NewY = obj_CurrentPad.Row - 1;
                                int_NewX2 = obj_CurrentPad.Column;
                                int_NewY2 = obj_CurrentPad.Row - 2;
                                break;
                            }

                        case 1: // move down
                            {
                                int_NewX = obj_CurrentPad.Column;
                                int_NewY = obj_CurrentPad.Row + 1;
                                int_NewX2 = obj_CurrentPad.Column;
                                int_NewY2 = obj_CurrentPad.Row + 2;
                                break;
                            }

                        case 2: // move left
                            {
                                int_NewX = obj_CurrentPad.Column - 1;
                                int_NewY = obj_CurrentPad.Row;
                                int_NewX2 = obj_CurrentPad.Column - 2;
                                int_NewY2 = obj_CurrentPad.Row;
                                break;
                            }

                        case 3: // move right
                            {
                                int_NewX = obj_CurrentPad.Column + 1;
                                int_NewY = obj_CurrentPad.Row;
                                int_NewX2 = obj_CurrentPad.Column + 2;
                                int_NewY2 = obj_CurrentPad.Row;
                                break;
                            }
                    }

                    bln_Pad1OK = false;
                    bln_Pad2OK = false;

                    if ((int_NewX != int_NewX2) | (int_NewY != int_NewY2))
                    {

                        // *** If the target pad already exists, make sure it is blank
                        if (_col_Pads.ContainsKey(int_NewX + ", " + int_NewY))
                        {
                            obj_TempPad1 = _col_Pads[int_NewX + ", " + int_NewY];
                            if (!obj_TempPad1.HasFrog)
                                bln_Pad1OK = true;
                        }
                        else
                        {
                            bln_Pad1OK = true;
                        }

                        // *** If the target pad already exists, make sure it is blank
                        if (_col_Pads.ContainsKey(int_NewX2 + ", " + int_NewY2))
                        {
                            obj_TempPad2 = _col_Pads[int_NewX2 + ", " + int_NewY2];
                            if (!obj_TempPad2.HasFrog)
                                bln_Pad2OK = true;
                        }
                        else
                        {
                            bln_Pad2OK = true;
                        }

                        if (bln_Pad1OK & bln_Pad2OK & (int_NewX2 >= 0) & (int_NewX2 <= 9) & (int_NewX >= 0) & (int_NewX <= 9) & (int_NewY2 >= 0) & (int_NewY2 <= 9) & (int_NewY >= 0) & (int_NewY <= 9))
                        {
                            if (_col_Pads.ContainsKey(int_NewX + ", " + int_NewY))
                            {
                                obj_TempPad1!.HasFrog = true;
                            }
                            else
                            {
                                obj_TempPad1 = new LilyPadCP(int_NewX, int_NewY, true);
                                _col_Pads.Add(int_NewX + ", " + int_NewY, obj_TempPad1);
                            }

                            if (_col_Pads.ContainsKey(int_NewX2 + ", " + int_NewY2))
                            {
                                obj_TempPad2!.HasFrog = true;
                            }
                            else
                            {
                                obj_TempPad2 = new LilyPadCP(int_NewX2, int_NewY2, true);
                                _col_Pads.Add(int_NewX2 + ", " + int_NewY2, obj_TempPad2);
                            }

                            int_TryCount = 0;
                            obj_CurrentPad.HasFrog = false;
                            int_FrogCount = GetNumberOfFrogs;
                        }
                    }
                }

                int_TryCount += 1;

                // *** Make sure we didn't get stuck - if we did, start over
                if (int_TryCount >= 1000)
                {
                    _col_Pads.Clear();
                    _col_Pads = new Dictionary<string, LilyPadCP>();
                    obj_CurrentPad = new LilyPadCP(4, 4, true);
                    _col_Pads.Add(int_ColCount + ", " + int_RowCount, obj_CurrentPad);
                    int_FrogCount = 1;
                }
            }
        }

        private int GetNumberOfFrogs => _col_Pads!.Values.Count(items => items.HasFrog);
        private int GetRandomInteger(int int_Span)
        {
            int int_RandomInteger;
            //string str_Random;
            if (int_Span <= 0)
                int_RandomInteger = 0;
            else
            {
                int_RandomInteger = _rs.GetRandomNumber(int_Span, 0); //because its 0 based.  try this way.  taking a risk.  hopefully this way works.
            }
            return int_RandomInteger;
        }
        private int MovesLeft()
        {
            IEnumerator enum_Pads;
            enum_Pads = _col_Pads!.GetEnumerator();
            LilyPadCP obj_TempTab;
            LilyPadCP obj_TargetPad;
            LilyPadCP obj_LeapPad;
            int int_CountMoves = 0;
            while (enum_Pads.MoveNext())
            {
                var ThisItem = (KeyValuePair<string, LilyPadCP>)enum_Pads.Current;
                obj_TempTab = ThisItem.Value;
                if (obj_TempTab.HasFrog)
                {

                    // *** Check up

                    if (_col_Pads.ContainsKey($"{obj_TempTab.Column}, {obj_TempTab.Row - 2}") & _col_Pads.ContainsKey($"{obj_TempTab.Column}, {obj_TempTab.Row - 1}"))
                    {
                        obj_TargetPad = _col_Pads[$"{obj_TempTab.Column}, {obj_TempTab.Row - 2}"];
                        obj_LeapPad = _col_Pads[$"{obj_TempTab.Column}, {obj_TempTab.Row - 1}"];
                        if ((!obj_TargetPad.HasFrog) & obj_LeapPad.HasFrog)
                            int_CountMoves += 1;
                    }

                    // *** Check down

                    if (_col_Pads.ContainsKey($"{obj_TempTab.Column}, {obj_TempTab.Row + 2}") & _col_Pads.ContainsKey($"{obj_TempTab.Column}, {obj_TempTab.Row + 1}"))
                    {
                        obj_TargetPad = _col_Pads[$"{obj_TempTab.Column}, {obj_TempTab.Row + 2}"];
                        obj_LeapPad = _col_Pads[$"{obj_TempTab.Column}, {obj_TempTab.Row + 1}"];
                        if ((!obj_TargetPad.HasFrog) & obj_LeapPad.HasFrog)
                            int_CountMoves += 1;
                    }



                    // *** Check left

                    if (_col_Pads.ContainsKey($"{obj_TempTab.Column - 2}, {obj_TempTab.Row}") & _col_Pads.ContainsKey($"{obj_TempTab.Column - 1}, {obj_TempTab.Row}"))
                    {
                        obj_TargetPad = _col_Pads[$"{obj_TempTab.Column - 2}, {obj_TempTab.Row}"];
                        obj_LeapPad = _col_Pads[$"{obj_TempTab.Column - 1}, {obj_TempTab.Row}"];
                        if ((!obj_TargetPad.HasFrog) & obj_LeapPad.HasFrog)
                            int_CountMoves += 1;
                    }


                    // *** Check right

                    if (_col_Pads.ContainsKey($"{obj_TempTab.Column + 2}, {obj_TempTab.Row}") & _col_Pads.ContainsKey($"{obj_TempTab.Column + 1}, {obj_TempTab.Row}"))
                    {
                        obj_TargetPad = _col_Pads[$"{obj_TempTab.Column + 2}, {obj_TempTab.Row}"];
                        obj_LeapPad = _col_Pads[$"{obj_TempTab.Column + 1}, {obj_TempTab.Row}"];
                        if ((!obj_TargetPad.HasFrog) & obj_LeapPad.HasFrog)
                            int_CountMoves += 1;
                    }


                }
            }

            return int_CountMoves;
        }

        private bool CheckIfWon()
        {
            IEnumerator enum_Pads;
            LilyPadCP obj_TempPad;
            int int_FrogCount = 0;
            enum_Pads = _col_Pads!.GetEnumerator();

            // *** Find the selected pad
            while (enum_Pads.MoveNext())
            {
                var ThisItem = (KeyValuePair<string, LilyPadCP>)enum_Pads.Current;
                obj_TempPad = ThisItem.Value;
                if (obj_TempPad.HasFrog)
                    int_FrogCount += 1;
            }

            if (int_FrogCount > 1)
                return false;
            else
                return true;
        }

        private async Task LeapFrogAsync(LilyPadCP obj_Target)
        {
            LilyPadCP obj_Leap;
            string str_Index = "";
            // Stop
            if (obj_Target.Row < _obj_SelectedPad!.Row)
                str_Index = $"{_obj_SelectedPad.Column}, {_obj_SelectedPad.Row - 1}";
            else if (obj_Target.Row > _obj_SelectedPad.Row)
                str_Index = $"{_obj_SelectedPad.Column}, {_obj_SelectedPad.Row + 1}";
            else if (obj_Target.Column < _obj_SelectedPad.Column)
                str_Index = $"{_obj_SelectedPad.Column - 1}, {_obj_SelectedPad.Row}";
            else if (obj_Target.Column > _obj_SelectedPad.Column)
                str_Index = $"{_obj_SelectedPad.Column + 1}, {_obj_SelectedPad.Row}";

            obj_Leap = _col_Pads![str_Index];
            obj_Leap.HasFrog = false;
            _obj_SelectedPad.HasFrog = false;
            obj_Leap.IsSelected = false;
            obj_Leap.IsTarget = false;
            _obj_SelectedPad.IsSelected = false;
            _obj_SelectedPad.IsTarget = false;
            obj_Target.HasFrog = true;
            _obj_SelectedPad = null;
            _col_TargetPads = new Dictionary<string, LilyPadCP>();
            NeedsToDraw();
            ThisMod.MovesLeft = NumberOfMoves();
            // *** Check if won
            if (CheckIfWon())
            {
                ThisMod.NumberOfFrogs++;
                NewGame();
            }
            else if (MovesLeft() == 0)
            {
                await ThisMod.ShowGameMessageAsync("No Moves Left");
                NewGame(); //i think.
                ThisMod.MovesLeft = NumberOfMoves();
            }
        }
        public async Task ProcessLilyClickAsync(LilyPadCP obj_TempPad)
        {
            LilyPadCP obj_TargetPad;
            LilyPadCP obj_LeapPad;
            foreach (var ThisItem in _col_Pads!)
            {
                ThisItem.Value.IsSelected = false; // i think
                ThisItem.Value.IsTarget = false;
            }
            if (obj_TempPad.HasFrog)
            {
                _obj_SelectedPad = obj_TempPad;
                _obj_SelectedPad.IsSelected = true;
            }
            else
                // *** Check if it is a target pad
                if (_col_TargetPads!.ContainsKey(obj_TempPad.Column + ", " + obj_TempPad.Row))
            {
                await LeapFrogAsync(obj_TempPad);
                return;
            }
            _col_TargetPads = new Dictionary<string, LilyPadCP>();
            // *** Find the target pads
            if (!(_obj_SelectedPad == null))
            {

                // *** Check the different directions we can leap

                // *** Check up

                if (_col_Pads.ContainsKey($"{_obj_SelectedPad.Column}, {_obj_SelectedPad.Row - 2}") & _col_Pads.ContainsKey($"{_obj_SelectedPad.Column}, {_obj_SelectedPad.Row - 1}"))
                {
                    obj_TargetPad = _col_Pads[$"{_obj_SelectedPad.Column}, {_obj_SelectedPad.Row - 2}"];
                    obj_LeapPad = _col_Pads[$"{_obj_SelectedPad.Column}, {_obj_SelectedPad.Row - 1}"];
                    if ((!obj_TargetPad.HasFrog) & obj_LeapPad.HasFrog)
                        _col_TargetPads.Add($"{obj_TargetPad.Column}, {obj_TargetPad.Row}", obj_TargetPad);
                }



                // *** Check down

                if (_col_Pads.ContainsKey($"{_obj_SelectedPad.Column}, {_obj_SelectedPad.Row + 2}") & _col_Pads.ContainsKey($"{_obj_SelectedPad.Column}, {_obj_SelectedPad.Row + 1}"))
                {
                    obj_TargetPad = _col_Pads[$"{_obj_SelectedPad.Column}, {_obj_SelectedPad.Row + 2}"];
                    obj_LeapPad = _col_Pads[$"{_obj_SelectedPad.Column}, {_obj_SelectedPad.Row + 1}"];
                    if ((!obj_TargetPad.HasFrog) & obj_LeapPad.HasFrog)
                        _col_TargetPads.Add($"{obj_TargetPad.Column}, {obj_TargetPad.Row}", obj_TargetPad);
                }



                // *** Check left

                if (_col_Pads.ContainsKey($"{_obj_SelectedPad.Column - 2}, {_obj_SelectedPad.Row}") & _col_Pads.ContainsKey($"{_obj_SelectedPad.Column - 1}, {_obj_SelectedPad.Row}"))
                {
                    obj_TargetPad = _col_Pads[$"{_obj_SelectedPad.Column - 2}, {_obj_SelectedPad.Row}"];
                    obj_LeapPad = _col_Pads[$"{_obj_SelectedPad.Column - 1}, {_obj_SelectedPad.Row}"];
                    if ((!obj_TargetPad.HasFrog) & obj_LeapPad.HasFrog)
                        _col_TargetPads.Add($"{obj_TargetPad.Column}, {obj_TargetPad.Row}", obj_TargetPad);
                }




                // *** Check right

                if (_col_Pads.ContainsKey($"{_obj_SelectedPad.Column + 2}, {_obj_SelectedPad.Row}") & _col_Pads.ContainsKey($"{_obj_SelectedPad.Column + 1}, {_obj_SelectedPad.Row}"))
                {
                    obj_TargetPad = _col_Pads[$"{_obj_SelectedPad.Column + 2}, {_obj_SelectedPad.Row}"];
                    obj_LeapPad = _col_Pads[$"{_obj_SelectedPad.Column + 1}, {_obj_SelectedPad.Row}"];
                    if ((!obj_TargetPad.HasFrog) & obj_LeapPad.HasFrog)
                        _col_TargetPads.Add($"{obj_TargetPad.Column}, {obj_TargetPad.Row}", obj_TargetPad);
                }
            }
            foreach (var ThisItem in _col_TargetPads)
                ThisItem.Value.IsTarget = true;
            NeedsToDraw();
        }

        private void CopyToRedo()
        {
            _col_RedoPads = new CustomBasicList<LilyPadCP>();
            _col_RedoPads.Clear();
            IEnumerator enum_Pads;
            LilyPadCP obj_TempPad;
            LilyPadCP obj_NewPad;
            enum_Pads = _col_Pads!.GetEnumerator();
            while (enum_Pads.MoveNext())
            {
                var FirstTemp = (KeyValuePair<string, LilyPadCP>)enum_Pads.Current;
                obj_TempPad = FirstTemp.Value;
                obj_NewPad = new LilyPadCP(obj_TempPad.Column, obj_TempPad.Row, obj_TempPad.HasFrog);
                if (obj_TempPad.HasFrog)
                    obj_NewPad.DidStartWithFrog();
                _col_RedoPads.Add(obj_NewPad);
            }
        }

        public void Redo()
        {
            LilyPadCP obj_TempPad;
            _col_Pads!.Clear();
            IEnumerator enum_Pads;
            enum_Pads = _col_RedoPads!.GetEnumerator();
            while (enum_Pads.MoveNext())
            {
                obj_TempPad = (LilyPadCP)enum_Pads.Current; //hopefully this works.
                _col_Pads.Add(obj_TempPad.Column + ", " + obj_TempPad.Row, new LilyPadCP(obj_TempPad.Column, obj_TempPad.Row, obj_TempPad.StartedWithFrog()));
            }
            _col_TargetPads = new Dictionary<string, LilyPadCP>();
            _obj_SelectedPad = null;
            NewLilyList();
        }
        public void NewGame()
        {
            CreateLilyPads();
            CopyToRedo();
            NewLilyList();
            ThisMod.MovesLeft = MovesLeft();
        }
    }
}