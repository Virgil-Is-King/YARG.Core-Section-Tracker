using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using YARG.Core.Chart;
using YARG.Core.Engine;
using YARG.Core.Engine.Guitar;
using YARG.Core.Extensions;
using YARG.Core.Replays;
using static YARG.Core.Engine.Guitar.EnhancedGuitarStats;


namespace YARG.Core.Engine.Guitar
{
    public class GuitarStats : BaseStats
    {


        /// <summary>
        /// Number of overstrums which have occurred.
        /// </summary>
        public int Overstrums;

        /// <summary>
        /// Number of hammer-ons/pull-offs which have been strummed.
        /// </summary>
        public int HoposStrummed;

        /// <summary>
        /// Number of ghost inputs the player has made.
        /// </summary>
        public int GhostInputs;


        /// <summary>
        /// Number of Tap Notes That Have Been Strummed
        /// </summary>
        public int TapsStrummed;

        /// <summary>
        /// Invoke an instance of the SubClass Enhanced Guitar Stats for tracking of additional profile information for Five Fret Instruments
        /// </summary>
        public EnhancedGuitarStats EnhancedFiveFretStats = new();

        public FiveFretSectionTracker SectionStatsTracker = null!;




        public GuitarStats()
        {
        }

        public GuitarStats(GuitarStats stats) : base(stats)
        {
            Overstrums = stats.Overstrums;
            HoposStrummed = stats.HoposStrummed;
            //TapsStrummed = stats.TapsStrummed;
            GhostInputs = stats.GhostInputs;
            SustainScore = stats.SustainScore;


        }

        public GuitarStats(UnmanagedMemoryStream stream, int version)
            : base(stream, version)
        {
            Overstrums = stream.Read<int>(Endianness.Little);
            HoposStrummed = stream.Read<int>(Endianness.Little);
            //TapsStrummed = stream.Read<int>(Endianness.Little);
            GhostInputs = stream.Read<int>(Endianness.Little);
            SustainScore = stream.Read<int>(Endianness.Little);

        }

        public override void Reset()
        {
            base.Reset();
            Overstrums = 0;
            HoposStrummed = 0;
            //TapsStrummed = 0;
            GhostInputs = 0;
            SustainScore = 0;
        }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(Overstrums);
            writer.Write(HoposStrummed);
            //writer.Write(TapsStrummed);
            writer.Write(GhostInputs);
            writer.Write(SustainScore);
        }

        public override ReplayStats ConstructReplayStats(string name)
        {
            return new GuitarReplayStats(name, this);
        }

    }






    public class EnhancedGuitarStats
    {
        public struct NoteTypesStorage
        {
            public int noteTypeStrum;
            public int noteTypeHOPO;
            public int noteTypeTap;
            public int fretGreen;
            public int fretRed;
            public int fretYellow;
            public int fretBlue;
            public int fretOrange;
            //public int fretBlack;
            //public int fretWhite;
            public int AllNoteCount;
            public int fretOpen;
            public int countNote;


            public void CountNotesInSong(GuitarNote notes)
            {


                AllNoteCount++;
                if (notes.IsStrum)
                {
                    noteTypeStrum++;
                }
                else if (notes.IsHopo)
                {
                    noteTypeHOPO++;
                }
                else if (notes.IsTap)
                {
                    noteTypeTap++;
                }

                if (notes.Fret == (int)FiveFretGuitarFret.Open)
                {
                    fretOpen++;
                }

            }
        }

        public NoteTypesStorage TotalNotesInSong = new();
        public NoteTypesStorage TotalNotesHitInSong = new();
        public NoteTypesStorage TotalNotesMissedInSong = new();








        public class FiveFretSectionTracker
        {

            public class SectionStats
            {
                public int SectionIndex;
                public NoteTypesStorage TotalNotesInSection = new();
                public NoteTypesStorage TotalNotesHitInSection = new();
                public NoteTypesStorage TotalNotesMissedInSection = new();
                public int TotalScoreInSection;
                public string sectionName;
            }


            public SectionStats[] SectionStatsArray;

            public FiveFretSectionTracker(List<Section> sections, InstrumentDifficulty<GuitarNote> difficulty)
            {

                SectionStatsArray = new SectionStats[sections.Count];

                for (int i = 0; i < sections.Count; i++)
                {
                    var section = sections[i];
                    var sectionStats = new SectionStats();

                    foreach (var note in difficulty.Notes)
                    {
                        if (note.Tick >= section.Tick && note.Tick < section.TickEnd)
                        {
                            sectionStats.TotalNotesInSection.CountNotesInSong(note);

                        }
                    }
                    SectionStatsArray[i] = sectionStats;
                }
            }
        }

    }




















}

















































































































































